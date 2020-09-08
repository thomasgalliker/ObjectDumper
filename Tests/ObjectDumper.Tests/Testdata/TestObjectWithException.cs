using System;

namespace ObjectDumping.Tests.Testdata
{
    public class TestObjectWithException
    {
        private Exception _ex;

        public Exception Exception { get => throw _ex; set => _ex = value; }
    }
}
