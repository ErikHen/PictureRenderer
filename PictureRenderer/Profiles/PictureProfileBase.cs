using System.Linq;

namespace PictureRenderer.Profiles
{
    public abstract class PictureProfileBase
    {
        private int _fallBackWidth;

        public int[] SrcSetWidths { get; set; }
        public string[] SrcSetSizes { get; set; }
        public int? Quality { get; set; }
        public bool IncludeWebp { get; } //always false until ImageSharp has support for Webp.

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

        protected PictureProfileBase()
        {
            Quality = 80;
            IncludeWebp = false;
        }
    }
}
