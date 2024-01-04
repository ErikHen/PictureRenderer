using System.Collections.Generic;

namespace PictureRenderer
{
    public class MediaImagesPictureData : PictureData
    {
        public IEnumerable<MediaImagePaths> MediaImages { get; set; }
    }

    public class MediaImagePaths
    {
        public string ImagePath { get; set; }
        public string ImagePathWebp { get; set; }
        public string MediaCondition { get; set; }
    }
}
