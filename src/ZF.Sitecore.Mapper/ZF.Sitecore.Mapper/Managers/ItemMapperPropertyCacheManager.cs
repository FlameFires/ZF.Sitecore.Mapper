using System;
using System.Linq;
using System.Reflection;
using Sitecore;
using Sitecore.Caching;

namespace ZF.Sitecore.Mapper
{
    internal class ItemMapperPropertyCacheManager
    {
        private const string TypePropertiesCacheKeyPrefix = "ItemMapperPropertyCache_";
        private static readonly HtmlCache _cache = CacheManager.GetHtmlCache(Context.Site);

        public static PropertyInfo[] GetCachedProperties(Type targetType)
        {
            string cacheKey = TypePropertiesCacheKeyPrefix + targetType.FullName;

            if (_cache.InnerCache.ContainsKey(cacheKey))
            {
                return (PropertyInfo[])_cache.InnerCache[cacheKey];
            }

            var properties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty).ToArray();
            _cache.InnerCache.Add(cacheKey, properties, TimeSpan.FromDays(1));

            return properties;
        }
    }
}