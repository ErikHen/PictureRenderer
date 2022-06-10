using System.Collections.Generic;

namespace PictureRenderer
{
    internal class MediaImagesPictureData : PictureData
    {
        public IEnumerable<MediaImagePaths> MediaImages { get; set; }
    }

    internal class MediaImagePaths
    {
        public string ImagePath { get; set; }
        public string ImagePathWebp { get; set; }
        public string MediaCondition { get; set; }
    }
}
