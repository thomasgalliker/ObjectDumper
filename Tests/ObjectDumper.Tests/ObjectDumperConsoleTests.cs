using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
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

        [Theory]
        [ClassData(typeof(BuiltInTypeTestdata))]
        public void ShouldDumpValueOfBuiltInType(object value, string expectedOutput)
        {
            // Act
            var dump = ObjectDumperConsole.Dump(value);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(expectedOutput);
        }

        public class BuiltInTypeTestdata : TheoryData<object, string>
        {
            public BuiltInTypeTestdata()
            {
                // string
                this.Add("", "\"\"");
                this.Add("test", "\"test\"");

                // short
                this.Add(short.MinValue, "MinValue");
                this.Add(short.MaxValue, "MaxValue");
                this.Add((short)123, "123");

                // ushort
                this.Add(ushort.MinValue, "0");
                this.Add(ushort.MaxValue, "MaxValue");
                this.Add((ushort)123, "123");

                // int
                this.Add(int.MinValue, "MinValue");
                this.Add(int.MaxValue, "MaxValue");
                this.Add((int)123, "123");

                // uint
                this.Add(uint.MinValue, "0");
                this.Add(uint.MaxValue, "MaxValue");
                this.Add((uint)123, "123");

                // long
                this.Add(long.MinValue, "MinValue");
                this.Add(long.MaxValue, "MaxValue");
                this.Add((long)123, "123");

                // ulong
                this.Add(ulong.MinValue, "0");
                this.Add(ulong.MaxValue, "MaxValue");
                this.Add((ulong)123, "123");

                // decimal
                this.Add(decimal.MinValue, "MinValue");
                this.Add(decimal.MaxValue, "MaxValue");
                this.Add(123.45678m, "123.45678");

                // double
                this.Add(double.MinValue, "MinValue");
                this.Add(double.MaxValue, "MaxValue");
                this.Add(double.NegativeInfinity, "NegativeInfinity");
                this.Add(double.PositiveInfinity, "PositiveInfinity");
                this.Add(double.NaN, "NaN");
                this.Add(123.45678d, "123.45678");

                // float
                this.Add(float.MinValue, "MinValue");
                this.Add(float.MaxValue, "MaxValue");
#if NETCORE
                this.Add(123.45678f, "123.45678");
#endif
            }
        }

        [Fact]
        public void ShouldDumpObject()
        {
            // Arrange
            var person = PersonFactory.GeneratePersons(count: 1).Single();

            // Act
            var dump = ObjectDumperConsole.Dump(person);

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

        [Fact(Skip = "Dumping fields is no longer supported; at least for now")]
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
            var person = PersonFactory.GeneratePersons(count: 1).Single();

            var options = new DumpOptions
            {
                IndentSize = 1,
                IndentChar = '\t',
                LineBreakChar = "\n",
                SetPropertiesOnly = true,
                MemberRenamer = m => m == "Name" ? "RenamedName" : m,
            };

            // Act
            var dump = ObjectDumperConsole.Dump(person, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("{Person}\n" +
                "	RenamedName: \"Person 1\"\n" +
                "	Char: ''\n" +
                "	Age: 2\n" +
                "	Bool: false\n" +
                "	Byte: 0\n	ByteArray: ...\n" +
                "		1\n" +
                "		2\n" +
                "		3\n" +
                "		4\n" +
                "	SByte: 0\n" +
                "	Float: 0\n" +
                "	Uint: 0\n" +
                "	Long: 0\n	ULong: 0\n" +
                "	Short: 0\n" +
                "	UShort: 0\n" +
                "	Decimal: 0\n" +
                "	Double: 0\n	DateTime: DateTime.MinValue\n" +
                "	NullableDateTime: null\n" +
                "	Enum: DateTimeKind.Unspecified");
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
            dump.Should().Be(
                "{Person}\r\n" +
                "  Name: \"Person 1\"\r\n" +
                "  Char: ''\r\n" +
                "  Age: 3\r\n" +
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
                "  Enum: DateTimeKind.Unspecified\r\n" +
                "{Person}\r\n" +
                "  Name: \"Person 2\"\r\n" +
                "  Char: ''\r\n" +
                "  Age: 3\r\n" +
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
        public void ShouldDumpEnumerable_EmptyCollection()
        {
            // Arrange
            var persons = new List<Person>();

            // Act
            var dump = ObjectDumperConsole.Dump(persons);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("");
        }

        [Fact]
        public void ShouldDumpException()
        {
            // Arrange
            var ex = new KeyNotFoundException("message text");
            var options = new DumpOptions { IgnoreDefaultValues = true };

            // Act
            var dump = ObjectDumperConsole.Dump(ex, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "{KeyNotFoundException}\r\n" +
                "  Message: \"message text\"\r\n" +
                "  Data: ...\r\n" +
                "  HResult: -2146232969");
        }

        [Fact]
        public void ShouldDumpExceptionAfterThrow()
        {
            // Arrange
            Exception ex;
            try
            {
                throw new KeyNotFoundException("message text");
            }
            catch (Exception e)
            {
                ex = e;
            }
            var options = new DumpOptions
            {
                IgnoreDefaultValues = true,
                ExcludeProperties =
                {
                    "CustomAttributes",
                    "Module",
                    "StackTrace",
                    "MetadataToken"
                }
            };

            // Act
            var dump = ObjectDumperConsole.Dump(ex, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();

#if NETCORE
            dump.Should().Be(
             "{KeyNotFoundException}\r\n" +
             "  TargetSite: {RuntimeMethodInfo}\r\n" +
             "    Name: \"ShouldDumpExceptionAfterThrow\"\r\n" +
             "    DeclaringType: ObjectDumperConsoleTests\r\n" +
             "    ReflectedType: ObjectDumperConsoleTests\r\n" +
             "    MemberType: MemberTypes.Method\r\n" +
             "    IsSecurityCritical: true\r\n" +
             "    MethodHandle: {RuntimeMethodHandle}\r\n" +
             "      Value: {IntPtr}\r\n" +
             "\r\n" +
             "    Attributes: PrivateScope | Public | HideBySig\r\n" +
             "    CallingConvention: Standard | HasThis\r\n" +
             "    ReturnType: void\r\n" +
             "    ReturnTypeCustomAttributes: {RuntimeParameterInfo}\r\n" +
             "      ParameterType: void\r\n" +
             "      HasDefaultValue: true\r\n" +
             "      Member: null --> Circular reference detected\r\n" +
             "      Position: -1\r\n" +
             "    ReturnParameter: {RuntimeParameterInfo}\r\n" +
             "      ParameterType: void\r\n" +
             "      HasDefaultValue: true\r\n" +
             "      Member: null --> Circular reference detected\r\n" +
             "      Position: -1\r\n" +
             "    IsHideBySig: true\r\n" +
             "    IsPublic: true\r\n" +
             "  Message: \"message text\"\r\n" +
             "  Data: ...\r\n" +
             "  Source: \"ObjectDumper.Tests\"\r\n" +
             "  HResult: -2146232969");
#endif
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
            dump.Should().Be(
                "{Organization}\r\n" +
                "  Name: \"superdev gmbh\"\r\n" +
                "  Persons: ...\r\n" +
                "    {Person}\r\n" +
                "      Name: \"Person 1\"\r\n" +
                "      Char: ''\r\n" +
                "      Age: 3\r\n" +
                "      GetOnly: 11\r\n" +
                "      Bool: false\r\n" +
                "      Byte: 0\r\n" +
                "      ByteArray: ...\r\n" +
                "        1\r\n" +
                "        2\r\n" +
                "        3\r\n" +
                "        4\r\n" +
                "      SByte: 0\r\n" +
                "      Float: 0\r\n" +
                "      Uint: 0\r\n" +
                "      Long: 0\r\n" +
                "      ULong: 0\r\n" +
                "      Short: 0\r\n" +
                "      UShort: 0\r\n" +
                "      Decimal: 0\r\n" +
                "      Double: 0\r\n" +
                "      DateTime: DateTime.MinValue\r\n" +
                "      NullableDateTime: null\r\n" +
                "      Enum: DateTimeKind.Unspecified\r\n" +
                "    {Person}\r\n" +
                "      Name: \"Person 2\"\r\n" +
                "      Char: ''\r\n" +
                "      Age: 3\r\n" +
                "      GetOnly: 11\r\n" +
                "      Bool: false\r\n" +
                "      Byte: 0\r\n" +
                "      ByteArray: ...\r\n" +
                "        1\r\n" +
                "        2\r\n" +
                "        3\r\n" +
                "        4\r\n" +
                "      SByte: 0\r\n" +
                "      Float: 0\r\n" +
                "      Uint: 0\r\n" +
                "      Long: 0\r\n" +
                "      ULong: 0\r\n" +
                "      Short: 0\r\n" +
                "      UShort: 0\r\n" +
                "      Decimal: 0\r\n" +
                "      Double: 0\r\n" +
                "      DateTime: DateTime.MinValue\r\n" +
                "      NullableDateTime: null\r\n" +
                "      Enum: DateTimeKind.Unspecified\r\n" +
                "  IsAfterCollection: true");
        }

        [Fact]
        public void ShouldDumpGenericClass_WithMultipleGenericTypeArguments()
        {
            // Arrange
            var person = PersonFactory.GeneratePersons(count: 1).First();
            var genericClass = new GenericClass<string, float, Person>
            {
                Prop1 = "Test",
                Prop2 = 123.45f,
                Prop3 = person
            };

            // Act
            var dump = ObjectDumperConsole.Dump(genericClass);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();

            dump.Should().Be(
                "{GenericClass<string, float, Person>}\r\n" +
                "  Prop1: \"Test\"\r\n" +
                "  Prop2: 123.45\r\n" +
                "  Prop3: {Person}\r\n" +
                "    Name: \"Person 1\"\r\n" +
                "    Char: ''\r\n" +
                "    Age: 2\r\n" +
                "    GetOnly: 11\r\n" +
                "    Bool: false\r\n" +
                "    Byte: 0\r\n" +
                "    ByteArray: ...\r\n" +
                "      1\r\n" +
                "      2\r\n" +
                "      3\r\n" +
                "      4\r\n" +
                "    SByte: 0\r\n" +
                "    Float: 0\r\n" +
                "    Uint: 0\r\n" +
                "    Long: 0\r\n" +
                "    ULong: 0\r\n" +
                "    Short: 0\r\n" +
                "    UShort: 0\r\n" +
                "    Decimal: 0\r\n" +
                "    Double: 0\r\n" +
                "    DateTime: DateTime.MinValue\r\n" +
                "    NullableDateTime: null\r\n" +
                "    Enum: DateTimeKind.Unspecified");
        }

        [Fact(Skip = "Test failed; to be fixed")]
        public void ShouldDumpGenericClass_WithNestedGenericTypeArguments()
        {
            // Arrange
            var array2D = new string[3, 2]
            {
                { "one", "two" },
                { "three", "four" },
                { "five", "six" }
            };

            var array3D = new int[,,]
            {
                { { 1, 2, 3 }, { 4, 5, 6 } },
                { { 7, 8, 9 }, { 10, 11, 12 } }
            };

            var complexDictionary = new Dictionary<string[,], List<int[,,]>>[1]
            {
                new Dictionary<string[,], List<int[,,]>>
                {
                    { array2D, new List<int[,,]>{ array3D } }
                }
            };

            // Act
            var dump = ObjectDumperConsole.Dump(complexDictionary);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("???");
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
            dump.Should().Be(
                "{Organization}\r\n" +
                "  Name: \"superdev gmbh\"\r\n" +
                "  Persons: ...\r\n" +
                "  IsAfterCollection: true");
        }

        // TODO: Bug in dumping random structs
        //[Fact]
        //public void ShouldDumpValueType()
        //{
        //    // Arrange
        //    var dictionaryEntry = new DictionaryEntry { Key = 1, Value = "Value1" };

        //    // Act
        //    var dump = ObjectDumperConsole.Dump(dictionaryEntry);

        //    // Assert
        //    this.testOutputHelper.WriteLine(dump);
        //    dump.Should().NotBeNull();
        //    dump.Should().Contain("<-- bidirectional reference found");
        //}

        [Fact]
        public void ShouldDumpRecursiveTypes_CircularReference_Case1()
        {
            // Arrange
            var person = new RecursivePerson();
            person.Parent = person;

            var dumpOptions = new DumpOptions
            {
                IgnoreDefaultValues = false
            };

            // Act
            var dump = ObjectDumperConsole.Dump(person, dumpOptions);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "{RecursivePerson}\r\n" +
                "  Id: 0\r\n" +
                "  Parent: null --> Circular reference detected");
        }

        [Fact]
        public void ShouldDumpRecursiveTypes_CircularReference_Case2()
        {
            // Arrange
            var nestedItemA = new NestedItemA
            {
                Property = 1,
                Next = new NestedItemB
                {
                    Property = 1,
                    Next = null
                }
            };

            // Act
            var dump = ObjectDumperConsole.Dump(nestedItemA);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "{NestedItemA}\r\n" +
                "  Next: {NestedItemB}\r\n" +
                "    Next: null\r\n" +
                "    Property: 1\r\n" +
                "  Property: 1");
        }

        [Fact]
        public void ShouldDumpRecursiveTypes_CircularReference_Case3()
        {
            // Arrange
            var nestedItemB = new NestedItemB
            {
                Property = 1,
                Next = null,
            };

            var nestedItemA = new NestedItemA
            {
                Property = 1,
                Next = nestedItemB,
            };

            nestedItemB.Next = nestedItemA;

            // Act
            var dump = ObjectDumperConsole.Dump(nestedItemA);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "{NestedItemA}\r\n" +
                "  Next: {NestedItemB}\r\n" +
                "    Next: null --> Circular reference detected\r\n" +
                "    Property: 1\r\n" +
                "  Property: 1");
        }

        [Fact]
        public void ShouldDumpRecursiveTypes_RuntimeProperties()
        {
            // Arrange
            var person = PersonFactory.GeneratePersons(count: 1).Single();
            var properties = person.GetType().GetRuntimeProperties();
            var options = new DumpOptions { MaxLevel = 2 };

            // Act
            var dump = ObjectDumperConsole.Dump(properties, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
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
            dump.Should().Be("{TestObject}\r\n  NullableDateTime: null");
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
            dump.Should().Be("{OrderPropertyTestObject}\r\n  A: null\r\n  B: null\r\n  C: null");
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
            dump.Should().Be("DateTimeKind.Utc");
        }

        [Fact]
        public void ShouldDumpEnum_WithMultipleFlags()
        {
            // Arrange
            var methodAttributes = MethodAttributes.PrivateScope | MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;

            // Act
            var dump = ObjectDumperConsole.Dump(methodAttributes);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("PrivateScope | Public | Static | HideBySig");
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
            dump.Should().Be("{024cc229-dea0-4d7a-9fc8-722e3a0c69a3}");
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
            dump.Should().Be("{ 1, \"Value1\" }\r\n{ 2, \"Value2\" }\r\n{ 3, \"Value3\" }");
        }

        [Fact]
        public void ShouldDumpArray_OneDimensional()
        {
            // Arrange
            var array = new string[] { "aaa", "bbb" };

            // Act
            var dump = ObjectDumperConsole.Dump(array);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("\"aaa\"\r\n\"bbb\"");
        }

        [Fact(Skip = "to be implemented")]
        public void ShouldDumpArray_TwoDimensional()
        {
            // Arrange
            var array = new int[3, 2]
            {
                {1, 2},
                {3, 4},
                {5, 6}
            };

            // Act
            var dump = ObjectDumperConsole.Dump(array);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("???");
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

        [Fact]
        public void ShouldDumpCultureInfo()
        {
            // Arrange            
            var cultureInfo = new CultureInfo("de-CH");

            // Act
            var dump = ObjectDumperConsole.Dump(cultureInfo, new DumpOptions { MaxLevel = 1 });

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().Be("de-CH");
        }

        [Fact]
        public void ShouldDumpRuntimeType()
        {
            // Arrange            
            var type = typeof(Person);

            // Act
            var dump = ObjectDumperConsole.Dump(type);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("Person");
        }

        [Fact]
        public void ShouldDumpRuntimeType_BuiltInType()
        {
            // Arrange            
            var type = typeof(string);

            // Act
            var dump = ObjectDumperConsole.Dump(type);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("string");
        }

        [Fact]
        public void ShouldDumpStruct()
        {
            // Arrange            
            var x509ChainStatusStruct = new System.Security.Cryptography.X509Certificates.X509ChainStatus
            {
                Status = System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError,
                StatusInformation = "Test status"
            };

            // Act
            var dump = ObjectDumperConsole.Dump(x509ChainStatusStruct);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "{X509ChainStatus}\r\n" +
                "  Status: X509ChainStatusFlags.NoError\r\n" +
                "  StatusInformation: \"Test status\"");
        }

        [Fact]
        public void ShouldDumpAnonymousObject()
        {
            // Arrange
            var dynamicObject = new
            {
                IntProperty = 10,
                StringProperty = "hello",
                DoubleProperty = 3.14d,
            };

            // Act
            var dump = ObjectDumperConsole.Dump(dynamicObject);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "{AnonymousObject}\r\n" +
                "  IntProperty: 10\r\n" +
                "  StringProperty: \"hello\"\r\n" +
                "  DoubleProperty: 3.14");
        }

        [Fact]
        public void ShouldDumpAnonymousObject_List()
        {
            // Arrange 
            var list = new List<dynamic>
            {
                new { Prop = new { SomeInnerProp = "test_test_test" } },
                new { Prop = new { SomeInnerProp = "test_test_test" } }
            };

            // Act
            var dump = ObjectDumperConsole.Dump(list);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "{AnonymousObject}\r\n" +
                "  Prop: {AnonymousObject}\r\n" +
                "    SomeInnerProp: \"test_test_test\"\r\n" +
                "{AnonymousObject}\r\n" +
                "  Prop: {AnonymousObject}\r\n" +
                "    SomeInnerProp: \"test_test_test\"");
        }

        [Fact]
        public void ShouldDumpAnonymousObject_Enumerable()
        {
            // Arrange 
            var obj = new { Prop = new { SomeInnerProp = "test_test_test" } };
            var list = Enumerable.Range(0, 2).Select(_ => obj).ToList();

            // Act
            var dump = ObjectDumperConsole.Dump(list);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "{AnonymousObject}\r\n" +
                "  Prop: {AnonymousObject}\r\n" +
                "    SomeInnerProp: \"test_test_test\"\r\n" +
                "{AnonymousObject}\r\n" +
                "  Prop: {AnonymousObject}\r\n" +
                "    SomeInnerProp: \"test_test_test\"");
        }

        [Fact]
        public void ShouldDumpEnumerable_CorrectCircularReferenceDetection()
        {
            // Arrange 
            var example1 = new Example { Name = "Name1" };
            var example2 = new Example { Name = "Name2", Reference = example1 };
            var array = new[] { example1, example2 };

            // Act
            var dump = ObjectDumperConsole.Dump(array);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().NotContain("// Circular reference detected");
            dump.Should().Be(
                "{Example}\r\n" +
                "  Name: \"Name1\"\r\n" +
                "  Reference: null\r\n" +
                "{Example}\r\n" +
                "  Name: \"Name2\"\r\n" +
                "  Reference: {Example}\r\n" +
                "    Name: \"Name1\"\r\n" +
                "    Reference: null");
        }

        [Fact]
        public void ShouldDumpExpandoObject()
        {
            // Arrange
            dynamic expandoObject = new ExpandoObject();
            expandoObject.IntProperty = 10;
            expandoObject.StringProperty = "hello";
            expandoObject.DoubleProperty = 3.14d;

            // Act
            string dump = ObjectDumperConsole.Dump(expandoObject);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "{ \"IntProperty\", 10 }\r\n" +
                "{ \"StringProperty\", \"hello\" }\r\n" +
                "{ \"DoubleProperty\", 3.14 }");
        }


