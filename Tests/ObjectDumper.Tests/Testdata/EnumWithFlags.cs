using System.Runtime.InteropServices;
using System;

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
