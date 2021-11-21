using System;
using System.Linq;

namespace PictureRenderer.Profiles
{
    public abstract class PictureProfileBase
    {
        private int _fallBackWidth;

        public int[] SrcSetWidths { get; set; }
        [Obsolete("Use Sizes instead.")]
        public string[] SrcSetSizes { get; set; }
        public string[] Sizes { get; set; }
        public int? Quality { get; set; }
        public bool IncludeWebp { get; } //always false until ImageSharp has support for Webp.

        /// <summary>
        /// Image width for browsers without support for picture element. Will use the largest SrcSetWidth if not set.
        /// </summary>
        public int FallbackWidth
        {
            get
            {
                if (_fallBackWidth == default && SrcSetWidths != null)
                {
                    return SrcSetWidths.Max();
                }
                return _fallBackWidth;
            }
            set => _fallBackWidth = value;
        }

        /// <summary>
        /// The wanted aspect ratio of the image (width/height).
        /// Example: An image with aspect ratio 16:9 = 16/9 = 1.777.
        /// </summary>
        public double AspectRatio { get; set; }

        /// <summary>
        /// If true, width and height attributes will not be rendered on the img element.
        /// </summary>
        public bool NoImgWidthHeight { get; set; }

        /// <summary>
        /// Img element decoding attribute.
        /// </summary>
        public ImageDecoding ImageDecoding {get; set;}

        protected PictureProfileBase()
        {
            Quality = 80;
            IncludeWebp = false;
            ImageDecoding = ImageDecoding.Async;
        }
    }
}
