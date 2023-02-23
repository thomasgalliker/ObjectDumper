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

#if NET5_0_OR_GREATER
        [Fact]
        public void ShouldCreatePropertyAndValue_Preamble()
        {
            // Arrange
            var encoding = Encoding.UTF8;
            var propertyInfo = typeof(Encoding).GetProperties().Single(p => p.Name == nameof(Encoding.Preamble));

            // Act
            var propertyAndValue = new PropertyAndValue(encoding, propertyInfo);

            // Assert
#if NET5 || NET6
            propertyAndValue.DefaultValue.Should().Be("{BadImageFormatException: An attempt was made to load a program with an incorrect format. (0x8007000B)}");
#elif NET7_0_OR_GREATER
            propertyAndValue.DefaultValue.Should().Be("{ArgumentException: GenericArguments[0], 'System.ReadOnlySpan`1[System.Byte]', on 'T GetDefaultGeneric[T]()' violates the constraint of type 'T'.}");
#endif
            propertyAndValue.Value.Should().Be("{NotSupportedException: Specified method is not supported.}");
        }
#endif
    }
}
