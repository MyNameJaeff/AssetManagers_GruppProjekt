﻿namespace AssetManagers_GruppProjekt
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Account> Accounts { get; set; } = new List<Account>();
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<Loan> Loans { get; set; } = new List<Loan>();

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public void TransferFunds(Account fromAccount, Account toAccount, decimal amount)
        {
            if (fromAccount.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds in the source account.");
            }

            fromAccount.Withdraw(amount);
            toAccount.Deposit(amount);

            Transaction transaction = new Transaction(fromAccount.AccountNumber.ToString(), toAccount.AccountNumber.ToString(), amount, fromAccount.Currency);
            Transactions.Add(transaction);
            //Console.WriteLine(transaction.ToString());
        }


        public void TransferToOtherUser(Account fromAccount, User recipient, Account recipientAccount, decimal amount)
        {
            fromAccount.Withdraw(amount);
            recipientAccount.Deposit(amount);

            Transaction transaction = new Transaction(fromAccount.AccountNumber.ToString(), recipientAccount.AccountNumber.ToString(), amount, fromAccount.Currency);
            Transactions.Add(transaction);
            recipient.Transactions.Add(transaction);
            Console.WriteLine(transaction.ToString());
        }

        public Account OpenNewAccount(string currency)
        {
            Account newAccount = new Account(currency);
            Accounts.Add(newAccount);
            return newAccount;
        }

        public SavingsAccount OpenNewSavingAccount(string currency, decimal interestRate)
        {
            SavingsAccount newAccount = new SavingsAccount(currency, interestRate);
            Accounts.Add(newAccount);
            return newAccount;
        }

        public void Loan(decimal interestRate, decimal amount, int periodMonths)
        {
            if (amount <= 0)
                throw new ArgumentException("Loan amount must be positive.", nameof(amount));
            if (periodMonths <= 0 || periodMonths > 240)
                throw new ArgumentException("Loan period must be between 1 and 240 months.", nameof(periodMonths));

            Loan loan = new(interestRate, amount, periodMonths);
            Loans.Add(loan);
        }
    }
}
