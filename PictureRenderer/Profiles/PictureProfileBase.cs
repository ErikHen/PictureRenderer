namespace PictureRenderer.Profiles
{
    public abstract class PictureProfileBase
    {
        public int[] SrcSetWidths { get; set; }
        public string[] SrcSetSizes { get; set; }
        public int DefaultWidth { get; set; }
        public int? Quality { get; set; }
        public bool IncludeWebp { get; set; }

        protected PictureProfileBase()
        {
            Quality = 80;
            IncludeWebp = true;
        }
    }
}
 