using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace ObjectDumping.Internal
{
    internal static class FastDefault
    {
        private static readonly ConcurrentDictionary<Type, object> defaults = new ConcurrentDictionary<Type, object>();
        private static readonly MethodInfo GetDefaultValueMethod = typeof(FastDefault)
            .GetRuntimeMethods()
            .Single(method => method.Name == nameof(GetDefaultValue));

        private static object CreateDefault(Type type)
        {
            try
            {
                return GetDefaultValueMethod.MakeGenericMethod(type).Invoke(null, null);
            }
            catch (Exception)
            {
                return default;
            }
        }

        private static T GetDefaultValue<T>() => default;

        /// <summary>
        /// Gets the default value for the specified type
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The default value</returns>
        public static object Get(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return defaults.GetOrAdd(type, CreateDefault);
        }
    }
}
