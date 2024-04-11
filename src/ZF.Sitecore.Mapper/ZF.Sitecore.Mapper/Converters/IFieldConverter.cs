using System.Reflection;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ZF.Sitecore.Mapper
{
    public interface IFieldConverter
    {
        bool Match(Field field, PropertyInfo propertyInfo);

        object Convert(Item item, Field field, PropertyInfo propertyInfo);
    }
}