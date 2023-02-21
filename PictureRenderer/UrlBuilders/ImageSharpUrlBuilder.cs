using PictureRenderer.Profiles;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace PictureRenderer.UrlBuilders
{
    internal static class ImageSharpUrlBuilder
    {
        internal static string BuildImageUrl(Uri uri, ImageSharpProfile profile, int imageWidth, string wantedFormat, (double x, double y) focalPoint)
        {
            var queryItems = HttpUtility.ParseQueryString(uri.Query);

            if (!string.IsNullOrEmpty(wantedFormat))
            {
                queryItems.Remove("format"); //remove if it already exists
                queryItems.Add("format", wantedFormat);
            }

            queryItems.Add("width", imageWidth.ToString());

            if (queryItems["height"] == null) //add height if it's not already in the querystring.
            {
                var height = PictureUtils.GetImageHeight(imageWidth, profile);
                if (height > 0)
                {
                    queryItems.Add("height", height.ToString());
                }
            }

            queryItems = AddFocalPointQuery(focalPoint, queryItems);

            queryItems = AddQualityQuery(queryItems, profile);

            return PictureUtils.GetImageDomain(uri) + uri.AbsolutePath + "?" + queryItems.ToString();
        }

        private static NameValueCollection AddFocalPointQuery((double x, double y) focalPoint, NameValueCollection queryItems)
        {
            if ((focalPoint.x > 0 || focalPoint.y > 0) && queryItems["rxy"] == null)
            {
                var (x, y) = PictureUtils.FocalPointAsString(focalPoint);
                queryItems.Add("rxy", $"{x},{y}");
            }

            return queryItems;
        }

        private static NameValueCollection AddQualityQuery(NameValueCollection queryItems, PictureProfileBase profile)
        {
            //TODO: Ignore quality for png etc
            if (queryItems["quality"] == null)
            {
                if (profile.Quality != null)
                {
                    //Add quality value from profile.
                    queryItems.Add("quality", profile.Quality.ToString());
                }
            }
            else
            {
                //Quality value already exists in querystring. Don't change it, but make sure it's last (after format).
                var quality = queryItems["quality"];
                queryItems.Remove("quality");
                queryItems.Add("quality", quality);
            }

            return queryItems;
        }
    }
}
