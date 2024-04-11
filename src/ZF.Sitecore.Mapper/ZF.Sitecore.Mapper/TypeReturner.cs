using System.Reflection;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ZF.Sitecore.Mapper
{
    public delegate object TypeReturner(Item item, Field field, PropertyInfo propertyInfo);
}