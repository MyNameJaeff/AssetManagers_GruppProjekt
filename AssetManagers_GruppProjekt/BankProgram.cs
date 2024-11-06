using System.Timers;

namespace AssetManagers_GruppProjekt
{
    internal class BankProgram
    {
        private static BankSystem _bankSystem = new();
        private static System.Timers.Timer? _transactionTimer;

        public BankProgram()
        {
            _bankSystem = new BankSystem();
        }

        // Every 5 minutes go through all transactions and run them as long as program is running
        public void InitializeTimer()
        {
            _transactionTimer = new System.Timers.Timer(5 * 60 * 1000); // 5 minutes in milliseconds
            _transactionTimer.Elapsed += OnTransactionTimerElapsed;
            _transactionTimer.AutoReset = true;
            _transactionTimer.Enabled = true;

            // Run the transaction execution immediately
            // OnTransactionTimerElapsed(null, null);

            // Ensure the timer is disposed of properly when the application exits
            AppDomain.CurrentDomain.ProcessExit += (s, e) => _transactionTimer.Dispose();
        }

        private static void OnTransactionTimerElapsed(object? source, ElapsedEventArgs? e)
        {
            _bankSystem.ExecutePendingTransactions();
        }

        static void PrintAsciiArt()
        {
            string ascii = @"
  /$$$$$$   /$$                      /$$                                    
 /$$__  $$ | $$                     | $$                                    
| $$  \__/ | $$   /$$$$$$   /$$$$$$$| $$   /$$  /$$$$$$   /$$$$$$   /$$$$$$$
|  $$$$$$  | $$  |____  $$ /$$_____/| $$  /$$/ /$$__  $$ /$$__  $$ /$$_____/
 \____  $$ | $$   /$$$$$$$| $$      | $$$$$$/ | $$$$$$$$| $$  \__/|  $$$$$$ 
 /$$  \ $$ | $$  /$$__  $$| $$      | $$_  $$ | $$_____/| $$       \____  $$
|  $$$$$$/ | $$ |  $$$$$$$|  $$$$$$$| $$ \  $$|  $$$$$$$| $$       /$$$$$$$/
 \______/  |__/  \_______/ \_______/|__/  \__/ \_______/|__/      |_______/ 
_______________________________________________________________________________________
            ";

            PrintCenteredText(ascii, false);
        }

        public void InitializeValues()
        {
            // Adding a sample user
            User user = new User("Jeff", "123");
            Admin admin = new Admin("Admin", "Admin");

            _bankSystem.Users.Add(user);
            _bankSystem.Users.Add(admin);

            // Create two distinct accounts for the user
            Account account1 = user.OpenNewAccount("USD");
            Account account2 = user.OpenNewAccount("USD");
            Account account3 = user.OpenNewAccount("EUR");

            Account adminaccount1 = admin.OpenNewAccount("USD");
            Account adminaccount2 = admin.OpenNewAccount("EUR");
            SavingsAccount savingsAccount = user.OpenNewSavingAccount("USD", 0.05m);

            // Deposit initial amounts into both accounts
            account1.Deposit(500);
            account2.Deposit(500);
            account3.Deposit(500);

            adminaccount1.Deposit(5000);
            adminaccount2.Deposit(5000);

            savingsAccount.Deposit(5000);


            // HÄR KAN VI LÄGGA TILL MER KOD FÖR ATT TESTA VÅR BANKSYSTEM

            _bankSystem.AddTransactionToPending(user.TransferFunds(user, user, account1, account2, 100));
        }

