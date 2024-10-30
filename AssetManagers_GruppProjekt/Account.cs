namespace AssetManagers_GruppProjekt
{
    public class Account
    {
        public Guid AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }

        public Account(string currency)
        {
            AccountNumber = Guid.NewGuid();
            Balance = 0;
            Currency = currency;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.", nameof(amount));
            }

            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be positive.", nameof(amount));
            }

            if (Balance >= amount)
            {
                Balance -= amount;
            }

            else
            {
                throw new Exception("Insufficient funds.");
            }
        }
    }
}
