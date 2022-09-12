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
        public static PictureData GetPictureData(string imagePath, PictureProfileBase profile, string altText, (double x, double y) focalPoint, string cssClass)
        {
            if (profile.SrcSetWidths == null || profile.Sizes == null)
            {
                throw new ArgumentException($"SrcSetWidths and/or Sizes are not defined in Picture profile.");
            }

            var uri = GetUriFromPath(imagePath);

            var pData = new PictureData
            {
                AltText = altText,
                ImgSrc = BuildQueryString(uri, profile, profile.FallbackWidth, string.Empty, focalPoint),
                CssClass = cssClass,
                SrcSet = BuildSrcSet(uri, profile, string.Empty, focalPoint),
                SizesAttribute = string.Join(", ", profile.Sizes)
            };

            if (ShouldCreateWebp(profile, uri))
            {
                pData.SrcSetWebp = BuildSrcSet(uri, profile, ImageFormat.Webp, focalPoint);
            }

            return pData;
        }

        public static MediaImagesPictureData GetMultiImagePictureData(string[] imagePaths, PictureProfileBase profile, string altText, (double x, double y)[] focalPoints, string cssClass)
        {
            if (profile.MultiImageMediaConditions == null || !profile.MultiImageMediaConditions.Any())
            {
                throw new ArgumentException($"MultiImageMediaConditions must be defined in Picture profile when rendering multiple images.");
            }

            if (focalPoints == null)
            {
                focalPoints = Array.Empty<(double x, double y)>();
            }
            Uri fallbackImageUri = default;
            (double x, double y) fallbackImageFocalPoint = default;
            var numberOfImages = imagePaths.Length;
            var numberOfFocalPoints = focalPoints.Length;
            var mediaImagePaths = new List<MediaImagePaths>();
            for (var i = 0; i < profile.MultiImageMediaConditions.Length; i++)
            {
                //If there isn't images for all media conditions, use last image in list.
                var imageIndex = i >= numberOfImages ? numberOfImages - 1 : i;
                var imagePath = imagePaths[imageIndex];
                //Get focal point for this image (if there is one)
                var imageFocalPoint = imageIndex >= numberOfFocalPoints ? default : focalPoints[imageIndex];

                var imageUri = GetUriFromPath(imagePath);
                mediaImagePaths.Add(new MediaImagePaths()
                {
                    ImagePath = BuildQueryString(imageUri, profile, profile.MultiImageMediaConditions[i].Width, null, imageFocalPoint),
                    ImagePathWebp = ShouldCreateWebp(profile, imageUri) ? BuildQueryString(imageUri, profile, profile.MultiImageMediaConditions[i].Width, ImageFormat.Webp, imageFocalPoint) : string.Empty,
                    MediaCondition = profile.MultiImageMediaConditions[i].Media
                });

                if (i == 0)
                {
                    //use first image as fallback image
                    fallbackImageUri = imageUri;
                    fallbackImageFocalPoint = imageFocalPoint;
                }
            }

            var pData = new MediaImagesPictureData
            {
                MediaImages = mediaImagePaths,
                AltText = altText,
                ImgSrc = BuildQueryString(fallbackImageUri, profile, profile.FallbackWidth, string.Empty, fallbackImageFocalPoint),
                CssClass = cssClass
            };

            return pData;
        }


        private static string BuildQueryString(Uri uri, PictureProfileBase profile, int imageWidth, string wantedFormat, (double x, double y) focalPoint)
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

            var domain = string.Empty;
            if (!uri.Host.Contains("dummy-xyz.com"))
            {
                //keep the original image url domain.
                domain = uri.GetLeftPart(UriPartial.Authority);
            }

            return domain + uri.AbsolutePath + "?" + queryItems.ToString();
        }

        private static NameValueCollection AddFocalPointQuery((double x, double y) focalPoint, NameValueCollection queryItems)
        {
            if ((focalPoint.x > 0 || focalPoint.y > 0) && queryItems["rxy"] == null)
            {
                var x = Math.Round(focalPoint.x, 3).ToString(CultureInfo.InvariantCulture);
                var y = Math.Round(focalPoint.y, 3).ToString(CultureInfo.InvariantCulture);
                queryItems.Add("rxy", $"{x},{y}");
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

        private static string BuildSrcSet(Uri imageUrl, PictureProfileBase profile, string wantedFormat, (double x, double y) focalPoint)
        {
            var srcSetBuilder = new StringBuilder();
            foreach (var width in profile.SrcSetWidths)
            {
                srcSetBuilder.Append(BuildQueryString(imageUrl, profile, width, wantedFormat, focalPoint) + " " + width + "w, ");
            }

            return srcSetBuilder.ToString().TrimEnd(',', ' ');
        }

        private static Uri GetUriFromPath(string imagePath)
        {
            if (!Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
            {
                imagePath = "https://dummy-xyz.com" + imagePath; //to be able to use the Uri object.
                if (!Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
                {
                    throw new ArgumentException($"Image url '{imagePath}' is not well formatted.");
                }
            }

            return new Uri(imagePath, UriKind.Absolute);
        }

        private static bool ShouldCreateWebp(PictureProfileBase profile, Uri imageUri)
        {
            var originalFormat = GetFormatFromExtension(imageUri.AbsolutePath);
            return profile.CreateWebpForFormat != null && profile.CreateWebpForFormat.Contains(originalFormat);
        }

        private static string GetFormatFromExtension(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var format = extension?.TrimStart('.');
            if (format == "jpeg")
                format = ImageFormat.Jpeg;
            return format ?? string.Empty;
        }
    }
}