        static void ClearAndPrintAsciiArt()
        {
            Console.Clear();
            PrintAsciiArt();
        }
        public void MainMenu()
        {
            PrintAsciiArt();
            var checkLogin = Login();
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

                string[] menuOptions = new string[]
                {
                    "1.      View Accounts        ",
                    "-----------------------------",
                    "2.      Deposit Funds        ",
                    "-----------------------------",
                    "3.      Withdraw Funds       ",
                    "-----------------------------",
                    "4.      Transfer Funds       ",
                    "-----------------------------",
                    "5.      View Transactions    ",
                    "-----------------------------",
                    "6.      Open New Account     ",
                    "-----------------------------",
                    "7.      Take/View Loans      ",
                    "-----------------------------"
                };

                // Add the admin option if the user is an admin
                if (userIsAdmin)
                {
                    menuOptions = menuOptions.Concat(new[]
                    {
                    "8.      Create New User      ",
                    "-----------------------------"
                    }).ToArray();
                }
                menuOptions = menuOptions.Concat(new[] {
                    "0.      Logout               ",
                    "-----------------------------",
                    "X.      Exit                 ",
                    "-----------------------------"
                }).ToArray();

                PrintCenteredText(string.Join(Environment.NewLine, menuOptions), true, ConsoleColor.Cyan, ConsoleColor.DarkCyan);
                var menuActions = new Dictionary<ConsoleKey, Action>
                {
                    { ConsoleKey.D1, () => { ClearAndPrintAsciiArt(); ViewAccounts(user, true); WaitForX(); } },
                    { ConsoleKey.D2, () => { ClearAndPrintAsciiArt(); Deposit(user); } },
                    { ConsoleKey.D3, () => { ClearAndPrintAsciiArt(); Withdraw(user); } },
                    { ConsoleKey.D4, () => { ClearAndPrintAsciiArt(); TransferFunds(user); } },
                    { ConsoleKey.D5, () => { ClearAndPrintAsciiArt(); CheckTransactions(user); } },
                    { ConsoleKey.D6, () => { ClearAndPrintAsciiArt(); OpenNewAccount(user); } },
                    { ConsoleKey.D7, () => { Loan(user); ClearAndPrintAsciiArt(); } },
                    { ConsoleKey.D8, () => { if (userIsAdmin) { ClearAndPrintAsciiArt(); CreateNewUser(); } } },
                    { ConsoleKey.D0, () => { ClearAndPrintAsciiArt(); Logout(this); } },
                    { ConsoleKey.X, () => { ConfirmExit(); } }
                };


                var key = WaitForReadkey();
                if (menuActions.ContainsKey(key))
                {
                    menuActions[key].Invoke();
                }
                else
                {
                    DisplayError("Invalid option. Please try again.");
                }
            }
        }

        private void ConfirmExit()
        {
            DisplayError("Are you sure? If so press X again to exit!");
            if (WaitForReadkey() == ConsoleKey.X)
            {
                PrintCenteredText("Bye!");
                Environment.Exit(0);
            }
        }
        static User? Login()
        {
            bool userIsLoggedIn = false;
            Console.Clear(); // Clear the console for a fresh start

            while (!userIsLoggedIn)
            {
                // Display a welcome message with instructions
                string[] loginTitle = new string[]
                {
                    "====================================",
                    "             User Login             ",
                    "===================================="
                };
                foreach (var line in loginTitle)
                {
                    PrintCenteredText(line, true, ConsoleColor.Cyan, ConsoleColor.DarkCyan);
                }
                // Prompt for username
                Console.ForegroundColor = ConsoleColor.Yellow;
                string username = ValidateNonEmptyString("Enter username: ", ConsoleColor.Yellow);

                // Prompt for password with masked input
                string prompt = "Enter password: ";
                PrintCenteredText(prompt, false, ConsoleColor.Yellow);
                Console.SetCursorPosition((int)((Console.WindowWidth - prompt.Length + Math.Round(prompt.Length * 0.1)) / 2), Console.CursorTop); // Centering input cursor
                string password = ReadPassword();

                Console.ResetColor();

                // Attempt to log in with provided credentials
                User user = _bankSystem.Login(username, password);
                if (user != null)
                {
                    // Successful login message
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nLogin successful. Welcome!");
                    Console.ResetColor();
                    userIsLoggedIn = true;
                    return user;
                }
                else
                {
                    // Error message for invalid login
                    DisplayError("\nInvalid username or password. Please try again.\n");
                }

                // Optional pause for readability
                Thread.Sleep(2000);
                Console.Clear();
            }
            return null;
        }

