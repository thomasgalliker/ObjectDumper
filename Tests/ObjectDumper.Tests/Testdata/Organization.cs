using System.Collections.Generic;

namespace ObjectDumping.Tests.Testdata
{
    public class Organization
    {
        public Organization()
        {
            this.Persons = new HashSet<Person>();
        }

        public string Name { get; set; }

        public ICollection<Person> Persons { get; set; }
    }

    public class GenericClass<T1, T2, T3>
    {
        public T1 Prop1 { get; set; }

        public T2 Prop2 { get; set; }

        public T3 Prop3 { get; set; }
    }
}