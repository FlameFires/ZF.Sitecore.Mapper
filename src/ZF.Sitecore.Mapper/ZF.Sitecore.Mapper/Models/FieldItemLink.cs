using System;
using System.Web;
using System.Web.Mvc;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ZF.Sitecore.Mapper
{
    /// <summary>
    /// Presentation of LinkField
    /// </summary>
    public class FieldItemLink : IHtmlStr
    {
        /// <summary>
        /// For page item
        /// </summary>
        /// <param name="item"></param>
        public FieldItemLink(Item item)
        {
            this.Url = GetLinkUrl(item, () => item?.GetItemUrl());
        }

        /// <summary>
        /// For linkField
        /// </summary>
        /// <param name="linkField"></param>
        public FieldItemLink(LinkField linkField)
        {
            LinkField lf = linkField;
            if (lf == null) return;
            InnerField = lf;

            var title = HttpUtility.HtmlAttributeEncode(lf.Title.IsNullOrEmpty() ? lf.Text : lf.Title);
            this.Url = GetLinkUrl(lf);
            this.Rel = lf.GetLinkRel();
            this.Title = title.EmptyOr(v => $"title={v}");
            this.Target = lf.Target.EmptyOr(v => $"target={v}");
            this.Class = lf.Class;
        }

        /// <summary>
        /// The inner linkField. It is null if the linkField is null.
        /// </summary>
        public LinkField InnerField { get; }

        /// <summary>
        /// <remarks>Item url / Page item alias url / Media url / External url</remarks>
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// If the linkType is "external" and the target is "_blank", the value of rel attribute is "rel=\"noopener noreferrer\""
        /// </summary>
        /// <example>
        /// rel=\"noopener noreferrer\"
        /// </example>
        public string Rel { get; set; }

        /// <summary>
        /// The title attribute is used to specify extra information about the link. It from the title attribute or the text attribute of the linkField.
        /// </summary>
        /// <example>
        /// title="This is a title"
        /// </example>
        public string Title { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the linked document. It from the target attribute of the linkField.
        /// </summary>
        /// <example>
        /// target="_blank"
        /// </example>
        public string Target { get; set; }

        /// <summary>
        /// The class attribute specifies one or more classnames for an element. It from the class attribute of the linkField.
        /// </summary>
        /// <example>
        /// class="btn btn-primary"
        /// </example>
        public string Class { get; set; }

        /// <summary>
        /// The attributes of the linkField
        /// </summary>
        /// <example>
        /// rel="noopener noreferrer" title="This is a title" target="_blank"
        /// </example>
        public string Attributes => $"{Rel} {Title} {Target}";

        /// <summary>
        /// Convert to HtmlString
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return this.ToHtmlStr().ToHtmlString();
        }

        /// <inheritdoc />
        public HtmlString ToHtmlStr()
        {
            return MvcHtmlString.Create($"<a href={this.Url} {this.Attributes}>{this.Title}</a>");
        }

        #region Private methods

        private string GetLinkUrl(LinkField linkField)
        {
            var targetItem = linkField.TargetItem;
            return GetLinkUrl(targetItem, linkField.GetLinkUrl);
        }

        private string GetLinkUrl(Item item, Func<string> defaultUrl)
        {
            var isPageItem = FieldItemLink.isPageItem(item?.Template.FullName);

            // Call GetLinkUrl() of Helper if target item is null or not page item
            if (item == null || !isPageItem) return defaultUrl?.Invoke();

            // Get item url or alias url of the page item if target item is page item
            return item.GetItemUrl();
        }

        /// <summary>
        /// Judge whether the item is page item
        /// </summary>
        /// <param name="fullPath">The template full path</param>
        /// <returns></returns>
        private static bool isPageItem(string fullPath) => !fullPath.IsNullOrEmpty() && fullPath.StartsWith(Settings.GetSetting(Constants.PageTemplatePath, "/sitecore/templates/User Defined/CorpSite/Page"));

        #endregion

        /// <summary>
        /// Empty FieldItemLink
        /// </summary>
        /// <returns></returns>
        public static FieldItemLink Empty()
        {
            return new FieldItemLink((LinkField)null);
        }
    }
}