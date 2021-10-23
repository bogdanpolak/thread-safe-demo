using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleLockAccountDemo
{
    internal static class AccountDemo
    {
        private const decimal InitialBalance = 1000.00m;
        private const int NumOfTasks = 100;
        private static readonly decimal[] TaskOperations = { 5, 2, -3, 6, -2, -1, 8, -5, 11, -6 };

        public static async Task Run()
        {
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
            PrintGeneralReport(account);
            PrintOperationLog(account);
        }

        private static void PrintGeneralReport(Account account)
        {
            var expected = InitialBalance + TaskOperations.Sum() * NumOfTasks;
            Console.WriteLine($"Account's balance is {account.GetBalance()} (expected is: {expected})");
            Console.WriteLine($"Total operations logged: {account.OperationLog.Count}");
        }

        private static void PrintOperationLog(Account account)
        {
            var accountOperationLog = account.OperationLog;
            var sb = new StringBuilder();
            for (var idx = 0; idx < accountOperationLog.Count; idx++)
            {
                if (sb.Length > 0) sb.Append("  ");
                if (idx > 0 && idx % 4 == 0) sb.AppendLine();
                var operation = accountOperationLog[idx];
                if (operation is null)
                {
                    sb.Append("---- -- ----");
                }
                else
                {
                    var amount = operation.AppliedAmount > 0
                        ? $"+{operation.AppliedAmount}"
                        : operation.AppliedAmount.ToString(CultureInfo.InvariantCulture);
                    sb.Append($"[{operation.TaskId}]{amount}={operation.Balance}");
                }
            }
            Console.WriteLine(sb.ToString());
        }
    }
}