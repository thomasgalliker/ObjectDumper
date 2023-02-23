using System;
using System.Linq;
using System.Reflection;

namespace ObjectDumping.Internal
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
                value = $"{{{ex.GetType().Name}: {ex.Message}}}";
            }

            return value;
        }

#if NET5_0_OR_GREATER
        public static bool IsInitOnly(this PropertyInfo propertyInfo)
        {
            var setMethod = propertyInfo.SetMethod;
            if (setMethod == null)
            {
                return false;
            }

            var isExternalInitType = typeof(System.Runtime.CompilerServices.IsExternalInit);
            return setMethod.ReturnParameter.GetRequiredCustomModifiers().Contains(isExternalInitType);
        }
#endif
    }
}
