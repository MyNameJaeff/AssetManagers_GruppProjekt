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
            BankSystem bankSystem = new BankSystem();

            // Adding a sample user
            User user = new User("Jeff", "123");
            Admin admin = new Admin("Admin", "Admin");

            bankSystem.Users.Add(user);
            bankSystem.Users.Add(admin);

            // Create two distinct accounts for the user
            Account account1 = user.OpenNewAccount("USD");
            Account account2 = user.OpenNewAccount("USD");
            SavingsAccount savingsAccount = user.OpenNewSavingAccount("USD", 0.05m);

            // Deposit initial amounts into both accounts
            account1.Deposit(500);
            account2.Deposit(500);
            savingsAccount.Deposit(5000);


            // HÄR KAN VI LÄGGA TILL MER KOD FÖR ATT TESTA VÅR BANKSYSTEM

            user.TransferFunds(account1, account2, 100);

            MainMenu(bankSystem);
        }

        static User? Login(BankSystem bankSystem)
        {
            bool userIsLoggedIn = false;
            while (!userIsLoggedIn)
            {
                Console.WriteLine("Enter username:");
                string username = Console.ReadLine();
                Console.WriteLine("Enter password:");
                string password = Console.ReadLine();

                User user = bankSystem.Login(username, password);
                if (user != null)
                {
                    userIsLoggedIn = true;
                    return user;
                }
            }
            return null;
        }

        static void MainMenu(BankSystem bankSystem)
        {
            PrintAsciiArt();
            var checkLogin = Login(bankSystem);
            if (checkLogin == null)
            {
                return;
            }
            User? user = (User)checkLogin;
            bool userIsAdmin = user is Admin;

            while (true)
            {
                Console.Clear();
                PrintAsciiArt();

                Console.WriteLine("Welcome to the main menu. Please select an option:\n" +
                      "1. View Accounts\n" +
                      "2. Deposit Funds\n" +
                      "3. Withdraw Funds\n" +
                      "4. Transfer Funds\n" +
                      "5. View Transactions\n" +
                      "6. Open New Account\n" +
                      (userIsAdmin ? "7. Create new user\n" : "") +
                      "0. Logout\n");

                string? input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        // View accounts logic
                        ViewAccounts(user);
                        break;
                    case "2":
                        // Deposit Funds logic
                        Deposit(user);
                        break;
                    case "3":
                        // Withdraw Funds logic
                        Withdraw(user);
                        break;
                    case "4":
                        // Transfer Funds logic
                        TransferFunds(user, bankSystem);
                        break;
                    case "5":
                        // View Transactions logic
                        CheckTransactions(user);
                        break;
                    case "6":
                        // Open New Account logic
                        OpenNewAccount(user);
                        break;
                    case "7":
                        if (userIsAdmin)
                        {
                            // Create new user logic
                            CreateNewUser(bankSystem);
                        }
                        break;
                    case "0":
                        // Logout logic
                        Logout(user, bankSystem);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static void ViewAccounts(User user)
        {
            var userAccounts = user.Accounts;

            Console.WriteLine("Accounts:");
            foreach (var account in userAccounts)
            {
                Console.WriteLine($"Account number: {account.AccountNumber}, Balance: {account.Balance}, Currency: {account.Currency} ({(account is SavingsAccount ? "Savings" : "Private")} account)");
            }
            WaitForX();
        }

        static void Deposit(User user)
        {
            Console.WriteLine($"Enter the account number you want to deposit funds to (0-{user.Accounts.Count() - 1}):");
            string? accountIndexInput = Console.ReadLine();

            if (!int.TryParse(accountIndexInput, out int accountIndex) || accountIndex < 0 || accountIndex >= user.Accounts.Count)
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            var account = user.Accounts[accountIndex];

            Console.WriteLine("Enter the amount you want to deposit:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            account.Deposit(amount);
            Console.WriteLine($"Successfully deposited {amount} {account.Currency} to account {account.AccountNumber}.");
            Thread.Sleep(2000);
        }

        static void Withdraw(User user)
        {
            Console.WriteLine($"Enter the account number you want to withdraw funds from (0-{user.Accounts.Count() - 1}):");
            string? accountIndexInput = Console.ReadLine();

            if (!int.TryParse(accountIndexInput, out int accountIndex) || accountIndex < 0 || accountIndex >= user.Accounts.Count)
            {
                Console.WriteLine("Invalid account number.");
                return;
            }

            var account = user.Accounts[accountIndex];

            Console.WriteLine("Enter the amount you want to withdraw:");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount.");
                return;
            }

            account.Withdraw(amount);
            Console.WriteLine($"Successfully withdrew {amount} {account.Currency} from account {account.AccountNumber}.");
            Thread.Sleep(2000);
        }
        static void TransferFunds(User user, BankSystem bankSystem)
        {
            Console.WriteLine("Choose transfer option:\n" +
                              "1. Transfer to another account (within your own accounts)\n" +
                              "2. Transfer to another user's account\n");

            string? transferOption = Console.ReadLine();

            if (transferOption == "1")
            {
                // Transfer within the user's own accounts
                Console.WriteLine($"Enter the account number you want to transfer funds from (0-{user.Accounts.Count() - 1}):");
                string? fromAccountIndexInput = Console.ReadLine();

                if (!int.TryParse(fromAccountIndexInput, out int fromAccountIndex) || fromAccountIndex < 0 || fromAccountIndex >= user.Accounts.Count)
                {
                    Console.WriteLine("Invalid account number.");
                    return;
                }

                var fromAccount = user.Accounts[fromAccountIndex];

                Console.WriteLine($"Enter the account number you want to transfer funds to (0-{user.Accounts.Count() - 1}):");
                string? toAccountIndexInput = Console.ReadLine();

                if (!int.TryParse(toAccountIndexInput, out int toAccountIndex) || toAccountIndex < 0 || toAccountIndex >= user.Accounts.Count || toAccountIndex == fromAccountIndex)
                {
                    Console.WriteLine("Invalid account number.");
                    return;
                }

                var toAccount = user.Accounts[toAccountIndex];

                Console.WriteLine("Enter the amount you want to transfer:");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                {
                    Console.WriteLine("Invalid amount.");
                    return;
                }

                try
                {
                    user.TransferFunds(fromAccount, toAccount, amount);
                    Console.WriteLine($"Successfully transferred {amount} {fromAccount.Currency} from account {fromAccount.AccountNumber} to account {toAccount.AccountNumber}.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else if (transferOption == "2")
            {
                // Transfer to another user's account
                Console.WriteLine("Enter the username of the recipient:");
                string recipientUsername = Console.ReadLine();

                User? recipient = bankSystem.Users.FirstOrDefault(u => u.Username.Equals(recipientUsername, StringComparison.OrdinalIgnoreCase));

                if (recipient == null)
                {
                    Console.WriteLine("Recipient not found.");
                    return;
                }

                Console.WriteLine($"Enter the account number you want to transfer funds from (0-{user.Accounts.Count() - 1}):");
                string? fromAccountIndexInput = Console.ReadLine();

                if (!int.TryParse(fromAccountIndexInput, out int fromAccountIndex) || fromAccountIndex < 0 || fromAccountIndex >= user.Accounts.Count)
                {
                    Console.WriteLine("Invalid account number.");
                    return;
                }

                var fromAccount = user.Accounts[fromAccountIndex];

                Console.WriteLine($"Available accounts for {recipient.Username}:");
                for (int i = 0; i < recipient.Accounts.Count; i++)
                {
                    var account = recipient.Accounts[i];
                    Console.WriteLine($"{i}. Account number: {account.AccountNumber}, Balance: {account.Balance}, Currency: {account.Currency}");
                }

                Console.WriteLine($"Enter the account number you want to transfer funds to (0-{recipient.Accounts.Count() - 1}):");
                string? toAccountIndexInput = Console.ReadLine();

                if (!int.TryParse(toAccountIndexInput, out int toAccountIndex) || toAccountIndex < 0 || toAccountIndex >= recipient.Accounts.Count)
                {
                    Console.WriteLine("Invalid account number.");
                    return;
                }

                var toAccount = recipient.Accounts[toAccountIndex];

                Console.WriteLine("Enter the amount you want to transfer:");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
                {
                    Console.WriteLine("Invalid amount.");
                    return;
                }

                try
                {
                    user.TransferToOtherUser(fromAccount, recipient, toAccount, amount);
                    Console.WriteLine($"Successfully transferred {amount} {fromAccount.Currency} from your account {fromAccount.AccountNumber} to {recipient.Username}'s account {toAccount.AccountNumber}.");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Invalid option selected.");
            }

            Thread.Sleep(2000);
        }


        static void OpenNewAccount(User user)
        {
            var availableCurrencies = new HashSet<string> { "USD", "EUR", "SEK" };
            string stringType = "";

            Console.WriteLine("Choose type of account:\n" +
                "1. Private Account\n" +
                "2. Saving Account\n");

            string? accountype;
            while (true)
            {
                Console.Write("Account: ");
                accountype = Console.ReadLine();

                if (accountype == "1" || accountype == "2")
                {
                    stringType = accountype == "1" ? "Account" : "Saving Account";

                    break;
                }

                Console.WriteLine("Invalid account type. Please enter 1 or 2.\n");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Available Currencies: USD, EUR, SEK");
            Console.ResetColor();
            string userCurrency;

            while (true)
            {
                Console.WriteLine("\nChoose Currency:");
                userCurrency = Console.ReadLine()?.ToUpper();

                if (availableCurrencies.Contains(userCurrency))
                {

                    Console.WriteLine($"{stringType} Created!");
                    Thread.Sleep(2000);
                    break;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid currency. Please enter on of the following: USD, EUR, SEK.");
                Console.ResetColor();
            }

            if (accountype == "1")
            {
                user.OpenNewAccount(userCurrency);
            }
            else
            {
                user.OpenNewSavingAccount("USD", 0.05m);
            }
        }

        static void CheckTransactions(User user)
        {
            foreach (var transaction in user.Transactions)
            {
                Console.WriteLine(transaction.ToString());
            }
            WaitForX();
        }

        static void CreateNewUser(BankSystem bankSystem)
        {
            string username = ValidateNonEmptyString("Enter the username of the new user:");
            string password = ValidateNonEmptyString("Enter the password of the new user:");
            string password2 = ValidateNonEmptyString("Enter the password again:");

            if (password != password2)
            {
                Console.WriteLine("Passwords do not match.");
                return;
            }

            string typeOfUser = ValidateAccountTypeInput("Enter the type of user (1: Regular, 2: Admin):");

            // Create the new user based on the type
            if (typeOfUser == "1")
            {
                bankSystem.Users.Add(new User(username, password));
            }
            else
            {
                bankSystem.Users.Add(new Admin(username, password));
            }

            Console.WriteLine("New user created successfully.");
        }

        static void Logout(User user, BankSystem bankSystem) // Gotta check such that this works
        {
            Console.WriteLine("You have been logged out.");
            Thread.Sleep(2000);
            Console.Clear();
            MainMenu(bankSystem);
        }

        static void WaitForX()
        {
            Console.WriteLine("\nWrite X to go back");
            while (true)
            {
                string? input = Console.ReadLine();
                if (input == "X" || input == "x")
                {
                    Console.Clear();
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid input. Press X to return to the main menu.");
                }
            }
        }

        static string ValidateNonEmptyString(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                Console.WriteLine("Input cannot be empty. Please enter a valid value.");
            }
        }

        static string ValidateAccountTypeInput(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                string? input = Console.ReadLine();
                if (input == "1" || input == "2")
                {
                    return input;
                }
                Console.WriteLine("Invalid account type. Please enter 1 or 2.");
            }
        }
    }
}