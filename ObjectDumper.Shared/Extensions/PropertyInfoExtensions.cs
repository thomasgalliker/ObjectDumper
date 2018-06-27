using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace System.Diagnostics.Extensions
{
    internal static class PropertyInfoExtensions
    {
        internal static object TryGetValue(this PropertyInfo property, object element)
        {
            object value;
            try
            {
                value = property.GetValue(element);
            }
            catch (Exception ex)
            {
                value = $"{{{ex.Message}}}";
            }

            return value;
        }
    }
}
