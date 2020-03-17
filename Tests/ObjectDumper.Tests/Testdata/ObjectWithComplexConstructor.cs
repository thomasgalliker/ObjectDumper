using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectDumping.Tests.Testdata
{

    public class ObjectWithComplexConstructorFactory
    {
        public class ObjectWithComplexConstructor
        {
            public string Foo { get; }

            public int Bar { get; }

            public float Baz { get; }

            internal ObjectWithComplexConstructor(string foo, int bar, float baz)
            {
                Foo = foo;
                Bar = bar;
                Baz = baz;
            }

        }


        public static ObjectWithComplexConstructor BuildIt(string foo, int bar, float baz)
        {
            return new ObjectWithComplexConstructor(foo, bar , baz);
        }

    }
}
