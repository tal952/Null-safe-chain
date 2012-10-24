using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace NullSafeChainTests
{
    public class Employee
    {
        public string Name { get; set; }
        public Person Person { get; set; }
        public List<Employee> Manages { get; set; }

        public Employee()
        {
            Manages = new List<Employee>();
        }

        public Employee GetFirstEmployee()
        {
            return Manages.FirstOrDefault();
        }

        public Employee GetEmployeeByName(string name)
        {
            return Manages.FirstOrDefault(x => x.Name == name);
        }
    }

    public class Person
    {
        public Address Address { get; set; }
        public Pet Pet { get; set; }
    }

    public class Address
    {
        public City City { get; set; }
    }

    public class City
    {
    }

    public abstract class Pet
    {
        public string Name { get; set; }
    }

    public class Dog : Pet
    {
        public Collar Collar { get; set; }
    }

    public class Collar
    {
        public Color Color { get; set; }
    }

    public class Cat : Pet
    {
        
    }
}
