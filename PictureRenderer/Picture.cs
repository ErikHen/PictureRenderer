using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Render different images in the same picture element.
        /// </summary>
        public static string Render(string[] imagePaths, PictureProfileBase profile, LazyLoading lazyLoading)
        {
            return Render(imagePaths, profile, string.Empty, lazyLoading);
        }

        public static string Render(string imagePath, PictureProfileBase profile, (double x, double y) focalPoint)
        {
            return Render(imagePath, profile, string.Empty, LazyLoading.Browser, focalPoint);
        }

        /// <summary>
        /// Render different images in the same picture element.
        /// </summary>
        public static string Render(string[] imagePaths, PictureProfileBase profile, (double x, double y)[] focalPoints)
        {
            return Render(imagePaths, profile, string.Empty, LazyLoading.Browser, focalPoints);
        }

        public static string Render(string imagePath, PictureProfileBase profile, string altText, (double x, double y) focalPoints)
        {
            return Render(imagePath, profile, altText, LazyLoading.Browser, focalPoints);
        }

        /// <summary>
        /// Render different images in the same picture element.
        /// </summary>
        public static string Render(string[] imagePaths, PictureProfileBase profile, string altText, (double x, double y)[] focalPoints)
        {
            return Render(imagePaths, profile, altText, LazyLoading.Browser, focalPoints);
        }

        public static string Render(string imagePath, PictureProfileBase profile, string altText, string cssClass)
        {
            return Render(imagePath, profile, altText, LazyLoading.Browser, cssClass: cssClass);
        }

        /// <summary>
        /// Render different images in the same picture element.
        /// </summary>
        public static string Render(string[] imagePaths, PictureProfileBase profile, string altText, string cssClass)
        {
            return Render(imagePaths, profile, altText, LazyLoading.Browser, focalPoints: default, cssClass: cssClass);
        }

        /// <summary>
        /// Render picture element.
        /// </summary>
        /// <param name="focalPoint">Value range: 0-1 for ImageSharp, 1-[image width/height] for Storyblok.</param>
        /// <returns></returns>
        public static string Render(string imagePath, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, (double x, double y) focalPoint = default, string cssClass = "", string imgWidth = "", string style = "", string wrapClass = "")
        {
            var pictureData = PictureUtils.GetPictureData(imagePath, profile, altText, focalPoint, cssClass);
           
            var sourceElement = RenderSourceElement(pictureData, usePlaceHolders: profile.UsePlaceholders);

            var sourceElementWebp = string.Empty;
            if (!string.IsNullOrEmpty(pictureData.SrcSetWebp))
            {
                sourceElementWebp = RenderSourceElement(pictureData, ImageFormat.Webp);
            }
            
            var imgElement = RenderImgElement(pictureData, profile, lazyLoading, imgWidth, style);
            var pictureClassAttribute = profile.UseFigureWrap ? "" : string.IsNullOrEmpty(wrapClass) ? "" : $" class=\"{HttpUtility.HtmlEncode(wrapClass)}\"";
            var figureClassAttribute = profile.UseFigureWrap && !string.IsNullOrEmpty(wrapClass) ? $" class=\"{HttpUtility.HtmlEncode(wrapClass)}\"" : "";
            var pictureElement = $"<picture{pictureClassAttribute}>{sourceElementWebp}{sourceElement}{imgElement}</picture>"; //Webp source element must be rendered first. Browser selects the first version it supports.

            var infoElements = RenderInfoElements(profile, pictureData);

            if (profile.UseFigureWrap)
            {
                var figcaptionElement = profile.IncludeFigcaption ? $"<figcaption>{HttpUtility.HtmlEncode(altText)}</figcaption>" : "";
                return $"<figure{figureClassAttribute}>{pictureElement}{figcaptionElement}</figure>{infoElements}";
            }
            else
            {
                return $"{pictureElement}{infoElements}";
            }
        }

        /// <summary>
        /// Render different images in the same picture element.
        /// </summary>
        public static string Render(string[] imagePaths, PictureProfileBase profile, string altText = "", LazyLoading lazyLoading = LazyLoading.Browser, (double x, double y)[] focalPoints = null, string cssClass = "", string wrapClass = "")
        {
            var pictureData = PictureUtils.GetMultiImagePictureData(imagePaths, profile, altText, focalPoints, cssClass);
            var sourceElements = RenderSourceElementsForMultiImage(pictureData);
            var imgElement = RenderImgElement(pictureData, profile, lazyLoading, string.Empty, string.Empty);
            var pictureClassAttribute = profile.UseFigureWrap ? "" : string.IsNullOrEmpty(wrapClass) ? "" : $" class=\"{HttpUtility.HtmlEncode(wrapClass)}\"";
            var figureClassAttribute = profile.UseFigureWrap && !string.IsNullOrEmpty(wrapClass) ? $" class=\"{HttpUtility.HtmlEncode(wrapClass)}\"" : "";
            var pictureElement = $"<picture{pictureClassAttribute}>{sourceElements}{imgElement}</picture>";


            var infoElements = RenderInfoElements(profile, pictureData);

            if (profile.UseFigureWrap)
            {
                var figcaptionElement = profile.IncludeFigcaption ? $"<figcaption>{HttpUtility.HtmlEncode(altText)}</figcaption>" : "";
                return $"<figure{figureClassAttribute}>{pictureElement}{figcaptionElement}</figure>{infoElements}";
            }
            else
            {
                return $"{pictureElement}{infoElements}";
            }
        }

        private static string RenderImgElement(PictureData pictureData, PictureProfileBase profile, LazyLoading lazyLoading, string imgWidth, string style)
        {
            var imgSrc = profile.UsePlaceholders ? "https://source.unsplash.com/random/?happy" : pictureData.ImgSrc;
            var idAttribute = string.IsNullOrEmpty(pictureData.UniqueId) ? string.Empty : $" id=\"{pictureData.UniqueId}\"";
            var widthAndHeightAttributes = GetImgWidthAndHeightAttributes(profile, imgWidth);
            var loadingAttribute = lazyLoading == LazyLoading.Browser ? "loading=\"lazy\" " : string.Empty;
            var cssClasses = string.Join(" ", new[] { pictureData.DefaultMediaClass, pictureData.CssClass }.Where(s => !string.IsNullOrEmpty(s)));
            var classAttribute = string.IsNullOrEmpty(cssClasses) ? string.Empty : $"class=\"{cssClasses}\"";
            var decodingAttribute = profile.ImageDecoding == ImageDecoding.None ? string.Empty :  $"decoding=\"{Enum.GetName(typeof(ImageDecoding), profile.ImageDecoding)?.ToLower()}\" ";
            var fetchPriorityAttribute = profile.FetchPriority == FetchPriority.None ? string.Empty :  $"fetchPriority=\"{Enum.GetName(typeof(FetchPriority), profile.FetchPriority)?.ToLower()}\" ";
            var styleAttribute = string.IsNullOrEmpty(style) ? string.Empty : $"style=\"{style}\" ";

            return $"<img{idAttribute} alt=\"{HttpUtility.HtmlEncode(pictureData.AltText)}\" src=\"{imgSrc}\" {widthAndHeightAttributes}{loadingAttribute}{decodingAttribute}{fetchPriorityAttribute}{classAttribute}{styleAttribute}/>";
        }

        private static string GetImgWidthAndHeightAttributes(PictureProfileBase profile, string imgWidth)
        {
            if (!string.IsNullOrEmpty(imgWidth))
            {
                return $"width=\"{imgWidth}\" ";
            }
            if(profile.ImgWidthHeight)
            {
                var widthAttribute = $"width=\"{profile.FallbackWidth}\" ";
                var heightAttribute = "";
                if (profile.AspectRatio > 0)
                {
                    heightAttribute = $"height=\"{Math.Round(profile.FallbackWidth / profile.AspectRatio)}\" ";
                } 
                else if (profile.FixedHeight != null && profile.FixedHeight > 0)
                {
                    heightAttribute = $"height=\"{profile.FixedHeight}\" ";

                }
                return widthAttribute + heightAttribute ;
            }

            return string.Empty;
        }

        private static string RenderSourceElement(PictureData pictureData, string format = "", bool usePlaceHolders = false)
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
            var classAttribute = string.IsNullOrEmpty(pictureData.DefaultMediaClass) ? string.Empty : $"class=\"{HttpUtility.HtmlEncode(pictureData.DefaultMediaClass)}\"";
            return $"<source {srcSetAttribute} {sizesAttribute} {formatAttribute} {classAttribute}/>";
        }

        private static string RenderSourceElementsForMultiImage(MediaImagesPictureData pictureData, bool usePlaceholders = false)
        {
            var sourceElementsBuilder = new StringBuilder();
            var classAttribute = string.IsNullOrEmpty(pictureData.DefaultMediaClass) ? string.Empty : $"class=\"{HttpUtility.HtmlEncode(pictureData.DefaultMediaClass)}\"";
            foreach (var mediaImage in pictureData.MediaImages)
            {
                var srcSetSrc = usePlaceholders ? GeneratePlaceholderUrl(mediaImage.MediaCondition) : mediaImage.ImagePath;
                var srcSetWebp = usePlaceholders ? GeneratePlaceholderUrl(mediaImage.MediaCondition) : mediaImage.ImagePathWebp;
                var mediaAttribute = $"media=\"{mediaImage.MediaCondition}\"";                

                //add webp source element first
                if (!string.IsNullOrEmpty(srcSetWebp))
                {
                    var formatAttribute = "type=\"image/webp\"";
                    sourceElementsBuilder.Append($"<source {mediaAttribute} srcset=\"{srcSetWebp}\" {formatAttribute} {classAttribute}/>");
                }

                var srcSetAttribute = $"srcset=\"{srcSetSrc}\"";
                sourceElementsBuilder.Append($"<source {mediaAttribute} {srcSetAttribute} {classAttribute}/>");
            }        

            return sourceElementsBuilder.ToString();
        }

        private static string GeneratePlaceholderUrl(string mediaCondition)
        {
            Random random = new Random();
            var rnd = new string(Enumerable.Range(0, 5).Select(_ => (char)random.Next('a', 'z' + 1)).ToArray());
            
            if (mediaCondition.Contains("(min-width: 1025px)")) // Desktop
            {
                return "https://source.unsplash.com/random/1280x720?dog,dogs&r=" + rnd;
            }
            else if (mediaCondition.Contains("(min-width: 641px)") && mediaCondition.Contains("(max-width: 1024px)")) // Tablets
            {
                return "https://source.unsplash.com/random/1024x576?cat,cats&r=" + rnd;
            }
            else // Mobile and others
            {
                return "https://source.unsplash.com/random/896x504?bug,bugs&r=" + rnd;
            }
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
