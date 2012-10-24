Null-safe-chain
===============
Null-safe-chain is a high-performance .NET library that simplifies the null checking in your system.

here are some examples:

```csharp

var result = employee.NullSafeChain(x => x.Person.Address.City); //Properties chain
var result = employee.NullSafeChain(x => ((Dog) x.Person.Pet).Collar); // Casting
var result = employee.NullSafeChain(x => (x.Person.Pet as Dog).Collar); // As operand
var result = employee.NullSafeChain(x => x.GetEmployeeByName("Jain").Person); // Method call
var result = employee.NullSafeChain(x => x.Manages.FirstOrDefault(y => y.Name == "Jain").Person); // Extension method call

```