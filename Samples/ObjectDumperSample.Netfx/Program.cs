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
                new Person { Name = "John", Age = 20, },
                new Person { Name = "Thomas", Age = 30, },
            };

            var personsDump = ObjectDumper.Dump(persons, DumpStyle.CSharp);

            Console.WriteLine(personsDump);
            Console.ReadLine();
        }
    }
}
