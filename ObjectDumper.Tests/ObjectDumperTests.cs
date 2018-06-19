namespace System.Diagnostics.Tests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public sealed class ObjectDumperTests
    {
        #region Public Methods

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void ShouldDumpEnumerable()
        {
            // Arrange
            var persons = new Collections.Generic.List<Person>
            {
                new Person {Name = "Person1", Age = 1},
                new Person {Name = "Person2", Age = 2}
            };

            // Act
            var dump = ObjectDumper.Dump(persons);

            // Assert
            const String expected = "{System.Diagnostics.Tests.Person}\r\n  Name: \"Person1\"\r\n  Age: 1\r\n  SetOnly: 99\r\n  GetOnly: 11\r\n  Private: 0\r\n{System.Diagnostics.Tests.Person}\r\n  Name: \"Person2\"\r\n  Age: 2\r\n  SetOnly: 99\r\n  GetOnly: 11\r\n  Private: 0\r\n";
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(dump);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, dump);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void ShouldDumpNullableObject()
        {
            // Act
            var dump = ObjectDumper.Dump((DateTime?)null);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(dump);
            const String expected = "null\r\n";
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, dump);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void ShouldDumpObject()
        {
            // Arrange
            var person = new Person { Name = "Thomas", Age = 30, SetOnly = 40 };

            // Act
            var dump = ObjectDumper.Dump(person);

            // Assert
            const String expected = "{System.Diagnostics.Tests.Person}\r\n  Name: \"Thomas\"\r\n  Age: 30\r\n  SetOnly: 40\r\n  GetOnly: 11\r\n  Private: 0\r\n";
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(dump);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, dump);
        }

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void ShouldDumpStruct()
        {
            // Arrange
            var datetime = new DateTime(2000, 01, 01, 23, 59, 59);

            // Act
            var dump = ObjectDumper.Dump(datetime);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(dump);
            const String expected = "01.01.2000 23:59:59\r\n";
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, dump);
        }

        #endregion Public Methods
    }
}
