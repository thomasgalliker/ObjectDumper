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

#if NET6_0_OR_GREATER
using ObjectDumping.Tests.Testdata.RecordTypes;
#endif

namespace ObjectDumping.Tests
{
    [Collection(TestCollections.CultureSpecific)]
    public class ObjectDumperCSharpTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ObjectDumperCSharpTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(BuiltInTypeTestdata))]
        public void ShouldDumpValueOfBuiltInType(object value, string expectedOutput)
        {
            // Act
            var dump = ObjectDumperCSharp.Dump(value);

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
                this.Add("", "var stringValue = \"\";");
                this.Add("test", "var stringValue = \"test\";");

                // short
                this.Add(short.MinValue, "var shortValue = short.MinValue;");
                this.Add(short.MaxValue, "var shortValue = short.MaxValue;");
                this.Add((short)123, "var shortValue = 123;");

                // ushort
                this.Add(ushort.MinValue, "var ushortValue = 0;");
                this.Add(ushort.MaxValue, "var ushortValue = ushort.MaxValue;");
                this.Add((ushort)123, "var ushortValue = 123;");

                // int
                this.Add(int.MinValue, "var intValue = int.MinValue;");
                this.Add(int.MaxValue, "var intValue = int.MaxValue;");
                this.Add((int)123, "var intValue = 123;");

                // uint
                this.Add(uint.MinValue, "var uintValue = 0u;");
                this.Add(uint.MaxValue, "var uintValue = uint.MaxValue;");
                this.Add((uint)123, "var uintValue = 123u;");

                // long
                this.Add(long.MinValue, "var longValue = long.MinValue;");
                this.Add(long.MaxValue, "var longValue = long.MaxValue;");
                this.Add((long)123, "var longValue = 123L;");

                // ulong
                this.Add(ulong.MinValue, "var ulongValue = 0UL;");
                this.Add(ulong.MaxValue, "var ulongValue = ulong.MaxValue;");
                this.Add((ulong)123, "var ulongValue = 123UL;");

                // decimal
                this.Add(decimal.MinValue, "var decimalValue = decimal.MinValue;");
                this.Add(decimal.MaxValue, "var decimalValue = decimal.MaxValue;");
                this.Add(123.45678m, "var decimalValue = 123.45678m;");

                // double
                this.Add(double.MinValue, "var doubleValue = double.MinValue;");
                this.Add(double.MaxValue, "var doubleValue = double.MaxValue;");
                this.Add(double.NegativeInfinity, "var doubleValue = double.NegativeInfinity;");
                this.Add(double.PositiveInfinity, "var doubleValue = double.PositiveInfinity;");
                this.Add(double.NaN, "var doubleValue = double.NaN;");
                this.Add(123.45678d, "var doubleValue = 123.45678d;");

                // float
                this.Add(float.MinValue, "var floatValue = float.MinValue;");
                this.Add(float.MaxValue, "var floatValue = float.MaxValue;");

#if NETCORE
                this.Add(123.45678f, "var floatValue = 123.45678f;");
#endif
            }
        }

        [Fact]
        public void ShouldDumpObject()
        {
            // Arrange
            var person = PersonFactory.GeneratePersons(count: 1).Single();

            // Act
            var dump = ObjectDumperCSharp.Dump(person);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var person = new Person\r\n" +
                "{\r\n" +
                "  Name = \"Person 1\",\r\n" +
                "  Char = '',\r\n" +
                "  Age = 2,\r\n" +
                "  GetOnly = 11,\r\n" +
                "  Bool = false,\r\n" +
                "  Byte = 0,\r\n" +
                "  ByteArray = new byte[]\r\n" +
                "  {\r\n    1,\r\n    2,\r\n    3,\r\n    4\r\n  },\r\n  SByte = 0,\r\n  Float = 0f,\r\n  Uint = 0u,\r\n  Long = 0L,\r\n  ULong = 0UL,\r\n  Short = 0,\r\n  UShort = 0,\r\n  Decimal = 0m,\r\n  Double = 0d,\r\n  DateTime = DateTime.MinValue,\r\n  NullableDateTime = null,\r\n  Enum = DateTimeKind.Unspecified\r\n};");
        }

        [Fact]
        public void ShouldDumpObject_Empty()
        {
            // Arrange
            var emptyClass = new EmptyClass();

            // Act
            var dump = ObjectDumperCSharp.Dump(emptyClass);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var emptyClass = new EmptyClass();");
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
                SetPropertiesOnly = true,
                MemberRenamer = m => m == "Name" ? "RenamedName" : m,
            };

            // Act
            var dump = ObjectDumperCSharp.Dump(person, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var person = new Person\n" +
                "{\n" +
                "	RenamedName = \"Thomas\",\n" +
                "	Char = '',\n" +
                "	Age = 30,\n" +
                "	Bool = false,\n" +
                "	Byte = 0,\n" +
                "	ByteArray = new byte[]\n" +
                "	{\n" +
                "		1,\n" +
                "		2,\n" +
                "		3,\n		4\n" +
                "	},\n" +
                "	SByte = 0,\n" +
                "	Float = 0f,\n" +
                "	Uint = 0u,\n" +
                "	Long = 0L,\n" +
                "	ULong = 0UL,\n" +
                "	Short = 0,\n" +
                "	UShort = 0,\n" +
                "	Decimal = 0m,\n" +
                "	Double = 0d,\n" +
                "	DateTime = DateTime.MinValue,\n" +
                "	NullableDateTime = null,\n" +
                "	Enum = DateTimeKind.Unspecified\n" +
                "};");
        }

        [Fact]
        public void ShouldDumpEnumerable()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();

            // Act
            var dump = ObjectDumperCSharp.Dump(persons);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var listOfPersons = new List<Person>\r\n" +
                "{\r\n" +
                "  new Person\r\n" +
                "  {\r\n" +
                "    Name = \"Person 1\",\r\n" +
                "    Char = '',\r\n" +
                "    Age = 3,\r\n" +
                "    GetOnly = 11,\r\n" +
                "    Bool = false,\r\n" +
                "    Byte = 0,\r\n" +
                "    ByteArray = new byte[]\r\n" +
                "    {\r\n      1,\r\n      2,\r\n      3,\r\n      4\r\n    },\r\n    SByte = 0,\r\n    Float = 0f,\r\n    Uint = 0u,\r\n    Long = 0L,\r\n    ULong = 0UL,\r\n    Short = 0,\r\n    UShort = 0,\r\n    Decimal = 0m,\r\n    Double = 0d,\r\n    DateTime = DateTime.MinValue,\r\n    NullableDateTime = null,\r\n    Enum = DateTimeKind.Unspecified\r\n" +
                "  },\r\n" +
                "  new Person\r\n" +
                "  {\r\n" +
                "    Name = \"Person 2\",\r\n" +
                "    Char = '',\r\n" +
                "    Age = 3,\r\n" +
                "    GetOnly = 11,\r\n" +
                "    Bool = false,\r\n" +
                "    Byte = 0,\r\n" +
                "    ByteArray = new byte[]\r\n" +
                "    {\r\n" +
                "      1,\r\n" +
                "      2,\r\n" +
                "      3,\r\n" +
                "      4\r\n" +
                "    },\r\n" +
                "    SByte = 0,\r\n" +
                "    Float = 0f,\r\n" +
                "    Uint = 0u,\r\n" +
                "    Long = 0L,\r\n" +
                "    ULong = 0UL,\r\n" +
                "    Short = 0,\r\n" +
                "    UShort = 0,\r\n" +
                "    Decimal = 0m,\r\n" +
                "    Double = 0d,\r\n" +
                "    DateTime = DateTime.MinValue,\r\n" +
                "    NullableDateTime = null,\r\n" +
                "    Enum = DateTimeKind.Unspecified\r\n" +
                "  }\r\n" +
                "};");
        }

        [Fact]
        public void ShouldDumpEnumerable_EmptyCollection()
        {
            // Arrange
            var persons = new List<Person>();

            // Act
            var dump = ObjectDumperCSharp.Dump(persons);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var listOfPersons = new List<Person>\r\n" +
                "{\r\n" +
                "};");
        }

        [Fact]
        public void ShouldDumpEnumerable_NestedEmptyCollections()
        {
            // Arrange
            var objectWithArrays = new ObjectWithArrays
            {
                IntArray = new int[] { },
                StringArray = new string[] { },
            };

            // Act
            var dump = ObjectDumperCSharp.Dump(objectWithArrays);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var objectWithArrays = new ObjectWithArrays\r\n" +
                "{\r\n" +
                "  IntArray = new int[]\r\n" +
                "  {\r\n" +
                "  },\r\n" +
                "  StringArray = new string[]\r\n" +
                "  {\r\n" +
                "  }\r\n" +
                "};");
        }
        [Fact]
        public void ShouldDumpException()
        {
            // Arrange
            var ex = new KeyNotFoundException("message text");
            var options = new DumpOptions { IgnoreDefaultValues = true };

            // Act
            var dump = ObjectDumperCSharp.Dump(ex, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var keyNotFoundException = new KeyNotFoundException\r\n" +
                "{\r\n" +
                "  Message = \"message text\",\r\n" +
                "  Data = new ListDictionaryInternal\r\n" +
                "  {\r\n" +
                "  },\r\n" +
                "  HResult = -2146232969\r\n" +
                "};");
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
            var dump = ObjectDumperCSharp.Dump(ex, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();


#if NETCORE
            dump.Should().Be(
               "var keyNotFoundException = new KeyNotFoundException\r\n" +
               "{\r\n" +
               "  TargetSite = new RuntimeMethodInfo\r\n" +
               "  {\r\n" +
               "    Name = \"ShouldDumpExceptionAfterThrow\",\r\n" +
               "    DeclaringType = typeof(ObjectDumperCSharpTests),\r\n" +
               "    ReflectedType = typeof(ObjectDumperCSharpTests),\r\n" +
               "    MemberType = MemberTypes.Method,\r\n" +
               "    IsSecurityCritical = true,\r\n" +
               "    MethodHandle = new RuntimeMethodHandle\r\n" +
               "    {\r\n" +
               "      Value = new IntPtr\r\n" +
               "      {\r\n" +
               "      }\r\n" +
               "    },\r\n" +
               "    Attributes = MethodAttributes.PrivateScope | MethodAttributes.Public | MethodAttributes.HideBySig,\r\n" +
               "    CallingConvention = CallingConventions.Standard | CallingConventions.HasThis,\r\n" +
               "    ReturnType = typeof(void),\r\n" +
               "    ReturnTypeCustomAttributes = new RuntimeParameterInfo\r\n" +
               "    {\r\n" +
               "      ParameterType = typeof(void),\r\n" +
               "      HasDefaultValue = true,\r\n" +
               "      Member = null, // Circular reference detected\r\n" +
               "      Position = -1\r\n" +
               "    },\r\n" +
               "    ReturnParameter = new RuntimeParameterInfo\r\n" +
               "    {\r\n" +
               "      ParameterType = typeof(void),\r\n" +
               "      HasDefaultValue = true,\r\n" +
               "      Member = null, // Circular reference detected\r\n" +
               "      Position = -1\r\n" +
               "    },\r\n" +
               "    IsHideBySig = true,\r\n" +
               "    IsPublic = true\r\n" +
               "  },\r\n" +
               "  Message = \"message text\",\r\n" +
               "  Data = new ListDictionaryInternal\r\n" +
               "  {\r\n" +
               "  },\r\n" +
               "  Source = \"ObjectDumper.Tests\",\r\n" +
               "  HResult = -2146232969\r\n" +
               "};");
#endif
        }

        [Fact]
        public void ShouldDumpNestedObjects()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();
            var organization = new Organization { Name = "superdev gmbh", Persons = persons };

            // Act
            var dump = ObjectDumperCSharp.Dump(organization);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var organization = new Organization\r\n" +
                "{\r\n" +
                "  Name = \"superdev gmbh\",\r\n" +
                "  Persons = new List<Person>\r\n" +
                "  {\r\n" +
                "    new Person\r\n" +
                "    {\r\n" +
                "      Name = \"Person 1\",\r\n" +
                "      Char = '',\r\n" +
                "      Age = 3,\r\n" +
                "      GetOnly = 11,\r\n" +
                "      Bool = false,\r\n" +
                "      Byte = 0,\r\n" +
                "      ByteArray = new byte[]\r\n" +
                "      {\r\n" +
                "        1,\r\n" +
                "        2,\r\n" +
                "        3,\r\n" +
                "        4\r\n" +
                "      },\r\n" +
                "      SByte = 0,\r\n" +
                "      Float = 0f,\r\n" +
                "      Uint = 0u,\r\n" +
                "      Long = 0L,\r\n" +
                "      ULong = 0UL,\r\n" +
                "      Short = 0,\r\n" +
                "      UShort = 0,\r\n" +
                "      Decimal = 0m,\r\n" +
                "      Double = 0d,\r\n" +
                "      DateTime = DateTime.MinValue,\r\n" +
                "      NullableDateTime = null,\r\n" +
                "      Enum = DateTimeKind.Unspecified\r\n" +
                "    },\r\n" +
                "    new Person\r\n" +
                "    {\r\n" +
                "      Name = \"Person 2\",\r\n" +
                "      Char = '',\r\n" +
                "      Age = 3,\r\n" +
                "      GetOnly = 11,\r\n" +
                "      Bool = false,\r\n" +
                "      Byte = 0,\r\n" +
                "      ByteArray = new byte[]\r\n" +
                "      {\r\n" +
                "        1,\r\n" +
                "        2,\r\n" +
                "        3,\r\n" +
                "        4\r\n" +
                "      },\r\n" +
                "      SByte = 0,\r\n" +
                "      Float = 0f,\r\n" +
                "      Uint = 0u,\r\n" +
                "      Long = 0L,\r\n" +
                "      ULong = 0UL,\r\n" +
                "      Short = 0,\r\n" +
                "      UShort = 0,\r\n" +
                "      Decimal = 0m,\r\n" +
                "      Double = 0d,\r\n" +
                "      DateTime = DateTime.MinValue,\r\n" +
                "      NullableDateTime = null,\r\n" +
                "      Enum = DateTimeKind.Unspecified\r\n" +
                "    }\r\n" +
                "  },\r\n" +
                "  IsAfterCollection = true\r\n" +
                "};");
        }

        [Fact]
        public void ShouldDumpGenericClass_WithMultipleGenericTypeArguments()
        {
            // Arrange
            var person = PersonFactory.GeneratePersons(count: 1).First();
            var genericClass = new GenericClass<string, float, Person> { Prop1 = "Test", Prop2 = 123.45f, Prop3 = person };

            // Act
            var dump = ObjectDumperCSharp.Dump(genericClass);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var genericClass = new GenericClass<string, float, Person>\r\n" +
                "{\r\n" +
                "  Prop1 = \"Test\",\r\n" +
                "  Prop2 = 123.45f,\r\n" +
                "  Prop3 = new Person\r\n" +
                "  {\r\n" +
                "    Name = \"Person 1\",\r\n" +
                "    Char = '',\r\n" +
                "    Age = 2,\r\n" +
                "    GetOnly = 11,\r\n" +
                "    Bool = false,\r\n    Byte = 0,\r\n    ByteArray = new byte[]\r\n    {\r\n      1,\r\n      2,\r\n      3,\r\n      4\r\n    },\r\n    SByte = 0,\r\n    Float = 0f,\r\n    Uint = 0u,\r\n    Long = 0L,\r\n    ULong = 0UL,\r\n    Short = 0,\r\n    UShort = 0,\r\n    Decimal = 0m,\r\n    Double = 0d,\r\n    DateTime = DateTime.MinValue,\r\n    NullableDateTime = null,\r\n    Enum = DateTimeKind.Unspecified\r\n  }\r\n};");
        }

        [Fact]
        public void ShouldDumpGenericClass_WithNestedGenericTypeArguments()
        {
            // Arrange
            var complexDictionary = new Dictionary<string[,], List<int?[,][]>[,,]>[1]
            {
                new Dictionary<string[,], List<int?[,][]>[,,]>
                {
                }
            };

            // Act
            var dump = ObjectDumperCSharp.Dump(complexDictionary);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var dictionary = new Dictionary<string[,], List<Nullable<int>[,][]>[,,]>[]\r\n{\r\n  new Dictionary<string[,], List<Nullable<int>[,][]>[,,]>\r\n  {\r\n  }\r\n};");
        }

        [Fact]
        public void ShouldDumpObjectWithMaxLevel()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();
            var organization = new Organization { Name = "superdev gmbh", Persons = persons };
            var options = new DumpOptions { MaxLevel = 1 };

            // Act
            var dump = ObjectDumperCSharp.Dump(organization, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be(
                "var organization = new Organization\r\n" +
                "{\r\n" +
                "  Name = \"superdev gmbh\",\r\n" +
                "  Persons = new List<Person>\r\n" +
                "  {\r\n" +
                "  },\r\n" +
                "  IsAfterCollection = true\r\n" +
                "};");
        }

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
            var dump = ObjectDumperCSharp.Dump(person, dumpOptions);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var recursivePerson = new RecursivePerson\r\n" +
                "{\r\n" +
                "  Id = 0,\r\n" +
                "  Parent = null // Circular reference detected\r\n" +
                "};");
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
            var dump = ObjectDumperCSharp.Dump(nestedItemA);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var nestedItemA = new NestedItemA\r\n" +
                "{\r\n" +
                "  Next = new NestedItemB\r\n" +
                "  {\r\n" +
                "    Next = null,\r\n" +
                "    Property = 1\r\n" +
                "  },\r\n" +
                "  Property = 1\r\n" +
                "};");
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
            var dump = ObjectDumperCSharp.Dump(nestedItemA);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var nestedItemA = new NestedItemA\r\n" +
                "{\r\n" +
                "  Next = new NestedItemB\r\n" +
                "  {\r\n" +
                "    Next = null, // Circular reference detected\r\n" +
                "    Property = 1\r\n" +
                "  },\r\n" +
                "  Property = 1\r\n" +
                "};");
        }

        [Fact]
        public void ShouldDumpRecursiveTypes_CircularReference_Case4()
        {
            // Arrange 
            var example1 = new Example { Name = "Name1" };
            var example2 = new Example { Name = "Name2", Reference = example1 };

            // TODO: New test case
            //example1.Reference = example2;

            // TODO: New test case
            //var example3 = new Example { Name = "Name3", Reference = example2 };

            var array = new[] { example1, example2, /*example3*/ };

            // Act
            var dump = ObjectDumperCSharp.Dump(array);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().NotContain("// Circular reference detected");

            dump.Should().Be(
                "var exampleArray = new Example[]\r\n" +
                "{\r\n" +
                "  new Example\r\n" +
                "  {\r\n" +
                "    Name = \"Name1\",\r\n" +
                "    Reference = null\r\n" +
                "  },\r\n" +
                "  new Example\r\n" +
                "  {\r\n" +
                "    Name = \"Name2\",\r\n" +
                "    Reference = new Example\r\n" +
                "    {\r\n" +
                "      Name = \"Name1\",\r\n" +
                "      Reference = null\r\n" +
                "    }\r\n" +
                "  }\r\n" +
                "};");
        }

        [Fact]
        public void ShouldDumpRecursiveTypes_CircularReference_Case5()
        {
            // Arrange 
            var example1 = new Example { Name = "Name1" };
            var example2 = new Example { Name = "Name2", Reference = example1 };
            example1.Reference = example2; // This assignment causes a circular reference

            var array = new[] { example1, example2 };

            // Act
            var dump = ObjectDumperCSharp.Dump(array);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Contain("// Circular reference detected");
            dump.Should().Be(
                "var exampleArray = new Example[]\r\n" +
                "{\r\n" +
                "  new Example\r\n" +
                "  {\r\n" +
                "    Name = \"Name1\",\r\n" +
                "    Reference = new Example\r\n" +
                "    {\r\n" +
                "      Name = \"Name2\",\r\n" +
                "      Reference = null // Circular reference detected\r\n" +
                "    }\r\n" +
                "  },\r\n" +
                "  new Example\r\n" +
                "  {\r\n" +
                "    Name = \"Name2\",\r\n" +
                "    Reference = new Example\r\n" +
                "    {\r\n" +
                "      Name = \"Name1\",\r\n" +
                "      Reference = null // Circular reference detected\r\n" +
                "    }\r\n" +
                "  }\r\n" +
                "};");
        }

        [Fact]
        public void ShouldDumpRecursiveTypes_CircularReference_Case6()
        {
            // Arrange 

            var array = new object[]
            {
                0,
                null,
                2,
                null
            };
            array[1] = array;

            // Act
            var dump = ObjectDumperCSharp.Dump(array);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var objectArray = new object[]\r\n" +
                "{\r\n" +
                "  0,\r\n" +
                "  null, // Circular reference detected\r\n" +  // TODO: No commat at the end of the comment here
                "  2,\r\n" +
                "  null\r\n" +
                "};");
        }

        [Fact]
        public void ShouldExcludeProperties()
        {
            // Arrange
            var testObject = new TestObject();
            var options = new DumpOptions { ExcludeProperties = { "Id", "NonExistent" } };

            // Act
            var dump = ObjectDumperCSharp.Dump(testObject, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("var testObject = new TestObject\r\n{\r\n  NullableDateTime = null\r\n};");
        }

        [Fact]
        public void ShouldOrderProperties()
        {
            // Arrange
            var testObject = new OrderPropertyTestObject();
            var options = new DumpOptions { PropertyOrderBy = p => p.Name };

            // Act
            var dump = ObjectDumperCSharp.Dump(testObject, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("var orderPropertyTestObject = new OrderPropertyTestObject\r\n{\r\n  A = null,\r\n  B = null,\r\n  C = null\r\n};");
        }

        [Fact]
        public void ShouldDumpDateTime_DateTimeKind_Unspecified()
        {
            // Arrange
            var dateTime = new DateTime(2000, 01, 01, 23, 59, 59, DateTimeKind.Unspecified);

            // Act
            var dump = ObjectDumperCSharp.Dump(dateTime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var dateTime = DateTime.ParseExact(\"2000-01-01T23:59:59.0000000\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);");

            var returnedDateTime = DateTime.ParseExact("2000-01-01T23:59:59.0000000", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            returnedDateTime.Should().Be(dateTime);
        }

        [Fact]
        public void ShouldDumpDateTime_DateTimeKind_Utc()
        {
            // Arrange
            var dateTime = new DateTime(2000, 01, 01, 23, 59, 59, DateTimeKind.Utc);

            // Act
            var dump = ObjectDumperCSharp.Dump(dateTime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var dateTime = DateTime.ParseExact(\"2000-01-01T23:59:59.0000000Z\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);");

            var returnedDateTime = DateTime.ParseExact("2000-01-01T23:59:59.0000000Z", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            returnedDateTime.Should().Be(dateTime);
        }

        [Fact]
        public void ShouldDumpTimeSpan()
        {
            // Arrange
            var timeSpan = new TimeSpan(1, 2, 3, 4, 5);

            // Act
            var dump = ObjectDumperCSharp.Dump(timeSpan);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var timeSpan = TimeSpan.ParseExact(\"1.02:03:04.0050000\", \"c\", CultureInfo.InvariantCulture, TimeSpanStyles.None);");

            var returnedTimeSpan = TimeSpan.ParseExact("1.02:03:04.0050000", "c", CultureInfo.InvariantCulture, TimeSpanStyles.None);
            returnedTimeSpan.Should().Be(timeSpan);
        }

        [Fact]
        public void ShouldDumpTimeSpan_Zero()
        {
            // Arrange
            var timeSpan = TimeSpan.Zero;

            // Act
            var dump = ObjectDumperCSharp.Dump(timeSpan);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var timeSpan = TimeSpan.Zero;");

            var returnedTimeSpan = TimeSpan.Zero;
            returnedTimeSpan.Should().Be(timeSpan);
        }

        [Fact]
        public void ShouldDumpTimeSpan_MinValue()
        {
            // Arrange
            var timeSpan = TimeSpan.MinValue;

            // Act
            var dump = ObjectDumperCSharp.Dump(timeSpan);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var timeSpan = TimeSpan.MinValue;");

            var returnedTimeSpan = TimeSpan.MinValue;
            returnedTimeSpan.Should().Be(timeSpan);
        }

        [Fact]
        public void ShouldDumpTimeSpan_MaxValue()
        {
            // Arrange
            var timeSpan = TimeSpan.MaxValue;

            // Act
            var dump = ObjectDumperCSharp.Dump(timeSpan);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var timeSpan = TimeSpan.MaxValue;");

            var returnedTimeSpan = TimeSpan.MaxValue;
            returnedTimeSpan.Should().Be(timeSpan);
        }

        [Fact]
        public void ShouldDumpTimeSpan_Negative()
        {
            // Arrange
            var timeSpan = (new TimeSpan(1, 2, 3, 4, 5)).Negate();

            // Act
            var dump = ObjectDumperCSharp.Dump(timeSpan);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var timeSpan = TimeSpan.ParseExact(\"-1.02:03:04.0050000\", \"c\", CultureInfo.InvariantCulture, TimeSpanStyles.None);");

            var returnedTimeSpan = TimeSpan.ParseExact("-1.02:03:04.0050000", "c", CultureInfo.InvariantCulture, TimeSpanStyles.None);
            returnedTimeSpan.Should().Be(timeSpan);
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
            var dump = ObjectDumperCSharp.Dump(dateTime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be($"var dateTime = DateTime.ParseExact(\"2000-01-01T23:59:59.0000000{GetUtcOffsetString()}\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);");

            var returnedDateTime = DateTime.ParseExact($"2000-01-01T23:59:59.0000000{GetUtcOffsetString()}", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            returnedDateTime.Should().Be(dateTime);
        }

        [Fact]
        public void ShouldDumpDateTimeMinValue()
        {
            // Arrange
            var datetime = DateTime.MinValue;

            // Act
            var dump = ObjectDumperCSharp.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var dateTime = DateTime.MinValue;");
        }

        [Fact]
        public void ShouldDumpDateTimeMaxValue()
        {
            // Arrange
            var datetime = DateTime.MaxValue;

            // Act
            var dump = ObjectDumperCSharp.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var dateTime = DateTime.MaxValue;");
        }

        [Fact]
        public void ShouldDumpNullableObject()
        {
            // Arrange
            DateTime? datetime = null;

            // Act
            var dump = ObjectDumperCSharp.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var x = null;");
        }

        [Fact]
        public void ShouldDumpEnum()
        {
            // Arrange
            var dateTimeKind = DateTimeKind.Utc;

            // Act
            var dump = ObjectDumperCSharp.Dump(dateTimeKind);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var dateTimeKind = DateTimeKind.Utc;");
        }

        [Fact]
        public void ShouldDumpEnum_WithMultipleFlags()
        {
            // Arrange
            var enumWithFlags = EnumWithFlags.Private | EnumWithFlags.Public | EnumWithFlags.Static;

            // Act
            var dump = ObjectDumperCSharp.Dump(enumWithFlags);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var enumWithFlags = EnumWithFlags.Private | EnumWithFlags.Public | EnumWithFlags.Static;");
        }

        [Fact]
        public void ShouldDumpGuid()
        {
            // Arrange
            var guid = new Guid("024CC229-DEA0-4D7A-9FC8-722E3A0C69A3");

            // Act
            var dump = ObjectDumperCSharp.Dump(guid);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var guid = new Guid(\"024cc229-dea0-4d7a-9fc8-722e3a0c69a3\");");
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
            var dump = ObjectDumperCSharp.Dump(dictionary);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var dictionary = new Dictionary<int, string>\r\n" +
                "{\r\n" +
                "  { 1, \"Value1\" },\r\n" +
                "  { 2, \"Value2\" },\r\n" +
                "  { 3, \"Value3\" }\r\n" +
                "};");
        }

        [Fact]
        public void ShouldDumpArray_OneDimensional()
        {
            // Arrange
            var array = new string[] { "aaa", "bbb" };

            // Act
            var dump = ObjectDumperCSharp.Dump(array);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var stringArray = new string[]\r\n{\r\n  \"aaa\",\r\n  \"bbb\"\r\n};");
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
            var dump = ObjectDumperCSharp.Dump(array);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var array = new int[3, 2]\r\n" +
                "{\r\n" +
                "  {1, 2},\r\n" +
                "  {3, 4},\r\n" +
                "  {5, 6}\r\n" +
                "};");
        }

        [Fact]
        public void ShouldEscapeStrings()
        {
            // Arrange
            var expectedPerson = new Person
            {
                Name = "Boris \"The Blade\", \\GANGSTA\\ aka 'The Bullet Dodger' \a \b \f \r\nOn a new\twith tab \v \0"
            };
            var dumpOptions = new DumpOptions
            {
                SetPropertiesOnly = true,
                IgnoreDefaultValues = true,
                MaxLevel = 1,
                ExcludeProperties = { "ByteArray" }
            };

            // Act
            var dump = ObjectDumperCSharp.Dump(expectedPerson, dumpOptions);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            /*dump.Should().Be(
                "var person = new Person\r\n" +
                "{\r\n" +
                "  Name = \"Boris \\\"The Blade\\\", \\\\GANGSTA\\\\ aka \\\'The Bullet Dodger\\\' \\a \\b \\f \r\nOn a new\\twith tab \\v \\0\"\r\n" +
                "};");*/

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
            var dump = ObjectDumperCSharp.Dump(offset);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be($"var dateTimeOffset = DateTimeOffset.ParseExact(\"2000-01-01T23:59:59.0000000+00:00\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);");
        }

        [Fact]
        public void ShouldDumpDateTimeOffsetMinValue()
        {
            // Arrange
            var offset = DateTimeOffset.MinValue;

            // Act
            var dump = ObjectDumperCSharp.Dump(offset);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be($"var dateTimeOffset = DateTimeOffset.MinValue;");
        }

        [Fact]
        public void ShouldDumpDateTimeOffsetMaxValue()
        {
            // Arrange
            var offset = DateTimeOffset.MaxValue;

            // Act
            var dump = ObjectDumperCSharp.Dump(offset);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be($"var dateTimeOffset = DateTimeOffset.MaxValue;");
        }

        [Fact]
        public void ShouldDumpCultureInfo()
        {
            // Arrange
            var cultureInfo = new CultureInfo("de-CH");

            // Act
            var dump = ObjectDumperCSharp.Dump(cultureInfo);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var cultureInfo = new CultureInfo(\"de-CH\");");
        }

        [Fact]
        public void ShouldDumpRuntimeType()
        {
            // Arrange
            var type = typeof(Person);

            // Act
            var dump = ObjectDumperCSharp.Dump(type);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var runtimeType = typeof(Person);");
        }

        [Fact]
        public void ShouldDumpRuntimeType_BuiltInType()
        {
            // Arrange
            var type = typeof(string);

            // Act
            var dump = ObjectDumperCSharp.Dump(type);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var runtimeType = typeof(string);");
        }

        [Fact]
        public void ShouldDumpTypes_UsingCustomTypeFormatter()
        {
            // Arrange
            var typeMap = new TypeMap
            {
                Map = new Dictionary<Type, Type>
                {
                    { typeof(KeyTypeOne), typeof(HandlerTypeOne) },
                    { typeof(KeyTypeTwo), typeof(HandlerTypeTwo) }
                }
            };

            var dumpOptions = new DumpOptions
            {
                CustomTypeFormatter = new Dictionary<Type, Func<Type, string>>()
                {
                    { typeof(Type), o => $"typeof({o.Name})" }
                }
            };

            // Act
            var dump = ObjectDumperCSharp.Dump(typeMap, dumpOptions);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var typeMap = new TypeMap\r\n{\r\n  Map = new Dictionary<Type, Type>\r\n  {\r\n    { typeof(KeyTypeOne), typeof(HandlerTypeOne) },\r\n    { typeof(KeyTypeTwo), typeof(HandlerTypeTwo) }\r\n  }\r\n};");
        }

        [Fact]
        public void ShouldDumpCustomConstructor()
        {
            // Arrange 
            var myObj = ObjectWithComplexConstructorFactory.BuildIt("string", 1, 32.4F);

            var dumpOptions = new DumpOptions();
            dumpOptions.CustomInstanceFormatters.AddFormatter<ObjectWithComplexConstructorFactory.ObjectWithComplexConstructor>(a => $"ObjectWithComplexConstructorFactory.BuildIt(\"{a.Foo}\", {a.Bar}, {a.Baz})");

            // Act
            var dump = ObjectDumperCSharp.Dump(myObj, dumpOptions);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var objectWithComplexConstructor = ObjectWithComplexConstructorFactory.BuildIt(\"string\", 1, 32.4);");
        }

        [Fact]
        public void ShouldDumpTrimmedCustomConstructor()
        {
            // Arrange 
            var myObj = ObjectWithComplexConstructorFactory.BuildIt("string", 1, 32.4F);

            var dumpOptions = new DumpOptions
            {
                TrimInitialVariableName = true,
                TrimTrailingColonName = true
            };

            dumpOptions.CustomInstanceFormatters.AddFormatter<ObjectWithComplexConstructorFactory.ObjectWithComplexConstructor>(a => $"ObjectWithComplexConstructorFactory.BuildIt(\"{a.Foo}\", {a.Bar}, {a.Baz})");

            // Act
            var dump = ObjectDumperCSharp.Dump(myObj, dumpOptions);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("ObjectWithComplexConstructorFactory.BuildIt(\"string\", 1, 32.4)");
        }

        [Fact]
        public void ShouldDumpObjectWithIndexer_IntegerArray_IgnoredByDefaultDumpOptions()
        {
            // Arrange
            var tempRecord = new TempRecord
            {
                [0] = 58.3f,
                [1] = 60.1f,
            };

            // Act
            var dump = ObjectDumperCSharp.Dump(tempRecord);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var tempRecord = new TempRecord\r\n{\r\n  AProp = 0,\r\n  ZProp = null\r\n};");
        }

        [Fact]
        public void ShouldDumpObjectWithIndexer_IntegerArray()
        {
            // Arrange
            var tempRecord = new TempRecord
            {
                AProp = 99,
                [0] = 58.3f,
                [1] = 60.1f,
                ZProp = "ZZ"
            };

            var dumpOptions = new DumpOptions
            {
                IgnoreIndexers = false
            };

            // Act
            var dump = ObjectDumperCSharp.Dump(tempRecord, dumpOptions);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var tempRecord = new TempRecord\r\n{\r\n  AProp = 99,\r\n  [0] = 58.3f,\r\n  [1] = 60.1f,\r\n  ZProp = \"ZZ\"\r\n};");
        }

        [Fact]
        public void ShouldDumpObjectWithIndexer_NonIntegerArray_Ignored()
        {
            // Arrange
            var viewModelValidation = new ViewModelValidation
            {
                ["property1"] = new List<string> { "error1" },
                ["property2"] = new List<string> { "error2" },
            };

            var dumpOptions = new DumpOptions
            {
                IgnoreIndexers = false
            };

            // Act
            var dump = ObjectDumperCSharp.Dump(viewModelValidation, dumpOptions);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var viewModelValidation = new ViewModelValidation\r\n" +
                "{\r\n" +
                "  Errors = null, // Circular reference detected\r\n" +
                "};");
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
            var dump = ObjectDumperCSharp.Dump(x509ChainStatusStruct);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var x509ChainStatus = new X509ChainStatus\r\n" +
                "{\r\n" +
                "  Status = X509ChainStatusFlags.NoError,\r\n" +
                "  StatusInformation = \"Test status\"\r\n" +
                "};");
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
            var dump = ObjectDumperCSharp.Dump(dynamicObject);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var x = new\r\n" +
                "{\r\n" +
                "  IntProperty = 10,\r\n" +
                "  StringProperty = \"hello\",\r\n" +
                "  DoubleProperty = 3.14d\r\n" +
                "};");
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
            var dump = ObjectDumperCSharp.Dump(list);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var listOfobjects = new List<object>\r\n" +
                "{\r\n" +
                "  new\r\n" +
                "  {\r\n" +
                "    Prop = new\r\n" +
                "    {\r\n" +
                "      SomeInnerProp = \"test_test_test\"\r\n" +
                "    }\r\n" +
                "  },\r\n" +
                "  new\r\n" +
                "  {\r\n" +
                "    Prop = new\r\n" +
                "    {\r\n" +
                "      SomeInnerProp = \"test_test_test\"\r\n" +
                "    }\r\n" +
                "  }\r\n" +
                "};");
        }

        [Fact]
        public void ShouldDumpAnonymousObject_Enumerable()
        {
            // Arrange 
            var obj = new { Prop = new { SomeInnerProp = "test_test_test" } };
            var list = Enumerable.Range(0, 2).Select(_ => obj).ToList();

            // Act
            var dump = ObjectDumperCSharp.Dump(list);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var listOfdynamics = new List<dynamic>\r\n" +
                "{\r\n" +
                "  new\r\n" +
                "  {\r\n" +
                "    Prop = new\r\n" +
                "    {\r\n" +
                "      SomeInnerProp = \"test_test_test\"\r\n" +
                "    }\r\n" +
                "  },\r\n" +
                "  new\r\n" +
                "  {\r\n" +
                "    Prop = new\r\n" +
                "    {\r\n" +
                "      SomeInnerProp = \"test_test_test\"\r\n" +
                "    }\r\n" +
                "  }\r\n" +
                "};");
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
            string dump = ObjectDumperCSharp.Dump(expandoObject);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var expandoObject = new ExpandoObject\r\n" +
                "{\r\n" +
                "  { \"IntProperty\", 10 },\r\n" +
                "  { \"StringProperty\", \"hello\" },\r\n" +
                "  { \"DoubleProperty\", 3.14d }\r\n" +
                "};");
        }

