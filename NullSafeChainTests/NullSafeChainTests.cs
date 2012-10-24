using System;
using NUnit.Framework;
using NullSafeChain;
using System.Linq;

namespace NullSafeChainTests
{
    [TestFixture]
    public class NullSafeChainTests
    {
        [Test]
        public void PropertyChain_GetTheLastElement()
        {
            // Arange
            var employee = new Employee { Person = new Person { Address = new Address { City = new City() } } };

            // Act
            var result = employee.NullSafeChain(x => x.Person.Address.City);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void PropertyChain_TheThirdElementIsNull()
        {
            // Arange
            var employee = new Employee { Person = new Person() };

            // Act
            var result = employee.NullSafeChain(x => x.Person.Address.City);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public void PropertyAndExplisitCastChain_GetTheLastElement()
        {
            // Arange
            var employee = new Employee { Person = new Person { Pet = new Dog { Collar = new Collar() } } };

            // Act
            var result = employee.NullSafeChain(x => ((Dog)x.Person.Pet).Collar);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        [ExpectedException(typeof(InvalidCastException))]
        public void PropertyAndExplisitCastChain_InvalidCast()
        {
            // Arange
            var employee = new Employee { Person = new Person { Pet = new Cat() } };

            // Act
            var result = employee.NullSafeChain(x => ((Dog)x.Person.Pet).Collar);
        }

        [Test]
        public void PropertyAndTypeAsChain_GetTheLastElement()
        {
            // Arange
            var employee = new Employee { Person = new Person { Pet = new Dog { Collar = new Collar() } } };

            // Act
            var result = employee.NullSafeChain(x => (x.Person.Pet as Dog).Collar);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void PropertyAndTypeAsCastChain_TheAsReturnsNull()
        {
            // Arange
            var employee = new Employee { Person = new Person { Pet = new Cat() } };

            // Act
            var result = employee.NullSafeChain(x => (x.Person.Pet as Dog).Collar);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public void PropertyAndMethodCallChain_CallExtensionMethod_GetTheLastElement()
        {
            // Arange
            var employee = new Employee { Manages = { new Employee { Person = new Person { Pet = new Cat() } } } };

            // Act
            var result = employee.NullSafeChain(x => x.Manages.FirstOrDefault().Person);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void PropertyAndMethodCallChain_CallExtensionMethodWithParam_GetTheLastElement()
        {
            // Arange
            var employee = new Employee { Manages = { new Employee { Person = new Person { Pet = new Cat() }, Name = "Jain" } } };

            // Act
            var result = employee.NullSafeChain(x => x.Manages.FirstOrDefault(y => y.Name == "Jain").Person);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void PropertyAndMethodCallChain_CallExtensionMethod_TheMethodReturnsNull()
        {
            // Arange
            var employee = new Employee();

            // Act
            var result = employee.NullSafeChain(x => x.Manages.FirstOrDefault().Person);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public void PropertyAndMethodCallChain_CallMethodWithParam_TheMethodReturnsNull()
        {
            // Arange
            var employee = new Employee { Manages = { new Employee { Person = new Person { Pet = new Cat() }, Name = "john" } } };

            // Act
            var result = employee.NullSafeChain(x => x.GetEmployeeByName("Jain").Person);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public void PropertyAndMethodCallChain_CallMethod_GetTheLastElement()
        {
            // Arange
            var employee = new Employee { Manages = { new Employee { Person = new Person { Pet = new Cat() }, Name = "Jain" } } };

            // Act
            var result = employee.NullSafeChain(x => x.GetFirstEmployee().Person);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void PropertyAndMethodCallChain_CallMethod_TheMethodReturnsNull()
        {
            // Arange
            var employee = new Employee();

            // Act
            var result = employee.NullSafeChain(x => x.GetFirstEmployee().Person);

            // Assert
            Assert.Null(result);
        }
    }
}