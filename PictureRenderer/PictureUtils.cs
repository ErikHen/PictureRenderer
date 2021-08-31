using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using PictureRenderer.Profiles;

namespace PictureRenderer
{
    internal static class PictureUtils
    {
        public static PictureData GetPictureData(string imagePath, PictureProfileBase profile, string altText, (double x, double y) focalPoint)
        {
            if (!Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
            {
                imagePath = "http://dummy.com" + imagePath; //to be able to use the Uri object.
                if (!Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
                {
                    throw new Exception("Image url not well formatted.");
                }
            }
            
            var uri = new Uri(imagePath, UriKind.Absolute);
            var originalFormat = GetFormatFromExtension(uri.AbsolutePath);

            var pData = new PictureData
            {
                AltText = altText,
                ImgSrc = BuildQueryString(uri, profile, profile.DefaultWidth, string.Empty, focalPoint)
            };


            if (profile.SrcSetWidths != null)
            {
                pData.SrcSet = BuildSrcSet(uri, profile);
                pData.SizesAttribute = string.Join(", ", profile.SrcSetSizes);

                //Add webp versions.
                if (profile.IncludeWebp && originalFormat == "jpg")
                {
                    //TODO pData.SrcSetWebp = BuildSrcSet(uri, profile, "webp");
                }
            }

            return pData;
        }

        private static string BuildQueryString(Uri uri, PictureProfileBase profile, int imageWidth, string wantedFormat, (double x, double y) focalPoint = default)
        {
            var queryItems = HttpUtility.ParseQueryString(uri.Query);

            if (!string.IsNullOrEmpty(wantedFormat))
            {
                queryItems.Remove("format"); //remove if it already exists
                queryItems.Add("format", wantedFormat);
            }

            queryItems.Add("width", imageWidth.ToString());

            queryItems = AddHeightQuery(imageWidth, queryItems, profile);

            queryItems = AddFocalPointQuery(focalPoint, queryItems);
            
            // "quality" have to be after "format".
            queryItems = AddQualityQuery(queryItems, profile);

            return uri.AbsolutePath + "?" + queryItems.ToString(); //string.Join("&", queryItems.AllKeys.Select(a => a + "=" + queryItems[a])); //
        }
        private static NameValueCollection AddFocalPointQuery((double x, double y) focalPoint, NameValueCollection queryItems)
        {
            if ((focalPoint.x > 0 || focalPoint.y > 0)&& queryItems["rxy"] == null)
            {
                queryItems.Add("rxy", $"{focalPoint.x.ToString(CultureInfo.InvariantCulture)},{focalPoint.y.ToString(CultureInfo.InvariantCulture)}");
            }

            return queryItems;
        }

        private static NameValueCollection AddHeightQuery(int imageWidth, NameValueCollection queryItems, PictureProfileBase profile)
        {
            //Add height if aspect ratio is set, and height is not already in the querystring.
            if (profile.AspectRatio > 0 && queryItems["height"] == null)
            {
                queryItems.Add("height", Convert.ToInt32(imageWidth / profile.AspectRatio).ToString()); 
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

        private static string BuildSrcSet(Uri imageUrl, PictureProfileBase profile, string wantedFormat = "")
        {
            var srcSet = string.Empty;
            foreach (var width in profile.SrcSetWidths)
            {
                srcSet += BuildQueryString(imageUrl, profile, width, wantedFormat) + " " + width + "w, ";
            }
            srcSet = srcSet.TrimEnd(',', ' ');

            return srcSet;
        }

        private static string GetFormatFromExtension(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var format = extension?.TrimStart('.');
            if (format == "jpeg")
                format = "jpg";
            return format ?? string.Empty;
        }
    }
}
