namespace My
{
    public class TestObject2
    {
#pragma warning disable IDE1006 // Naming Styles
        public object body;

        public string name;
#pragma warning restore IDE1006 // Naming Styles

        public object Body { get; set; }

        public string Name { get; set; }
    }
}