#if NETCORE
        [Fact]
        public void ShouldDumpValueTuple_Arity0()
        {
            // Arrange 
            var valueTuple = ValueTuple.Create();

            // Act
            var dump = ObjectDumperCSharp.Dump(valueTuple);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var valueTuple = ValueTuple.Create();");
        }

        [Fact]
        public void ShouldDumpValueTuple_Arity3()
        {
            // Arrange 
            var valueTuple = (1, "Bill", "Gates");

            // Act
            var dump = ObjectDumperCSharp.Dump(valueTuple);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var valueTuple = (1, \"Bill\", \"Gates\");");
        }

        [Fact]
        public void ShouldDumpValueTuple_WithDefaultValue()
        {
            // Arrange 
            (int Id, string FirstName, string LastName) valueTuple = default;

            // Act
            var dump = ObjectDumperCSharp.Dump(valueTuple);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var valueTuple = (0, null, null);");
        }

        [Fact]
        public void ShouldDumpEnumerable_ValueTuples()
        {
            // Arrange 
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();
            var valueTuples = persons.Select(s => (s.Name, s.Age)).ToList();

            // Act
            var dump = ObjectDumperCSharp.Dump(valueTuples);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var list = new List<(string, int)>\r\n" +
                "{\r\n" +
                "  (\"Person 1\", 3),\r\n" +
                "  (\"Person 2\", 3)\r\n" +
                "};");
        }
