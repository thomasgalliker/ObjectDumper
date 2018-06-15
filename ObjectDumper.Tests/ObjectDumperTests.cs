using System.Collections.Generic;
using System.Diagnostics.Tests.Testdata;
using System.Linq;
using FluentAssertions;

using Xunit;
using Xunit.Abstractions;

namespace System.Diagnostics.Tests
{
    public class ObjectDumperTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public ObjectDumperTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldDumpObject()
        {
            // Arrange
            var person = PersonFactory.GetPersonThomas();

            // Act
            var dump = ObjectDumper.Dump(person);

            // Assert
            dump.Should().NotBeNull();
            dump.Should().Be("{System.Diagnostics.Tests.Testdata.Person}\r\n  Name: \"Thomas\"\r\n  Age: 30\r\n  SetOnly: 40\r\n  GetOnly: 11\r\n  Private: 0\r\n");
        }

        [Fact]
        public void ShouldDumpEnumerable()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2);

            // Act
            var dump = ObjectDumper.Dump(persons);

            // Assert
            dump.Should().NotBeNull();
            dump.Should().Be("{System.Diagnostics.Tests.Testdata.Person}\r\n  Name: \"Person 1\"\r\n  Age: 3\r\n  SetOnly: 3\r\n  GetOnly: 11\r\n  Private: 0\r\n{System.Diagnostics.Tests.Testdata.Person}\r\n  Name: \"Person 2\"\r\n  Age: 3\r\n  SetOnly: 3\r\n  GetOnly: 11\r\n  Private: 0\r\n");
        }

        [Fact]
        public void ShouldDumpNestedObjects()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2).ToList();
            var organization = new Organization { Name = "superdev gmbh", Persons = persons };

            // Act
            var dump = ObjectDumper.Dump(organization);

            // Assert
            this.testOutputHelper.WriteLine(dump);

            dump.Should().NotBeNull();
            dump.Should().Be("{System.Diagnostics.Tests.Testdata.Organization}\r\n" +
                             "  Name: \"superdev gmbh\"\r\n" +
                             "  Persons: ...\r\n" +
                             "    {System.Diagnostics.Tests.Testdata.Person}\r\n" +
                             "      Name: \"Person 1\"\r\n" +
                             "      Age: 3\r\n" +
                             "      SetOnly: 3\r\n" +
                             "      GetOnly: 11\r\n" +
                             "      Private: 0\r\n" +
                             "    {System.Diagnostics.Tests.Testdata.Person}\r\n" +
                             "      Name: \"Person 2\"\r\n" +
                             "      Age: 3\r\n" +
                             "      SetOnly: 3\r\n" +
                             "      GetOnly: 11\r\n" +
                             "      Private: 0\r\n");
        }

        [Fact]
        public void ShouldDumpStruct()
        {
            // Arrange
            var datetime = new DateTime(2000, 01, 01, 23, 59, 59);

            // Act
            var dump = ObjectDumper.Dump(datetime);

            // Assert
            dump.Should().NotBeNull();
            dump.Should().Be("01.01.2000 23:59:59\r\n");
        }


        [Fact]
        public void ShouldDumpNullableObject()
        {
            // Arrange
            var datetime = new DateTime?();

            // Act
            var dump = ObjectDumper.Dump(datetime);

            // Assert
            dump.Should().NotBeNull();
            dump.Should().Be("null\r\n");
        }
    }
}
