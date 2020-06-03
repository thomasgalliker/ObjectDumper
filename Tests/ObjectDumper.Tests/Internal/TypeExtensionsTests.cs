using System;
using System.Collections.Generic;
using FluentAssertions;
using My;
using ObjectDumping.Tests.Testdata;
using Xunit;
using Xunit.Abstractions;
using ObjectDumping.Internal;

namespace ObjectDumping.Tests.Internal
{
    public class TypeExtensionsTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public TypeExtensionsTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ShouldGetFormattedName_NonGenericType()
        {
            // Arrange
            var type = typeof(Person);

            // Act
            var dump = type.GetFormattedName(false);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().Be("Person");
        }

        [Fact]
        public void ShouldGetFormattedName_GenericType()
        {
            // Arrange
            var type = typeof(GenericClass<string, Person, object>);

            // Act
            var dump = type.GetFormattedName(false);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().Be("GenericClass<String, Person, Object>");
        }

        [Fact]
        public void ShouldGetFormattedName_OpenGenericType()
        {
            // Arrange
            var type = typeof(GenericClass<,,>);

            // Act
            var dump = type.GetFormattedName(false);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().Be("GenericClass<,,>");
        }

        [Fact]
        public void ShouldGetFormattedName_Complex()
        {
            // Arrange
            var type = typeof(Dictionary<String[,], List<Nullable<Int32>[,][]>[,,]>[]);
            //var type = typeof(Dictionary<string[,], List<int?[,][]>[,,]>[]);

            // Act
            var dump = type.GetFormattedName(false);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().Be("Dictionary<String[,], List<Nullable<Int32>[,][]>[,,]>[]");
            //dump.Should().Be("Dictionary<string[,], List<int?[,][]>[,,]>[]");
        }
    }
}
