using Microsoft.AspNetCore.Identity;
using MoviesApp.Models;

namespace MoviesApp.Services
{    public interface IUserActivityService
    {
        Task LogUserActivityAsync(string userId, string activity, string? details = null);
        Task LogActivityAsync(string userName, string activity, string? details = null);
        Task<List<UserActivity>> GetUserActivityAsync(string userId, int count = 10);
        Task SetLastSeenAsync(string userId);
        Task<DateTime?> GetLastSeenAsync(string userId);
    }

    public class UserActivityService : IUserActivityService
    {
        private readonly ISessionService _sessionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserActivityService> _logger;

        public UserActivityService(
            ISessionService sessionService,
            UserManager<ApplicationUser> userManager,
            ILogger<UserActivityService> logger)
        {
            _sessionService = sessionService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task LogUserActivityAsync(string userId, string activity, string? details = null)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return;

                var userActivity = new UserActivity
                {
                    UserId = userId,
                    UserName = user.UserName ?? "Unknown",
                    Activity = activity,
                    Details = details,
                    Timestamp = DateTime.Now,
                    IpAddress = GetCurrentIpAddress()
                };

                // Store in session (for current session tracking)
                var sessionKey = $"UserActivity_{userId}";
                var activities = _sessionService.GetObject<List<UserActivity>>(sessionKey) ?? new List<UserActivity>();
                activities.Insert(0, userActivity);

                // Keep only last 20 activities in session
                if (activities.Count > 20)
                {
                    activities = activities.Take(20).ToList();
                }

                _sessionService.SetObject(sessionKey, activities);

                _logger.LogInformation("User activity logged: {UserId} - {Activity}", userId, activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging user activity for user {UserId}", userId);
            }
        }        public Task<List<UserActivity>> GetUserActivityAsync(string userId, int count = 10)
        {
            try
            {
                var sessionKey = $"UserActivity_{userId}";
                var activities = _sessionService.GetObject<List<UserActivity>>(sessionKey) ?? new List<UserActivity>();
                return Task.FromResult(activities.Take(count).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user activity for user {UserId}", userId);
                return Task.FromResult(new List<UserActivity>());
            }
        }        public Task SetLastSeenAsync(string userId)
        {
            try
            {
                var sessionKey = $"LastSeen_{userId}";
                _sessionService.SetString(sessionKey, DateTime.Now.ToString("O"));
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting last seen for user {UserId}", userId);
                return Task.CompletedTask;
            }
        }        public Task<DateTime?> GetLastSeenAsync(string userId)
        {
            try
            {
                var sessionKey = $"LastSeen_{userId}";
                var lastSeenStr = _sessionService.GetString(sessionKey);
                
                if (DateTime.TryParse(lastSeenStr, out var lastSeen))
                {
                    return Task.FromResult<DateTime?>(lastSeen);
                }
                
                return Task.FromResult<DateTime?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting last seen for user {UserId}", userId);
                return Task.FromResult<DateTime?>(null);
            }
        }

        private string GetCurrentIpAddress()
        {
            // This would need HttpContextAccessor to get real IP
            return "127.0.0.1"; // Placeholder
        }

        public async Task LogActivityAsync(string userName, string activity, string? details = null)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    await LogUserActivityAsync(user.Id, activity, details);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging activity for user {UserName}", userName);
            }
        }
    }

    public class UserActivity
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Activity { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; } = string.Empty;
    }
}
