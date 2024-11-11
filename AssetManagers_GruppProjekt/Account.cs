namespace AssetManagers_GruppProjekt
{
    public class Account
    {
        public Guid AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }

        // constructor for the account class, generates a new unique account number and sets the balance to 0
        public Account(string currency)
        {
            AccountNumber = Guid.NewGuid();
            Balance = 0;
            Currency = currency;
        }

        // a method to deposit money into the account balance, checks the amount is positive
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.", nameof(amount));
            }

            Balance += amount;
        }

        // a method to withdraw money from the account balance, checks the amount is positive and that the balance is sufficient
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