        // Helper function to mask password input
        static string ReadPassword()
        {
            string password = "";
            ConsoleKeyInfo key;

            // Read each key entered by the user
            do
            {
                key = Console.ReadKey(intercept: true);

                // Ignore Enter key, Backspace, and add to password if character key
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password.Remove(password.Length - 1);
                    Console.Write("\b \b"); // Remove last asterisk
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password += key.KeyChar;
                    Console.Write("*"); // Display asterisk for each character
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }


        static void Loan(User user)
        {
            string loanOption = ValidateNonEmptyString("Select 1 to view all your loans, select 2 to take a loan:");

            if (loanOption == "1")
            {
                int LoanLenght = user.Loans.Count;
                if (LoanLenght == 0)
                {
                    PrintCenteredText("You currently have no loans");
                    WaitForX();
                    return;
                }

                user.Loans.ForEach(loan => PrintCenteredText(loan.ToString()));
                WaitForX();

                return;
            }

            string selectedIndex = ValidateNonEmptyString($"Enter the account number to loan money to (0-{user.Accounts.Count() - 1}):");
            if (!int.TryParse(selectedIndex, out int accountIndex) || accountIndex < 0 || accountIndex >= user.Accounts.Count)
            {
                DisplayError("Invalid account number.");
                WaitForX();
                return;
            }

            var account = user.Accounts[accountIndex];

            string moneyCount = ValidateNonEmptyString("How much money would you like to borrow?");

            int moneyAmount = 0;
            try
            {
                moneyAmount = Convert.ToInt32(moneyCount);
            }

            catch (Exception A)
            {
                DisplayError(A.Message);
                WaitForX();
                return;
            }

            if (!decimal.TryParse(moneyCount, out decimal loanedMoney) || loanedMoney <= 0)
            {
                DisplayError("Invalid input. Please enter a valid loan amount greater than 0.");
                WaitForX();
                return;
            }

            string monthCount = ValidateNonEmptyString("Over how many months do you plan to repay the loan? (1-240)");
            if (!int.TryParse(monthCount, out int loanPeriod) || loanPeriod <= 0 || loanPeriod > 240)
            {
                DisplayError("Invalid input. Please enter a valid loan period between 1 and 240.");
                WaitForX();
                return;
            }

            // Apply loan to the selected account
            try
            {
                user.Loan(0.05m, loanedMoney, loanPeriod);
                Console.WriteLine($"Loan of {loanedMoney} {account.Currency} approved. You have {loanPeriod} months to repay.");
                Console.WriteLine($"Loan start date: {DateTime.Now}, end date: {DateTime.Now.AddMonths(loanPeriod)}");
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
            WaitForX();
        }

        static void ViewAccounts(User user, bool detailed)
        {
            var userAccounts = user.Accounts;

            if (user.Accounts.Count == 0)
            {
                PrintCenteredText("You have no accounts yet, go make one!", true, ConsoleColor.Yellow);
                WaitForX();
                return;
            }

            PrintCenteredText("Accounts:", true, ConsoleColor.White);
            PrintCenteredText("---------------------------------------------------------------------------------------------", true, ConsoleColor.Gray);
            PrintCenteredText("| #   | Account ID                           | Balance      | Currency | Account Type    |", true, ConsoleColor.Gray);
            PrintCenteredText("---------------------------------------------------------------------------------------------", true, ConsoleColor.Gray);

            int accountNumber = 1;
            foreach (var account in userAccounts)
            {
                if (account is SavingsAccount savingsAccount)
                {
                    PrintCenteredText(
                        $"| {accountNumber,-3} | {account.AccountNumber,-36} | {account.Balance,-12:C} | {account.Currency,-8} | Savings Account |", true, ConsoleColor.White);
                    PrintCenteredText("---------------------------------------------------------------------------------------------", true, ConsoleColor.Gray);

                    if (detailed)
                    {
                        PrintCenteredText($"  Interest Income: {savingsAccount.Balance * savingsAccount.InterestRate:C}, Interest Rate: {savingsAccount.InterestRate * 100}%", true, ConsoleColor.DarkYellow);
                    }
                }
                else
                {
                    PrintCenteredText(
                        $"| {accountNumber,-3} | {account.AccountNumber,-36} | {account.Balance,-12:C} | {account.Currency,-8} | Private Account |", true, ConsoleColor.White);
                    PrintCenteredText("---------------------------------------------------------------------------------------------", true, ConsoleColor.Gray);
                }
                accountNumber++;
            }
            Console.WriteLine();
        }


        static void Deposit(User user)
        {
            ViewAccounts(user, false);
            string? accountIndexInput = ValidateNonEmptyString($"\nEnter the account number you want to deposit funds to (1-{user.Accounts.Count()}):");

            if (!int.TryParse(accountIndexInput, out int accountIndex) || accountIndex - 1 < 0 || accountIndex - 1 >= user.Accounts.Count)
            {
                DisplayError("Invalid account number.");
                WaitForX();
                return;
            }

            var account = user.Accounts[accountIndex - 1];

            string depositAmount = ValidateNonEmptyString("Enter the amount you want to deposit:");
            if (!decimal.TryParse(depositAmount, out decimal amount) || amount <= 0)
            {
                DisplayError("Invalid amount.");
                WaitForX();
                return;
            }

            try
            {
                account.Deposit(amount);
                PrintCenteredText($"Successfully deposited {amount} {account.Currency} to account {account.AccountNumber}.");
                PrintCenteredText($"The accounts new balance is: {account.Balance} {account.Currency}.");
                WaitForX();
            }
            catch (Exception ex)
            {
                DisplayError($"Occurred during deposit: {ex.Message}");
            }
        }

        static void Withdraw(User user)
        {
            ViewAccounts(user, false);
            string accountIndexInput = ValidateNonEmptyString($"\nEnter the account number you want to withdraw funds from (1-{user.Accounts.Count()}):");

            if (!int.TryParse(accountIndexInput, out int accountIndex) || accountIndex - 1 < 0 || accountIndex - 1 >= user.Accounts.Count)
            {
                DisplayError("Invalid account number.");
                return;
            }

            var account = user.Accounts[accountIndex - 1];

            string amountInput = ValidateNonEmptyString("Enter the amount you want to withdraw:");
            if (!decimal.TryParse(amountInput, out decimal amount) || amount <= 0)
            {
                DisplayError("Invalid amount.");
                return;
            }

            try
            {
                Console.WriteLine();
                account.Withdraw(amount);
                PrintCenteredText($"Successfully withdrew {amount} {account.Currency} from account {account.AccountNumber}.");
                PrintCenteredText($"The accounts new balance is: {account.Balance} {account.Currency}.");
                WaitForX();
            }
            catch (Exception ex)
            {
                DisplayError($"Ooccurred during withdrawal: {ex.Message}");
            }
        }
        static void TransferFunds(User user)
        {
            string transferOption = ValidateNonEmptyString("Choose transfer option:\n" +
                              "1. Transfer to another account (within your own accounts)\n" +
                              "2. Transfer to another user's account\n");

            if (transferOption == "1")
            {
                ClearAndPrintAsciiArt();
                ViewAccounts(user, false);
                // Transfer within the user's own accounts
                string? fromAccountIndexInput = ValidateNonEmptyString($"\nEnter the account number you want to transfer funds from (1-{user.Accounts.Count()}):");

                if (!int.TryParse(fromAccountIndexInput, out int fromAccountIndex) || fromAccountIndex - 1 < 0 || fromAccountIndex - 1 >= user.Accounts.Count)
                {
                    DisplayError("Invalid account number.");
                    return;
                }

                var fromAccount = user.Accounts[fromAccountIndex - 1];

                string? toAccountIndexInput = ValidateNonEmptyString($"Enter the account number you want to transfer funds to (1-{user.Accounts.Count()}):");

                if (!int.TryParse(toAccountIndexInput, out int toAccountIndex) || toAccountIndex - 1 < 0 || toAccountIndex - 1 >= user.Accounts.Count || toAccountIndex - 1 == fromAccountIndex - 1)
                {
                    DisplayError("Invalid account number.");
                    return;
                }

                var toAccount = user.Accounts[toAccountIndex - 1];

                if (fromAccount.Currency != toAccount.Currency)
                {
                    PrintCenteredText($"You are trying to transfer {fromAccount.Currency} to {toAccount.Currency} the current conversion rate is: ");
                    PrintCenteredText(_bankSystem.DisplayExchangeRates());
                }

                string transferAmount = ValidateNonEmptyString("Enter the amount you want to transfer:");

                if (!decimal.TryParse(transferAmount, out decimal amount) || amount <= 0)
                {
                    DisplayError("Invalid amount.");
                    return;
                }

                decimal convertedAmount = _bankSystem.ConvertCurrency(amount, fromAccount.Currency, toAccount.Currency);

                try
                {
                    _bankSystem.AddTransactionToPending(user.TransferFunds(user, user, fromAccount, toAccount, convertedAmount));
                    PrintCenteredText($"Successfully transferred {amount} {fromAccount.Currency} from account {fromAccount.AccountNumber} to account {toAccount.AccountNumber}. {convertedAmount} {toAccount.Currency}.");
                    WaitForX();
                }
                catch (InvalidOperationException ex)
                {
                    DisplayError(ex.Message);
                }
            }
            else if (transferOption == "2")
            {
                // Transfer to another user's account
                string recipientUsername = ValidateNonEmptyString("Enter the username of the recipient:");

                User? recipient = _bankSystem.Users.FirstOrDefault(u => u.Username.Equals(recipientUsername, StringComparison.OrdinalIgnoreCase));

                if (recipient == null)
                {
                    DisplayError("Recipient not found.");
                    return;
                }

                if (recipient.Accounts.Count == 0)
                {
                    DisplayError("They dont have any accounts!");
                    return;
                }

                ClearAndPrintAsciiArt();
                ViewAccounts(user, false);
                string? fromAccountIndexInput = ValidateNonEmptyString($"\nEnter the account number you want to transfer funds from (1-{user.Accounts.Count()}):");

                if (!int.TryParse(fromAccountIndexInput, out int fromAccountIndex) || fromAccountIndex - 1 < 0 || fromAccountIndex - 1 >= user.Accounts.Count)
                {
                    DisplayError("Invalid account number.");
                    return;
                }

                var fromAccount = user.Accounts[fromAccountIndex - 1];

                PrintCenteredText($"Available accounts for {recipient.Username}:");
                for (int i = 0; i < recipient.Accounts.Count; i++)
                {
                    var account = recipient.Accounts[i];
                    PrintCenteredText($"{i}. Account number: {account.AccountNumber}, Balance: {account.Balance}, Currency: {account.Currency}");
                }

                string? toAccountIndexInput = ValidateNonEmptyString($"Enter the account number you want to transfer funds to (1-{recipient.Accounts.Count()}):");

                if (!int.TryParse(toAccountIndexInput, out int toAccountIndex) || toAccountIndex - 1 < 0 || toAccountIndex - 1 >= recipient.Accounts.Count)
                {
                    DisplayError("Invalid account number.");
                    WaitForX();
                    return;
                }

                var toAccount = recipient.Accounts[toAccountIndex - 1];

                if (fromAccount.Currency != toAccount.Currency)
                {
                    PrintCenteredText($"You are trying to transfer {fromAccount.Currency} to {toAccount.Currency} the current conversion rate is: ");
                    _bankSystem.DisplayExchangeRates();
                }

                string transferAmount = ValidateNonEmptyString("Enter the amount you want to transfer:");

                if (!decimal.TryParse(transferAmount, out decimal amount) || amount <= 0)
                {
                    DisplayError("Invalid amount.");
                    return;
                }

                decimal convertedAmount = _bankSystem.ConvertCurrency(amount, fromAccount.Currency, toAccount.Currency);

                try
                {
                    _bankSystem.AddTransactionToPending(user.TransferFunds(user, recipient, fromAccount, toAccount, convertedAmount));
                    PrintCenteredText($"Successfully transferred {amount} {fromAccount.Currency} from account {fromAccount.AccountNumber} to account {toAccount.AccountNumber}. {convertedAmount} {toAccount.Currency}.");
                    WaitForX();
                }

                catch (InvalidOperationException ex)
                {
                    DisplayError(ex.Message);
                }
            }
            else
            {
                PrintCenteredText("Invalid option selected.");
            }

            WaitForX();
        }


        static void OpenNewAccount(User user)
        {
            var availableCurrencies = new HashSet<string> { "USD", "EUR", "SEK" };
            string stringType = "";

            PrintCenteredText("Choose type of account:\n");
            PrintCenteredText("1. Private Account");
            PrintCenteredText("2. Saving Account");

            string? accountype;
            while (true)
            {
                accountype = ValidateNonEmptyString("Account: ");

                if (accountype == "1" || accountype == "2")
                {
                    stringType = accountype == "1" ? "Account" : "Saving Account";

                    break;
                }

                DisplayError("Invalid account type. Please enter 1 or 2.\n");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            PrintCenteredText("Available Currencies: USD, EUR, SEK");
            Console.ResetColor();
            string userCurrency;

            while (true)
            {
                Console.WriteLine("\nChoose Currency:");
                userCurrency = Console.ReadLine()?.ToUpper();

                if (availableCurrencies.Contains(userCurrency))
                {

                    Console.WriteLine($"{stringType} Created!");
                    WaitForX();
                    break;
                }

                DisplayError("Invalid currency. Please enter on of the following: USD, EUR, SEK.");
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
            Console.WriteLine("Transactions:");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("|    Date and Time    | Amount | Currency |               Sender ID              |               Receiver ID            |");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------");

            foreach (var transaction in user.Transactions)
            {
                Console.WriteLine($"| {transaction.Timestamp} | {transaction.Amount,-6} | {transaction.Currency,-6}   | {transaction.FromAccount.AccountNumber,-10} | {transaction.ToAccount.AccountNumber,-28} |");
            }

            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------");

            // Pauses execution to allow user to view transactions
            WaitForX();
        }

        static void CreateNewUser()
        {
            string username;

            while (true)
            {
                username = ValidateNonEmptyString("Enter the username of the new user:");
                if (!_bankSystem.Users.Any(user => user.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    break;
                }
                DisplayError("Username already exists. Please choose a different username.");
            }

            string password;
            string password2;
            while (true) // Loop until the user successfully confirms the password
            {
                password = ReadPasswordWithValidation("Enter the password for the new user: ");
                Console.Write("Confirm Password: ");
                password2 = ReadPassword();

                if (password != password2)
                {
                    DisplayError("Passwords do not match. Please try again.\n");
                }
                else
                {
                    break;
                }
            }

            string typeOfUser = ValidateAccountTypeInput("Enter the type of user (1: Regular, 2: Admin):");

            // Create the new user based on the type
            if (typeOfUser == "1")
            {
                _bankSystem.Users.Add(new User(username, password));
            }
            else if (typeOfUser == "2")
            {
                _bankSystem.Users.Add(new Admin(username, password));
            }
            else
            {
                DisplayError("Invalid user type specified.");
                return;
            }

            PrintCenteredText("New user created successfully.");
            WaitForX();
        }

        static void Logout(BankProgram bankProgram) // Gotta check such that this works
        {
            PrintCenteredText("You have been logged out.");
            Thread.Sleep(4000);
            Console.Clear();
            bankProgram.MainMenu();
        }



        static void WaitForX()
        {
            // Set instruction text color
            PrintCenteredText("\nPress 'X' to go back.");

            while (true)
            {
                // Capture the key press
                ConsoleKey keyInfo = WaitForReadkey();

                // Check if the key pressed was 'X' or 'x'
                if (keyInfo == ConsoleKey.X)
                {
                    Console.Clear();
                    Console.ResetColor(); // Reset color to default before returning
                    return;
                }
                else
                {
                    DisplayError("Invalid input. Please press 'X' to go back.");

                    // Reset to instruction color for clarity
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
            }
        }

        static ConsoleKey WaitForReadkey()
        {
            return Console.ReadKey(intercept: true).Key;
        }

        static void DisplayError(string message)
        {
            PrintCenteredText(message, false, ConsoleColor.Red, ConsoleColor.White); // Display the error message
        }

        static void PrintCenteredText(string text, bool alternateColors = false, ConsoleColor color1 = ConsoleColor.Cyan, ConsoleColor color2 = ConsoleColor.DarkCyan)
        {
            string[] lines = text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int windowWidth = Console.WindowWidth;
            int i = 0;

            // Print each menu option, optionally centered and with alternating colors
            foreach (var line in lines)
            {
                if (alternateColors)
                {
                    Console.ForegroundColor = (i % 2 == 0) ? color1 : color2;
                }
                else
                {
                    Console.ForegroundColor = color1;
                }
                i++;

                // Calculate padding to center the line
                int padding = (windowWidth - line.Length) / 2;

                // Print spaces for centering
                Console.Write(new string(' ', Math.Max(0, padding))); // Ensure padding is non-negative

                Console.WriteLine(line);
            }

            Console.ResetColor();
        }

        // Validation methods under here:

        static string ReadPasswordWithValidation(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string password = ReadPassword();

                if (IsValidPassword(password))
                {
                    return password;
                }

                DisplayError("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit.");
            }
        }

        static bool IsValidPassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);

            return hasUpperCase && hasLowerCase && hasDigit;
        }

        static string ValidateNonEmptyString(string prompt, ConsoleColor color1 = ConsoleColor.White)
        {
            while (true)
            {
                PrintCenteredText(prompt, false, color1);
                Console.SetCursorPosition((int)((Console.WindowWidth - prompt.Length + Math.Round(prompt.Length * 0.1)) / 2), Console.CursorTop); // Centering input cursor
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                DisplayError("Input cannot be empty. Please enter a valid value.");
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
                DisplayError("Invalid account type. Please enter 1 or 2.");
            }
        }
    }
}
