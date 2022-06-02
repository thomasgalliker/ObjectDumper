namespace ObjectDumping.Tests.Testdata
{
    public class Example
    {
        public string Name { get; set; }

        public object Reference { get; set; }

        public override string ToString()
        {
            return $"Name: {this.Name}, Reference: {this.Reference?.GetType().Name ?? "null"}";
        }
    }
}
