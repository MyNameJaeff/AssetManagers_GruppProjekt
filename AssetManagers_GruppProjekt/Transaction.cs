namespace AssetManagers_GruppProjekt
{
    public class Transaction
    {
        public string FromAccount { get; private set; }
        public string ToAccount { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public DateTime Timestamp { get; private set; }
        public Transaction(string fromAccount, string toAccount, decimal amount, string currency)
        {
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
            Currency = currency;
            Timestamp = DateTime.Now;
        }
        public override string ToString()
        {
            return $"{Timestamp}: {Amount} {Currency} sent from {FromAccount} to {ToAccount}";
        }
    }
}
