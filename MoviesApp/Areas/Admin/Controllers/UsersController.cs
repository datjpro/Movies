using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApp.Areas.Admin.Models;
using MoviesApp.Models;
using MoviesApp.Services;

namespace MoviesApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = UserRoles.Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserActivityService _userActivityService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserActivityService userActivityService,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userActivityService = userActivityService;
            _logger = logger;
        }        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, string? filterRole = null)
        {
            try
            {
                _logger.LogInformation($"Users Index called with: page={page}, pageSize={pageSize}, search={search}, filterRole={filterRole}");
                
                var query = _userManager.Users.AsQueryable();

                // Search filter
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.Trim();
                    query = query.Where(u => u.FullName.Contains(search) || 
                                           u.Email.Contains(search) || 
                                           (u.UserName != null && u.UserName.Contains(search)));
                    _logger.LogInformation($"Applied search filter: {search}");
                }

                // Lấy tất cả users để có thể filter theo role
                var allUsers = await query.ToListAsync();
                _logger.LogInformation($"Found {allUsers.Count} users after search filter");

                // Filter by role if specified
                var filteredUsers = allUsers;
                if (!string.IsNullOrEmpty(filterRole))
                {
                    var usersWithRole = new List<ApplicationUser>();
                    foreach (var user in allUsers)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains(filterRole))
                        {
                            usersWithRole.Add(user);
                        }
                    }
                    filteredUsers = usersWithRole;
                    _logger.LogInformation($"Found {filteredUsers.Count} users with role: {filterRole}");
                }

                // Tính toán phân trang sau khi filter
                var totalUsers = filteredUsers.Count;
                var users = filteredUsers
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation($"Final result: {users.Count} users on page {page}");

                // Get roles for each user
                var userRoles = new Dictionary<string, List<string>>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userRoles[user.Id] = roles.ToList();
                }

                var viewModel = new UserManagementViewModel
                {
                    Users = users,
                    UserRoles = userRoles,
                    TotalUsers = totalUsers,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize),
                    SearchTerm = search,
                    FilterRole = filterRole
                };

                await _userActivityService.LogActivityAsync(
                    User.Identity!.Name!, 
                    "Viewed Users Management", 
                    $"Page {page}, Search: {search ?? "None"}, Filter: {filterRole ?? "None"}"
                );

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users management");
                TempData["Error"] = "Có lỗi xảy ra khi tải danh sách người dùng: " + ex.Message;
                return View(new UserManagementViewModel());
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                SelectedRoles = userRoles.ToList(),
                AvailableRoles = allRoles!
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                // Update user properties
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.IsActive = model.IsActive;
                user.EmailConfirmed = model.EmailConfirmed;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    model.AvailableRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
                    return View(model);
                }

                // Update roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                var rolesToRemove = currentRoles.Except(model.SelectedRoles);
                var rolesToAdd = model.SelectedRoles.Except(currentRoles);

                if (rolesToRemove.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                }

                if (rolesToAdd.Any())
                {
                    await _userManager.AddToRolesAsync(user, rolesToAdd);
                }

                await _userActivityService.LogActivityAsync(
                    User.Identity!.Name!, 
                    "Updated User", 
                    $"Updated user: {user.Email}"
                );

                TempData["Success"] = "Cập nhật người dùng thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", model.Id);
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật người dùng.";
                model.AvailableRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng." });
                }

                user.IsActive = !user.IsActive;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await _userActivityService.LogActivityAsync(
                        User.Identity!.Name!, 
                        "Toggled User Status", 
                        $"User: {user.Email}, Active: {user.IsActive}"
                    );

                    return Json(new { success = true, isActive = user.IsActive });
                }

                return Json(new { success = false, message = "Không thể cập nhật trạng thái." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status for {UserId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng." });
                }

                // Don't allow deleting the current admin user
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser?.Id == user.Id)
                {
                    return Json(new { success = false, message = "Không thể xóa tài khoản hiện tại." });
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await _userActivityService.LogActivityAsync(
                        User.Identity!.Name!, 
                        "Deleted User", 
                        $"Deleted user: {user.Email}"
                    );

                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Không thể xóa người dùng." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra." });
            }
        }
    }
}
