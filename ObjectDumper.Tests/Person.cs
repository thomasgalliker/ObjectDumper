namespace System.Diagnostics.Tests
{
    public class Person
    {
        public Person()
        {
            this.GetOnly = 11;
            this.SetOnly = 99;
        }

        public string Name { get; set; }

        public int Age { get; set; }

        public int SetOnly { private get; set; }

        public int GetOnly { get; private set; }


        private  int Private { get; set; }
    }
}