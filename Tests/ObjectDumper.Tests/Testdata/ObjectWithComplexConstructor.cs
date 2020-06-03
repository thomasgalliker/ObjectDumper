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
                this.Foo = foo;
                this.Bar = bar;
                this.Baz = baz;
            }
        }

        public static ObjectWithComplexConstructor BuildIt(string foo, int bar, float baz)
        {
            return new ObjectWithComplexConstructor(foo, bar, baz);
        }
    }
}
