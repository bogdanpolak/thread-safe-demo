# Thread Safe Demo

Access to shared memory from parallel tasks. Demo is using mutual-exclusion lock. 
More information about `lock` statement in C# [(docs.microsoft.com)](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement)

Sample usage:
```c#
lock (_balanceLock)
{
    _balance += amount;
    LogOperation(taskId, amount);
}
```

where `_balanceLock` is a dedicated object instance which should not be used by
other side of the code (other classes).

Internally lock statement is a syntax sugar compiled to `System.Threading.Monitor`.

## Demo remarks

**ImmutableArray<T>**

Project is using `ImmutableArray<T>` which is recommended list collection used in the 
thread-safe code. Immutable arrays allows a random items access and have item ordered.

```c#
decimal[] taskOperations = { 5, 2, -3, 6, -2, -1, 8, -5, 11, -6 };
ImmutableArray<decimal> operations = taskOperations.ToImmutableArray();
```

**Guard clauses**

Project is also using Ardalis Guard Clauses [github link](https://github.com/ardalis/GuardClauses)

```c#
Guard.Against.Negative(amount, nameof(amount));
```

## Demo

Account class: [Account.cs](./src/Account.cs)

Execution:
```c#
// InitialBalance = 1000.00m
// NumOfTasks = 100
// TaskOperations = { 5, 2, -3, 6, -2, -1, 8, -5, 11, -6 }

var account = new Account(InitialBalance);
var tasks = Enumerable.Range(0, NumOfTasks)
    .Select(idx =>
    {
        var operations = TaskOperations.ToImmutableArray();
        return Task.Run(() => {
            var taskId = 100 + idx;
            foreach (var amount in operations)
            {
                if (amount >= 0)
                    account.Credit(taskId, amount);
                else
                    account.Debit(taskId, -amount);
            }
        });
    });
await Task.WhenAll(tasks);
```
