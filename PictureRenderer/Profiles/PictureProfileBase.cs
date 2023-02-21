using System;
using System.Collections.Generic;
using System.Linq;

namespace PictureRenderer.Profiles
{
    public abstract class PictureProfileBase
    {
        private int _fallBackWidth;

        public int[] SrcSetWidths { get; set; }
        public string[] Sizes { get; set; }

        /// <summary>
        /// Use this when you want to show different images depending on media condition (for example different image for mobile sized screen and desktop sized screen).
        /// </summary>
        public MediaCondition[] MultiImageMediaConditions { get; set; }

        /// <summary>
        /// Default value is 80.
        /// </summary>
        public int? Quality { get; set; }

        

        /// <summary>
        /// Image width for browsers without support for picture element. Will use the largest image if not set.
        /// </summary>
        public int FallbackWidth
        {
            get
            {
                if (_fallBackWidth == default && MultiImageMediaConditions != null)
                {
                    return MultiImageMediaConditions.OrderByDescending(mcw => mcw.Width).FirstOrDefault()?.Width ?? default;
                }

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
        /// Set a fixed height for all image sizes. Overrides the aspect ratio setting.  
        /// </summary>
        public int? FixedHeight { get; set; }

        /// <summary>
        /// If true, width and height attributes will be rendered on the img element.
        /// </summary>
        public bool ImgWidthHeight { get; set; }

        /// <summary>
        /// Img element decoding attribute.
        /// </summary>
        public ImageDecoding ImageDecoding {get; set;}

        public bool ShowInfo { get; set; }


        protected PictureProfileBase()
        {
            Quality = 80;
            ImageDecoding = ImageDecoding.Async;
            ShowInfo = false;
        }
    }
}
