using System;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Sitecore.Text;

namespace ZF.Sitecore.Mapper
{
    internal static class HelperExtension
    {
        public static bool IsNullOrEmpty(this object target)
        {
            return target == null || string.IsNullOrEmpty(target.ToString()) || string.IsNullOrWhiteSpace(target.ToString());
        }

        public static TInput IfEmpty<TInput>(this TInput value, TInput defaultValue) where TInput : class
        {
            return IsNullOrEmpty(value) ? defaultValue : value;
        }

        public static TOutput IfTrue<TOutput>(this bool value, TOutput defaultValue) where TOutput : class
        {
            return value ? defaultValue : default(TOutput);
        }

        public static TOutput IfFalse<TOutput>(this bool value, TOutput defaultValue) where TOutput : class
        {
            return !value ? defaultValue : default(TOutput);
        }

        public static TOutput EmptyOr<TInput, TOutput>(this TInput value, Func<TInput, TOutput> selector) where TInput : class
        {
            return IsNullOrEmpty(value) ? default(TOutput) : selector.Invoke(value);
        }

        public static bool IsContextSite(this Item item)
        {
            Assert.ArgumentNotNull(item, nameof(Item));

            return item.Paths.FullPath.StartsWith(Context.Site.StartPath, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetLinkRel(this LinkField lf)
        {
            Assert.ArgumentNotNull(lf, nameof(LinkField));

            if (lf.LinkType.Equals("external", StringComparison.OrdinalIgnoreCase)
                && lf.Target.Equals("_blank", StringComparison.OrdinalIgnoreCase)
                && Settings.GetBoolSetting(Constants.ProtectExternalLinks, true))
                return " rel=\"noopener noreferrer\"";

            return string.Empty;
        }

        public static string GetResolveUrl(this Item item)
        {
            var option = LinkManager.GetDefaultUrlBuilderOptions();
            return item.IsContextSite() ? GetItemUrl(item, option) : LinkManager.GetItemUrl(item, option);
        }

        public static string GetLinkUrl(this LinkField lf)
        {
            Assert.ArgumentNotNull(lf, nameof(LinkField));

            var url = lf.GetFriendlyUrl();
            var targetItem = lf.TargetItem;
            if (lf.IsInternal && targetItem != null)
            {
                if (targetItem.Paths.IsMediaItem)
                    return HashingUtils.ProtectAssetUrl(url);

                if (!targetItem.IsContextSite())
                {
                    string queryString = lf.QueryString?.RemoveHashSectionFromQueryString();
                    UrlString urlString = new UrlString(targetItem.GetResolveUrl())
                    {
                        Hash = lf.Anchor,
                        Query = queryString
                    };

                    return urlString.GetUrl();
                }

                if (targetItem.IsContextSite())
                {
                    UrlString urlString = new UrlString(targetItem.GetResolveUrl())
                    {
                        Hash = lf.Anchor,
                    };
                    string queryString = lf.QueryString?.RemoveHashSectionFromQueryString();
                    if (!string.IsNullOrWhiteSpace(queryString))
                        urlString.Query = queryString;

                    return urlString.GetUrl();
                }

                return url;
            }
            else if (lf.IsMediaLink)
                return HashingUtils.ProtectAssetUrl(url);
            else
                return url;
        }
        
        public static string RemoveHashSectionFromQueryString(this string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString)) return string.Empty;
            
            int num = queryString.IndexOf("#", StringComparison.Ordinal);
            if (num >= 0)
                return StringUtil.Left(queryString, num);
            return queryString;
        }

        public static string GetItemUrl(this Item item, ItemUrlBuilderOptions options = null)
        {
            Assert.ArgumentNotNull(item, nameof(Item));

            if (options == null)
                options = LinkManager.GetDefaultUrlBuilderOptions();

            // Check if the setting of AliasesActive is activated.
            if (!Settings.AliasesActive)
                return LinkManager.GetItemUrl(item, options);

            // Check if the item has an alias, if not return the default url, otherwise return the alias url.
            var url = item.GetAliasUrl(options);
            return !url.IsNullOrEmpty() ? url : LinkManager.GetItemUrl(item, options);
        }

        public static string GetAliasUrl(this Item item, ItemUrlBuilderOptions options = null)
        {
            Assert.ArgumentNotNull(item, nameof(Item));

            if (!Settings.AliasesActive)
                return string.Empty;

            var itemLinks = Globals.LinkDatabase.GetReferrers(item);
            if (itemLinks == null)
                return string.Empty;

            string ItemUrl = LinkManager.GetItemUrl(item, new ItemUrlBuilderOptions { AlwaysIncludeServerUrl = true, LowercaseUrls = true, SiteResolving = true });
            Uri uri = new Uri(ItemUrl);

            foreach (Item aliasItem in itemLinks.Select(z => z.GetSourceItem()).Where(z => z != null && z.TemplateID.Equals(TemplateIDs.Alias)))
            {
                // Get the alias path
                var url = StringUtil.EnsurePrefix('/', aliasItem.Paths.FullPath.Replace(Constants.AliasPath, ""));

                // Check if the alias has a query string
                if (options != null && options.LanguageEmbedding == LanguageEmbedding.Always)
                {
                    var lang = options.Language.Name;
                    if (options.LowercaseUrls != null && options.LowercaseUrls.Value)
                        lang = lang.ToLower();

                    if (options.LanguageLocation == LanguageLocation.FilePath)
                        url = $"/{lang}{url}";
                }

                // Return the alias url
                return $"{uri.Scheme}://{uri.Host}{url}";
            }

            return string.Empty;
        }
    }
}