using System.Collections.Generic;
using System.Collections.Immutable;
using Ardalis.GuardClauses;

namespace ConsoleLockAccountDemo
{
    public record AccountOperation(int TaskId, decimal AppliedAmount, decimal Balance);
    
    public class Account
    {
        private readonly object _balanceLock = new();
        private decimal _balance;
        private readonly List<AccountOperation> _operationLog = new();

        public Account(decimal initialBalance) => _balance = initialBalance;

        public void Debit(int taskId, decimal amount)
        {
            Guard.Against.Negative(amount, nameof(amount));
            lock (_balanceLock)
            {
                var appliedAmount = amount<=_balance ? amount : 0;
                _balance -= appliedAmount;
                Log(taskId, -appliedAmount);
            }
        }

        public void Credit(int taskId, decimal amount)
        {
            Guard.Against.Negative(amount, nameof(amount));
            lock (_balanceLock)
            {
                _balance += amount;
                Log(taskId, amount);
            }
        }

        public decimal GetBalance()
        {
            lock (_balanceLock)
            {
                return _balance;
            }
        }

        private void Log(int taskId, decimal amount)
        {
            lock (_balanceLock)
            {
                _operationLog.Add(new AccountOperation(taskId, amount, _balance));
            }
        }

        public ImmutableList<AccountOperation> GetOperationLog()
        {
            lock (_balanceLock)
            {
                return _operationLog.ToImmutableList();
            }
        }

        public int GetOperationLogCount()
        {
            lock (_balanceLock)
            {
                return _operationLog.Count;
            }
        }
    }
}