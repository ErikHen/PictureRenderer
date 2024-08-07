﻿using PictureRenderer.Profiles;
using Xunit;
using Assert = Xunit.Assert;

namespace PictureRenderer.Tests
{
    // simplify escaping by using http://easyonlineconverter.com/converters/dot-net-string-escape.html

    public class StoryblokTests
    {
        [Fact()]
        public void RenderWithQualityTest()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg/m/375x211/filters:quality(80) 375w, /myImage.jpg/m/750x422/filters:quality(80) 750w, /myImage.jpg/m/980x551/filters:quality(80) 980w, /myImage.jpg/m/1500x844/filters:quality(80) 1500w\" sizes=\"(max-width: 980px) calc((100vw - 40px)), (max-width: 1200px) 368px, 750px\" /><img alt=\"\" src=\"/myImage.jpg/m/1500x844/filters:quality(80)\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = new StoryblokProfile()
            {
                SrcSetWidths = new[] { 375, 750, 980, 1500 },
                Sizes = new[] { "(max-width: 980px) calc((100vw - 40px))", "(max-width: 1200px) 368px", "750px" },
                AspectRatio = 1.777,
                Quality = 80
            };

            var result = Picture.Render("/myImage.jpg", profile);

            Assert.Equal(expected, result);
        }


        //[Fact()]
        //public void RenderWithImgWidthTest()
        //{
        //    const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=400&height=400&quality=80\" width=\"50%\" loading=\"lazy\" decoding=\"async\" /></picture>";
        //    var profile = GetTestImageProfile();

        //    var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text", LazyLoading.Browser, default, "", "50%");

        //    Assert.Equal(expected, result);
        //}

   

        [Fact()]
        public void RenderWithDomainAndAltTest()
        {
            const string expected = "<picture><source srcset=\"https://mydomain.com/myImage.jpg/m/150x150 150w, https://mydomain.com/myImage.jpg/m/300x300 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"https://mydomain.com/myImage.jpg/m/300x300\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();

            var result = PictureRenderer.Picture.Render("https://mydomain.com/myImage.jpg", profile, new PictureAttributes() { ImgAlt = "alt text" });

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithFocalPointTest()
        {
            const string expected = "<picture><source srcset=\"https://mydomain.com/myImage.jpg/m/150x150/filters:focal(50x30:51x31) 150w, https://mydomain.com/myImage.jpg/m/300x300/filters:focal(50x30:51x31) 300w\" sizes=\"150px\" /><img alt=\"\" src=\"https://mydomain.com/myImage.jpg/m/300x300/filters:focal(50x30:51x31)\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();

            var result = PictureRenderer.Picture.Render("https://mydomain.com/myImage.jpg", profile, StoryblokProfile.ConvertStoryblokFocalPoint("50x30:51x31"));

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithInfoTest()
        {
            const string expectedStart = "<picture><source srcset=\"https://mydomain.com/myImage.jpg/m/150x150 150w, https://mydomain.com/myImage.jpg/m/300x300 300w\" sizes=\"150px\" /><img id=\"";
            const string expectedContains1 = "<div id=\"pinfo";
            const string expectedContains2 = "style=\"position: absolute; margin-top:-60px; padding:0 5px 2px 5px; font-size:0.8rem; text-align:left; background-color:rgba(255, 255, 255, 0.8);\"></div>";
            const string expectedContains3 = "window.addEventListener(\"load\",function () { const pictureInfo = document.getElementById('";
            const string expectedEnd = "(input) { return input.split('/m/').pop().replaceAll('/', ', '); } }, false);</script>";
            var profile = GetTestImageProfile();
            profile.ShowInfo = true;

            var result = PictureRenderer.Picture.Render("https://mydomain.com/myImage.jpg", profile);

            Assert.StartsWith(expectedStart, result);
            Assert.Contains(expectedContains1, result);
            Assert.Contains(expectedContains2, result);
            Assert.Contains(expectedContains3, result);
            Assert.EndsWith(expectedEnd, result);
        }

        private static StoryblokProfile GetTestImageProfile()
        {
            return new StoryblokProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1
            };
        }
    }
}