using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using PictureRenderer.Profiles;

namespace PictureRenderer
{
    public static class Picture
    {
        public static string Render(string imagePath, PictureProfileBase profile, LazyLoading lazyLoading)
        {
            return Render(imagePath, profile, string.Empty, lazyLoading);
        }

        public static string Render(string[] imagePaths, PictureProfileBase profile, LazyLoading lazyLoading)
        {
            return Render(imagePaths, profile, string.Empty, lazyLoading);
        }

        public static string Render(string imagePath, PictureProfileBase profile, (double x, double y) focalPoint)
        {
            return Render(imagePath, profile, string.Empty, LazyLoading.Browser, focalPoint);
        }

        public static string Render(string[] imagePaths, PictureProfileBase profile, (double x, double y)[] focalPoints)
        {
            return Render(imagePaths, profile, string.Empty, LazyLoading.Browser, focalPoints);
        }

        public static string Render(string imagePath, PictureProfileBase profile, string altText, (double x, double y) focalPoints)
        {
            return Render(imagePath, profile, altText, LazyLoading.Browser, focalPoints);
        }

        public static string Render(string[] imagePaths, PictureProfileBase profile, string altText, (double x, double y)[] focalPoints)
        {
            return Render(imagePaths, profile, altText, LazyLoading.Browser, focalPoints);
        }

        public static string Render(string imagePath, PictureProfileBase profile, string altText, string cssClass)
        {
            return Render(imagePath, profile, altText, LazyLoading.Browser, cssClass: cssClass);
        }

        public static string Render(string[] imagePaths, PictureProfileBase profile, string altText, string cssClass)
        {
            return Render(imagePaths, profile, altText, LazyLoading.Browser, focalPoints: default, cssClass: cssClass);
        }

        public static string Render(string imagePath, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, (double x, double y) focalPoint = default, string cssClass = "", string imgWidth = "")
        {
            var pictureData = PictureUtils.GetPictureData(imagePath, profile, altText, focalPoint, cssClass);
           
            var sourceElement = RenderSourceElement(pictureData);

            var sourceElementWebp = string.Empty;
            if (!string.IsNullOrEmpty(pictureData.SrcSetWebp))
            {
                sourceElementWebp = RenderSourceElement(pictureData, ImageFormat.Webp);
            }

            var imgElement = RenderImgElement(pictureData, profile, lazyLoading, imgWidth);
            
            //Webp source element must be rendered first. Browser selects the first version it supports.
            return $"<picture>{sourceElementWebp}{sourceElement}{imgElement}</picture>";
        }

        /// <summary>
        /// Render different images in the same picture element.
        /// </summary>
        public static string Render(string[] imagePaths, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, (double x, double y)[] focalPoints = null, string cssClass = "")
        {
            var pictureData = PictureUtils.GetMultiImagePictureData(imagePaths, profile, altText, focalPoints, cssClass);
            var sourceElements = RenderSourceElementsForMultiImage(pictureData);
            var imgElement = RenderImgElement(pictureData, profile, lazyLoading);

            return $"<picture>{sourceElements}{imgElement}</picture>";
        }

        private static string RenderImgElement(PictureData pictureData, PictureProfileBase profile, LazyLoading lazyLoading, string imgWidth = "")
        {
            var widthAndHeightAttributes = GetImgWidthAndHeightAttributes(profile, imgWidth);
            var loadingAttribute = lazyLoading == LazyLoading.Browser ? "loading=\"lazy\" " : string.Empty;
            var classAttribute = string.IsNullOrEmpty(pictureData.CssClass) ? string.Empty : $"class=\"{HttpUtility.HtmlEncode(pictureData.CssClass)}\"";
            var decodingAttribute = profile.ImageDecoding == ImageDecoding.None ? string.Empty :  $"decoding=\"{Enum.GetName(typeof(ImageDecoding), profile.ImageDecoding)?.ToLower()}\" ";

            return $"<img alt=\"{HttpUtility.HtmlEncode(pictureData.AltText)}\" src=\"{pictureData.ImgSrc}\" {widthAndHeightAttributes}{loadingAttribute}{decodingAttribute}{classAttribute}/>";
        }

        private static string GetImgWidthAndHeightAttributes(PictureProfileBase profile, string imgWidth)
        {
            if (!string.IsNullOrEmpty(imgWidth))
            {
                return $"width=\"{imgWidth}\" ";
            }

            return profile.ImgWidthHeight ? $"width=\"{profile.FallbackWidth}\" height=\"{Math.Round(profile.FallbackWidth / profile.AspectRatio)}\" " : string.Empty;
        }

        private static string RenderSourceElement(PictureData pictureData, string format = "")
        {
            var srcSet = pictureData.SrcSet;
            var formatAttribute = string.Empty;
            if (format == ImageFormat.Webp)
            {
                srcSet = pictureData.SrcSetWebp;
                formatAttribute = "type=\"image/" + format + "\"";
            }
            var srcSetAttribute = $"srcset=\"{srcSet}\"";
            var sizesAttribute = $"sizes=\"{pictureData.SizesAttribute}\"";

            return $"<source {srcSetAttribute} {sizesAttribute} {formatAttribute}/>";
        }

        private static string RenderSourceElementsForMultiImage(MediaImagesPictureData pictureData)
        {
            var sourceElementsBuilder = new StringBuilder();
            foreach (var mediaImage in pictureData.MediaImages)
            {
                var mediaAttribute = $"media=\"{mediaImage.MediaCondition}\"";

                //add webp source element first
                if (!string.IsNullOrEmpty(mediaImage.ImagePathWebp))
                {
                    var srcSetWebpAttribute = $"srcset=\"{mediaImage.ImagePathWebp}\"";
                    var formatAttribute = "type=\"image/webp\"";
                    sourceElementsBuilder.Append($"<source {mediaAttribute} {srcSetWebpAttribute} {formatAttribute}/>");
                }

                var srcSetAttribute = $"srcset=\"{mediaImage.ImagePath}\"";
                sourceElementsBuilder.Append($"<source {mediaAttribute} {srcSetAttribute}/>");
            }

            return sourceElementsBuilder.ToString();
        }
    }
}
