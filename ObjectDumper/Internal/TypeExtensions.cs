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

            TryGetInnerElementType(ref typeInfo, out var arrayBrackets);

            if (!typeInfo.IsGenericType)
            {
                return typeName;
            }

            string genericTypeParametersString;
            if (typeInfo.IsGenericTypeDefinition)
            {
                // Used for open generic types
                genericTypeParametersString = $"{string.Join(",", typeInfo.GenericTypeParameters.Select(t => string.Empty))}";
            }
            else
            {
                // Used for regular generic types
                genericTypeParametersString = $"{string.Join(", ", typeInfo.GenericTypeArguments.Select(t => t.GetFormattedName(useFullName)))}";
            }

            int iBacktick = typeName.IndexOf('`');
            if (iBacktick > 0)
            {
                typeName = typeName.Remove(iBacktick);
            }

            return $"{typeName}<{genericTypeParametersString}>{arrayBrackets}";
        }

        private static void TryGetInnerElementType(ref TypeInfo type, out string arrayBrackets)
        {
            arrayBrackets = null;
            if (!type.IsArray) return;
            do
            {
                arrayBrackets += "[" + new string(',', type.GetArrayRank() - 1) + "]";
                type = type.GetElementType().GetTypeInfo();
            }
            while (type.IsArray);
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
