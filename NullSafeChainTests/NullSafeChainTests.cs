using NUnit.Framework;
using NullSafeChain;

namespace NullSafeChainTests
{
    [TestFixture]
    public class NullSafeChainTests
    {
        [Test]
        public void Test()
        {
            // Arange
            var employee = new Employee {Person = new Person {Address = new Address {City = new City()}}};

            // Act
            var result = employee.NullSafeChain(x => x.Person.Address.City);

            // Assert
            Assert.NotNull(result);
        }
    }
}