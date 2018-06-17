using System.Collections.Generic;
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
            dump.Should().NotBeNull();
            dump.Should().Be("var person1 = new System.Diagnostics.Tests.Testdata.Person\r\n" +
                             "{\r\n" +
                             "  Name: \"Thomas\"\r\n" +
                             "  Age: 30\r\n" +
                             "  SetOnly: 40\r\n" +
                             "  GetOnly: 11\r\n" +
                             "  Private: 0\r\n" +
                             "};");
        }

        [Fact]
        public void ShouldDumpEnumerable()
        {
            // Arrange
            var persons = PersonFactory.GeneratePersons(count: 2);

            // Act
            var dump = ObjectDumperCSharp.Dump(persons);

            // Assert
            dump.Should().NotBeNull();
            dump.Should().Be("var personList1 = new List<System.Diagnostics.Tests.Testdata.Person>\r\n" +
                             "{" +
                             "  new System.Diagnostics.Tests.Testdata.Person\r\n" +
                             "  {\r\n" +
                             "    Name: \"Thomas\"\r\n" +
                             "    Age: 30\r\n" +
                             "    SetOnly: 40\r\n" +
                             "    GetOnly: 11\r\n" +
                             "    Private: 0\r\n" +
                             "  }," +
                             "  {" +
                             "  new System.Diagnostics.Tests.Testdata.Person\r\n" +
                             "  {\r\n" +
                             "    Name: \"Thomas\"\r\n" +
                             "    Age: 30\r\n" +
                             "    SetOnly: 40\r\n" +
                             "    GetOnly: 11\r\n" +
                             "    Private: 0\r\n" +
                             "  }" +
                             "};");
        }

        //[Fact]
        //public void ShouldDumpNestedObjects()
        //{
        //    // Arrange
        //    var persons = PersonFactory.GeneratePersons(count: 2).ToList();
        //    var organization = new Organization { Name = "superdev gmbh", Persons = persons };

        //    // Act
        //    var dump = ObjectDumperCSharp.Dump(organization);

        //    // Assert
        //    this.testOutputHelper.WriteLine(dump);

        //    dump.Should().NotBeNull();
        //    dump.Should().Be("{System.Diagnostics.Tests.Testdata.Organization}\r\n" +
        //                     "  Name: \"superdev gmbh\"\r\n" +
        //                     "  Persons: ...\r\n" +
        //                     "    {System.Diagnostics.Tests.Testdata.Person}\r\n" +
        //                     "      Name: \"Person 1\"\r\n" +
        //                     "      Age: 3\r\n" +
        //                     "      SetOnly: 3\r\n" +
        //                     "      GetOnly: 11\r\n" +
        //                     "      Private: 0\r\n" +
        //                     "    {System.Diagnostics.Tests.Testdata.Person}\r\n" +
        //                     "      Name: \"Person 2\"\r\n" +
        //                     "      Age: 3\r\n" +
        //                     "      SetOnly: 3\r\n" +
        //                     "      GetOnly: 11\r\n" +
        //                     "      Private: 0\r\n");
        //}

        [Fact]
        public void ShouldDumpStruct()
        {
            // Arrange
            var datetime = new DateTime(2000, 01, 01, 23, 59, 59);

            // Act
            var dump = ObjectDumperCSharp.Dump(datetime);

            // Assert
            dump.Should().NotBeNull();
            dump.Should().Be("var dateTime1 = new DateTime(2000, 01, 01, 23, 59, 59);\r\n");
        }

        [Fact]
        public void ShouldDumpNullableObject()
        {
            // Arrange
            var datetime = new DateTime?();

            // Act
            var dump = ObjectDumperCSharp.Dump(datetime);

            // Assert
            dump.Should().NotBeNull();
            dump.Should().Be("var dateTime1 = (DateTime?)null;\r\n");
        }
    }
}
