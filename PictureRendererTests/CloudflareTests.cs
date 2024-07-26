using PictureRenderer.Profiles;
using Xunit;
using Assert = Xunit.Assert;

namespace PictureRenderer.Tests
{
    // simplify escaping by using http://easyonlineconverter.com/converters/dot-net-string-escape.html

    public class CloudflareTests
    {
        [Fact()]
        public void RenderWithQualityTest()
        {
            const string expected = "<picture><source srcset=\"/cdn-cgi/image/width=375,format=auto,fit=crop,height=211,quality=80/https://mydomain.com/myImage.jpg 375w, /cdn-cgi/image/width=750,format=auto,fit=crop,height=422,quality=80/https://mydomain.com/myImage.jpg 750w, /cdn-cgi/image/width=980,format=auto,fit=crop,height=551,quality=80/https://mydomain.com/myImage.jpg 980w, /cdn-cgi/image/width=1500,format=auto,fit=crop,height=844,quality=80/https://mydomain.com/myImage.jpg 1500w\" sizes=\"(max-width: 980px) calc((100vw - 40px)), (max-width: 1200px) 368px, 750px\" /><img alt=\"\" src=\"/cdn-cgi/image/width=1500,format=auto,fit=crop,height=844,quality=80/https://mydomain.com/myImage.jpg\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = new CloudflareProfile()
            {
                SrcSetWidths = new[] { 375, 750, 980, 1500 },
                Sizes = new[] { "(max-width: 980px) calc((100vw - 40px))", "(max-width: 1200px) 368px", "750px" },
                AspectRatio = 1.777,
                Quality = 80
            };

            var result = Picture.Render("https://mydomain.com/myImage.jpg", profile);

            Assert.Equal(expected, result);
        }


        [Fact()]
        public void RenderWithAltTextTest()
        {
            const string expected = "<picture><source srcset=\"/cdn-cgi/image/width=150,format=auto,fit=crop,height=150/https://mydomain.com/myImage.jpg 150w, /cdn-cgi/image/width=300,format=auto,fit=crop,height=300/https://mydomain.com/myImage.jpg 300w\" sizes=\"150px\" /><img alt=\"alt text\" src=\"/cdn-cgi/image/width=300,format=auto,fit=crop,height=300/https://mydomain.com/myImage.jpg\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();

            var result = PictureRenderer.Picture.Render("https://mydomain.com/myImage.jpg", profile, new PictureAttributes() { ImgAlt = "alt text" });

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithoutDomainTest()
        {
            const string expected = "<picture><source srcset=\"/cdn-cgi/image/width=150,format=auto,fit=crop,height=150/myImage.jpg 150w, /cdn-cgi/image/width=300,format=auto,fit=crop,height=300/myImage.jpg 300w\" sizes=\"150px\" /><img alt=\"\" src=\"/cdn-cgi/image/width=300,format=auto,fit=crop,height=300/myImage.jpg\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile);

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWithFocalPointTest()
        {
            const string expected = "<picture><source srcset=\"/cdn-cgi/image/width=150,format=auto,fit=crop,height=150,gravity=0.5x0.321/https://mydomain.com/myImage.jpg 150w, /cdn-cgi/image/width=300,format=auto,fit=crop,height=300,gravity=0.5x0.321/https://mydomain.com/myImage.jpg 300w\" sizes=\"150px\" /><img alt=\"\" src=\"/cdn-cgi/image/width=300,format=auto,fit=crop,height=300,gravity=0.5x0.321/https://mydomain.com/myImage.jpg\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();

            var result = PictureRenderer.Picture.Render("https://mydomain.com/myImage.jpg", profile, focalPoint: (0.50, 0.321));

            Assert.Equal(expected, result);
        }

        [Fact()]
        public void RenderWhenDisabled()
        {
            const string expected = "<picture><source srcset=\"/myImage.jpg 150w, /myImage.jpg 300w\" sizes=\"150px\" /><img alt=\"\" src=\"/myImage.jpg\" loading=\"lazy\" decoding=\"async\" /></picture>";
            var profile = GetTestImageProfile();
            profile.IsDisabled = true;

            var result = PictureRenderer.Picture.Render("/myImage.jpg", profile);

            Assert.Equal(expected, result);
        }


        private static CloudflareProfile GetTestImageProfile()
        {
            return new CloudflareProfile()
            {
                SrcSetWidths = new[] { 150, 300 },
                Sizes = new[] { "150px" },
                AspectRatio = 1
            };
        }
    }
}