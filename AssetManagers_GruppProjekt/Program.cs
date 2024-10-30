namespace AssetManagers_GruppProjekt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StartBankProgram();
        }

        static void PrintAsciiArt()
        {
            string ASSCI = "\r\n   _____                        __       _____                                                   \r\n  /  _  \\   ______ ______ _____/  |_    /     \\ _____    ____   ____   ____   ___________  ______\r\n /  /_\\  \\ /  ___//  ___// __ \\   __\\  /  \\ /  \\\\__  \\  /    \\_/ __ \\ / ___\\_/ __ \\_  __ \\/  ___/\r\n/    |    \\\\___ \\ \\___ \\\\  ___/|  |   /    Y    \\/ __ \\|   |  \\  ___// /_/  >  ___/|  | \\/\\___ \\ \r\n\\____|__  /____  >____  >\\___  >__|   \\____|__  (____  /___|  /\\___  >___  / \\___  >__|  /____  >\r\n        \\/     \\/     \\/     \\/               \\/     \\/     \\/     \\/_____/      \\/           \\/ \r\n";
            Console.WriteLine(ASSCI);
        }

        static void StartBankProgram()
        {
            PrintAsciiArt();
            BankSystem bankSystem = new BankSystem();

            // Adding a sample user
            User user = new User("Jeff", "123");
            bankSystem.Users.Add(user);

            // Create two distinct accounts for the user
            Account account1 = user.OpenNewAccount("USD");
            Account account2 = user.OpenNewAccount("USD");
            SavingsAccount savingsAccount = user.OpenNewSavingAccount("USD", 0.05m);

            // Deposit initial amounts into both accounts
            account1.Deposit(500);
            account2.Deposit(500);

            savingsAccount.Deposit(5000);

            Console.WriteLine("In 1 year: " + (savingsAccount.Balance * (1 + savingsAccount.InterestRate)));

            //// Print initial balances
            //Console.WriteLine(account1.Balance); // Should print 500
            //Console.WriteLine(account2.Balance); // Should print 500

            //// Transfer funds from account1 to account2
            //user.TransferFunds(account1, account2, 10); // Transfer $10 from account1 to account2

            //// Print balances after transfer
            //Console.WriteLine(account1.Balance); // Should print 490
            //Console.WriteLine(account2.Balance); // Should print 510

            //try
            //{
            //    while (!isLoggedIn)
            //    {
            //        Console.WriteLine("Enter username:");
            //        string username = Console.ReadLine();
            //        Console.WriteLine("Enter password:");
            //        string password = Console.ReadLine();

            //        isLoggedIn = bankSystem.Login(username, password);
            //    }
            //}
            //catch (InvalidOperationException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    return;
            //}
        }
    }
}
