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

    internal static class ImageFormat
    {
        public static string Webp => "webp";
        public static string Jpeg => "jpg";
        public static string Png => "png";
    }

    public enum ImageDecoding
    {
        Async,
        Sync,
        Auto,
        None
    }
}
