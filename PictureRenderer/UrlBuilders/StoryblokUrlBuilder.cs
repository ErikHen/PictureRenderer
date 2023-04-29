using PictureRenderer.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PictureRenderer.UrlBuilders
{
    // Documentation: https://www.storyblok.com/docs/image-service
    internal static class StoryblokUrlBuilder
    {
        internal static string BuildStoryblokUrl(Uri uri, StoryblokProfile profile, int imageWidth, (double x, double y) focalPoint)
        {
            var imageHeight = PictureUtils.GetImageHeight(imageWidth, profile);
            var size = $"{imageWidth}x{imageHeight}";

            var focalFilter = GetFocalFilter(focalPoint);
            var qualityFilter = profile.Quality != null ? $":quality({profile.Quality})" : string.Empty;
            var filters = qualityFilter + focalFilter != string.Empty ? $"/filters{qualityFilter}{focalFilter}" : string.Empty;

            return $"{PictureUtils.GetImageDomain(uri) + uri.AbsolutePath}/m/{size}{filters}";
        }

        private static string GetFocalFilter((double x, double y) focalPoint)
        {
            if (focalPoint.x > 1 && focalPoint.y > 1)
            {
                return $":focal({focalPoint.x}x{focalPoint.y}:{focalPoint.x+1}x{focalPoint.y+1})";
            }

            return string.Empty;
        }
    }
}
