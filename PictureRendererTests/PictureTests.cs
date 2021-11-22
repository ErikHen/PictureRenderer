using Xunit;
using PictureRenderer;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PictureRenderer.Profiles;
using Assert = Xunit.Assert;

namespace PictureRenderer.Tests
{
    public class PictureTests
    {
        [Fact()]
        public void RenderWithoutWebpTest()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?width=375&height=211&quality=80 375w, /myImage.jpg?width=750&height=422&quality=80 750w, /myImage.jpg?width=980&height=551&quality=80 980w, /myImage.jpg?width=1500&height=844&quality=80 1500w\"sizes=\"(max-width: 980px) calc((100vw - 40px)), (max-width: 1200px) 368px, 750px\"/><img alt=\"\"src=\"/myImage.jpg?width=1500&height=844&quality=80\"width=\"1500\"height=\"844\"loading=\"lazy\"decoding=\"async\"/></picture>";

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
        public void RenderWithWebpTest()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\"sizes=\"150px\"type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\"sizes=\"150px\"/><img alt=\"alt text\"src=\"/myImage.jpg?width=300&height=300&quality=80\"width=\"300\"height=\"300\"loading=\"lazy\"decoding=\"async\"/></picture>";

            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1
            };

            
            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text");

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithCssClassAndImageDecodingAuto()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\"sizes=\"150px\"type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\"sizes=\"150px\"/><img alt=\"alt text\"src=\"/myImage.jpg?width=300&height=300&quality=80\"width=\"300\"height=\"300\"loading=\"lazy\"decoding=\"auto\"class=\"my-css-class\"/></picture>";
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
        public void RenderWithoutWidthHeightAndDecoding()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg?format=webp&width=150&height=150&quality=80 150w, /myImage.jpg?format=webp&width=300&height=300&quality=80 300w\"sizes=\"150px\"type=\"image/webp\"/><source srcset=\"/myImage.jpg?width=150&height=150&quality=80 150w, /myImage.jpg?width=300&height=300&quality=80 300w\"sizes=\"150px\"/><img alt=\"alt text\"src=\"/myImage.jpg?width=300&height=300&quality=80\"loading=\"lazy\"/></picture>";

            var profile = new ImageSharpProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1,
                NoImgWidthHeight = true,
                ImageDecoding = ImageDecoding.None,
            };


            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile, "alt text");

            Assert.Equal(expected, result);
        }
    }
}