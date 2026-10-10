
namespace PictureRenderer.Profiles
{
    public abstract class PictureProfileBase
    {
        public int[] SrcSetWidths { get; set; }
        public string[] Sizes { get; set; }

        /// <summary>
        /// Default value is 80.
        /// </summary>
        public int? Quality { get; set; }

        /// <summary>
        /// The wanted aspect ratio of the image (width/height).
        /// Example: An image with aspect ratio 16:9 = 16/9 = 1.777.
        /// </summary>
        public double AspectRatio { get; set; }

        /// <summary>
        /// Set a fixed height for all image sizes. Overrides the aspect ratio setting.  
        /// </summary>
        public int? FixedHeight { get; set; }

        public bool ShowInfo { get; set; }


        protected PictureProfileBase()
        {
            ShowInfo = false;
        }
    }
}
