using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace ZF.Sitecore.Mapper
{
    public sealed class ItemMapper : IItemMapper
    {
        private object ConverterInvoke(Item item, Field field, PropertyInfo propertyInfo, out bool matched)
        {
            matched = false;
            if (field == null) return null;


            foreach (var converter in ConverterManager.Converters)
            {
                if (converter.Value.Match(field, propertyInfo))
                {
                    matched = true;
                    return converter.Value.Convert(item, field, propertyInfo);
                }
            }

            return null;
        }

        private Field ResolveField(PropertyInfo propertyInfo, Item item, out string fieldName)
        {
            fieldName = propertyInfo.Name;
            if (propertyInfo.TryGetAttribute<FieldNameAttribute>(out var fieldNameAttribute))
                fieldName = string.IsNullOrWhiteSpace(fieldNameAttribute.Name) ? fieldName : fieldNameAttribute.Name;
            else
            {
                fieldName = NamingConvention.ToNamingConvention(fieldName);
            }

            // Try get the field by the section
            if (propertyInfo.TryGetAttribute<SectionNameAttribute>(out var sectionNameAttribute))
            {
                var sectionName = sectionNameAttribute.Name;
                if (!string.IsNullOrWhiteSpace(sectionName))
                {
                    var fName = fieldName;
                    return item.Fields.FirstOrDefault(z => z.Section == sectionName && z.Name == fName);
                }
            }

            return string.IsNullOrWhiteSpace(fieldName) ? null : item.Fields[fieldName];
        }

        public T Map<T>(Item item) where T : class, new()
        {
            return (T)Map(typeof(T), item);
        }

        public object Map(Type type, Item item)
        {
            if (item == null) return null;
            var targetType = type;
            var instance = Activator.CreateInstance(type);

            var propertyInfos = ItemMapperPropertyCacheManager.GetCachedProperties(targetType);
            foreach (var propertyInfo in propertyInfos)
            {
                if (!propertyInfo.CanWrite) continue;
                
                if (propertyInfo.TryGetAttribute<IgnoreFieldAttribute>(out var _)) continue;
                
                propertyInfo.TryGetAttribute<SilentLogFieldAttribute>(out var silentLogFieldAttribute);
                
                var silentLogLevel = silentLogFieldAttribute?.SilentLogLevel ?? SilentLogLevel.None;

                Field field = null;
                var fieldName = string.Empty;
                
                var propertyMsg = $"{propertyInfo.DeclaringType?.FullName ?? "null"}-{propertyInfo.Name}";
                try
                {
                    field = ResolveField(propertyInfo, item, out fieldName);
                }
                catch (Exception ex)
                {
                    if (!silentLogLevel.ContainsLevel(SilentLogLevel.Error))
                        Log.Error($"[{propertyMsg}] Failed to resolve.", ex, this.GetType());
                }

                if (field == null)
                {
                    var pType = propertyInfo.PropertyType;
                    if (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(List<>))
                        continue;

                    if (!silentLogLevel.ContainsLevel(SilentLogLevel.Warning))
                        Log.Warn($"No corresponding field[{propertyMsg}{fieldName.EmptyOr(v => $"【{v}】")}] was found in the item[{item.ID}-{item.Paths.FullPath}].", this);
                    continue;
                }

                try
                {
                    var propertyValue = ConverterInvoke(item, field, propertyInfo, out var matched);
                    if (!matched && !silentLogLevel.ContainsLevel(SilentLogLevel.Warning))
                        Log.Warn($"The value of field[{item.ID}-{item.Paths.FullPath}] does not have a corresponding converter[{propertyMsg}].", this);
                    if (propertyValue != null)
                        propertyInfo.SetValue(instance, propertyValue);
                }
                catch (Exception ex)
                {
                    if (!silentLogLevel.ContainsLevel(SilentLogLevel.Error))
                        Log.Error($"[{field.ID}-{field.Name}=>{propertyMsg}] Failed to convert.", ex, this.GetType());
                }
            }

            return instance;
        }
    }
}