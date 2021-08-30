using System;
using System.Collections.Generic;
using System.Text;

namespace PictureRenderer.Profiles
{
    public class ImageSharpProfile : PictureProfileBase
    {
        /// <summary>
        /// False until ImageSharp has support for Webp.
        /// </summary>
        public new bool IncludeWebp => false; 
    }
}
