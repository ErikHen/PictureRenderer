# ASP.Net Picture Renderer
The Picture renderer makes it easy to optimize images in (pixel) size, quality, and file size. Images will be responsive, and can be lazy loaded.

The Picture Renderer renders an HTML picture element. The picture element presents a set of images in different sizes and formats. 
It’s then up to the browser to select the most appropriate image depending on screen resolution, viewport width, network speed, and the rules that you set up.

Picture Renderer is used together with [SixLabors/ImageSharp.Web](https://github.com/SixLabors/ImageSharp.Web) to create the different image versions.

## Current status
Preview/beta mode.

## Roadmap
From nearly done to possible that it will be done:
* Finish v1, add instructions how to use, publish repo with sample projects.
* Create PictureRenderer.Optimizely to simplify usage together with Optimizely CMS even more.
* Create PictureRenderer.Umbraco to simplify usage together with Umbraco CMS even more.
* Add WebP support as soon as ImageSharp.Web supports it.
* Add more samples, more CMS sample usage.
* Add support for Contentful image resizer.
* Add support for ImageProcessor.Web to support ASP.Net Framework.
