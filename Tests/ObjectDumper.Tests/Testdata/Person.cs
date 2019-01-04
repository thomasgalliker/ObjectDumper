using System;

namespace ObjectDumping.Tests.Testdata
{
    public class Person
    {
        public Person()
        {
            this.GetOnly = 11;
            this.SetOnly = 99;
            this.ByteArray = new byte[] { 1, 2, 3, 4 };
        }

        public string Name { get; set; }

        public char Char { get; set; }

        public int Age { get; set; }

        public int SetOnly { private get; set; }

        public int GetOnly { get; private set; }

        private int Private { get; set; }

        public bool Bool { get; set; }

        public byte Byte { get; set; }

        public byte[] ByteArray { get; set; }

        public sbyte SByte { get; set; }

        public float Float { get; set; }

        public uint Uint { get; set; }

        public long Long { get; set; }

        public ulong ULong { get; set; }

        public short Short { get; set; }

        public ushort UShort { get; set; }

        public decimal Decimal { get; set; }

        public double Double { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime? NullableDateTime { get; set; }

        public DateTimeKind Enum { get; set; }
    }
}
