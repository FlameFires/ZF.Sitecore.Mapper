using System;
using Sitecore.Data.Items;

namespace ZF.Sitecore.Mapper
{
    public interface IItemMapper
    {
        T Map<T>(Item item) where T : class, new();

        object Map(Type type, Item item);
    }
}