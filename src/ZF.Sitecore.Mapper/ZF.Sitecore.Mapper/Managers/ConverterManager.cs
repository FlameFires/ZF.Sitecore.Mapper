using System;
using System.Collections.Generic;

namespace ZF.Sitecore.Mapper
{
    public class ConverterManager
    {
        private static readonly Dictionary<Type, Lazy<IFieldConverter>> _converters = new Dictionary<Type, Lazy<IFieldConverter>>(10);
        
        public static IEnumerable<Lazy<IFieldConverter>> Converters => _converters.Values;

        public static bool Add(IFieldConverter converter)
        {
            var type = converter.GetType();
            if (_converters.ContainsKey(type))
            {
                return false;
            }

            _converters.Add(type, new Lazy<IFieldConverter>(() => converter, true));
            return true;
        }

        public static bool Add<TConverter>() where TConverter : class, IFieldConverter, new()
        {
            return Add(Activator.CreateInstance<TConverter>());
        }
    }
}