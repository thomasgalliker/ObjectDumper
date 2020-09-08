using System;
using System.Reflection;

namespace ObjectDumping.Internal
{
    internal static class PropertyInfoExtensions
    {
        internal static object TryGetValue(this PropertyInfo property, object element, bool throwException)
        {
            object value;
            try
            {
                value = property.GetValue(element);
            }
            catch (Exception ex)
            {
                if (throwException)
                    throw;
                value = $"{{{ex.Message}}}";
            }

            return value;
        }
    }
}
