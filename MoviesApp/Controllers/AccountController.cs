using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviesApp.Models;
using MoviesApp.Services;

namespace MoviesApp.Controllers
{
    public class AccountController : Controller    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IUserActivityService _userActivityService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            IUserActivityService userActivityService,
            ISessionService sessionService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _userActivityService = userActivityService;
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "User with this email already exists");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                EmailConfirmed = true
            };            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.FreeUser);
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                // Log user activity and set session data
                await _userActivityService.LogUserActivityAsync(user.Id, "User Registration", $"New user registered: {model.Email}");
                await _userActivityService.SetLastSeenAsync(user.Id);
                _sessionService.SetString("UserRegistrationTime", DateTime.Now.ToString("O"));
                
                _logger.LogInformation("User {Email} registered successfully", model.Email);
                TempData["Success"] = "Registration successful! Welcome to Movies App.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (result.Succeeded)
            {
                // Log user activity and set session data
                await _userActivityService.LogUserActivityAsync(user.Id, "User Login", $"User logged in from IP: {HttpContext.Connection.RemoteIpAddress}");
                await _userActivityService.SetLastSeenAsync(user.Id);
                
                // Store login info in session
                _sessionService.SetString("LoginTime", DateTime.Now.ToString("O"));
                _sessionService.SetString("UserId", user.Id);
                _sessionService.SetString("UserEmail", user.Email ?? "");
                _sessionService.SetString("UserFullName", user.FullName ?? "");
                
                var roles = await _userManager.GetRolesAsync(user);
                _sessionService.SetObject("UserRoles", roles.ToList());
                
                _logger.LogInformation("User {Email} logged in successfully", model.Email);
                TempData["Success"] = $"Welcome back, {user.FullName ?? user.Email}!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View(model);
        }        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _userActivityService.LogUserActivityAsync(user.Id, "User Logout", "User logged out");
            }
            
            // Clear session data
            _sessionService.Clear();
            
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out");
            TempData["Info"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = roles;

            // Get session information
            var sessionInfo = new
            {
                LoginTime = _sessionService.GetString("LoginTime"),
                UserId = _sessionService.GetString("UserId"),
                UserEmail = _sessionService.GetString("UserEmail"),
                UserFullName = _sessionService.GetString("UserFullName"),
                UserRoles = _sessionService.GetObject<List<string>>("UserRoles") ?? new List<string>(),
                LastSeen = await _userActivityService.GetLastSeenAsync(user.Id)
            };
            ViewBag.SessionInfo = sessionInfo;

            // Get recent user activities
            var recentActivities = await _userActivityService.GetUserActivityAsync(user.Id, 5);
            ViewBag.RecentActivities = recentActivities;

            return View(user);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GenerateToken()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var token = await _tokenService.GenerateJwtTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var tokenInfo = new
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            };

            ViewBag.TokenInfo = tokenInfo;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName ?? string.Empty,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                TempData["Error"] = "Role does not exist";
                return RedirectToAction(nameof(ManageUsers));
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                TempData["Success"] = $"Role {role} assigned to {user.Email} successfully";
                _logger.LogInformation("Role {Role} assigned to user {Email}", role, user.Email);
            }
            else
            {
                TempData["Error"] = "Failed to assign role";
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (result.Succeeded)
            {
                TempData["Success"] = $"Role {role} removed from {user.Email} successfully";
                _logger.LogInformation("Role {Role} removed from user {Email}", role, user.Email);
            }
            else
            {
                TempData["Error"] = "Failed to remove role";
            }

            return RedirectToAction(nameof(ManageUsers));
        }
    }

    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
