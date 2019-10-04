using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiDemo.Utils
{
    public static class Utils
    {
        public static string ImageURL(string public_url, string profile_image)
        {
            var serverMapPath = $"{public_url}/profile_images/";
            return $"{serverMapPath}{(string.IsNullOrEmpty(profile_image) ? "user123.png" : profile_image)}";
        }
    }
}