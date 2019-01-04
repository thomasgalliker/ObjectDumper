using System.Collections.Generic;

namespace ObjectDumping.Tests.Testdata
{
    public static class PersonFactory
    {
        public static Person GetPersonThomas()
        {
            return new Person { Name = "Thomas", Age = 30, SetOnly = 40 };
        }

        public static IEnumerable<Person> GeneratePersons(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Person { Name = $"Person {i + 1}", Age = count + 1, SetOnly = count + 1 };
            }
        }
    }
}
