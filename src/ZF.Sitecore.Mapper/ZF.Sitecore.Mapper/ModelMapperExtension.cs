using System;
using Sitecore.Data.Items;

namespace ZF.Sitecore.Mapper
{
    public static class ModelMapperExtension
    {
        private static readonly IItemMapper ItemMapper = new ItemMapper();

        static ModelMapperExtension()
        {
            var returnTypeConverter = ReturnTypeConverter.CreateInstance();

            ConverterManager.Add(returnTypeConverter);
            ConverterManager.Add<DropLinkConverter>();
            ConverterManager.Add<SimpleConverter>();
        }

        public static object Map(this Item item, Type type)
        {
            return ItemMapper.Map(type, item);
        }

        public static TReturn Map<TReturn>(this Item item, Type type) where TReturn : class, new()
        {
            return (TReturn)ItemMapper.Map(type, item);
        }

        public static T Map<T>(this Item item) where T : class, new()
        {
            return ItemMapper.Map<T>(item);
        }
    }
}