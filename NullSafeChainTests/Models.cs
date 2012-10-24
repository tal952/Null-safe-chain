using System.Drawing;

namespace NullSafeChainTests
{
    public class Employee
    {
        public Person Person { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
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