#endif
        [Fact]
        public void ShouldDumpMailMessage()
        {
            // Arrange
            var mailmessage = new MailMessage("sender@mail.com", "receiver@mail.com", "Subject", "Body");

            // Act
            var dump = ObjectDumperCSharp.Dump(mailmessage);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var mailMessage = new MailMessage\r\n" +
                "{\r\n" +
                "  From = new MailAddress\r\n" +
                "  {\r\n" +
                "    DisplayName = \"\",\r\n" +
                "    User = \"sender\",\r\n" +
                "    Host = \"mail.com\",\r\n" +
                "    Address = \"sender@mail.com\"\r\n" +
                "  },\r\n" +
                "  Sender = null,\r\n" +
                "  ReplyTo = null,\r\n" +
                "  ReplyToList = new MailAddressCollection\r\n" +
                "  {\r\n" +
                "  },\r\n" +
                "  To = new MailAddressCollection\r\n" +
                "  {\r\n" +
                "    new MailAddress\r\n" +
                "    {\r\n" +
                "      DisplayName = \"\",\r\n" +
                "      User = \"receiver\",\r\n" +
                "      Host = \"mail.com\",\r\n" +
                "      Address = \"receiver@mail.com\"\r\n" +
                "    }\r\n" +
                "  },\r\n" +
                "  Bcc = new MailAddressCollection\r\n" +
                "  {\r\n" +
                "  },\r\n" +
                "  CC = new MailAddressCollection\r\n" +
                "  {\r\n" +
                "  },\r\n" +
                "  Priority = MailPriority.Normal,\r\n" +
                "  DeliveryNotificationOptions = DeliveryNotificationOptions.None,\r\n" +
                "  Subject = \"Subject\",\r\n" +
                "  SubjectEncoding = null,\r\n" +
                "  Headers = new HeaderCollection\r\n" +
                "  {\r\n" +
                "  },\r\n" +
                "  HeadersEncoding = null,\r\n" +
                "  Body = \"Body\",\r\n" +
#if NETFRAMEWORK
                "  BodyEncoding = new ASCIIEncoding\r\n" +
#else
                "  BodyEncoding = new ASCIIEncodingSealed\r\n" +
#endif
                "  {\r\n" +
                "    IsSingleByte = true,\r\n" +
#if NET5_0_OR_GREATER
                "    Preamble = \"{NotSupportedException: Specified method is not supported.}\",\r\n" +
#endif
                "    BodyName = \"us-ascii\",\r\n" +
                "    EncodingName = \"US-ASCII\",\r\n" +
                "    HeaderName = \"us-ascii\",\r\n" +
                "    WebName = \"us-ascii\",\r\n" +
                "    WindowsCodePage = 1252,\r\n" +
                "    IsBrowserDisplay = false,\r\n" +
                "    IsBrowserSave = false,\r\n" +
                "    IsMailNewsDisplay = true,\r\n" +
                "    IsMailNewsSave = true,\r\n" +
                "    EncoderFallback = new EncoderReplacementFallback\r\n" +
                "    {\r\n" +
                "      DefaultString = \"?\",\r\n" +
                "      MaxCharCount = 1\r\n" +
                "    },\r\n" +
                "    DecoderFallback = new DecoderReplacementFallback\r\n" +
                "    {\r\n" +
                "      DefaultString = \"?\",\r\n" +
                "      MaxCharCount = 1\r\n" +
                "    },\r\n" +
                "    IsReadOnly = true,\r\n" +
                "    CodePage = 20127\r\n" +
                "  },\r\n" +
                "  BodyTransferEncoding = TransferEncoding.Unknown,\r\n" +
                "  IsBodyHtml = false,\r\n" +
                "  Attachments = new AttachmentCollection\r\n" +
                "  {\r\n" +
                "  },\r\n" +
                "  AlternateViews = new AlternateViewCollection\r\n" +
                "  {\r\n" +
                "  }\r\n" +
                "};");
        }

