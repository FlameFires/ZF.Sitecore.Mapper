using System;
using System.Linq;
using System.Reflection;

namespace ZF.Sitecore.Mapper
{
    public static class MapperAttributesExtension
    {
        public static bool TryGetAttribute<T>(this PropertyInfo propertyInfo, out T attribute) where T : Attribute =>
            (attribute = propertyInfo.GetCustomAttributes<T>()?.FirstOrDefault()) != null;

        public static bool ContainsLevel(this SilentLogLevel silentLogLevel, SilentLogLevel target) => (silentLogLevel & target) == target;
    }
}