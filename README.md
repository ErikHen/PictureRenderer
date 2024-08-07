# ASP.Net Picture Renderer
The Picture renderer makes it easy to optimize images in (pixel) size, quality, file size, and image format. 
Images will be responsive, and can be lazy loaded.
It's a light-weight library, suitable for Blazor Webassembly.

The Picture Renderer renders an [HTML picture element](https://webdesign.tutsplus.com/tutorials/quick-tip-how-to-use-html5-picture-for-responsive-images--cms-21015). The picture element presents a set of images in different sizes and formats. 
It’s then up to the browser to select the most appropriate image depending on screen resolution, viewport width, network speed, and the rules that you set up.

If you are unfamiliar with the details of the Picture element i highly recommed reading
 [this](https://webdesign.tutsplus.com/tutorials/quick-tip-how-to-use-html5-picture-for-responsive-images--cms-21015) and/or [this](https://www.smashingmagazine.com/2014/05/responsive-images-done-right-guide-picture-srcset/).

Picture Renderer currently works together with [SixLabors/ImageSharp.Web](https://github.com/SixLabors/ImageSharp.Web), [Storyblok's Image service](https://www.storyblok.com/docs/image-service), and [Cloudflare image resizing](https://developers.cloudflare.com/images/image-resizing/) (other image processors can be added on request).

## Why should you use this?
You want the images on your web site to be as optimized as possible. For example, having the most optimal image for any screen size and device type, 
will make the web page load faster, 
and is a [Google search rank factor](https://developers.google.com/search/docs/advanced/guidelines/google-images#optimize-for-speed).
<br>
 
The Picture Renderer works very well together with a CMS where you might not be in control of the exact images that will be used. 
The content editor doesn't have to care about what aspect ratio, or size, the image has. The most optimal image will always be used.<br>
See also [PictureRenderer.Optimizely](https://github.com/ErikHen/PictureRenderer.Optimizely) and [PictureRenderer.Umbraco](https://github.com/ErikHen/PictureRenderer.Umbraco)


### Webp/AVIF format
If using [ImageSharp.Web](https://www.nuget.org/packages/SixLabors.ImageSharp.Web/) as image processor, the rendered picture element will also contain [webp](https://developers.google.com/speed/webp/) versions of the image. 
By default this will be rendered for jpg images.<br>
Storyblok and Cloudflare image services automatically converts images to Webp or AVIF format, if the browser supports it.

## How to use
* Add the [PictureRenderer](https://www.nuget.org/packages/PictureRenderer/) nuget.
* Create Picture profiles (ImageSharpProfile, StoryblokProfile, or CloudflareProfile) for the different types of images that you have on your web site. A Picture profile describes how an image should be scaled in various cases. <br>
You could for example create Picture profiles for: “Top hero image”, “Teaser image”, “Image gallery thumbnail”.
* Let Picture Renderer create the picture HTML element.
* Fine-tune how the picture element is rendered by setting parameters like css class, fetch priority, etc.

If using ImageSharp you need to make sure that [ImageSharp.Web](https://www.nuget.org/packages/SixLabors.ImageSharp.Web/) is enabled on the server that takes care of the actual resizing of the images (see also [Setup and configuration](https://docs.sixlabors.com/articles/imagesharp.web/gettingstarted.html#setup-and-configuration)).


### Picture profile

#### Examples
```c#
using PictureRenderer.Profiles;

public static class PictureProfiles
{
    // Sample image
    // Up to 640 pixels viewport width, the picture width will be 100% of the viewport..
    // Up to 1200 pixels viewport width, the picture width will be 320 pixels.
    // On larger viewport width, the picture width will be 750 pixels.
    // Note that picture width is not the same as image width (but it can be, on screens with a "device pixel ratio" of 1).
    public static readonly ImageSharpProfile SampleImage = new() 
        {
            SrcSetWidths = new[] { 320, 640, 750, 1500 },
            Sizes = new[] { "(max-width: 640px) 100vw", "(max-width: 1200px) 320px", "750px" },
            AspectRatio = 1.777 // 16:9 = 16/9 = 1.777
        };

    // Top hero
    // Picture width is always 100% of the viewport width.
    public static readonly ImageSharpProfile TopHero = new()
        {
            SrcSetWidths = new[] { 1024, 1366, 1536, 1920 },
            Sizes = new[] { "100vw" },
            AspectRatio = 2
        };

    // Thumbnail
    // Thumbnail is always 150px wide. But the browser may still select the 300px image for a high resolution screen (e.g. mobile or tablet screens).
    public static readonly ImageSharpProfile Thumbnail = new()
        {
            SrcSetWidths = new[] { 150, 300 },
            Sizes = new[] { "150px" },
            AspectRatio = 1  //square image (equal height and width).
        };
}
```

* **SrcSetWidths** – The different image widths you want the browser to select from. These values are used when rendering the `srcset` attribute.
* **Sizes** – Define the size (width) the image should be according to a set of [media conditions](https://developer.mozilla.org/en-US/docs/Learn/HTML/Multimedia_and_embedding/Responsive_images) (similar to css media queries). Values are used to render the `sizes` attribute.
* **AspectRatio (optional)** – The wanted aspect ratio of the image (width/height). Ex: An image with aspect ratio 16:9 = 16/9 = 1.777.
* **FixedHeight (optional)** – Set a fixed height for all image sizes. Fixed height is ignored if aspect ratio is set.
* **CreateWebpForFormat (optional, ImageSharp only)** - The image formats that should be offered as webp versions. Jpg format is aded by default.
* **Quality (optional)** - Image quality. Lower value = less file size. Not valid for all image formats. Default value: `80`.
* **IsDisabled (optional, Cloudflare only)** - Do not alter the image url at all. May be useful in a development environment where you are not using the CloudFlare CDN.



### Render picture element
Render the picture element by calling `Picture.Render`
<br>
#### Parameters
* **imagePath** - Relative path, or full Url to the image.
* **profile** - The Picture profile that specifies image widths, etc.
* **focalPoint (optional)** - Use focal point when image is cropped. 
* **attributes (optional)** - A PictureAttributes object with additional attributes/settings that fine-tunes the rendering.<br>
  * **ImgAlt** - `alt` attribute for img element.
  * **ImgClass** - `class` attribute for img element.
  * **ImgFetchPriority** - `fetchPriority` attribute for img element. Default value: none.
  * **ImgDecoding** - `decoding` attribute for img element. Default value: async.
  * **LazyLoading** - Type of lazy loading. Currently supports browser native or none. Default value: browser native.
  * **RenderImgWidthHeight** - If true, width and height attributes will be rendered on the img element. Default value: false.
  * **ImgAdditionalAttributes** - Key-value dictionary that may be used to add additional attributes (like data or itemprop attributes) to the img element.

Picture.Render returns a string, so you need to make sure the string is not HTML-escaped by using Html.Raw or similar. <br>
I recommend wrapping the Picture.Render an Html helper/Tag helper (for MVC/Razor pages) or a component (for Blazor).
<br> 

Basic MVC/Razor page sample
```
@Html.Raw(Picture.Render("/img/test.jpg", PictureProfiles.SampleImage)) 
@Html.Raw(Picture.Render("/img/test.jpg", PictureProfiles.SampleImage, new PictureAttributes { ImgAlt = "alt text", ImgClass = "my-css-class")) 
```
<br>

Basic Blazor sample
```
@((MarkupString)Picture.Render("/images/test.jpg", PictureProfiles.SampleImage))
```

<br>

See also [sample projects](https://github.com/ErikHen/PictureRenderer.Samples).
<br><br>

<!-- ### How to see that it actually works -->


## Version history
* **3.12** Possible to set any attribute on the img element. Prepare for v4.
    > [!NOTE]
    > Switch to using a PictureAttributes object, instead of individual parameters, when calling `Picture.Render` . <br>
    Some settings have been moved from PictureProfile to the new PictureAttributes object. <br>
  
* **3.11** Make PictureUtil/GetPictureData public as requested in [#19](https://github.com/ErikHen/PictureRenderer/issues/19)
* **3.10** Possible to set style attribute (added to img element).
* **3.9** Possible to set [fetchpriority](https://developer.mozilla.org/en-US/docs/Web/API/HTMLImageElement/fetchPriority) attribute. Thanks [Karol](https://github.com/karolberezicki)!
* **3.8** Possible to disable Cloudflare processing (may be wanted on dev environment).
* **3.7** Added support for Storyblok and Cloudflare image services.
* **3.6** Possible to set a fixed height. Thanks [Karl](https://github.com/karlsvan)!
* **3.5** Possible to show info about the currently selected image.
* **3.4** Keep domain in image urls (earlier it always returned an absolute path).
* **3.3.1** Add space between attributes.
* **3.3** Adding possibility to set width attribute on img element. Needed for rare edge case in Optimizely CMS.
* **3.2** Changing some things reported as warnings by Sonarcloud.
* **3.1** Making PictureRenderer.ImageFormat public. Don't understand why it was internal in the first place 😊.

* **3.0** Possible to have multiple focal points when having multiple images. Minor breaking change, so bump major version. <br> 
* **2.2** Added support for "art direction" (e.g. having completely different images for different screen sizes) <br> 
* **2.1** Just adding some more XML commens + added png format constant. <br> 
* **2.0** Support for WebP format. Removed deprecated property. Added unit tests. <br> 
Note that you need to use ImageSharp.Web v2.0+ for WebP support.
* **1.3** Added possibility to render `class`, `decoding`, and `width` + `height` attributes on the img element. <br>
* **1.2.2** Bugfix (unit tests would have found this issue... lesson learned)<br>
* **1.2** Renamed picture profile property to "Sizes" (old name marked as deprecated).<br>
<br>
