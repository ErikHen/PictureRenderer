# ASP.Net Picture Renderer
The Picture renderer makes it easy to optimize images in (pixel) size, quality, and file size. 
Images will be responsive, and can be lazy loaded.
It's a light-weight library, suitable for Blazor Webassembly.

The Picture Renderer renders an [HTML picture element](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/picture). The picture element presents a set of images in different sizes and formats. 
It’s then up to the browser to select the most appropriate image depending on screen resolution, viewport width, network speed, and the rules that you set up.

Picture Renderer is used together with [SixLabors/ImageSharp.Web](https://github.com/SixLabors/ImageSharp.Web) to create the different image versions (support for other image processors may be added in the future).

## Why should you use this?
You want the images on your web site to be as optimized as possible. For example, having the most optimal image for any screen size and device type, 
will make the web page load faster, 
and is a [Google search rank factor](https://developers.google.com/search/docs/advanced/guidelines/google-images#optimize-for-speed).
<br>
 
The Picture Renderer works very well together with a CMS where you might not be in control of the exact images that will be used. 
The content editor doesn't have to care about what aspect ratio, or size, the image has. The most optimal image will always be used.    

## How to use
*More instructions will be added...*

* Add [ImageSharp.Web nuget](https://www.nuget.org/packages/SixLabors.ImageSharp.Web/) to the slution on the server that will take care of the actual resizing of the images.
* Add [PictureRenderer nuget](https://www.nuget.org/packages/PictureRenderer/) to the solution that renders the HTML.
* Create Picture profiles for the different types of images that you have on your web site. A Picture profile describes how an image should be scaled in various cases. <br>
You could for example create Picture profiles for: “Top hero image”, “Teaser image”, “Image gallery thumbnail”.
* Let Picture Renderer create the picture HTML element. *Sample code will be added*

### Picture profile
```
using PictureRenderer.Profiles;

public static class PictureProfiles
{
    public static ImageSharpProfile SampleImage =
        new()
        {
            SrcSetWidths = new[] { 375, 750, 980, 1500 },
            SrcSetSizes = new[] { "(max-width: 980px) calc((100vw - 40px))", "(max-width: 1200px) 368px", "750px" },
            AspectRatio = 1.777 
        };
}
```

* **SrcSetWidths** – The different image widths you want the browser to select from. These values are used when rendering the srcset attribute.
* **SrcSetSizes** – Allows us to define the size the image should be according to a set of “media conditions” (similar to css media queries). Values are used to render the sizes attribute.
* **AspectRatio (optional)** – The wanted aspect ratio of the image (width/height). Ex: An image with aspect ratio 16:9 = 16/9 = 1.777.
* **Quality (optional)** - Image quality. Lower value = less file size. Not valid for all image formats.
* **FallbackWidth (optional)** – This image width will be used in browsers that don’t support the picture element. Will use the largest SrcSetWidth if not set.

### Render picture element
Render the picture element by calling `Picture.Render`
<br><br>
#### Parameters
* **ImagePath** - Image path and filename.
* **profile** - The Picture profile that specifies image widths, etc..
* **altText (optional)** - Img element alt attribute.
* **lazyLoading (optional)** - Type of lazy loading. Currently only [browser native lazy loading](https://developer.mozilla.org/en-US/docs/Web/Performance/Lazy_loading#images_and_iframes) (or none).
* **focalPoint (optional)** - Use a focal point when image is cropped. *More description will be added*

Picture.Render returns a string, so you need to make sure the string is not HTML-escaped byt using Html.Raw or similar.
<br> *Sample code for proper Html helper will be added*

Basic MVC/Razor page sample
```
@Html.Raw(Picture.Render("/img/test.jpg", PictureProfiles.SampleImage)) 
```

<br>
<br>

## Current status
Preview/beta mode.

## Roadmap
From nearly done, to may possibly be done:
* Finish v1 (encode alt text), add instructions how to use, publish repo with sample projects.
* Create PictureRenderer.Optimizely to simplify usage together with Optimizely CMS even more.
* Create PictureRenderer.Umbraco to simplify usage together with Umbraco CMS even more.
* Add WebP support as soon as ImageSharp.Web supports it.
* Add automated testing.
* Add more samples, more CMS sample usage.
* Add support for Contentful image resizer.
* Add support for ImageProcessor.Web to support ASP.Net Framework.
