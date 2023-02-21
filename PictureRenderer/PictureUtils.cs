﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using PictureRenderer.Profiles;
using PictureRenderer.UrlBuilders;

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

            return string.Empty;
            //var queryItems = HttpUtility.ParseQueryString(uri.Query);

            //if (!string.IsNullOrEmpty(wantedFormat))
            //{
            //    queryItems.Remove("format"); //remove if it already exists
            //    queryItems.Add("format", wantedFormat);
            //}

            //queryItems.Add("width", imageWidth.ToString());

            //queryItems = AddHeightQuery(imageWidth, queryItems, profile);

            //queryItems = AddFocalPointQuery(focalPoint, queryItems);

            //// "quality" have to be after "format".
            //queryItems = AddQualityQuery(queryItems, profile);

            //var domain = string.Empty;
            //if (!uri.Host.Contains("dummy-xyz.com"))
            //{
            //    //keep the original image url domain.
            //    domain = uri.GetLeftPart(UriPartial.Authority);
            //}

            //return domain + uri.AbsolutePath + "?" + queryItems.ToString();
        }

        

        

        internal static (string x, string y) FocalPointAsString((double x, double y) focalPoint)
        {
            var x = Math.Round(focalPoint.x, 3).ToString(CultureInfo.InvariantCulture);
            var y = Math.Round(focalPoint.y, 3).ToString(CultureInfo.InvariantCulture);
            
            return (x,y);
        }

        //private static NameValueCollection AddHeightQuery(int imageWidth, NameValueCollection queryItems, PictureProfileBase profile)
        //{
        //    //Do nothing if height is already in the querystring.
        //    if (queryItems["height"] != null)
        //    {
        //        return queryItems;
        //    }

        //    ////Add height based on aspect ratio, or from FixedHeight.
        //    //if (profile.AspectRatio > 0)
        //    //{
        //    //    queryItems.Add("height", Convert.ToInt32(imageWidth / profile.AspectRatio).ToString()); 
        //    //} 
        //    //else if (profile.FixedHeight != null && profile.FixedHeight > 0)
        //    //{
        //    //    queryItems.Add("height", profile.FixedHeight.ToString());
        //    //}

        //    var height = GetImageHeight(imageWidth, profile);
        //    if (height > 0)
        //    {
        //        queryItems.Add("height", height.ToString());
        //    }

        //    return queryItems;
        //}

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
