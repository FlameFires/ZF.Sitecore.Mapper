using System.Web;

namespace ZF.Sitecore.Mapper
{
    public interface IHtmlStr
    {
        /// <summary>
        /// Convert to HtmlString
        /// </summary>
        /// <returns><see cref="HtmlString"/></returns>
        HtmlString ToHtmlStr();
    }
}