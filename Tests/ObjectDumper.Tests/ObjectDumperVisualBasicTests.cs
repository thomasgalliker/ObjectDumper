using FluentAssertions;
using ObjectDumping.Internal;
using ObjectDumping.Tests.Testdata;
using ObjectDumping.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ObjectDumping.Tests
{
    [Collection(TestCollections.CultureSpecific)]
    public class ObjectDumperVisualBasicTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ObjectDumperVisualBasicTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldDumpObject()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();

            // Act
            var dump = ObjectDumperVisualBasic.Dump(person);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [person] = new Person\r\n{\r\n  Name = \"Thomas\",\r\n  Char = '',\r\n  Age = 30,\r\n  GetOnly = 11,\r\n  Bool = false,\r\n  Byte = 0,\r\n  ByteArray = new Byte[]\r\n  {\r\n    1,\r\n    2,\r\n    3,\r\n    4\r\n  },\r\n  SByte = 0,\r\n  Float = 0f,\r\n  Uint = 0,\r\n  Long = 0L,\r\n  ULong = 0L,\r\n  Short = 0,\r\n  UShort = 0,\r\n  Decimal = 0m,\r\n  Double = 0d,\r\n  DateTime = DateTime.MinValue,\r\n  NullableDateTime = null,\r\n  Enum = System.DateTimeKind.Unspecified\r\n}");
        }

        [Fact]
        public void ShouldDumpObject_WithDumpOptions()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();
            var options = new DumpOptions
            {
                IndentSize = 1,
                IndentChar = '\t',
                LineBreakChar = "\n",
                SetPropertiesOnly = true
            };

            // Act
            var dump = ObjectDumperVisualBasic.Dump(person, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [person] = new Person\n{\n	Name = \"Thomas\",\n	Char = '',\n	Age = 30,\n	Bool = false,\n	Byte = 0,\n	ByteArray = new Byte[]\n	{\n		1,\n		2,\n		3,\n		4\n	},\n	SByte = 0,\n	Float = 0f,\n	Uint = 0,\n	Long = 0L,\n	ULong = 0L,\n	Short = 0,\n	UShort = 0,\n	Decimal = 0m,\n	Double = 0d,\n	DateTime = DateTime.MinValue,\n	NullableDateTime = null,\n	Enum = System.DateTimeKind.Unspecified\n}");
        }

        [Fact]
        public void ShouldDumpEnumerable()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();

            // Act
            var dump = ObjectDumperVisualBasic.Dump(persons);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [listPerson] = new List<Person>\r\n{\r\n  new Person\r\n  {\r\n    Name = \"Person 1\",\r\n    Char = '',\r\n    Age = 3,\r\n    GetOnly = 11,\r\n    Bool = false,\r\n    Byte = 0,\r\n    ByteArray = new Byte[]\r\n    {\r\n      1,\r\n      2,\r\n      3,\r\n      4\r\n    },\r\n    SByte = 0,\r\n    Float = 0f,\r\n    Uint = 0,\r\n    Long = 0L,\r\n    ULong = 0L,\r\n    Short = 0,\r\n    UShort = 0,\r\n    Decimal = 0m,\r\n    Double = 0d,\r\n    DateTime = DateTime.MinValue,\r\n    NullableDateTime = null,\r\n    Enum = System.DateTimeKind.Unspecified\r\n  },\r\n  new Person\r\n  {\r\n    Name = \"Person 2\",\r\n    Char = '',\r\n    Age = 3,\r\n    GetOnly = 11,\r\n    Bool = false,\r\n    Byte = 0,\r\n    ByteArray = new Byte[]\r\n    {\r\n      1,\r\n      2,\r\n      3,\r\n      4\r\n    },\r\n    SByte = 0,\r\n    Float = 0f,\r\n    Uint = 0,\r\n    Long = 0L,\r\n    ULong = 0L,\r\n    Short = 0,\r\n    UShort = 0,\r\n    Decimal = 0m,\r\n    Double = 0d,\r\n    DateTime = DateTime.MinValue,\r\n    NullableDateTime = null,\r\n    Enum = System.DateTimeKind.Unspecified\r\n  }\r\n}");
        }

        [Fact]
        public void ShouldDumpNestedObjects()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();
            var organization = new Organization { Name = "superdev gmbh", Persons = persons };

            // Act
            var dump = ObjectDumperVisualBasic.Dump(organization);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [organization] = new Organization\r\n{\r\n  Name = \"superdev gmbh\",\r\n  Persons = new List<Person>\r\n  {\r\n    new Person\r\n    {\r\n      Name = \"Person 1\",\r\n      Char = '',\r\n      Age = 3,\r\n      GetOnly = 11,\r\n      Bool = false,\r\n      Byte = 0,\r\n      ByteArray = new Byte[]\r\n      {\r\n        1,\r\n        2,\r\n        3,\r\n        4\r\n      },\r\n      SByte = 0,\r\n      Float = 0f,\r\n      Uint = 0,\r\n      Long = 0L,\r\n      ULong = 0L,\r\n      Short = 0,\r\n      UShort = 0,\r\n      Decimal = 0m,\r\n      Double = 0d,\r\n      DateTime = DateTime.MinValue,\r\n      NullableDateTime = null,\r\n      Enum = System.DateTimeKind.Unspecified\r\n    },\r\n    new Person\r\n    {\r\n      Name = \"Person 2\",\r\n      Char = '',\r\n      Age = 3,\r\n      GetOnly = 11,\r\n      Bool = false,\r\n      Byte = 0,\r\n      ByteArray = new Byte[]\r\n      {\r\n        1,\r\n        2,\r\n        3,\r\n        4\r\n      },\r\n      SByte = 0,\r\n      Float = 0f,\r\n      Uint = 0,\r\n      Long = 0L,\r\n      ULong = 0L,\r\n      Short = 0,\r\n      UShort = 0,\r\n      Decimal = 0m,\r\n      Double = 0d,\r\n      DateTime = DateTime.MinValue,\r\n      NullableDateTime = null,\r\n      Enum = System.DateTimeKind.Unspecified\r\n    }\r\n  }\r\n}");
        }

        [Fact]
        public void ShouldDumpMultipleGenericTypes()
        {
            // Arrange
            var person = PersonFactory.GeneratePersons(count: 1).First();
            var genericClass = new GenericClass<string, float, Person> { Prop1 = "Test", Prop2 = 123.45f, Prop3 = person };

            // Act
            var dump = ObjectDumperVisualBasic.Dump(genericClass);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [genericClass] = new GenericClass<String, Single, Person>\r\n{\r\n  Prop1 = \"Test\",\r\n  Prop2 = 123.45f,\r\n  Prop3 = new Person\r\n  {\r\n    Name = \"Person 1\",\r\n    Char = '',\r\n    Age = 2,\r\n    GetOnly = 11,\r\n    Bool = false,\r\n    Byte = 0,\r\n    ByteArray = new Byte[]\r\n    {\r\n      1,\r\n      2,\r\n      3,\r\n      4\r\n    },\r\n    SByte = 0,\r\n    Float = 0f,\r\n    Uint = 0,\r\n    Long = 0L,\r\n    ULong = 0L,\r\n    Short = 0,\r\n    UShort = 0,\r\n    Decimal = 0m,\r\n    Double = 0d,\r\n    DateTime = DateTime.MinValue,\r\n    NullableDateTime = null,\r\n    Enum = System.DateTimeKind.Unspecified\r\n  }\r\n}");
        }

        [Fact]
        public void ShouldDumpObjectWithMaxLevel()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();
            var organization = new Organization { Name = "superdev gmbh", Persons = persons };
            var options = new DumpOptions { MaxLevel = 1 };

            // Act
            var dump = ObjectDumperVisualBasic.Dump(organization, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("Dim [organization] = new Organization With {\r\n  .Name = \"superdev gmbh\",\r\n  .Persons = new List<Person> From {\r\n  }\r\n}");
        }

        [Fact]
        public void ShouldExcludeProperties()
        {
            // Arrange
            var testObject = new TestObject();
            var options = new DumpOptions { ExcludeProperties = { "Id", "NonExistent" } };

            // Act
            var dump = ObjectDumperVisualBasic.Dump(testObject, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("Dim [testObject] = new TestObject With {\r\n  .NullableDateTime = Nothing\r\n}");
        }

        [Fact]
        public void ShouldOrderProperties()
        {
            // Arrange
            var testObject = new OrderPropertyTestObject();
            var options = new DumpOptions { PropertyOrderBy = p => p.Name };

            // Act
            var dump = ObjectDumperVisualBasic.Dump(testObject, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("Dim [orderPropertyTestObject] = new OrderPropertyTestObject With {\r\n  .A = Nothing,\r\n  .B = Nothing,\r\n  .C = Nothing\r\n}");
        }

        [Fact]
        public void ShouldDumpDateTime_DateTimeKind_Unspecified()
        {
            // Arrange
            var dateTime = new DateTime(2000, 01, 01, 23, 59, 59, DateTimeKind.Unspecified);

            // Act
            var dump = ObjectDumperVisualBasic.Dump(dateTime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [dateTime] = DateTime.ParseExact(\"2000-01-01T23:59:59.0000000\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)");

            var returnedDateTime = DateTime.ParseExact("2000-01-01T23:59:59.0000000", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            returnedDateTime.Should().Be(dateTime);
        }

        [Fact]
        public void ShouldDumpDateTime_DateTimeKind_Utc()
        {
            // Arrange
            var dateTime = new DateTime(2000, 01, 01, 23, 59, 59, DateTimeKind.Utc);

            // Act
            var dump = ObjectDumperVisualBasic.Dump(dateTime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [dateTime] = DateTime.ParseExact(\"2000-01-01T23:59:59.0000000Z\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)");

            var returnedDateTime = DateTime.ParseExact("2000-01-01T23:59:59.0000000Z", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            returnedDateTime.Should().Be(dateTime);
        }

        private static string GetUtcOffsetString()
        {
            var utcOffset = TimeZoneInfo.Local.BaseUtcOffset;
            return $"{(utcOffset >= TimeSpan.Zero ? "+" : "-")}{utcOffset:hh\\:mm}";
        }

        [Fact]
        public void ShouldDumpDateTime_DateTimeKind_Local()
        {
            // Arrange
            var dateTime = new DateTime(2000, 01, 01, 23, 59, 59, DateTimeKind.Local);

            // Act
            var dump = ObjectDumperVisualBasic.Dump(dateTime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be($"Dim [dateTime] = DateTime.ParseExact(\"2000-01-01T23:59:59.0000000{GetUtcOffsetString()}\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)");

            var returnedDateTime = DateTime.ParseExact($"2000-01-01T23:59:59.0000000{GetUtcOffsetString()}", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            returnedDateTime.Should().Be(dateTime);
        }

        [Fact]
        public void ShouldDumpDateTimeMinValue()
        {
            // Arrange
            var datetime = DateTime.MinValue;

            // Act
            var dump = ObjectDumperVisualBasic.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [dateTime] = DateTime.MinValue");
        }

        [Fact]
        public void ShouldDumpDateTimeMaxValue()
        {
            // Arrange
            var datetime = DateTime.MaxValue;

            // Act
            var dump = ObjectDumperVisualBasic.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [dateTime] = DateTime.MaxValue");
        }

        [Fact]
        public void ShouldDumpNullableObject()
        {
            // Arrange
            DateTime? datetime = null;

            // Act
            var dump = ObjectDumperVisualBasic.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [x] = Nothing");
        }

        [Fact]
        public void ShouldDumpEnum()
        {
            // Arrange
            var dateTimeKind = DateTimeKind.Utc;

            // Act
            var dump = ObjectDumperVisualBasic.Dump(dateTimeKind);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [dateTimeKind] = System.DateTimeKind.Utc");
        }

        [Fact]
        public void ShouldDumpGuid()
        {
            // Arrange
            var guid = new Guid("024CC229-DEA0-4D7A-9FC8-722E3A0C69A3");

            // Act
            var dump = ObjectDumperVisualBasic.Dump(guid);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [guid] = new Guid(\"024cc229-dea0-4d7a-9fc8-722e3a0c69a3\")");
        }

        [Fact]
        public void ShouldDumpDictionary()
        {
            // Arrange
            var dictionary = new Dictionary<int, string>
            {
                { 1, "Value1" },
                { 2, "Value2" },
                { 3, "Value3" }
            };

            // Act
            var dump = ObjectDumperVisualBasic.Dump(dictionary);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [dictionaryInt32String] = new Dictionary(Of Int32, String) From {\r\n  { 1, \"Value1\" },\r\n  { 2, \"Value2\" },\r\n  { 3, \"Value3\" }\r\n}");
        }

        [Fact]
        public void ShouldEscapeStrings()
        {
            // Arrange
            var expectedPerson = new Person { Name = "Boris \"The Blade\", \\GANGSTA\\ aka 'The Bullet Dodger' \a \b \f \r\nOn a new\twith tab \v \0" };
            var dumpOptions = new DumpOptions { SetPropertiesOnly = true, IgnoreDefaultValues = true, MaxLevel = 1, ExcludeProperties = { "ByteArray" } };

            // Act
            var dump = ObjectDumperVisualBasic.Dump(expectedPerson, dumpOptions);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();

            // Compare generated object with input
            var person = new Person
            {
                Name = "Boris \"The Blade\", \\GANGSTA\\ aka \'The Bullet Dodger\' \a \b \f \r\nOn a new\twith tab \v \0"
            };

            person.Should().BeEquivalentTo(expectedPerson);
        }

        [Fact]
        public void ShouldDumpDateTimeOffset()
        {
            // Arrange            
            var offset = new DateTimeOffset(2000, 01, 01, 23, 59, 59, TimeSpan.Zero);

            // Act
            var dump = ObjectDumperVisualBasic.Dump(offset);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be($"Dim [dateTimeOffset] = DateTimeOffset.ParseExact(\"2000-01-01T23:59:59.0000000+00:00\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)");
        }

        [Fact]
        public void ShouldDumpDateTimeOffsetMinValue()
        {
            // Arrange            
            var offset = DateTimeOffset.MinValue;

            // Act
            var dump = ObjectDumperVisualBasic.Dump(offset);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be($"Dim [dateTimeOffset] = DateTimeOffset.MinValue");
        }


        [Fact]
        public void ShouldDumpByteArray()
        {
            var testVal = new byte[] { 1, 2, 3, 3, 3 };

            var dump = ObjectDumperVisualBasic.Dump(testVal);
            dump.Should().NotBeNull();
            dump.Should().Be("Dim [byte] = new Byte() From {\r\n  1,\r\n  2,\r\n  3,\r\n  3,\r\n  3\r\n}");
        }

        [Fact]
        public void ShouldDumpDateTimeOffsetMaxValue()
        {
            // Arrange            
            var offset = DateTimeOffset.MaxValue;

            // Act
            var dump = ObjectDumperVisualBasic.Dump(offset);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be($"Dim [dateTimeOffset] = DateTimeOffset.MaxValue");
        }
    }
}
