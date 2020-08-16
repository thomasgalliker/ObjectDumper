using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class ObjectDumperCSharpTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ObjectDumperCSharpTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
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
            dump.Should().Be("var person = new Person\r\n{\r\n  Name = \"Person 1\",\r\n  Char = '',\r\n  Age = 2,\r\n  GetOnly = 11,\r\n  Bool = false,\r\n  Byte = 0,\r\n  ByteArray = new Byte[]\r\n  {\r\n    1,\r\n    2,\r\n    3,\r\n    4\r\n  },\r\n  SByte = 0,\r\n  Float = 0f,\r\n  Uint = 0,\r\n  Long = 0L,\r\n  ULong = 0L,\r\n  Short = 0,\r\n  UShort = 0,\r\n  Decimal = 0m,\r\n  Double = 0d,\r\n  DateTime = DateTime.MinValue,\r\n  NullableDateTime = null,\r\n  Enum = System.DateTimeKind.Unspecified\r\n};");
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
            var dump = ObjectDumperCSharp.Dump(person, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var person = new Person\n{\n	Name = \"Thomas\",\n	Char = '',\n	Age = 30,\n	Bool = false,\n	Byte = 0,\n	ByteArray = new Byte[]\n	{\n		1,\n		2,\n		3,\n		4\n	},\n	SByte = 0,\n	Float = 0f,\n	Uint = 0,\n	Long = 0L,\n	ULong = 0L,\n	Short = 0,\n	UShort = 0,\n	Decimal = 0m,\n	Double = 0d,\n	DateTime = DateTime.MinValue,\n	NullableDateTime = null,\n	Enum = System.DateTimeKind.Unspecified\n};");
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
                "    ByteArray = new Byte[]\r\n" +
                "    {\r\n      1,\r\n      2,\r\n      3,\r\n      4\r\n    },\r\n    SByte = 0,\r\n    Float = 0f,\r\n    Uint = 0,\r\n    Long = 0L,\r\n    ULong = 0L,\r\n    Short = 0,\r\n    UShort = 0,\r\n    Decimal = 0m,\r\n    Double = 0d,\r\n    DateTime = DateTime.MinValue,\r\n    NullableDateTime = null,\r\n    Enum = System.DateTimeKind.Unspecified\r\n" +
                "  },\r\n" +
                "  new Person\r\n" +
                "  {\r\n" +
                "    Name = \"Person 2\",\r\n" +
                "    Char = '',\r\n" +
                "    Age = 3,\r\n" +
                "    GetOnly = 11,\r\n" +
                "    Bool = false,\r\n" +
                "    Byte = 0,\r\n" +
                "    ByteArray = new Byte[]\r\n" +
                "    {\r\n" +
                "      1,\r\n" +
                "      2,\r\n" +
                "      3,\r\n" +
                "      4\r\n" +
                "    },\r\n" +
                "    SByte = 0,\r\n" +
                "    Float = 0f,\r\n" +
                "    Uint = 0,\r\n" +
                "    Long = 0L,\r\n" +
                "    ULong = 0L,\r\n" +
                "    Short = 0,\r\n" +
                "    UShort = 0,\r\n" +
                "    Decimal = 0m,\r\n" +
                "    Double = 0d,\r\n" +
                "    DateTime = DateTime.MinValue,\r\n" +
                "    NullableDateTime = null,\r\n" +
                "    Enum = System.DateTimeKind.Unspecified\r\n" +
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
            dump.Should().Be(@"var keyNotFoundException = new KeyNotFoundException
{
  Message = ""message text"",
  Data = new ListDictionaryInternal
  {
  },
  HResult = -2146232969
};");
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
            var options = new DumpOptions { IgnoreDefaultValues = true, ExcludeProperties = { "CustomAttributes", "Module", "StackTrace" } };

            // Act
            var dump = ObjectDumperCSharp.Dump(ex, options);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be(@"var keyNotFoundException = new KeyNotFoundException
{
  Message = ""message text"",
  Data = new ListDictionaryInternal
  {
  },
  TargetSite = new RuntimeMethodInfo
  {
    Name = ""ShouldDumpExceptionAfterThrow"",
    DeclaringType = typeof(ObjectDumping.Tests.ObjectDumperCSharpCSharpTests),
    ReflectedType = typeof(ObjectDumping.Tests.ObjectDumperCSharpCSharpTests),
    MemberType = System.Reflection.MemberTypes.Method,
    MetadataToken = 100663330,
    IsSecurityCritical = true,
    MethodHandle = new RuntimeMethodHandle
    {
      Value = new IntPtr
      {
      }
    },
    Attributes = System.Reflection.MethodAttributes.PrivateScope, Public, HideBySig,
    CallingConvention = System.Reflection.CallingConventions.Standard, HasThis,
    ReturnType = typeof(System.Void),
    ReturnTypeCustomAttributes = new RuntimeParameterInfo
    {
      ParameterType = typeof(System.Void),
      HasDefaultValue = true,
      MetadataToken = 134217728,
      Position = -1,
    },
    IsPublic = true,
    IsHideBySig = true
  },
  Source = ""ObjectDumper.Tests"",
  HResult = -2146232969
};");
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
                "      ByteArray = new Byte[]\r\n" +
                "      {\r\n" +
                "        1,\r\n" +
                "        2,\r\n" +
                "        3,\r\n" +
                "        4\r\n" +
                "      },\r\n" +
                "      SByte = 0,\r\n" +
                "      Float = 0f,\r\n" +
                "      Uint = 0,\r\n" +
                "      Long = 0L,\r\n" +
                "      ULong = 0L,\r\n" +
                "      Short = 0,\r\n" +
                "      UShort = 0,\r\n" +
                "      Decimal = 0m,\r\n" +
                "      Double = 0d,\r\n" +
                "      DateTime = DateTime.MinValue,\r\n" +
                "      NullableDateTime = null,\r\n" +
                "      Enum = System.DateTimeKind.Unspecified\r\n" +
                "    },\r\n" +
                "    new Person\r\n" +
                "    {\r\n" +
                "      Name = \"Person 2\",\r\n" +
                "      Char = '',\r\n" +
                "      Age = 3,\r\n" +
                "      GetOnly = 11,\r\n" +
                "      Bool = false,\r\n" +
                "      Byte = 0,\r\n" +
                "      ByteArray = new Byte[]\r\n" +
                "      {\r\n" +
                "        1,\r\n" +
                "        2,\r\n" +
                "        3,\r\n" +
                "        4\r\n" +
                "      },\r\n" +
                "      SByte = 0,\r\n" +
                "      Float = 0f,\r\n" +
                "      Uint = 0,\r\n" +
                "      Long = 0L,\r\n" +
                "      ULong = 0L,\r\n" +
                "      Short = 0,\r\n" +
                "      UShort = 0,\r\n" +
                "      Decimal = 0m,\r\n" +
                "      Double = 0d,\r\n" +
                "      DateTime = DateTime.MinValue,\r\n" +
                "      NullableDateTime = null,\r\n" +
                "      Enum = System.DateTimeKind.Unspecified\r\n" +
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
            dump.Should().Be("var genericClass = new GenericClass<String, Single, Person>\r\n{\r\n  Prop1 = \"Test\",\r\n  Prop2 = 123.45f,\r\n  Prop3 = new Person\r\n  {\r\n    Name = \"Person 1\",\r\n    Char = '',\r\n    Age = 2,\r\n    GetOnly = 11,\r\n    Bool = false,\r\n    Byte = 0,\r\n    ByteArray = new Byte[]\r\n    {\r\n      1,\r\n      2,\r\n      3,\r\n      4\r\n    },\r\n    SByte = 0,\r\n    Float = 0f,\r\n    Uint = 0,\r\n    Long = 0L,\r\n    ULong = 0L,\r\n    Short = 0,\r\n    UShort = 0,\r\n    Decimal = 0m,\r\n    Double = 0d,\r\n    DateTime = DateTime.MinValue,\r\n    NullableDateTime = null,\r\n    Enum = System.DateTimeKind.Unspecified\r\n  }\r\n};");
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
            dump.Should().Be("var dictionary = new Dictionary<String[,], List<Nullable<Int32>[,][]>[,,]>[]\r\n{\r\n  new Dictionary<String[,], List<Nullable<Int32>[,][]>[,,]>\r\n  {\r\n  }\r\n};");
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
        public void ShouldDumpRecursiveTypes_RecursivePerson()
        {
            // Arrange
            var person = new RecursivePerson();
            person.Parent = person;

            // Act
            var dump = ObjectDumperCSharp.Dump(person);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var recursivePerson = new RecursivePerson\r\n{\r\n};");
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
            dump.Should().Be("var dateTimeKind = System.DateTimeKind.Utc;");
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
                "var dictionary = new Dictionary<Int32, String>\r\n" +
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
            dump.Should().Be("var stringArray = new String[]\r\n{\r\n  \"aaa\",\r\n  \"bbb\"\r\n};");
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
            var expectedPerson = new Person { Name = "Boris \"The Blade\", \\GANGSTA\\ aka 'The Bullet Dodger' \a \b \f \r\nOn a new\twith tab \v \0" };
            var dumpOptions = new DumpOptions { SetPropertiesOnly = true, IgnoreDefaultValues = true, MaxLevel = 1, ExcludeProperties = { "ByteArray" } };

            // Act
            var dump = ObjectDumperCSharp.Dump(expectedPerson, dumpOptions);

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
            dump.Should().Be("var runtimeType = typeof(ObjectDumping.Tests.Testdata.Person);");
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
            dump.Should().Be("var viewModelValidation = new ViewModelValidation\r\n{\r\n};");
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
                "  Status = System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError,\r\n" +
                "  StatusInformation = \"Test status\"\r\n" +
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
                "var list = new List<(String, Int32)>\r\n" +
                "{\r\n" +
                "  (\"Person 1\", 3),\r\n" +
                "  (\"Person 2\", 3)\r\n" +
                "};");
        }
#endif
    }
}
