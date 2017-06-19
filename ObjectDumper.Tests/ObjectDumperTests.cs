using System.Collections.Generic;

using FluentAssertions;

using Xunit;

namespace System.Diagnostics.Tests
{
    public class ObjectDumperTests
    {
        [Fact]
        public void ShouldDumpObject()
        {
            // Arrange
            var person = new Person { Name = "Thomas", Age = 30, SetOnly = 40};

            // Act
            var dump = ObjectDumper.Dump(person);

            // Assert
            dump.Should().NotBeNull();
            dump.Should().Be("{System.Diagnostics.Tests.Person}\r\n  Name: \"Thomas\"\r\n  Age: 30\r\n");
        }

        [Fact]
        public void ShouldDumpEnumerable()
        {
            // Arrange
            var persons = new List<Person> { new Person { Name = "Person1", Age = 1, }, new Person { Name = "Person2", Age = 2, } };

            // Act
            var dump = ObjectDumper.Dump(persons);

            // Assert
            dump.Should().NotBeNull();
            dump.Should().Be("{System.Diagnostics.Tests.Person}\r\n  Name: \"Person1\"\r\n  Age: 1\r\n{System.Diagnostics.Tests.Person}\r\n  Name: \"Person2\"\r\n  Age: 2\r\n");
        }
    }
}
