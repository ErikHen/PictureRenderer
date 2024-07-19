
using System.Collections.Generic;

namespace PictureRenderer
{
    public class PictureAttributes
    {
        /// <summary>
        /// img element alt attribute
        /// </summary>
        public string ImgAlt { get; set; }

        /// <summary>
        /// img element class attribute
        /// </summary>
        public string ImgClass { get; set; }

        /// <summary>
        /// img element decoding attribute. Default value: async.
        /// </summary>
        public ImageDecoding ImgDecoding { get; set; }

        /// <summary>
        /// img element fetchPriority attribute. Default value: auto
        /// </summary>
        public FetchPriority ImgFetchPriority { get; set; }

        /// <summary>
        /// Type of lazy loading. Currently supports browser native or none. Default value: browser native)
        /// </summary>
        public LazyLoading LazyLoading { get; set; }

        /// <summary>
        /// If true, width and height attributes will be rendered on the img element.
        /// </summary>
        public bool RenderImgWidthHeight { get; set; }

        /// <summary>
        /// May be used to add additional attributes (like data or itemprop attributes) to the img element.
        /// </summary>
        public Dictionary<string, string> ImgAdditionalAttributes { get; set; }

        public PictureAttributes()        {
            ImgDecoding = ImageDecoding.Async;
            ImgFetchPriority = FetchPriority.None;
            RenderImgWidthHeight = false;
            LazyLoading = LazyLoading.Browser;
            ImgAdditionalAttributes = new Dictionary<string, string>();
        }
    }
}
