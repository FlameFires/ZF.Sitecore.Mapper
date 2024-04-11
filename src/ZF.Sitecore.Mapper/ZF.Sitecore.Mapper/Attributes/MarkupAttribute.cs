using System;

namespace ZF.Sitecore.Mapper
{
    public class MarkupAttribute : Attribute
    {
        public string FieldName { get; set; }
        
        
    }

    public enum FieldType
    {
        Text,
        RichText,
        Image,
        Link,
        Date,
        Checkbox,
        Number,
        File,
        GeneralLink,
        InternalLink,
        ExternalLink,
        Email,
        Phone,
        ImageField,
    }
}