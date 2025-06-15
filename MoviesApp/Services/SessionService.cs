using System.Text.Json;

namespace MoviesApp.Services
{
    public interface ISessionService
    {
        void SetString(string key, string value);
        string? GetString(string key);
        void SetObject<T>(string key, T value);
        T? GetObject<T>(string key) where T : class;
        void Remove(string key);
        void Clear();
        bool ContainsKey(string key);
    }

    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession? Session => _httpContextAccessor.HttpContext?.Session;

        public void SetString(string key, string value)
        {
            Session?.SetString(key, value);
        }

        public string? GetString(string key)
        {
            return Session?.GetString(key);
        }

        public void SetObject<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            SetString(key, json);
        }

        public T? GetObject<T>(string key) where T : class
        {
            var json = GetString(key);
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return null;
            }
        }

        public void Remove(string key)
        {
            Session?.Remove(key);
        }

        public void Clear()
        {
            Session?.Clear();
        }

        public bool ContainsKey(string key)
        {
            return Session?.Keys.Contains(key) ?? false;
        }
    }
}
