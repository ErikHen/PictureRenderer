using System;
using System.Collections.Generic;
using System.Text;

namespace PictureRenderer.Profiles
{
    public class StoryblokProfile : PictureProfileBase
    {
        /// <summary>
        /// Converts a Storyblok "focus" value from string to numeric values (only using the [left] + [top] values).
        /// </summary>
        /// <param name="storyblokFocalPoint">Value in the format [left]x[top]:[right]x[bottom].</param>
        /// <returns></returns>
        public static (double x, double y) ConvertStoryblokFocalPoint(string storyblokFocalPoint)
        {
            var xValue = storyblokFocalPoint.Split(':')[0].Split('x')[0];
            var yValue = storyblokFocalPoint.Split(':')[0].Split('x')[1];

            return (double.Parse(xValue, System.Globalization.CultureInfo.InvariantCulture), double.Parse(yValue, System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}