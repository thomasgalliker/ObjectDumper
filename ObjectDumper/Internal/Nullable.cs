#if NET48_OR_GREATER || NETSTANDARD1_2 || NETSTANDARD2_0
// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    /*
     * Nullable Reference Types (NRT) are a compiler feature introduced with C# 8.
     * Older target frameworks (.NET Framework 4.8, .NET Standard 2.0 and earlier)
     * do not provide the nullable-analysis attributes that newer frameworks include.
     *
     * These attribute stubs are included only for those older TFMs so the compiler
     * can emit correct nullability metadata and consumers can benefit from NRT
     * annotations. They have no runtime behavior and should NOT be included when the
     * framework already provides them (.NET Core 3.0+, .NET Standard 2.1+, .NET 5+).
     */

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class MaybeNullWhenAttribute : Attribute
    {
        public bool ReturnValue { get; }

        public MaybeNullWhenAttribute(bool returnValue)
        {
            this.ReturnValue = returnValue;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        public bool ReturnValue { get; }

        public NotNullWhenAttribute(bool returnValue)
        {
            this.ReturnValue = returnValue;
        }
    }
}
#endif