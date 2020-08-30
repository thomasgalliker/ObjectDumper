using System;
using System.Collections.Generic;
using FluentAssertions;
using ObjectDumping.Internal;
using ObjectDumping.Tests.Testdata;
using Xunit;
using Xunit.Abstractions;

namespace ObjectDumping.Tests.Internal
{
    public class TypeExtensionsTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public TypeExtensionsTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Theory]
        [ClassData(typeof(ValidTypeToKeywordMappings))]
        public void TryGetBuiltInTypeName_Valid(Type type, string expectedKeyword)
        {
            // Act
            var success = type.TryGetBuiltInTypeName(out var value);

            // Assert
            success.Should().BeTrue();
            value.Should().Be(expectedKeyword);
        }

        public class ValidTypeToKeywordMappings : TheoryData<Type, string>
        {
            public ValidTypeToKeywordMappings()
            {
                this.Add(typeof(string), "string");
                this.Add(typeof(String), "string");
            }
        }

        [Theory]
        [ClassData(typeof(InvalidTypeToKeywordMappings))]
        public void TryGetBuiltInTypeName_Invalid(Type type)
        {
            // Act
            var success = type.TryGetBuiltInTypeName(out var _);

            // Assert
            success.Should().BeFalse();
        }

        public class InvalidTypeToKeywordMappings : TheoryData<Type>
        {
            public InvalidTypeToKeywordMappings()
            {
                this.Add(typeof(DateTime));
                this.Add(typeof(TimeSpan));
            }
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
            var dump = type.GetFormattedName(useFullName: false);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().Be("GenericClass<string, Person, object>");
        }

        [Fact]
        public void ShouldGetFormattedName_GenericType_UseFullName()
        {
            // Arrange
            var type = typeof(GenericClass<string, Person, object>);

            // Act
            var dump = type.GetFormattedName(useFullName: true);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().Be("ObjectDumping.Tests.Testdata.GenericClass<System.String, ObjectDumping.Tests.Testdata.Person, System.Object>");
        }

        [Fact]
        public void ShouldGetFormattedName_GenericArrayType()
        {
            // Arrange
            var type = typeof(GenericClass<string, Person, object>[,,,]);

            // Act
            var dump = type.GetFormattedName(false);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().Be("GenericClass<string, Person, object>[,,,]");
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

            // Act
            var dump = type.GetFormattedName(useFullName: false, useValueTupleFormatting: false);

            // Assert
            this.testOutputHelper.WriteLine(dump);
            dump.Should().Be("Dictionary<string[,], List<Nullable<int>[,][]>[,,]>[]");
            //dump.Should().Be("Dictionary<string[,], List<int?[,][]>[,,]>[]");
        }
    }
}
