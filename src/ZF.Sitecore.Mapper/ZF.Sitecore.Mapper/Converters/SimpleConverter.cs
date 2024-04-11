using System;
using System.Reflection;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ZF.Sitecore.Mapper
{
    public class SimpleConverter : IFieldConverter
    {
        public virtual bool Match(Field field, PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;
            // https://learn.microsoft.com/en-us/dotnet/api/system.type.isprimitive?view=netframework-4.8
            return propertyType.IsPrimitive || propertyType == typeof(DateTime) || propertyType == typeof(string);
        }

        public virtual object Convert(Item item, Field field, PropertyInfo propertyInfo)
        {
            var value = field?.Value;
            object result = value;
            if (value == null) return null;

            var propertyType = propertyInfo.PropertyType;

            // bool
            if (propertyType == typeof(bool) || propertyType == typeof(bool?))
            {
                return decimal.TryParse(value, out var numericValue) && numericValue != 0;
            }

            // datetime
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            {
                DateField dateField = field;
                return dateField?.DateTime;
            }

            // value type
            if (propertyType.IsSubclassOf(typeof(ValueType)))
            {
                var methodInfo = propertyType.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string), propertyType.MakeByRefType() }, null);
                if (methodInfo != null)
                {
                    var tempParams = new object[] { value, null };
                    var isSuccessful = (bool)methodInfo.Invoke(null, tempParams);
                    result = isSuccessful ? tempParams[1] : null;
                }
            }

            return result;
        }
    }
}