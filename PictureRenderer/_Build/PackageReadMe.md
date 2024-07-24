# ASP.Net Picture Renderer
Makes it easy to optimize images in (pixel) size, quality, file size, and image format. 
Images will be responsive, and can be lazy loaded.<br>
It's a light-weight library, suitable for Blazor Webassembly.

The Picture Renderer renders an [HTML picture element](https://webdesign.tutsplus.com/tutorials/quick-tip-how-to-use-html5-picture-for-responsive-images--cms-21015). The picture element presents a set of images in different sizes and formats. 
It’s then up to the browser to select the most appropriate image depending on screen resolution, viewport width, network speed, and the rules that you set up. <br>

Picture Renderer works very well together with a CMS where you might not be in control of the exact images that will be used. 
The content editor doesn't have to care about what aspect ratio, or size, the image has. The most optimal image will always be used.<br>
Picture Renderer currently works together with [SixLabors/ImageSharp.Web](https://github.com/SixLabors/ImageSharp.Web), [Storyblok's Image service](https://www.storyblok.com/docs/image-service), and [Cloudflare image resizing](https://developers.cloudflare.com/images/image-resizing/).

Read more about Picture Renderer [here](https://github.com/ErikHen/PictureRenderer).
