using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using PictureRenderer.Profiles;

namespace PictureRenderer
{
    internal static class PictureUtils
    {
        public static PictureData GetPictureData(string imagePath, PictureProfileBase profile, string altText)
        {
            if (!Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
            {
                imagePath = "http://dummy.com" + imagePath;
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
                ImgSrc = BuildQueryString(uri, profile, profile.DefaultWidth, originalFormat)
            };


            if (profile.SrcSetWidths != null)
            {
                pData.SrcSet = BuildSrcSet(uri, profile, originalFormat);
                pData.SizesAttribute = string.Join(", ", profile.SrcSetSizes);

                //Add webp versions.
                if (profile.IncludeWebp && originalFormat == "jpg")
                {
                    pData.SrcSetWebp = BuildSrcSet(uri, profile, originalFormat, "webp");
                }
            }

            return pData;
        }

        private static string BuildQueryString(Uri uri, PictureProfileBase profile, int? imageWidth, string originalFormat, string wantedFormat = "")
        {
            var queryItems = HttpUtility.ParseQueryString(uri.Query);

            if (!string.IsNullOrEmpty(wantedFormat))
            {
                queryItems.Remove("format"); //remove if it already exists
                queryItems.Add("format", wantedFormat);
            }

            queryItems.Add("width", imageWidth.ToString());

            //if (imageType.HeightRatio > 0)
            //{
            //    if (!currentQueryKeys.Contains("mode")) //don't change mode value if it already exists
            //    {
            //        qc.Add("mode", "crop");
            //    }
            //    qc.Add("heightratio", imageType.HeightRatio.ToString(CultureInfo.InvariantCulture));
            //}

            // "quality" have to be after "format".
            queryItems = AddQualityQuery(queryItems, profile);

            return uri.AbsolutePath + "?" + queryItems.ToString();
        }

        private static NameValueCollection AddQualityQuery(NameValueCollection queryItems, PictureProfileBase profile)
        {
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

        private static string BuildSrcSet(Uri imageUrl, PictureProfileBase profile, string originalFormat, string wantedFormat = "")
        {
            var srcset = string.Empty;
            foreach (var width in profile.SrcSetWidths)
            {
                srcset += BuildQueryString(imageUrl, profile, width, originalFormat, wantedFormat) + " " + width + "w, ";
            }
            srcset = srcset.TrimEnd(',', ' ');

            return srcset;
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
