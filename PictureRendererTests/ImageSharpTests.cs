using System.Collections.Generic;
using PictureRenderer.Profiles;
using Xunit;
using Assert = Xunit.Assert;

namespace PictureRenderer.Tests
{
    // simplify escaping by using http://easyonlineconverter.com/converters/dot-net-string-escape.html

    public class ImageSharpTests
    {
        [Fact()]
        public void RenderWithoutWebpTest()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?width=375&height=211&quality=80 375w, /myImage.jpg?width=750&height=422&quality=80 750w, /myImage.jpg?width=980&height=551&quality=80 980w, /myImage.jpg?width=1500&height=844&quality=80 1500w\" sizes=\"(max-width: 980px) calc((100vw - 40px)), (max-width: 1200px) 368px, 750px\" /><img alt=\"\" src=\"/myImage.jpg?width=1500&height=844&quality=80\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 375, 750, 980, 1500 },
                Sizes = new[] { "(max-width: 980px) calc((100vw - 40px))", "(max-width: 1200px) 368px", "750px" },
                AspectRatio = 1.777,
                CreateWebpForFormat = null
            };

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile);

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithWebpTestOld()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();
                
            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text");

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithWebpTest()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, new PictureAttributes() { ImgAlt = "alt text"});

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithStyleTestOld()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" loading=\"lazy\" decoding=\"async\" style=\"float: right;\" /></picture>";
            var profile = GetTestImageProfile();

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text", style: "float: right;");

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithStyleTest()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" loading=\"lazy\" decoding=\"async\" style=\"float: right;\" /></picture>";
            var profile = GetTestImageProfile();

            var attributes = new PictureAttributes() {ImgAlt = "alt text"};
            attributes.ImgAdditionalAttributes.Add("style", "float: right;");

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, attributes);

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithCssClassAndImageDecodingAutoOLD()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" loading=\"lazy\" decoding=\"auto\" class=\"my-css-class\"/></picture>";
            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1,
                ImageDecoding = ImageDecoding.Auto,
            };

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text", "my-css-class");

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithCssClassAndImageDecodingAuto()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" loading=\"lazy\" decoding=\"auto\" class=\"my-css-class\"/></picture>";
            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1,
            };
            var attributes = new PictureAttributes { ImgAlt = "alt text", ImgClass = "my-css-class", ImgDecoding = ImageDecoding.Auto};

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, attributes);

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithWidthAndHeightAndNoDecodingOLD()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" width=\"300\" height=\"300\" loading=\"lazy\" /></picture>";
            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1,
                ImgWidthHeight = true,
                ImageDecoding = ImageDecoding.None,
            };

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text");

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithWidthAndHeightAndNoDecoding()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" width=\"300\" height=\"300\" loading=\"lazy\" /></picture>";
            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1,
            };
            var attributes = new PictureAttributes() { ImgAlt = "alt text", RenderImgWidthHeight = true, ImgDecoding = ImageDecoding.None };

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, attributes);

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithWidthAndHeightAndFetchPriorityNoneOLD()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" width=\"300\" height=\"300\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1,
                ImgWidthHeight = true,
                FetchPriority = FetchPriority.None,
            };

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text");

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithWidthAndHeightAndFetchPriorityNone()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" width=\"300\" height=\"300\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1,
            };
            var attributes = new PictureAttributes() { ImgAlt = "alt text", RenderImgWidthHeight = true, ImgFetchPriority = FetchPriority.None};

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, attributes);

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithWidthAndHeightAndFetchPriorityAutoOLD()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" width=\"300\" height=\"300\" loading=\"lazy\" decoding=\"async\" fetchPriority=\"auto\" /></picture>";
            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1,
                ImgWidthHeight = true,
                FetchPriority = FetchPriority.Auto,
            };

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text");

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithWidthAndHeightAndFetchPriorityAuto()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" width=\"300\" height=\"300\" loading=\"lazy\" decoding=\"async\" fetchPriority=\"auto\" /></picture>";
            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1,
            };
            var attributes = new PictureAttributes() { ImgAlt = "alt text", RenderImgWidthHeight = true, ImgFetchPriority = FetchPriority.Auto };
            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, attributes);

            Assert.Equal(expected, result);
        }

        //[Fact()]
        //public void RenderWithWidthAndHeightAndFetchPriorityHigh()
        //{
        //    const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" width=\"300\" height=\"300\" loading=\"lazy\" decoding=\"async\" fetchPriority=\"high\" /></picture>";
        //    var profile = new ImageSharpProfile()
        //    {
        //        SrcSetWidths = new[] { 150, 300 },
        //        Sizes = new[] { "150px" },
        //        AspectRatio = 1,
        //        ImgWidthHeight = true,
        //        FetchPriority = FetchPriority.High,
        //    };

        //    var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text");

        //    Assert.Equal(expected, result);
        //}

        //[Fact()]
        //public void RenderWithWidthAndHeightAndFetchPriorityLow()
        //{
        //    const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" width=\"300\" height=\"300\" loading=\"lazy\" decoding=\"async\" fetchPriority=\"low\" /></picture>";
        //    var profile = new ImageSharpProfile()
        //    {
        //        SrcSetWidths = new[] { 150, 300 },
        //        Sizes = new[] { "150px" },
        //        AspectRatio = 1,
        //        ImgWidthHeight = true,
        //        FetchPriority = FetchPriority.Low,
        //    };

        //    var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text");

        //    Assert.Equal(expected, result);
        //}

        [Fact()]
        public void RenderWithFixedHeightOLD()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=100&quality=80 150w, /myImage.jpg?format=webp&width=300&height=100&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=100&quality=80 150w, /myImage.jpg?width=300&height=100&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=100&quality=80\" width=\"300\" height=\"100\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                ImgWidthHeight = true,
                FixedHeight = 100,
               // AspectRatio = 1,
            };

            var result = Picture.Render("/myImage.jpg", profile, "alt text");

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithFixedHeight()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=100&quality=80 150w, /myImage.jpg?format=webp&width=300&height=100&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=100&quality=80 150w, /myImage.jpg?width=300&height=100&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=100&quality=80\" width=\"300\" height=\"100\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                FixedHeight = 100,
            };
            var attributes = new PictureAttributes() { ImgAlt = "alt text", RenderImgWidthHeight = true };

            var result = Picture.Render("/myImage.jpg", profile, attributes);

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderMultiImageTest()
        {
            const string expected = "<picture><source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?format=webp&width=400&height=400&quality=80\" type=\"image/webp\"/><source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?width=400&height=400&quality=80\"/><source media=\"(min-width: 600px)\" srcset=\"/myImage2.png?width=200&height=200&quality=80\"/><source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?format=webp&width=100&height=100&quality=80\" type=\"image/webp\"/><source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?width=100&height=100&quality=80\"/><img alt=\"\" src=\"/myImage.jpg?width=400&height=400&quality=80\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var result = Picture.Render(new []{"/myImage.jpg", "/myImage2.png", "/myImage3.jpg" }, GetTestImageProfile());

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderMultiImageMissingImageTest()
        {
            const string expected = "<picture><source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?format=webp&width=400&height=400&quality=80\" type=\"image/webp\"/><source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?width=400&height=400&quality=80\"/><source media=\"(min-width: 600px)\" srcset=\"/myImage2.jpg?format=webp&width=200&height=200&quality=80\" type=\"image/webp\"/><source media=\"(min-width: 600px)\" srcset=\"/myImage2.jpg?width=200&height=200&quality=80\"/><source media=\"(min-width: 300px)\" srcset=\"/myImage2.jpg?format=webp&width=100&height=100&quality=80\" type=\"image/webp\"/><source media=\"(min-width: 300px)\" srcset=\"/myImage2.jpg?width=100&height=100&quality=80\"/><img alt=\"alt text\" src=\"/myImage.jpg?width=400&height=400&quality=80\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var result = Picture.Render(new[] { "/myImage.jpg", "/myImage2.jpg" }, GetTestImageProfile(), "alt text");

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderMultiImageWithFocalPointsTest()
        {
            const string expected = "<picture><source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?format=webp&width=400&height=400&rxy=0.1%2c0.1&quality=80\" type=\"image/webp\"/><source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?width=400&height=400&rxy=0.1%2c0.1&quality=80\"/><source media=\"(min-width: 600px)\" srcset=\"/myImage2.png?width=200&height=200&rxy=0.2%2c0.2&quality=80\"/><source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?format=webp&width=100&height=100&rxy=0.3%2c0.3&quality=80\" type=\"image/webp\"/><source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?width=100&height=100&rxy=0.3%2c0.3&quality=80\"/><img alt=\"\" src=\"/myImage.jpg?width=400&height=400&rxy=0.1%2c0.1&quality=80\" loading=\"lazy\" decoding=\"async\" /></picture>";

            var result = Picture.Render(new[] { "/myImage.jpg", "/myImage2.png", "/myImage3.jpg" }, GetTestImageProfile(), focalPoints: new [] { (0.1, 0.1), (0.2, 0.2), (0.3, 0.3) });

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderMultiImageWithEmptyFocalPointsTest()
        {
            const string expected = "<picture><source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?format=webp&width=400&height=400&rxy=0.1%2c0.1&quality=80\" type=\"image/webp\"/><source media=\"(min-width: 1200px)\" srcset=\"/myImage.jpg?width=400&height=400&rxy=0.1%2c0.1&quality=80\"/><source media=\"(min-width: 600px)\" srcset=\"/myImage2.png?width=200&height=200&quality=80\"/><source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?format=webp&width=100&height=100&rxy=0.3%2c0.3&quality=80\" type=\"image/webp\"/><source media=\"(min-width: 300px)\" srcset=\"/myImage3.jpg?width=100&height=100&rxy=0.3%2c0.3&quality=80\"/><img alt=\"\" src=\"/myImage.jpg?width=400&height=400&rxy=0.1%2c0.1&quality=80\" loading=\"lazy\" decoding=\"async\" /></picture>";

            var result = Picture.Render(new[] { "/myImage.jpg", "/myImage2.png", "/myImage3.jpg" }, GetTestImageProfile(), focalPoints: new[] { (0.1, 0.1), default, (0.3, 0.3) });

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithImgWidthTestOLD()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" width=\"50%\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text", LazyLoading.Browser, default, "", "50%");

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithImgWidthTest()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=80\" width=\"50%\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();
            var attributes = new PictureAttributes() { ImgAlt = "alt text", LazyLoading = LazyLoading.Browser };
            attributes.ImgAdditionalAttributes.Add("width", "50%");
            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, attributes);

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithQuerystringTest()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=20 150w, /myImage.jpg?format=webp&width=300&height=300&quality=20 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=20 150w, /myImage.jpg?width=300&height=300&quality=20 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/myImage.jpg?width=300&height=300&quality=20\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();

            var result = PictureRenderer.Picture.Render("/myImage.jpg?quality=20", profile, new PictureAttributes() { ImgAlt = "alt text"});

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithDomainTest()
        {
            const string expected = "<picture><source srcset=\"https://mydomain.com/myImage.jpg?format=webp&width=150&height=150&quality=7 150w, https://mydomain.com/myImage.jpg?format=webp&width=300&height=300&quality=7 300w\" sizes=\"150px\" type=\"image/webp\"/><source srcset=\"https://mydomain.com/myImage.jpg?width=150&height=150&quality=7 150w, https://mydomain.com/myImage.jpg?width=300&height=300&quality=7 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"https://mydomain.com/myImage.jpg?width=300&height=300&quality=7\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();

            var result = PictureRenderer.Picture.Render("https://mydomain.com/myImage.jpg?quality=7", profile, new PictureAttributes() { ImgAlt = "alt text" });

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithAdditionalAttributeTest()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img alt=\"\" src=\"/myImage.jpg?width=300&height=300&quality=80\" loading=\"lazy\" decoding=\"async\" itemprop=\"test\" /></picture>";
            var profile = GetTestImageProfile();
            profile.CreateWebpForFormat = null;

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, new PictureAttributes() { ImgAdditionalAttributes = new Dictionary<string, string>() { {"itemprop", "test"} } });

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithInfoTest()
        {
            const string expectedStart = "<picture><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\" sizes=\"150px\" /><img id=\""; 
            const string expectedContains1 = "<div id=\"pinfo";
            const string expectedContains2 = "style=\"position: absolute; margin-top:-60px; padding:0 5px 2px 5px; font-size:0.8rem; text-align:left; background-color:rgba(255, 255, 255, 0.8);\"></div>";
            const string expectedContains3 = "window.addEventListener(\"load\",function () { const pictureInfo = document.getElementById('";
            const string expectedEnd = "(input) { return input.split('/').pop().replace('?', '\\n').replaceAll('&', ', ').replace('%2c', ',').replace('rxy', 'focal point'); } }, false);</script>";
            var profile = GetTestImageProfile();
            profile.ShowInfo = true;
            profile.CreateWebpForFormat = null;

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile);

            Assert.StartsWith(expectedStart, result);
            Assert.Contains(expectedContains1, result);
            Assert.Contains(expectedContains2, result);
            Assert.Contains(expectedContains3, result);
            Assert.EndsWith(expectedEnd, result);
        }

        private static ImageSharpProfile GetTestImageProfile()
        {
            //use this to test with both single and multiple images
            return new ImageSharpProfile()
            {
                MultiImageMediaConditions = new[] { new MediaCondition("(min-width: 1200px)", 400), new MediaCondition("(min-width: 600px)", 200), new MediaCondition("(min-width: 300px)", 100) },
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1
            };
        }
    }
}