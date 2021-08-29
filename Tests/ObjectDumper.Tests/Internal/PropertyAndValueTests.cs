using System.Linq;
using System.Text;
using FluentAssertions;
using ObjectDumping.Internal;
using ObjectDumping.Tests.Testdata;
using Xunit;

namespace ObjectDumping.Tests.Internal
{
    public class PropertyAndValueTests
    {
        [Fact]
        public void ShouldCreatePropertyAndValue()
        {
            // Arrange
            var person = new Person { Age = 30 };
            var propertyInfo = typeof(Person).GetProperties().Single(p => p.Name == nameof(Person.Age));

            // Act
            var propertyAndValue = new PropertyAndValue(person, propertyInfo);

            // Assert
            propertyAndValue.DefaultValue.Should().Be(0);
            propertyAndValue.Value.Should().Be(30);
        }

#if NETCOREAPP3_1_OR_GREATER
        [Fact]
        public void ShouldCreatePropertyAndValue_Preamble()
        {
            // Arrange
            var encoding = Encoding.UTF8;
            var propertyInfo = typeof(Encoding).GetProperties().Single(p => p.Name == nameof(Encoding.Preamble));

            // Act
            var propertyAndValue = new PropertyAndValue(encoding, propertyInfo);

            // Assert
            propertyAndValue.DefaultValue.Should().Be("{BadImageFormatException: An attempt was made to load a program with an incorrect format. (0x8007000B)}");
            propertyAndValue.Value.Should().Be("{NotSupportedException: Specified method is not supported.}");
        }
#endif
    }
}
