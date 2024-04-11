using System;

namespace ZF.Sitecore.Mapper
{
    /// <summary>
    /// Suppress log generation
    /// </summary>
    public class SilentLogFieldAttribute : Attribute
    {
        public SilentLogLevel SilentLogLevel { get; set; }

        public SilentLogFieldAttribute() : this(SilentLogLevel.Warning) { }

        public SilentLogFieldAttribute(SilentLogLevel silentLogLevel)
        {
            SilentLogLevel = silentLogLevel;
        }
    }
}