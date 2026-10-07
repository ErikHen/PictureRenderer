using PictureRenderer.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PictureRenderer
{
    public static class Picture
    {

        /// <summary>
        /// Render picture element.
        /// </summary>
        public static string Render(string imagePath, PictureProfileBase profile, (double x, double y) focalPoint)
        {
            return Render(imagePath, profile, new PictureAttributes(), focalPoint);
        }

        /// <summary>
        /// Render picture element.
        /// </summary>
        /// <param name="focalPoint">Value range: 0-1 for ImageSharp, 1-[image width/height] for Storyblok.</param>
        public static string Render(string imagePath, PictureProfileBase profile, PictureAttributes attributes = null, (double x, double y) focalPoint = default)
        {
            if (attributes == null)
            {
                attributes = new PictureAttributes();
            }

            var pictureData = PictureUtils.GetPictureData(imagePath, profile, attributes.ImgAlt, focalPoint, attributes.ImgClass);

            var sourceElement = RenderSourceElement(pictureData);

            var sourceElementWebp = string.Empty;
            if (!string.IsNullOrEmpty(pictureData.SrcSetWebp))
            {
                sourceElementWebp = RenderSourceElement(pictureData, ImageFormat.Webp);
            }

            var imgElement = RenderImgElement(pictureData, profile, attributes);
            var pictureElement = $"<picture>{sourceElementWebp}{sourceElement}{imgElement}</picture>"; //Webp source element must be rendered first. Browser selects the first version it supports.
            var infoElements = RenderInfoElements(profile, pictureData);

            return $"{pictureElement}{infoElements}";
        }

        

        private static string RenderImgElement(PictureData pictureData, PictureProfileBase profile, PictureAttributes attributes)
        {
            var idAttribute = string.IsNullOrEmpty(pictureData.UniqueId) ? string.Empty : $" id=\"{pictureData.UniqueId}\"";
            var widthAndHeightAttributes = GetImgWidthAndHeightAttributes(profile, attributes);
            var loadingAttribute = attributes.LazyLoading == LazyLoading.Browser ? "loading=\"lazy\" " : string.Empty;
            var classAttribute = string.IsNullOrEmpty(pictureData.CssClass) ? string.Empty : $"class=\"{HttpUtility.HtmlEncode(pictureData.CssClass)}\"";
            var decodingAttribute = attributes.ImgDecoding == ImageDecoding.None ? string.Empty : $"decoding=\"{Enum.GetName(typeof(ImageDecoding), attributes.ImgDecoding)?.ToLower()}\" ";
            var fetchPriorityAttribute = attributes.ImgFetchPriority == FetchPriority.None ? string.Empty : $"fetchPriority=\"{Enum.GetName(typeof(FetchPriority), attributes.ImgFetchPriority)?.ToLower()}\" ";
            var additionalAttributes = GetAdditionalAttributes(attributes.ImgAdditionalAttributes);

            return $"<img{idAttribute} alt=\"{HttpUtility.HtmlEncode(pictureData.AltText)}\" src=\"{pictureData.ImgSrc}\" {widthAndHeightAttributes}{loadingAttribute}{decodingAttribute}{fetchPriorityAttribute}{classAttribute}{additionalAttributes}/>";
        }

        private static string GetAdditionalAttributes(Dictionary<string, string> additionalAttributes)
        {
            var additionalAttributesBuilder = new StringBuilder();
            foreach (var key in additionalAttributes.Keys)
            {
                if (key == "width" || key == "height" || key == "loading" || key == "class" || key == "decoding" || key == "fetchPriority")
                {
                    //these attributes are handled separately, so ignore them if added to additionalAttributes.
                    continue;
                }
                additionalAttributesBuilder.Append($"{key}=\"{additionalAttributes[key]}\" ");
            }

            return additionalAttributesBuilder.ToString();
        }

        private static string GetImgWidthAndHeightAttributes(PictureProfileBase profile, PictureAttributes attributes)
        {
            if (attributes.ImgAdditionalAttributes.TryGetValue("width", out var width))
            {
                return $"width=\"{width}\" ";
            }

            if (attributes.RenderImgWidthHeight)
            {
                var maxWidth = profile.SrcSetWidths.Max();
                var widthAttribute = $"width=\"{maxWidth}\" ";
                var heightAttribute = "";
                if (profile.AspectRatio > 0)
                {
                    heightAttribute = $"height=\"{Math.Round(maxWidth / profile.AspectRatio)}\" ";
                }
                else if (profile.FixedHeight != null && profile.FixedHeight > 0)
                {
                    heightAttribute = $"height=\"{profile.FixedHeight}\" ";

                }
                return widthAttribute + heightAttribute;
            }

            return string.Empty;
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

        private static string RenderInfoElements(PictureProfileBase pictureProfile,  PictureData pictureData)
        {
            if (!pictureProfile.ShowInfo)
            {
                return string.Empty;
            }
            
            var formatFunction = $"function format{pictureData.UniqueId}(input) {{ return input.split('/').pop().replace('?', '\\n').replaceAll('&', ', ').replace('%2c', ',').replace('rxy', 'focal point'); }}";
            if (pictureProfile is StoryblokProfile)
            {
                formatFunction = $"function format{pictureData.UniqueId}(input) {{ return input.split('/m/').pop().replaceAll('/', ', '); }}";
            }
            if (pictureProfile is CloudflareProfile)
            {
                formatFunction = $"function format{pictureData.UniqueId}(input) {{ return input.split('/cdn-cgi/image/').pop().replace('/http', ', http'); }}";
            }
            var infoDiv = $"<div id=\"pinfo{pictureData.UniqueId}\" style=\"position: absolute; margin-top:-60px; padding:0 5px 2px 5px; font-size:0.8rem; text-align:left; background-color:rgba(255, 255, 255, 0.8);\"></div>";
            var script =$"<script type=\"text/javascript\"> window.addEventListener(\"load\",function () {{ const pictureInfo = document.getElementById('pinfo{pictureData.UniqueId}'); var image = document.getElementById('{pictureData.UniqueId}'); pictureInfo.innerText = format{pictureData.UniqueId}(image.currentSrc); image.onload = function () {{ pictureInfo.innerText = format{pictureData.UniqueId}(image.currentSrc); }}; {formatFunction} }}, false);</script>";
            return "\n" + infoDiv + "\n" + script;
        }
    }
}
