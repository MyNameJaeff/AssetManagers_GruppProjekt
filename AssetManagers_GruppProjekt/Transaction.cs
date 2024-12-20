﻿namespace AssetManagers_GruppProjekt
{
    public class Transaction
    {
        public User FromUser { get; private set; }
        public User ToUser { get; private set; }
        public Account FromAccount { get; private set; }
        public Account ToAccount { get; private set; }
        public decimal Amount { get; private set; }
        public decimal ConvertedAmount { get; private set; }
        public string Currency { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string ErrorMsg { get; set; }
        public Transaction(User fromUser, User toUser, Account fromAccount, Account toAccount, decimal amount, decimal convertedAmount, string currency, string errorMsg = "")
        {
            FromUser = fromUser;
            ToUser = toUser;
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
            ConvertedAmount = convertedAmount;
            Currency = currency;
            Timestamp = DateTime.Now;
            ErrorMsg = errorMsg;
        }
        public override string ToString()
        {
            return $"{Timestamp}: {Amount} {Currency} sent from {FromAccount.AccountNumber} to {ToAccount.AccountNumber}";
        }
    }
}
