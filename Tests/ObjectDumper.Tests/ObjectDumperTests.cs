using System;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

using ObjectDumperLib_Tests.Testdata;
using ObjectDumperLib_Tests.Utils;

namespace ObjectDumperLib_Tests
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
            var person = PersonFactory.GetPersonThomas();

            // Act
            var dump = ObjectDumper.Dump(person);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("{ObjectDumperLib_Tests.Testdata.Person}\n\r  Name: \"Thomas\"\n\r  Char: \n\r  Age: 30\n\r  GetOnly: 11\n\r  Bool: False\n\r  Byte: 0\n\r  ByteArray: ...\n\r    1\n\r    2\n\r    3\n\r    4\n\r  SByte: 0\n\r  Float: 0\n\r  Uint: 0\n\r  Long: 0\n\r  ULong: 0\n\r  Short: 0\n\r  UShort: 0\n\r  Decimal: 0\n\r  Double: 0\n\r  DateTime: 01.01.0001 00:00:00\n\r  NullableDateTime: null\n\r  Enum: Unspecified\n\r");
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
            dump.Should().Be("var person = new Person\n\r{\n\r  Name = \"Thomas\",\n\r  Char = '',\n\r  Age = 30,\n\r  GetOnly = 11,\n\r  Bool = false,\n\r  Byte = 0,\n\r  ByteArray = new Byte[]\n\r  {\n\r    1,\n\r    2,\n\r    3,\n\r    4\n\r  },\n\r  SByte = 0,\n\r  Float = 0f,\n\r  Uint = 0,\n\r  Long = 0L,\n\r  ULong = 0L,\n\r  Short = 0,\n\r  UShort = 0,\n\r  Decimal = 0m,\n\r  Double = 0d,\n\r  DateTime = DateTime.MinValue,\n\r  NullableDateTime = null,\n\r  Enum = System.DateTimeKind.Unspecified\n\r};");
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
    }
}
