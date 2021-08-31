using System;
using System.Collections.Generic;
using System.Text;
using PictureRenderer.Profiles;

namespace PictureRenderer
{
    public static class Picture
    {
        public static string Render(string imagePath, PictureProfileBase profile, LazyLoading lazyLoading)
        {
            return Render(imagePath, profile, string.Empty, lazyLoading);
        }

        public static string Render(string imagePath, PictureProfileBase profile, (double x, double y) focalPoint)
        {
            return Render(imagePath, profile, string.Empty, LazyLoading.Native, focalPoint);
        }

        public static string Render(string imagePath, PictureProfileBase profile, string altText, (double x, double y) focalPoint)
        {
            return Render(imagePath, profile, altText, LazyLoading.Native, focalPoint);
        }

        public static string Render(string imagePath, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Native, (double x, double y) focalPoint = default)
        {
            //TODO: lazy loading
            var pictureData = PictureUtils.GetPictureData(imagePath, profile, altText, focalPoint);
            var imgElement = RenderImgElement(pictureData, profile);
            var sourceElement = RenderSourceElement(pictureData);

            var sourceElementWebp = string.Empty;
            if (!string.IsNullOrEmpty(pictureData.SrcSetWebp))
            {
                sourceElementWebp = RenderSourceElement(pictureData, ImageFormat.Webp);
            }

            //Webp source element must be rendered first. Browser selects the first version it supports.
            return $"<picture>{sourceElementWebp}{sourceElement}{imgElement}</picture>";
        }

        private static string RenderImgElement(PictureData pictureData, PictureProfileBase profile)
        {
            return $"<img alt=\"{pictureData.AltText}\" src=\"{pictureData.ImgSrc}\" />";
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

            return $"<source {srcSetAttribute} {sizesAttribute} {formatAttribute} />";
        }
    }
}
