using System.Diagnostics.Tests.Testdata;
using System.Linq;
using FluentAssertions;

using Xunit;
using Xunit.Abstractions;

namespace System.Diagnostics.Tests
{
    public class ObjectDumperCSharpCSharpTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ObjectDumperCSharpCSharpTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldDumpObject()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();

            // Act
            var dump = ObjectDumperCSharp.Dump(person);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var person = new Person\n\r" +
                             "{\n\r" +
                             "  Name = \"Thomas\",\n\r" +
                             "  Age = 30,\n\r" +
                             "  SetOnly = 40,\n\r" +
                             "  GetOnly = 11,\n\r" +
                             "  Private = 0\n\r" +
                             "};");
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
            dump.Should().Be("var person = new Person\n" +
                             "{\n" +
                             "	Name = \"Thomas\",\n" +
                             "	Age = 30,\n" +
                             "	SetOnly = 40,\n" +
                             "	GetOnly = 11,\n" +
                             "	Private = 0\n" +
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
            dump.Should().Be("var listperson = new List<Person>\n\r" +
                             "{\n\r  new Person\n\r" +
                             "  {\n\r    Name = \"Person 1\",\n\r" +
                             "    Age = 3,\n\r" +
                             "    SetOnly = 3,\n\r" +
                             "    GetOnly = 11,\n\r" +
                             "    Private = 0\n\r" +
                             "  },\n\r" +
                             "  new Person\n\r" +
                             "  {\n\r" +
                             "    Name = \"Person 2\",\n\r" +
                             "    Age = 3,\n\r" +
                             "    SetOnly = 3,\n\r" +
                             "    GetOnly = 11,\n\r" +
                             "    Private = 0\n\r" +
                             "  }\n\r" +
                             "};");
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
            dump.Should().Be("var organization = new Organization\n\r" +
                             "{\n\r" +
                             "  Name = \"superdev gmbh\",\n\r" +
                             "  Persons = new List<Person>\n\r" +
                             "  {\n\r" +
                             "    new Person\n\r" +
                             "    {\n\r" +
                             "      Name = \"Person 1\",\n\r" +
                             "      Age = 3,\n\r" +
                             "      SetOnly = 3,\n\r" +
                             "      GetOnly = 11,\n\r" +
                             "      Private = 0\n\r" +
                             "    },\n\r" +
                             "    new Person\n\r" +
                             "    {\n\r" +
                             "      Name = \"Person 2\",\n\r" +
                             "      Age = 3,\n\r" +
                             "      SetOnly = 3,\n\r" +
                             "      GetOnly = 11,\n\r" +
                             "      Private = 0\n\r" +
                             "    }\n\r" +
                             "  }\n\r" +
                             "};");
        }

        [Fact]
        public void ShouldDumpDateTime()
        {
            // Arrange
            var datetime = new DateTime(2000, 01, 01, 23, 59, 59);

            // Act
            var dump = ObjectDumperCSharp.Dump(datetime);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var datetime = DateTime.Parse(\"01.01.2000 23:59:59\");");
        }

        [Fact]
        public void ShouldDumpNullableObject()
        {
            // Arrange
            var datetime = new DateTime?();

            // Act
            var dump = ObjectDumperCSharp.Dump(datetime);

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
            var dump = ObjectDumperCSharp.Dump(dateTimeKind);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().NotBeNull();
            dump.Should().Be("var datetimekind = System.DateTimeKind.Utc;");
        }
    }
}
