namespace ObjectDumping.Tests.Testdata
{
    public class TempRecord
    {
        public int AProp { get; set; }

        private readonly float[] temps = new float[2]
        { 0F, 0F };

        // Indexer declaration: ObjectDumper can only handle index properties of type int[].
        public float this[int index]
        {
            get => this.temps[index];
            set => this.temps[index] = value;
        }

        public string ZProp { get; set; }
    }
}
