using System;

namespace ZF.Sitecore.Mapper
{
    /// <summary>
    /// FieldName
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldNameAttribute : Attribute
    {
        public FieldNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}