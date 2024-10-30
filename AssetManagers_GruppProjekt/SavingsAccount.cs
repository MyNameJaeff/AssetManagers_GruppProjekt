namespace AssetManagers_GruppProjekt
{
    public class SavingsAccount : Account
    {
        public decimal InterestRate { get; set; }

        public SavingsAccount(string currency, decimal interestRate) : base(currency)
        {
            if (interestRate < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(interestRate), "Interest rate cannot be negative.");
            }
            InterestRate = interestRate;
        }
    }
}
