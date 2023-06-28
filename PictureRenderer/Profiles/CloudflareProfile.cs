using System;
using System.Collections.Generic;
using System.Text;

namespace PictureRenderer.Profiles
{
    public class CloudflareProfile : PictureProfileBase
    {
        public bool IsDisabled { get; set; }

        public CloudflareProfile() : base()
        {
            IsDisabled = false;
        }
    }
}