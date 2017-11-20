using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Diagnostics
{
    internal static class ReflectionExtensions
    {
        internal static bool IsCOMObject(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return (typeInfo.Attributes & TypeAttributes.Import) == TypeAttributes.Import;
        }
    }
}
