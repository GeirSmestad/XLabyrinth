using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace LabyrinthEngine.Helpers
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Returns true if the XPathNavigator has a parameter of attributeName equal to attributeValue.
        /// </summary>
        public static bool HasAttributeEqualTo(this XPathNavigator element,
            string attributeName, string attributeValue)
        {
            var attribute = element.GetAttribute(attributeName, string.Empty);

            if (attribute.Equals(attributeValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns an attribute from an XPathNavigator with an empty namespace URI.
        /// </summary>
        public static string GetAttribute(this XPathNavigator element, string localName)
        {
            return element.GetAttribute(localName, string.Empty);
        }

        //public static T ToEnumOrDefault<T>(this string value, T defaultValue)
        //{
        //    if (string.IsNullOrEmpty(value))
        //    {
        //        return defaultValue;
        //    }

        //    T result;
        //    return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
        //}
    }
}
