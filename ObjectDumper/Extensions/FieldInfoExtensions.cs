using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ObjectDumperLib.Extensions
{
    internal static class FieldInfoExtensions
    {
        internal static object TryGetValue(this FieldInfo field, object element)
        {
            object value;
            try
            {
                value = field.GetValue(element);
            }
            catch (Exception ex)
            {
                value = $"{{{ex.Message}}}";
            }

            return value;
        }
    }
}
