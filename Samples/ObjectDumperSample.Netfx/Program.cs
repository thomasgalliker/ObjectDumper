using System;
using System.Collections.Generic;

using ObjectDumping;

namespace ObjectDumperSample.Netfx
{
    class Program
    {
        static void Main(string[] args)
        {
            var persons = new List<Person>
            {
                new Person { Name = "John", Age = 20, PersonType = typeof(Person)},
                new Person { Name = "Thomas", Age = 30, PersonType = typeof(Person) },
            };

            var personsDump = ObjectDumper.Dump(persons, new DumpOptions() { DumpStyle = DumpStyle.CSharp, CustomTypeFormatter = new Dictionary<Type, Func<Type, string>>() { { typeof(Type), o => $"typeof({o.Name})" } } });

            Console.WriteLine(personsDump);
            Console.ReadLine();
        }
    }
}
