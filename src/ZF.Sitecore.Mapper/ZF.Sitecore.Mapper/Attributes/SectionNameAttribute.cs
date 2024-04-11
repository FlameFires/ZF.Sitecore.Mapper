using System;

namespace ZF.Sitecore.Mapper
{
    /// <summary>
    /// SectionName
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SectionNameAttribute : Attribute
    {
        public SectionNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}