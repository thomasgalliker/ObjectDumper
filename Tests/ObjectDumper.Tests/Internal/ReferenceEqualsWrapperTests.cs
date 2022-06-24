using System.Collections.Generic;
using FluentAssertions;
using ObjectDumping.Internal;
using ObjectDumping.Tests.Testdata;
using Xunit;

namespace ObjectDumping.Tests.Internal
{
    public class ReferenceEqualsWrapperTests
    {
        [Fact]
        public void ShouldContain_IfReferecenIsEqual()
        {
            // Arrange
            var person = new Person { Age = 30 };

            var hashSet = new HashSet<ReferenceEqualsWrapper>
            {
                new ReferenceEqualsWrapper(person),
                new ReferenceEqualsWrapper(person),
            };

            // Act
            var contains = hashSet.Contains(new ReferenceEqualsWrapper(person));

            // Assert
            contains.Should().BeTrue();
        }

        [Fact]
        public void ShouldNotContain_IfReferecenIsNotEqual()
        {
            // Arrange
            var list = new List<ReferenceEqualsWrapper>
            {
                new ReferenceEqualsWrapper(new Person { Age = 30 }),
                new ReferenceEqualsWrapper(new Person { Age = 30 }),
            };

            // Act
            var contains = list.Contains(new ReferenceEqualsWrapper(new Person { Age = 30 }));

            // Assert
            contains.Should().BeFalse();
        }


        [Fact]
        public void ShouldBeEqual_IfReferenceEquals()
        {
            // Arrange
            var person = new Person { Age = 30 };
            var referenceEqualsWrapper1 = new ReferenceEqualsWrapper(person);
            var referenceEqualsWrapper2 = new ReferenceEqualsWrapper(person);

            // Act
            var isEqual = referenceEqualsWrapper1.Equals(referenceEqualsWrapper2);

            // Assert
            isEqual.Should().BeTrue();
        }

        [Fact]
        public void ShouldNotBeEqual_IfReferenceNotEquals()
        {
            // Arrange
            var referenceEqualsWrapper1 = new ReferenceEqualsWrapper(new Person { Age = 30 });
            var referenceEqualsWrapper2 = new ReferenceEqualsWrapper(new Person { Age = 30 });

            // Act
            var isEqual = Equals(referenceEqualsWrapper1, referenceEqualsWrapper2);

            // Assert
            isEqual.Should().BeFalse();
        }
    }
}
