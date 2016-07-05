using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace RPGSite.Tools
{
    public static class ToJsonExtension
    {
        /// <summary>
        /// Serializes an object to json.
        /// </summary>
        /// <param name="obj"> The object to be serialized.</param>
        /// <returns>A serialized json object.</returns>
        public static string ToJson(this object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }
    }
}