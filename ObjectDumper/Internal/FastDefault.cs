using System.Collections.Concurrent;
using System.Reflection;

namespace ObjectDumping.Internal
{
    internal static class FastDefault
    {
        private static readonly ConcurrentDictionary<Type, object?> Defaults = new ConcurrentDictionary<Type, object?>();
        private static readonly MethodInfo GetDefaultValueMethod = typeof(FastDefault)
            .GetRuntimeMethods()
            .Single(method => method.Name == nameof(GetDefaultValue));

        private static object? CreateDefault(Type type)
        {
            try
            {
                return GetDefaultValueMethod.MakeGenericMethod(type).Invoke(null, null);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static T? GetDefaultValue<T>() => default;

        /// <summary>
        /// Gets the default value for the specified type
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The default value</returns>
        public static object? Get(Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Defaults.GetOrAdd(type, CreateDefault);
        }
    }
}
