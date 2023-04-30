using PictureRenderer.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PictureRenderer.UrlBuilders
{
    internal static class CloudflareUrlBuilder
    {
        internal static string BuildCloudflareUrl(Uri uri, CloudflareProfile profile, int imageWidth, (double x, double y) focalPoint)
        {
            var imageOptions = new Dictionary<string, string>
            {
                { "width", imageWidth.ToString() },
                { "format", "auto" },
                { "fit", "crop"}
            };

            var height = PictureUtils.GetImageHeight(imageWidth, profile);
            if (height > 0)
            {
                imageOptions.Add("height", height.ToString());
            }

            if (profile.Quality != null) 
            {
                imageOptions.Add("quality", profile.Quality.ToString());
            }

            if (focalPoint.x > 0 || focalPoint.y > 0)
            {
                var (x, y) = PictureUtils.FocalPointAsString(focalPoint);
                imageOptions.Add("gravity", $"{x}x{y}");
            }

            var imageDomain = PictureUtils.GetImageDomain(uri);
            if (!string.IsNullOrEmpty(imageDomain))
            {
                imageDomain = "/" + imageDomain;
            }
            return "/cdn-cgi/image/" + string.Join(",", imageOptions.Select(o => $"{o.Key}={o.Value}")) + imageDomain + uri.AbsolutePath;
        }
    }
}
