namespace PictureRenderer.Profiles
{
    public abstract class PictureProfileBase
    {
        public int[] SrcSetWidths { get; set; }
        public string[] SrcSetSizes { get; set; }
        public int DefaultWidth { get; set; }
        public int? Quality { get; set; }
        public bool IncludeWebp { get; set; }
        /// <summary>
        /// The wanted aspect ratio of the image (width/height).
        /// Example: An image with aspect ratio 16:9 = 16/9 = 1.777.
        /// </summary>
        public double AspectRatio { get; set; }

        protected PictureProfileBase()
        {
            Quality = 80;
            IncludeWebp = true;
        }
    }
}
 