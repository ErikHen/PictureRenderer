using System;
using System.Collections.Generic;
using System.Text;

namespace PictureRenderer
{
	public enum LazyLoading
    {
        None,
        Browser,
        //Progressive,
    }

    public static class ImageFormat
    {
        public static string Webp => "webp";
    }

    public enum ImageDecoding
    {
        Async,
        Sync,
        Auto,
        None
    }
}
