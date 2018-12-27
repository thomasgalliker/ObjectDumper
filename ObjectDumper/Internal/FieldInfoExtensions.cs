using System;
using System.Reflection;

namespace ObjectDumping.Internal
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
