using System.Text.RegularExpressions;
using Sitecore.Configuration;

namespace ZF.Sitecore.Mapper
{
    public class NamingConvention
    {
        public static bool EnableCamelSpaceNaming =>
            Settings.GetBoolSetting(Constants.EnableCamelSpaceNaming, false);


        public static Nomenclature Nomenclature =>
            EnableCamelSpaceNaming ? Nomenclature.CamelSpaceNaming : Nomenclature.CamelNaming;

        private const string Pattern = "([a-z]|[A-Z]+)([A-Z]|(\\d+))";

        public static string ToCamelSpaceNaming(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name;
            return Regex.Replace(name, "([a-z]|[A-Z]+)([A-Z]|(\\d+))", "$1 $2", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        }

        public static string ToCamelNaming(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return name;
            return Regex.Replace(name, "([a-z]|[A-Z]+)([A-Z]|(\\d+))", "$1$2", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        }

        public static string ToNamingConvention(string name)
        {
            switch (Nomenclature)
            {
                case Nomenclature.CamelSpaceNaming:
                    return ToCamelSpaceNaming(name);
                case Nomenclature.CamelNaming:
                    return ToCamelNaming(name);
            }

            return name;
        }
    }
}