#if NET5_0_OR_GREATER
        [Fact]
        public void ShouldDumpRecordType_RecordClass()
        {
            // Arrange
            var recordClass = new RecordClass(Property1: 20d, Property2: "Test");

            // Act
            var dump = ObjectDumperCSharp.Dump(recordClass);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var recordClass = new RecordClass(\r\n" +
                "  Property1: 20d,\r\n" +
                "  Property2: \"Test\"\r\n" +
                ");");
        }

        [Fact]
        public void ShouldDumpRecordType_EmptyRecordClass()
        {
            // Arrange
            var emptyRecordClass = new EmptyRecordClass();

            // Act
            var dump = ObjectDumperCSharp.Dump(emptyRecordClass);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var emptyRecordClass = new EmptyRecordClass();");
        }

        [Fact]
        public void ShouldDumpRecordType_WithAdditionalProperties()
        {
            // Arrange
            var dailyTemperature = new DailyTemperature(HighTemp: 20d, LowTemp: -2d)
            {
                InitOnlyProperty = "init",
                ReadWriteProperty = true,
            };

            // Act
            var dump = ObjectDumperCSharp.Dump(dailyTemperature);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var dailyTemperature = new DailyTemperature(\r\n" +
                "  HighTemp: 20d,\r\n" +
                "  LowTemp: -2d\r\n" +
                ")\r\n" +
                "{\r\n" +
                "  InitOnlyProperty = \"init\",\r\n" +
                "  ReadWriteProperty = true\r\n" +
                "};");
        }

        [Fact]
        public void ShouldDumpRecordType_WithObjectParameter()
        {
            // Arrange
            var recordWithNestedObject = new RecordWithNestedObject(Age: 20, Organization: new Organization { Name = "Test Inc." });

            // Act
            var dump = ObjectDumperCSharp.Dump(recordWithNestedObject);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var recordWithNestedObject = new RecordWithNestedObject(\r\n" +
                "  Age: 20,\r\n" +
                "  Organization: new Organization\r\n" +
                "  {\r\n" +
                "    Name = \"Test Inc.\",\r\n" +
                "    Persons = new HashSet<Person>\r\n" +
                "    {\r\n" +
                "    },\r\n" +
                "    IsAfterCollection = true\r\n" +
                "  }\r\n" +
                ");");
        }

        [Fact]
        public void ShouldDumpRecordType_Sprint()
        {
            // Arrange
            var sprint = new Sprint(
                SprintId: 12,
                StartDate: DateTimeOffset.ParseExact("2021-02-18T00:00:00.0000000+01:00", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                EndDate: DateTimeOffset.ParseExact("2021-03-03T00:00:00.0000000+01:00", "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
            );

            // Act
            var dump = ObjectDumperCSharp.Dump(sprint);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var sprint = new Sprint(\r\n" +
                "  SprintId: 12,\r\n" +
                "  StartDate: DateTimeOffset.ParseExact(\"2021-02-18T00:00:00.0000000+01:00\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),\r\n" +
                "  EndDate: DateTimeOffset.ParseExact(\"2021-03-03T00:00:00.0000000+01:00\", \"O\", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)\r\n" +
                ");");
        }
#endif

#if NET7_0_OR_GREATER

        [Fact]
        public void ShouldDumpRecordType_CtorlessReadonlyRecord()
        {
            // Arrange
            var ctorlessReadonlyRecord = new CtorlessReadonlyRecord()
            {
                Name = "Test"
            };

            // Act
            var dump = ObjectDumperCSharp.Dump(ctorlessReadonlyRecord);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(
                "var ctorlessReadonlyRecord = new CtorlessReadonlyRecord()\r\n" +
                "{\r\n" +
                "  Name = \"Test\"\r\n" +
                "};");
        }
#endif
    }
}
