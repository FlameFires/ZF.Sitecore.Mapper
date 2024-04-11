using System;

namespace ZF.Sitecore.Mapper
{
    [Flags]
    public enum SilentLogLevel
    {
        None = 0x0,
        Warning = 0x1,
        Error = 0x2,
    }
}