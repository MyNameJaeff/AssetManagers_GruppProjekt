namespace AssetManagers_GruppProjekt
{
    public class Loan
    {
        public decimal InterestRate { get; set; }
        public decimal BorrowedMoney { get; set; }
        public int LoanPeriod { get; set; }
        public DateTime StartLoanDate { get; private set; }
        public DateTime EndLoanDate { get; private set; }


        public Loan(decimal interestRate, decimal borrowedMoney, int loanPeriod)
        {
            InterestRate = interestRate;
            BorrowedMoney = borrowedMoney;
            LoanPeriod = loanPeriod;
            StartLoanDate = DateTime.Now;
            EndLoanDate = StartLoanDate.AddMonths(loanPeriod);
        }

        public override string ToString()
        {
            return $"Loan: {BorrowedMoney} kr, Interest rate: {InterestRate}%, Period: {LoanPeriod} months, Startdate: {StartLoanDate}, End-date: {EndLoanDate}";
        }
    }
}