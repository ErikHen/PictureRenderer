namespace PictureRenderer
{
    public class PictureData
    {
        public string SrcSet { get; set; }
        public string SrcSetWebp { get; set; }
        //public string SrcSetLowQuality { get; set; }
        //public string SrcSetLowQualityWebp { get; set; }
        public string SizesAttribute { get; set; }
        public string ImgSrc { get; set; }
        //public string ImgSrcLowQuality { get; set; }
        public string AltText { get; set; }
        public string CssClass { get; set; }
        /// <summary>
        /// Id used when rendering picture info.
        /// </summary>
        public string UniqueId { get; set; }
    }
}
