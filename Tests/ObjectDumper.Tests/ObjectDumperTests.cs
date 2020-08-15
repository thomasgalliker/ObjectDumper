using System.Linq;
using FluentAssertions;
using ObjectDumping.Tests.Testdata;
using ObjectDumping.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace ObjectDumping.Tests
{
    [Collection(TestCollections.CultureSpecific)]
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
            var person = PersonFactory.GeneratePersons(count: 1).Single();

            // Act
            var dump = ObjectDumper.Dump(person);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "{Person}\r\n" +
                "  Name: \"Person 1\"\r\n" +
                "  Char: ''\r\n" +
                "  Age: 2\r\n" +
                "  GetOnly: 11\r\n" +
                "  Bool: false\r\n" +
                "  Byte: 0\r\n" +
                "  ByteArray: ...\r\n" +
                "    1\r\n    2\r\n    3\r\n    4\r\n" +
                "  SByte: 0\r\n" +
                "  Float: 0\r\n" +
                "  Uint: 0\r\n" +
                "  Long: 0\r\n" +
                "  ULong: 0\r\n" +
                "  Short: 0\r\n" +
                "  UShort: 0\r\n" +
                "  Decimal: 0\r\n" +
                "  Double: 0\r\n" +
                "  DateTime: DateTime.MinValue\r\n" +
                "  NullableDateTime: null\r\n" +
                "  Enum: DateTimeKind.Unspecified");
        }

        [Fact]
        public void ShouldDumpObject_WithDumpStyle()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();
            var dumpStyle = DumpStyle.CSharp;

            // Act
            var dump = ObjectDumper.Dump(person, dumpStyle);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var person = new Person\r\n{\r\n  Name = \"Thomas\",\r\n  Char = '',\r\n  Age = 30,\r\n  GetOnly = 11,\r\n  Bool = false,\r\n  Byte = 0,\r\n  ByteArray = new Byte[]\r\n  {\r\n    1,\r\n    2,\r\n    3,\r\n    4\r\n  },\r\n  SByte = 0,\r\n  Float = 0f,\r\n  Uint = 0,\r\n  Long = 0L,\r\n  ULong = 0L,\r\n  Short = 0,\r\n  UShort = 0,\r\n  Decimal = 0m,\r\n  Double = 0d,\r\n  DateTime = DateTime.MinValue,\r\n  NullableDateTime = null,\r\n  Enum = System.DateTimeKind.Unspecified\r\n};");
        }

        [Fact]
        public void ShouldDumpObject_WithOptions()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();
            var dumpOptions = new DumpOptions
            {
                DumpStyle = DumpStyle.CSharp,
                ExcludeProperties = new[] { "Name", "Char" },
                IndentChar = ' ',
                IndentSize = 8,
                MaxLevel = 1,
                LineBreakChar = "\n",
                PropertyOrderBy = pi => pi.Name,
                SetPropertiesOnly = true
            };

            // Act
            var dump = ObjectDumper.Dump(person, dumpOptions);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var person = new Person\n{\n        Age = 30,\n        Bool = false,\n        Byte = 0,\n        ByteArray = new Byte[]\n        {\n        },\n        DateTime = DateTime.MinValue,\n        Decimal = 0m,\n        Double = 0d,\n        Enum = System.DateTimeKind.Unspecified,\n        Float = 0f,\n        Long = 0L,\n        NullableDateTime = null,\n        SByte = 0,\n        Short = 0,\n        Uint = 0,\n        ULong = 0L,\n        UShort = 0\n};");
        }

        [Fact]
        public void ShouldDumpObject_WithOptions_IgnoreDefaultValues()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();
            var dumpOptions = new DumpOptions
            {
                DumpStyle = DumpStyle.CSharp,
                IgnoreDefaultValues = true
            };

            // Act
            var dump = ObjectDumper.Dump(person, dumpOptions);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var person = new Person\r\n{\r\n  Name = \"Thomas\",\r\n  Age = 30,\r\n  GetOnly = 11,\r\n  ByteArray = new Byte[]\r\n  {\r\n    1,\r\n    2,\r\n    3,\r\n    4\r\n  }\r\n};");
        }
    }
}
