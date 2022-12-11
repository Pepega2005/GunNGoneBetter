﻿using System.Text.Json;

namespace GunNGoneBetter.Utility
{
    // Методы расширения - гуглить, чтобы поянть, как это работает
    public static class ExtensionsSession
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
