using System.Diagnostics.Tests.Testdata;
using System.Linq;
using FluentAssertions;

using Xunit;
using Xunit.Abstractions;

namespace System.Diagnostics.Tests
{
    public class ObjectDumperTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ObjectDumperTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldDumpObject_WithDefaultDumpOptions()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();

            // Act
            var dump = ObjectDumper.Dump(person);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("{System.Diagnostics.Tests.Testdata.Person}\n\r  Name: \"Thomas\"\n\r  Char: \n\r  Age: 30\n\r  GetOnly: 11\n\r  Bool: False\n\r  Byte: 0\n\r  ByteArray: ...\n\r    1\n\r    2\n\r    3\n\r    4\n\r  SByte: 0\n\r  Float: 0\n\r  Uint: 0\n\r  Long: 0\n\r  ULong: 0\n\r  Short: 0\n\r  UShort: 0\n\r  Decimal: 0\n\r  Double: 0\n\r  DateTime: 01.01.0001 00:00:00\n\r  NullableDateTime: null\n\r  Enum: Unspecified\n\r");
        }

        [Fact]
        public void ShouldDumpObject_WithExplicityDumpStyle()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();

            // Act
            var dump = ObjectDumper.Dump(person, DumpStyle.CSharp);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var person = new Person\n\r{\n\r  Name = \"Thomas\",\n\r  Char = '',\n\r  Age = 30,\n\r  GetOnly = 11,\n\r  Bool = false,\n\r  Byte = 0,\n\r  ByteArray = new Byte[]\n\r  {\n\r    1,\n\r    2,\n\r    3,\n\r    4\n\r  },\n\r  SByte = 0,\n\r  Float = 0f,\n\r  Uint = 0,\n\r  Long = 0l,\n\r  ULong = 0l,\n\r  Short = 0,\n\r  UShort = 0,\n\r  Decimal = 0m,\n\r  Double = 0d,\n\r  DateTime = DateTime.Parse(\"01.01.0001 00:00:00\"),\n\r  NullableDateTime = null,\n\r  Enum = System.DateTimeKind.Unspecified\n\r};");
        }
    }
}
