using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NullSafeChainTests
{
    public class Employee
    {
        public Person Person { get; set; }
    }

    public class Person
    {
        public Address Address { get; set; }
    }

    public class Address
    {
        public City City { get; set; }
    }

    public class City
    {
        
    }
}
