Null-safe-chain
===============
Null-safe-chain is a high-performance .NET library that simplifies the null checking in your system.

here are some examples:

```csharp

var result = employee.NullSafeChain(x => x.Person.Address.City);
var result = employee.NullSafeChain(x => ((Dog) x.Person.Pet).Collar);
var result = employee.NullSafeChain(x => (x.Person.Pet as Dog).Collar);

```