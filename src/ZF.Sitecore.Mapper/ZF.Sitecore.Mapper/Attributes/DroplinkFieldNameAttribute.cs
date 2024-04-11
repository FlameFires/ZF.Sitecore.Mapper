using System;

namespace ZF.Sitecore.Mapper
{
    /// <summary>
    /// DroplinkFieldName
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DroplinkFieldNameAttribute : Attribute
    {
        public DroplinkFieldNameAttribute()
        {
        }

        public DroplinkFieldNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Default: Value
        /// </summary>
        public string Name { get; set; } = "Value";
    }
}