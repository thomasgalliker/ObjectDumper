using System;
using System.Linq;
using System.Reflection;

namespace ObjectDumping.Internal
{
    internal static class TypeExtensions
    {
        internal static string GetFormattedName(this Type type, bool useFullName = false, bool useValueTupleFormatting = true)
        {
            var typeName = GetTypeName(type, useFullName, useValueTupleFormatting);

            var typeInfo = type.GetTypeInfo();

            TryGetInnerElementType(ref typeInfo, out var arrayBrackets);

            if (!typeInfo.IsGenericType)
            {
                return typeName;
            }

#if NETSTANDARD_2
            if (useValueTupleFormatting && type.IsValueTuple())
            {
                return typeName.RemoveGenericBackTick();
            }
#endif

            string genericTypeParametersString;
            if (typeInfo.IsGenericTypeDefinition)
            {
                // Used for open generic types
                genericTypeParametersString = $"{string.Join(",", typeInfo.GenericTypeParameters.Select(t => string.Empty))}";
            }
            else
            {
                // Used for regular generic types
                genericTypeParametersString = $"{string.Join(", ", typeInfo.GenericTypeArguments.Select(t => t.GetFormattedName(useFullName, useValueTupleFormatting)))}";
            }

            typeName = typeName.RemoveGenericBackTick();

            return $"{typeName}<{genericTypeParametersString}>{arrayBrackets}";
        }

        private static string RemoveGenericBackTick(this string typeName)
        {
            int iBacktick = typeName.IndexOf('`');
            if (iBacktick > 0)
            {
                typeName = typeName.Remove(iBacktick);
            }

            return typeName;
        }

        private static string GetTypeName(Type type, bool useFullName, bool useValueTupleFormatting)
        {
            string typeName;

#if NETSTANDARD_2
            if (useValueTupleFormatting && type.IsValueTuple())
            {
                typeName = $"({string.Join(", ", type.GenericTypeArguments.Select(t => GetTypeName(t, useFullName, useValueTupleFormatting)))})";
            }
            else
#endif
            {
                typeName = useFullName ? type.FullName : type.Name;
            }

            return typeName;
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

#if NETSTANDARD_2
        public static bool IsValueTuple(this Type type)
        {
            return
                type.IsValueType &&
                //type.IsGenericType &&
                type.FullName is string fullName &&
                (fullName.StartsWith("System.ValueTuple") || fullName.StartsWith("System.ValueTuple`"));
        }
#endif
    }
}
