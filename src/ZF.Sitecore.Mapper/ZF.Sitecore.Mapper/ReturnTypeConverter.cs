using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;

namespace ZF.Sitecore.Mapper
{
    public class ReturnTypeConverter : IFieldConverter
    {
        private readonly Dictionary<Type, TypeReturner> _returners = new Dictionary<Type, TypeReturner>(10);

        public ReturnTypeConverter AddTypeReturner<T>(TypeReturner returner) where T : class
        {
            _returners.Add(typeof(T), returner);
            return this;
        }

        public bool Match(Field field, PropertyInfo propertyInfo)
        {
            return _returners.ContainsKey(propertyInfo.PropertyType);
        }

        public object Convert(Item item, Field field, PropertyInfo propertyInfo)
        {
            if (_returners.TryGetValue(propertyInfo.PropertyType, out var operation))
            {
                return operation.Invoke(item, field, propertyInfo);
            }

            return null;
        }

        public static ReturnTypeConverter CreateInstance()
        {
            var returnTypeConverter = new ReturnTypeConverter();

            // HtmlString
            returnTypeConverter.AddTypeReturner<HtmlString>((item, field, propertyInfo) =>
            {
                var value = FieldRenderer.Render(item, field.Name);
                return string.IsNullOrEmpty(value) ? null : new HtmlString(value);
            });
            
            // MvcHtmlString
            returnTypeConverter.AddTypeReturner<MvcHtmlString>((item, field, propertyInfo) =>
            {
                var value = FieldRenderer.Render(item, field.Name);
                return string.IsNullOrEmpty(value) ? null : MvcHtmlString.Create(value);
            });

            // ImageField
            returnTypeConverter.AddTypeReturner<FieldMediaItem>((item, field, propertyInfo) =>
            {
                ImageField imageField = field;
                return imageField == null ? null : new FieldMediaItem(imageField);
            });

            // LinkField
            returnTypeConverter.AddTypeReturner<FieldItemLink>((item, field, propertyInfo) =>
            {
                LinkField linkField = field;
                return linkField == null ? null : new FieldItemLink(linkField);
            });
            
            // ReferenceField
            returnTypeConverter.AddTypeReturner<Item>((item, field, propertyInfo) =>
            {
                ReferenceField referenceField = field;
                return referenceField?.TargetItem;
            });
            
            // MultilistField
            returnTypeConverter.AddTypeReturner<Item[]>((item, field, propertyInfo) =>
            {
                MultilistField multilistField = field;
                return multilistField?.GetItems();
            });

            return returnTypeConverter;
        }
    }
}