using System.Web;
using System.Web.Mvc;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;

namespace ZF.Sitecore.Mapper
{
    /// <summary>
    /// Presentation of ImageField or MediaItem
    /// </summary>
    public class FieldMediaItem : IHtmlStr
    {
        /// <summary>
        /// For mediaItem
        /// </summary>
        /// <param name="item"></param>
        public FieldMediaItem(Item item)
        {
            if (item == null) return;

            _mediaItem = new MediaItem(item);

            var imageWidth = int.TryParse(Settings.GetSetting(Constants.ImageMaxWidth, "1920"), out int width) ? width : 1920;
            var mediaUrl = MediaManager.GetMediaUrl(_mediaItem, new MediaUrlBuilderOptions { MaxWidth = imageWidth });
            
            this.Url = HashingUtils.ProtectAssetUrl(StringUtil.EnsurePrefix('/', mediaUrl));
            this.Alt = _mediaItem.Alt;
        }

        /// <summary>
        /// For imageField
        /// </summary>
        /// <param name="field"></param>
        public FieldMediaItem(ImageField field) : this(field?.MediaItem)
        {
            _imageField = field;
        }

        /// <summary>
        /// The imageField
        /// </summary>
        private readonly ImageField _imageField;

        /// <summary>
        /// Them mediaItem which the imageField point to
        /// </summary>
        private readonly MediaItem _mediaItem;

        /// <summary>
        /// The url of the imageField
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The alt attribute of the imageField
        /// </summary>
        public string Alt { get; set; }

        /// <summary>
        /// Convert to HtmlString
        /// </summary>
        /// <returns><see cref="string"/></returns>
        public override string ToString()
        {
            return this.ToHtmlStr().ToHtmlString();
        }

        /// <inheritdoc cref="ToHtmlStr" />
        public HtmlString ToHtmlStr()
        {
            if (string.IsNullOrWhiteSpace(this.Url))
            {
                return MvcHtmlString.Empty;
            }

            return MvcHtmlString.Create($"<img src=\"{this.Url}\" alt=\"{this.Alt.IfEmpty("image")}\" />");
        }

        /// <summary>
        /// Get the url of the mediaItem
        /// </summary>
        /// <param name="options"><see cref="MediaUrlBuilderOptions"/></param>
        /// <returns></returns>
        public string GetUrl(MediaUrlBuilderOptions options = null)
        {
            return HashingUtils.ProtectAssetUrl(global::Sitecore.StringUtil.EnsurePrefix('/', options == null ? MediaManager.GetMediaUrl(_mediaItem) : MediaManager.GetMediaUrl(_mediaItem, options)));
        }

        public static FieldMediaItem Empty()
        {
            return new FieldMediaItem((ImageField)null);
        }
    }
}