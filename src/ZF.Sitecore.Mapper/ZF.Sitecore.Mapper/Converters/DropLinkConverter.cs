using System.Linq;
using System.Reflection;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ZF.Sitecore.Mapper
{
    public class DropLinkConverter : SimpleConverter
    {
        public override bool Match(Field field, PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<DroplinkFieldNameAttribute>() != null;
        }

        public override object Convert(Item item, Field field, PropertyInfo propertyInfo)
        {
            ReferenceField referenceField = field;
            var dropLinkFieldName = field.Name;
            var dplAttr = propertyInfo.GetCustomAttributes<DroplinkFieldNameAttribute>()?.FirstOrDefault();
            if (dplAttr != null)
            {
                dropLinkFieldName = dplAttr.Name;
            }

            var value = referenceField?.TargetItem?.Fields[dropLinkFieldName]?.Value;
            return value;
        }
    }
}