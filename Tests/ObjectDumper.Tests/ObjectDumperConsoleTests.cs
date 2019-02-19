using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using ObjectDumping.Internal;
using ObjectDumping.Tests.Testdata;
using ObjectDumping.Tests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace ObjectDumping.Tests
{
    [Collection(TestCollections.CultureSpecific)]
    public class ObjectDumperConsoleTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ObjectDumperConsoleTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldDumpObject()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();

            // Act
            var dump = ObjectDumperConsole.Dump(person);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("{ObjectDumping.Tests.Testdata.Person}\n\r  Name: \"Thomas\"\n\r  Char: \n\r  Age: 30\n\r  GetOnly: 11\n\r  Bool: False\n\r  Byte: 0\n\r  ByteArray: ...\n\r    1\n\r    2\n\r    3\n\r    4\n\r  SByte: 0\n\r  Float: 0\n\r  Uint: 0\n\r  Long: 0\n\r  ULong: 0\n\r  Short: 0\n\r  UShort: 0\n\r  Decimal: 0\n\r  Double: 0\n\r  DateTime: 01.01.0001 00:00:00\n\r  NullableDateTime: null\n\r  Enum: Unspecified\n\r");
        }

        [Fact]
        public void ShouldDumpObject_WithNullFieldsAndProperties()
        {
            // Arrange
            var myObject = new My.TestObject2
            {
                body = null,
                Body = null,
                name = null,
                Name = null,
            };

            // Act
            var dump = ObjectDumperConsole.Dump(myObject);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("{My.TestObject2}\n\r  body: null\n\r  name: null\n\r  Body: null\n\r  Name: null\n\r");
        }

        [Fact]
        public void ShouldDumpObject_WithDumpOptions()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();
            var options = new DumpOptions
            {
                IndentChar = '\t',
                IndentSize = 1,
                SetPropertiesOnly = true
            };

            // Act
            var dump = ObjectDumperConsole.Dump(person, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("{ObjectDumping.Tests.Testdata.Person}\n\r	Name: \"Thomas\"\n\r	Char: \n\r	Age: 30\n\r	Bool: False\n\r	Byte: 0\n\r	ByteArray: ...\n\r		1\n\r		2\n\r		3\n\r		4\n\r	SByte: 0\n\r	Float: 0\n\r	Uint: 0\n\r	Long: 0\n\r	ULong: 0\n\r	Short: 0\n\r	UShort: 0\n\r	Decimal: 0\n\r	Double: 0\n\r	DateTime: 01.01.0001 00:00:00\n\r	NullableDateTime: null\n\r	Enum: Unspecified\n\r");
        }

        [Fact]
        public void ShouldDumpEnumerable()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2);

            // Act
            var dump = ObjectDumperConsole.Dump(persons);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("{ObjectDumping.Tests.Testdata.Person}\n\r  Name: \"Person 1\"\n\r  Char: \n\r  Age: 3\n\r  GetOnly: 11\n\r  Bool: False\n\r  Byte: 0\n\r  ByteArray: ...\n\r    1\n\r    2\n\r    3\n\r    4\n\r  SByte: 0\n\r  Float: 0\n\r  Uint: 0\n\r  Long: 0\n\r  ULong: 0\n\r  Short: 0\n\r  UShort: 0\n\r  Decimal: 0\n\r  Double: 0\n\r  DateTime: 01.01.0001 00:00:00\n\r  NullableDateTime: null\n\r  Enum: Unspecified\n\r\n\r{ObjectDumping.Tests.Testdata.Person}\n\r  Name: \"Person 2\"\n\r  Char: \n\r  Age: 3\n\r  GetOnly: 11\n\r  Bool: False\n\r  Byte: 0\n\r  ByteArray: ...\n\r    1\n\r    2\n\r    3\n\r    4\n\r  SByte: 0\n\r  Float: 0\n\r  Uint: 0\n\r  Long: 0\n\r  ULong: 0\n\r  Short: 0\n\r  UShort: 0\n\r  Decimal: 0\n\r  Double: 0\n\r  DateTime: 01.01.0001 00:00:00\n\r  NullableDateTime: null\n\r  Enum: Unspecified\n\r\n\r");
        }

        [Fact]
        public void ShouldDumpNestedObjects()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();
            var organization = new Organization { Name = "superdev gmbh", Persons = persons };

            // Act
            var dump = ObjectDumperConsole.Dump(organization);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("{ObjectDumping.Tests.Testdata.Organization}\n\r  Name: \"superdev gmbh\"\n\r  Persons: ...\n\r    {ObjectDumping.Tests.Testdata.Person}\n\r      Name: \"Person 1\"\n\r      Char: \n\r      Age: 3\n\r      GetOnly: 11\n\r      Bool: False\n\r      Byte: 0\n\r      ByteArray: ...\n\r        1\n\r        2\n\r        3\n\r        4\n\r      SByte: 0\n\r      Float: 0\n\r      Uint: 0\n\r      Long: 0\n\r      ULong: 0\n\r      Short: 0\n\r      UShort: 0\n\r      Decimal: 0\n\r      Double: 0\n\r      DateTime: 01.01.0001 00:00:00\n\r      NullableDateTime: null\n\r      Enum: Unspecified\n\r\n\r    {ObjectDumping.Tests.Testdata.Person}\n\r      Name: \"Person 2\"\n\r      Char: \n\r      Age: 3\n\r      GetOnly: 11\n\r      Bool: False\n\r      Byte: 0\n\r      ByteArray: ...\n\r        1\n\r        2\n\r        3\n\r        4\n\r      SByte: 0\n\r      Float: 0\n\r      Uint: 0\n\r      Long: 0\n\r      ULong: 0\n\r      Short: 0\n\r      UShort: 0\n\r      Decimal: 0\n\r      Double: 0\n\r      DateTime: 01.01.0001 00:00:00\n\r      NullableDateTime: null\n\r      Enum: Unspecified\n\r\n\r");
        }

        [Fact]
        public void ShouldDumpMultipleGenericTypes()
        {
            // Arrange
            var person = PersonFactory.GeneratePersons(count: 1).First();
            var genericClass = new GenericClass<string, float, Person> { Prop1 = "Test", Prop2 = 123.45f, Prop3 = person };

            // Act
            var dump = ObjectDumperConsole.Dump(genericClass);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("{ObjectDumping.Tests.Testdata.GenericClass<System.String, System.Single, ObjectDumping.Tests.Testdata.Person>}\n\r  Prop1: \"Test\"\n\r  Prop2: 123.45\n\r  Prop3: { }\n\r    {ObjectDumping.Tests.Testdata.Person}\n\r      Name: \"Person 1\"\n\r      Char: \n\r      Age: 2\n\r      GetOnly: 11\n\r      Bool: False\n\r      Byte: 0\n\r      ByteArray: ...\n\r        1\n\r        2\n\r        3\n\r        4\n\r      SByte: 0\n\r      Float: 0\n\r      Uint: 0\n\r      Long: 0\n\r      ULong: 0\n\r      Short: 0\n\r      UShort: 0\n\r      Decimal: 0\n\r      Double: 0\n\r      DateTime: 01.01.0001 00:00:00\n\r      NullableDateTime: null\n\r      Enum: Unspecified\n\r");
        }

        [Fact]
        public void ShouldDumpObjectWithMaxLevel()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();
            var organization = new Organization { Name = "superdev gmbh", Persons = persons };
            var options = new DumpOptions { MaxLevel = 1 };

            // Act
            var dump = ObjectDumperConsole.Dump(organization, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("{ObjectDumping.Tests.Testdata.Organization}\n\r  Name: \"superdev gmbh\"\n\r  Persons: ...\n\r");
        }

        // TODO: Bug in dumping random structs
        //[Fact]
        //public void ShouldDumpValueType()
        //{
        //    // Arrange
        //    var dictionaryEntry = new DictionaryEntry { Key = 1, Value = "Value1" };

        //    // Act
        //    var dump = ObjectDumperCSharp.Dump(dictionaryEntry);

        //    // Assert
        //    this.testOutputHelper.WriteLine(dump);
        //    dump.Should().NotBeNull();
        //    dump.Should().Contain("<-- bidirectional reference found");
        //}

        [Fact]
        public void ShouldDumpRecursiveTypes()
        {
            // Arrange
            var person = PersonFactory.GeneratePersons(count: 1).First();
            var properties = person.GetType().GetRuntimeProperties();
            var options = new DumpOptions { MaxLevel = 2 };

            // Act
            var dump = ObjectDumperConsole.Dump(properties, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Contain("<-- bidirectional reference found");
        }

        [Fact]
        public void ShouldExcludeProperties()
        {
            // Arrange
            var testObject = new TestObject();
            var options = new DumpOptions { ExcludeProperties = { "Id", "NonExistent" } };

            // Act
            var dump = ObjectDumperConsole.Dump(testObject, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("{ObjectDumping.Tests.Testdata.TestObject}\n\r  NullableDateTime: null\n\r");
        }

        [Fact]
        public void ShouldOrderProperties()
        {
            // Arrange
            var testObject = new OrderPropertyTestObject();
            var options = new DumpOptions { PropertyOrderBy = p => p.Name };

            // Act
            var dump = ObjectDumperConsole.Dump(testObject, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("{ObjectDumping.Tests.Testdata.OrderPropertyTestObject}\n\r  A: null\n\r  B: null\n\r  C: null\n\r");
        }

        [Fact]
        public void ShouldDumpDateTime()
        {
            // Arrange
            var datetime = new DateTime(2000, 01, 01, 23, 59, 59);

            // Act
            var dump = ObjectDumperConsole.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("01.01.2000 23:59:59");
        }

        [Fact]
        public void ShouldDumpNullableObject()
        {
            // Arrange
            var datetime = new DateTime?();

            // Act
            var dump = ObjectDumperConsole.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("null");
        }
    }
}
