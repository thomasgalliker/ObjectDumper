using System;
using System.Runtime.InteropServices;

namespace ObjectDumping.Tests.Testdata
{
    [Serializable]
    [Flags]
    [ComVisible(true)]
    public enum EnumWithFlags
    {
        Private = 1,
        Public = 2,
        Static = 4,
    }
}
