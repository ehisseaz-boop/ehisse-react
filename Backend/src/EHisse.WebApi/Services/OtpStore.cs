using System;
using System.Collections.Concurrent;

namespace EHisse.WebApi.Services
{
    public static class OtpStore
    {
        private static readonly ConcurrentDictionary<string, (string Code, DateTime Expiry)> _codes = new();

        public static void SetCode(string key, string code, TimeSpan validity)
        {
            _codes[key.ToLower()] = (code, DateTime.UtcNow.Add(validity));
        }

        public static bool ValidateCode(string key, string code)
        {
            if (_codes.TryGetValue(key.ToLower(), out var item))
            {
                if (DateTime.UtcNow <= item.Expiry && item.Code == code.Trim())
                {
                    _codes.TryRemove(key.ToLower(), out _);
                    return true;
                }
            }
            return false;
        }
    }
}
