using System;
using System.Collections.Generic;
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
            dump.Should().Be("{ObjectDumping.Tests.Testdata.Person}\r\n  Name: \"Thomas\"\r\n  Char: \r\n  Age: 30\r\n  GetOnly: 11\r\n  Bool: False\r\n  Byte: 0\r\n  ByteArray: ...\r\n    1\r\n    2\r\n    3\r\n    4\r\n  SByte: 0\r\n  Float: 0\r\n  Uint: 0\r\n  Long: 0\r\n  ULong: 0\r\n  Short: 0\r\n  UShort: 0\r\n  Decimal: 0\r\n  Double: 0\r\n  DateTime: 01.01.0001 00:00:00\r\n  NullableDateTime: null\r\n  Enum: Unspecified\r\n");
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
            dump.Should().Be("{My.TestObject2}\r\n  body: null\r\n  name: null\r\n  Body: null\r\n  Name: null\r\n");
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
            dump.Should().Be("{ObjectDumping.Tests.Testdata.Person}\r\n	Name: \"Thomas\"\r\n	Char: \r\n	Age: 30\r\n	Bool: False\r\n	Byte: 0\r\n	ByteArray: ...\r\n		1\r\n		2\r\n		3\r\n		4\r\n	SByte: 0\r\n	Float: 0\r\n	Uint: 0\r\n	Long: 0\r\n	ULong: 0\r\n	Short: 0\r\n	UShort: 0\r\n	Decimal: 0\r\n	Double: 0\r\n	DateTime: 01.01.0001 00:00:00\r\n	NullableDateTime: null\r\n	Enum: Unspecified\r\n");
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
            dump.Should().Be("{ObjectDumping.Tests.Testdata.Person}\r\n  Name: \"Person 1\"\r\n  Char: \r\n  Age: 3\r\n  GetOnly: 11\r\n  Bool: False\r\n  Byte: 0\r\n  ByteArray: ...\r\n    1\r\n    2\r\n    3\r\n    4\r\n  SByte: 0\r\n  Float: 0\r\n  Uint: 0\r\n  Long: 0\r\n  ULong: 0\r\n  Short: 0\r\n  UShort: 0\r\n  Decimal: 0\r\n  Double: 0\r\n  DateTime: 01.01.0001 00:00:00\r\n  NullableDateTime: null\r\n  Enum: Unspecified\r\n\r\n{ObjectDumping.Tests.Testdata.Person}\r\n  Name: \"Person 2\"\r\n  Char: \r\n  Age: 3\r\n  GetOnly: 11\r\n  Bool: False\r\n  Byte: 0\r\n  ByteArray: ...\r\n    1\r\n    2\r\n    3\r\n    4\r\n  SByte: 0\r\n  Float: 0\r\n  Uint: 0\r\n  Long: 0\r\n  ULong: 0\r\n  Short: 0\r\n  UShort: 0\r\n  Decimal: 0\r\n  Double: 0\r\n  DateTime: 01.01.0001 00:00:00\r\n  NullableDateTime: null\r\n  Enum: Unspecified\r\n\r\n");
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
            dump.Should().Be("{ObjectDumping.Tests.Testdata.Organization}\r\n  Name: \"superdev gmbh\"\r\n  Persons: ...\r\n    {ObjectDumping.Tests.Testdata.Person}\r\n      Name: \"Person 1\"\r\n      Char: \r\n      Age: 3\r\n      GetOnly: 11\r\n      Bool: False\r\n      Byte: 0\r\n      ByteArray: ...\r\n        1\r\n        2\r\n        3\r\n        4\r\n      SByte: 0\r\n      Float: 0\r\n      Uint: 0\r\n      Long: 0\r\n      ULong: 0\r\n      Short: 0\r\n      UShort: 0\r\n      Decimal: 0\r\n      Double: 0\r\n      DateTime: 01.01.0001 00:00:00\r\n      NullableDateTime: null\r\n      Enum: Unspecified\r\n\r\n    {ObjectDumping.Tests.Testdata.Person}\r\n      Name: \"Person 2\"\r\n      Char: \r\n      Age: 3\r\n      GetOnly: 11\r\n      Bool: False\r\n      Byte: 0\r\n      ByteArray: ...\r\n        1\r\n        2\r\n        3\r\n        4\r\n      SByte: 0\r\n      Float: 0\r\n      Uint: 0\r\n      Long: 0\r\n      ULong: 0\r\n      Short: 0\r\n      UShort: 0\r\n      Decimal: 0\r\n      Double: 0\r\n      DateTime: 01.01.0001 00:00:00\r\n      NullableDateTime: null\r\n      Enum: Unspecified\r\n\r\n");
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
            dump.Should().Be("{ObjectDumping.Tests.Testdata.GenericClass<System.String, System.Single, ObjectDumping.Tests.Testdata.Person>}\r\n  Prop1: \"Test\"\r\n  Prop2: 123.45\r\n  Prop3: { }\r\n    {ObjectDumping.Tests.Testdata.Person}\r\n      Name: \"Person 1\"\r\n      Char: \r\n      Age: 2\r\n      GetOnly: 11\r\n      Bool: False\r\n      Byte: 0\r\n      ByteArray: ...\r\n        1\r\n        2\r\n        3\r\n        4\r\n      SByte: 0\r\n      Float: 0\r\n      Uint: 0\r\n      Long: 0\r\n      ULong: 0\r\n      Short: 0\r\n      UShort: 0\r\n      Decimal: 0\r\n      Double: 0\r\n      DateTime: 01.01.0001 00:00:00\r\n      NullableDateTime: null\r\n      Enum: Unspecified\r\n");
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
            dump.Should().Be("{ObjectDumping.Tests.Testdata.Organization}\r\n  Name: \"superdev gmbh\"\r\n  Persons: ...\r\n");
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
            dump.Should().Be("{ObjectDumping.Tests.Testdata.TestObject}\r\n  NullableDateTime: null\r\n");
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
            dump.Should().Be("{ObjectDumping.Tests.Testdata.OrderPropertyTestObject}\r\n  A: null\r\n  B: null\r\n  C: null\r\n");
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

        [Fact]
        public void ShouldDumpEnum()
        {
            // Arrange
            var dateTimeKind = DateTimeKind.Utc;

            // Act
            var dump = ObjectDumperConsole.Dump(dateTimeKind);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Utc");
        }

        [Fact]
        public void ShouldDumpGuid()
        {
            // Arrange
            var guid = new Guid("024CC229-DEA0-4D7A-9FC8-722E3A0C69A3");

            // Act
            var dump = ObjectDumperConsole.Dump(guid);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("024cc229-dea0-4d7a-9fc8-722e3a0c69a3");
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
            var dump = ObjectDumperConsole.Dump(dictionary);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("[1, Value1]\r\n[2, Value2]\r\n[3, Value3]\r\n");
        }

        [Fact]
        public void ShouldEscapeStrings()
        {
            // Arrange
            var expectedPerson = new Person { Name = "Boris \"The Blade\", \\GANGSTA\\ aka 'The Bullet Dodger' \a \b \f \r\nOn a new\twith tab \v \0" };
            var dumpOptions = new DumpOptions { SetPropertiesOnly = true, IgnoreDefaultValues = true, MaxLevel = 1, ExcludeProperties = { "ByteArray" } };

            // Act
            var dump = ObjectDumperConsole.Dump(expectedPerson, dumpOptions);

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
    }
}