#if NETCORE
        [Fact]
        public void ShouldDumpValueTuple_Arity0()
        {
            // Arrange 
            var valueTuple = ValueTuple.Create();

            // Act
            var dump = ObjectDumperConsole.Dump(valueTuple);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("()");
        }

        [Fact]
        public void ShouldDumpValueTuple_Arity3()
        {
            // Arrange 
            var valueTuple = (1, "Bill", "Gates");

            // Act
            var dump = ObjectDumperConsole.Dump(valueTuple);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("(1, \"Bill\", \"Gates\")");
        }

        [Fact]
        public void ShouldDumpValueTuple_WithDefaultValue()
        {
            // Arrange 
            (int Id, string FirstName, string LastName) valueTuple = default;

            // Act
            var dump = ObjectDumperConsole.Dump(valueTuple);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("(0, null, null)");
        }

        [Fact]
        public void ShouldDumpEnumerable_ValueTuples()
        {
            // Arrange 
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();
            var valueTuples = persons.Select(s => (s.Name, s.Age)).ToList();

            // Act
            var dump = ObjectDumperConsole.Dump(valueTuples);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "(\"Person 1\", 3)\r\n" +
                "(\"Person 2\", 3)");
        }
#endif

        [Fact]
        public void ShouldDumpMailMessage()
        {
            // Arrange
            var mailmessage = new MailMessage("sender@mail.com", "receiver@mail.com", "Subject", "Body");

            // Act
            var dump = ObjectDumperConsole.Dump(mailmessage);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "{MailMessage}\r\n" +
                "  From: {MailAddress}\r\n" +
                "    DisplayName: \"\"\r\n" +
                "    User: \"sender\"\r\n" +
                "    Host: \"mail.com\"\r\n" +
                "    Address: \"sender@mail.com\"\r\n" +
                "  Sender: null\r\n" +
                "  ReplyTo: null\r\n" +
                "  ReplyToList: ...\r\n" +
                "  To: ...\r\n" +
                "    {MailAddress}\r\n" +
                "      DisplayName: \"\"\r\n" +
                "      User: \"receiver\"\r\n" +
                "      Host: \"mail.com\"\r\n" +
                "      Address: \"receiver@mail.com\"\r\n" +
                "  Bcc: ...\r\n" +
                "  CC: ...\r\n" +
                "  Priority: MailPriority.Normal\r\n" +
                "  DeliveryNotificationOptions: DeliveryNotificationOptions.None\r\n" +
                "  Subject: \"Subject\"\r\n" +
                "  SubjectEncoding: null\r\n" +
                "  Headers: ...\r\n" +
                "  HeadersEncoding: null\r\n" +
                "  Body: \"Body\"\r\n" +
#if NET452
                "  BodyEncoding: {ASCIIEncoding}\r\n" +
#else
                "  BodyEncoding: {ASCIIEncodingSealed}\r\n" +
#endif
                "    IsSingleByte: true\r\n" +
#if NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
                "    Preamble: \"{NotSupportedException: Specified method is not supported.}\"\r\n" +
#endif
                "    BodyName: \"us-ascii\"\r\n" +
                "    EncodingName: \"US-ASCII\"\r\n" +
                "    HeaderName: \"us-ascii\"\r\n" +
                "    WebName: \"us-ascii\"\r\n" +
                "    WindowsCodePage: 1252\r\n" +
                "    IsBrowserDisplay: false\r\n" +
                "    IsBrowserSave: false\r\n" +
                "    IsMailNewsDisplay: true\r\n" +
                "    IsMailNewsSave: true\r\n" +
                "    EncoderFallback: {EncoderReplacementFallback}\r\n" +
                "      DefaultString: \"?\"\r\n" +
                "      MaxCharCount: 1\r\n" +
                "    DecoderFallback: {DecoderReplacementFallback}\r\n" +
                "      DefaultString: \"?\"\r\n" +
                "      MaxCharCount: 1\r\n" +
                "    IsReadOnly: true\r\n" +
                "    CodePage: 20127\r\n" +
                "  BodyTransferEncoding: TransferEncoding.Unknown\r\n" +
                "  IsBodyHtml: false\r\n" +
                "  Attachments: ...\r\n" +
                "  AlternateViews: ...");
        }
    }
}
