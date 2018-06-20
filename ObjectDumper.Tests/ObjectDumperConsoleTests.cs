using System.Diagnostics.Tests.Testdata;
using System.Linq;
using FluentAssertions;

using Xunit;
using Xunit.Abstractions;

namespace System.Diagnostics.Tests
{
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
            dump.Should().Be("{System.Diagnostics.Tests.Testdata.Person}\n\r" +
                             "  Name: \"Thomas\"\n\r" +
                             "  Age: 30\n\r" +
                             "  SetOnly: 40\n\r" +
                             "  GetOnly: 11\n\r" +
                             "  Private: 0\n\r");
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
            dump.Should().Be("{System.Diagnostics.Tests.Testdata.Person}\n\r" +
                             "	Name: \"Thomas\"\n\r" +
                             "	Age: 30\n\r" +
                             "	SetOnly: 40\n\r" +
                             "	GetOnly: 11\n\r" +
                             "	Private: 0\n\r");
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
            dump.Should().Be("{System.Diagnostics.Tests.Testdata.Person}\n\r" +
                             "  Name: \"Person 1\"\n\r" +
                             "  Age: 3\n\r" +
                             "  SetOnly: 3\n\r" +
                             "  GetOnly: 11\n\r" +
                             "  Private: 0\n\r" +
                             "{System.Diagnostics.Tests.Testdata.Person}\n\r" +
                             "  Name: \"Person 2\"\n\r" +
                             "  Age: 3\n\r" +
                             "  SetOnly: 3\n\r" +
                             "  GetOnly: 11\n\r" +
                             "  Private: 0\n\r");
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
            dump.Should().Be("{System.Diagnostics.Tests.Testdata.Organization}\n\r" +
                             "  Name: \"superdev gmbh\"\n\r" +
                             "  Persons: ...\n\r" +
                             "    {System.Diagnostics.Tests.Testdata.Person}\n\r" +
                             "      Name: \"Person 1\"\n\r" +
                             "      Age: 3\n\r" +
                             "      SetOnly: 3\n\r" +
                             "      GetOnly: 11\n\r" +
                             "      Private: 0\n\r" +
                             "    {System.Diagnostics.Tests.Testdata.Person}\n\r" +
                             "      Name: \"Person 2\"\n\r" +
                             "      Age: 3\n\r" +
                             "      SetOnly: 3\n\r" +
                             "      GetOnly: 11\n\r" +
                             "      Private: 0\n\r");
        }

        [Fact]
        public void ShouldDumpDateTime()
        {
            // Arrange
            var datetime = new DateTime(2000, 01, 01, 23, 59, 59);

            // Act
            var dump = ObjectDumperConsole.Dump(datetime);

            // Assert
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
            dump.Should().NotBeNull();
            dump.Should().Be("null");
        }
    }
}
