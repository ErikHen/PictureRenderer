using PictureRenderer.Profiles;
using PictureRenderer.UrlBuilders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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
                ImgSrc = BuildImageUrl(uri, profile, profile.FallbackWidth, string.Empty, focalPoint),
                CssClass = cssClass,
                SrcSet = BuildSrcSet(uri, profile, string.Empty, focalPoint),
                SizesAttribute = string.Join(", ", profile.Sizes),
                UniqueId = profile.ShowInfo ? Guid.NewGuid().ToString("n").Substring(0, 10) : string.Empty
            };

            if (ShouldRenderWebp(profile, uri))
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
                    ImagePath = BuildImageUrl(imageUri, profile, profile.MultiImageMediaConditions[i].Width, null, imageFocalPoint),
                    ImagePathWebp = ShouldRenderWebp(profile, imageUri) ? BuildImageUrl(imageUri, profile, profile.MultiImageMediaConditions[i].Width, ImageFormat.Webp, imageFocalPoint) : string.Empty,
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
                ImgSrc = BuildImageUrl(fallbackImageUri, profile, profile.FallbackWidth, string.Empty, fallbackImageFocalPoint),
                CssClass = cssClass,
                UniqueId = profile.ShowInfo ? Guid.NewGuid().ToString("n").Substring(0, 10) : string.Empty
            };

            return pData;
        }

        private static string BuildImageUrl(Uri uri, PictureProfileBase profile, int imageWidth, string wantedFormat, (double x, double y) focalPoint)
        {
            if (profile is ImageSharpProfile imageSharpProfile)
            {
                return ImageSharpUrlBuilder.BuildImageUrl(uri, imageSharpProfile, imageWidth, wantedFormat, focalPoint);
            }
            
            if (profile is StoryblokProfile storyblokProfile)
            {
                return StoryblokUrlBuilder.BuildStoryblokUrl(uri, storyblokProfile, imageWidth, focalPoint);
            }

            if (profile is CloudflareProfile cloudflareProfile)
            {
                return CloudflareUrlBuilder.BuildCloudflareUrl(uri, cloudflareProfile, imageWidth, focalPoint);
            }
            
            return string.Empty;
        }

        internal static (string x, string y) FocalPointAsString((double x, double y) focalPoint)
        {
            var x = Math.Round(focalPoint.x, 3).ToString(CultureInfo.InvariantCulture);
            var y = Math.Round(focalPoint.y, 3).ToString(CultureInfo.InvariantCulture);
            
            return (x,y);
        }

        internal static int GetImageHeight(int imageWidth, PictureProfileBase profile)
        {
            //Add height based on aspect ratio, or from FixedHeight.
            if (profile.AspectRatio > 0)
            {
                return Convert.ToInt32(imageWidth / profile.AspectRatio);
            }
            else if (profile.FixedHeight != null && profile.FixedHeight > 0)
            {
                return profile.FixedHeight.Value;
            }

            return 0;
        }

        private static string BuildSrcSet(Uri imageUrl, PictureProfileBase profile, string wantedFormat, (double x, double y) focalPoint)
        {
            var srcSetBuilder = new StringBuilder();
            foreach (var width in profile.SrcSetWidths)
            {
                srcSetBuilder.Append(BuildImageUrl(imageUrl, profile, width, wantedFormat, focalPoint) + " " + width + "w, ");
            }

            return srcSetBuilder.ToString().TrimEnd(',', ' ');
        }

        private static Uri GetUriFromPath(string imagePath)
        {
            if (!IsValidHttpUri(imagePath, out var uri))
            {
                //A Uri object must have a domain, but imagePath might be just a path. Add dummy domain, and test again.
                imagePath = "https://dummy-xyz.com" + imagePath; 
                if (!IsValidHttpUri(imagePath, out uri))
                {
                    throw new ArgumentException($"Image url '{imagePath}' is not well formatted.");
                }
            }

            return uri;
        }

        private static bool IsValidHttpUri(string uriString, out Uri uri) {
            return Uri.TryCreate(uriString, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        internal static string GetImageDomain(Uri uri)
        {
            var domain = string.Empty;
            if (!uri.Host.Contains("dummy-xyz.com"))
            {
                //return the original image url domain.
                domain = uri.GetLeftPart(UriPartial.Authority);
            }
            return domain;
        }

        private static bool ShouldRenderWebp(PictureProfileBase profile, Uri imageUri)
        {
            if (profile is ImageSharpProfile imageSharpProfile)
            {
                var originalFormat = GetFormatFromExtension(imageUri.AbsolutePath);
                return imageSharpProfile.CreateWebpForFormat != null && imageSharpProfile.CreateWebpForFormat.Contains(originalFormat);
            }

            return false;
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
