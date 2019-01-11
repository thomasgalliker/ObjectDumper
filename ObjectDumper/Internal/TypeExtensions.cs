using System;
using System.Linq;
using System.Reflection;

namespace ObjectDumping.Internal
{
    internal static class TypeExtensions
    {
        /// <summary>
        ///     Source:
        ///     https://github.com/thomasgalliker/CrossPlatformLibrary/blob/0ea2e849dfccee3f68e719c19daef2eaabec190e/CrossPlatformLibrary/Extensions/TypeExtensions.cs
        /// </summary>
        internal static string GetFormattedName(this Type type, bool useFullName = false)
        {
            var typeName = useFullName ? type.FullName : type.Name;

            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsGenericType)
            {
                return typeName;
            }

            return $"{typeName.Substring(0, typeName.IndexOf('`'))}<{string.Join(", ", typeInfo.GenericTypeArguments.Select(t => t.GetFormattedName(useFullName)))}>";
        }

        public static object GetDefault(this Type t)
        {
            var defaultValue = typeof(TypeExtensions).GetRuntimeMethod("GetDefaultGeneric", new Type[] { }).MakeGenericMethod(t).Invoke(null, null);
            return defaultValue;
        }

        public static T GetDefaultGeneric<T>()
        {
            return default(T);
        }
    }
}
