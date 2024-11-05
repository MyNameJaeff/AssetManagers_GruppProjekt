namespace AssetManagers_GruppProjekt
{
    internal class BankProgram
    {
        private static BankSystem _bankSystem = new();

        public BankProgram()
        {
            _bankSystem = new BankSystem();
        }

        public void PrintAsciiArt()
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

            // Split the ASCII art into lines
            string[] lines = ascii.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // List to hold individual characters
            List<string> combinedLines = new List<string>();

            // Process each line to split letters and combine them
            foreach (string line in lines)
            {
                // Split each character and combine them into a single string
                string combinedLine = string.Join("", line.Select(c => c.ToString()));
                combinedLines.Add(combinedLine);
            }

            // Get the width of the terminal window
            int windowWidth = Console.WindowWidth;

            Console.ForegroundColor = ConsoleColor.Cyan;

            // Print each combined line, centered
            foreach (string combinedLine in combinedLines)
            {
                // Calculate padding to center the line
                int padding = (windowWidth - combinedLine.Length) / 2;

                // Print spaces for centering
                Console.Write(new string(' ', padding));

                // Print the combined line
                Console.WriteLine(combinedLine);
            }
            Console.ResetColor();
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
            SavingsAccount savingsAccount = user.OpenNewSavingAccount("USD", 0.05m);

            // Deposit initial amounts into both accounts
            account1.Deposit(500);
            account2.Deposit(500);
            savingsAccount.Deposit(5000);


            // HÄR KAN VI LÄGGA TILL MER KOD FÖR ATT TESTA VÅR BANKSYSTEM

            user.TransferFunds(account1, account2, 100);
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

                // Get the width of the console window
                int windowWidth = Console.WindowWidth;

                Console.ForegroundColor = ConsoleColor.Cyan; // Set text color
                int i = 0;
                // Print each menu option centered
                foreach (var option in menuOptions)
                {
                    if (i % 2 == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    i++;
                    // Calculate padding to center the line
                    int padding = (windowWidth - option.Length) / 2;

                    // Print spaces for centering
                    Console.Write(new string(' ', Math.Max(0, padding))); // Ensure padding is non-negative
                    Console.WriteLine(option);
                }

                Console.ResetColor(); // Reset text color to default

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
                        TransferFunds(user);
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
                        {
                            Loan(user);
                            break;
                        }
                    case "8":
                        if (userIsAdmin)
                        {
                            // Create new user logic
                            CreateNewUser();
                        }
                        break;
                    case "0":
                        // Logout logic
                        Logout(this);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
        static User? Login()
        {
            bool userIsLoggedIn = false;
            Console.Clear(); // Clear the console for a fresh start

            while (!userIsLoggedIn)
            {
                // Display a welcome message with instructions
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("====================================");
                Console.WriteLine("           User Login");
                Console.WriteLine("====================================");
                Console.ResetColor();

                // Prompt for username
                Console.ForegroundColor = ConsoleColor.Yellow;
                string username = ValidateNonEmptyString("Enter username: ");

                // Prompt for password with masked input
                Console.WriteLine("Enter password: ");
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
                System.Threading.Thread.Sleep(1000);
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
                    Console.WriteLine("You currently have no loans");
                    WaitForX();
                    return;
                }

                user.Loans.ForEach(loan => Console.WriteLine(loan.ToString()));
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

        static void ViewAccounts(User user)
        {
            var userAccounts = user.Accounts;

            Console.WriteLine("Accounts:");
            foreach (var account in userAccounts)
            {
                if (account is SavingsAccount savingsAccount)
                {
                    Console.WriteLine($"Account number: {account.AccountNumber}, Balance: {account.Balance}, Currency: {account.Currency} (Savings account), {savingsAccount.Balance * savingsAccount.InterestRate}kr/år ({savingsAccount.InterestRate * 100}%)");
                }
                else
                {
                    Console.WriteLine($"Account number: {account.AccountNumber}, Balance: {account.Balance}, Currency: {account.Currency} (Private account)");
                }
            }
            WaitForX();
        }

        static void Deposit(User user)
        {
            string? accountIndexInput = ValidateNonEmptyString($"Enter the account number you want to deposit funds to (0-{user.Accounts.Count() - 1}):");

            if (!int.TryParse(accountIndexInput, out int accountIndex) || accountIndex < 0 || accountIndex >= user.Accounts.Count)
            {
                DisplayError("Invalid account number.");
                return;
            }

            var account = user.Accounts[accountIndex];

            string depositAmount = ValidateNonEmptyString("Enter the amount you want to deposit:");
            if (!decimal.TryParse(depositAmount, out decimal amount) || amount <= 0)
            {
                DisplayError("Invalid amount.");
                return;
            }

            try
            {
                account.Deposit(amount);
                Console.WriteLine($"Successfully deposited {amount} {account.Currency} to account {account.AccountNumber}.");
                Console.WriteLine($"The accounts new balance is: {account.Balance} {account.Currency}.");
                Thread.Sleep(2000);

            }
            catch (Exception ex)
            {
                DisplayError($"Occurred during deposit: {ex.Message}");
            }
        }

        static void Withdraw(User user)
        {
            string accountIndexInput = ValidateNonEmptyString($"Enter the account number you want to withdraw funds from (0-{user.Accounts.Count() - 1}):");

            if (!int.TryParse(accountIndexInput, out int accountIndex) || accountIndex < 0 || accountIndex >= user.Accounts.Count)
            {
                DisplayError("Invalid account number.");
                return;
            }

            var account = user.Accounts[accountIndex];

            string amountInput = ValidateNonEmptyString("Enter the amount you want to withdraw:");
            if (!decimal.TryParse(amountInput, out decimal amount) || amount <= 0)
            {
                DisplayError("Invalid amount.");
                return;
            }

            try
            {
                account.Withdraw(amount);
                Console.WriteLine($"Successfully withdrew {amount} {account.Currency} from account {account.AccountNumber}.");
                Console.WriteLine($"The accounts new balance is: {account.Balance} {account.Currency}.");
                Thread.Sleep(2000);
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
                // Transfer within the user's own accounts
                string? fromAccountIndexInput = ValidateNonEmptyString($"Enter the account number you want to transfer funds from (0-{user.Accounts.Count() - 1}):");

                if (!int.TryParse(fromAccountIndexInput, out int fromAccountIndex) || fromAccountIndex < 0 || fromAccountIndex >= user.Accounts.Count)
                {
                    DisplayError("Invalid account number.");
                    return;
                }

                var fromAccount = user.Accounts[fromAccountIndex];

                string? toAccountIndexInput = ValidateNonEmptyString($"Enter the account number you want to transfer funds to (0-{user.Accounts.Count() - 1}):");

                if (!int.TryParse(toAccountIndexInput, out int toAccountIndex) || toAccountIndex < 0 || toAccountIndex >= user.Accounts.Count || toAccountIndex == fromAccountIndex)
                {
                    DisplayError("Invalid account number.");
                    return;
                }

                var toAccount = user.Accounts[toAccountIndex];

                string transferAmount = ValidateNonEmptyString("Enter the amount you want to transfer:");
                if (!decimal.TryParse(transferAmount, out decimal amount) || amount <= 0)
                {
                    DisplayError("Invalid amount.");
                    return;
                }

                try
                {
                    user.TransferFunds(fromAccount, toAccount, amount);
                    Console.WriteLine($"Successfully transferred {amount} {fromAccount.Currency} from account {fromAccount.AccountNumber} to account {toAccount.AccountNumber}.");
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

                string? fromAccountIndexInput = ValidateNonEmptyString($"Enter the account number you want to transfer funds from (0-{user.Accounts.Count() - 1}):");

                if (!int.TryParse(fromAccountIndexInput, out int fromAccountIndex) || fromAccountIndex < 0 || fromAccountIndex >= user.Accounts.Count)
                {
                    DisplayError("Invalid account number.");
                    return;
                }

                var fromAccount = user.Accounts[fromAccountIndex];

                Console.WriteLine($"Available accounts for {recipient.Username}:");
                for (int i = 0; i < recipient.Accounts.Count; i++)
                {
                    var account = recipient.Accounts[i];
                    Console.WriteLine($"{i}. Account number: {account.AccountNumber}, Balance: {account.Balance}, Currency: {account.Currency}");
                }

                string? toAccountIndexInput = ValidateNonEmptyString($"Enter the account number you want to transfer funds to (0-{recipient.Accounts.Count() - 1}):");

                if (!int.TryParse(toAccountIndexInput, out int toAccountIndex) || toAccountIndex < 0 || toAccountIndex >= recipient.Accounts.Count)
                {
                    DisplayError("Invalid account number.");
                    return;
                }

                var toAccount = recipient.Accounts[toAccountIndex];

                string transferAmount = ValidateNonEmptyString("Enter the amount you want to transfer:");
                if (!decimal.TryParse(transferAmount, out decimal amount) || amount <= 0)
                {
                    DisplayError("Invalid amount.");
                    return;
                }

                try
                {
                    user.TransferToOtherUser(fromAccount, recipient, toAccount, amount);
                    Console.WriteLine($"Successfully transferred {amount} {fromAccount.Currency} from your account {fromAccount.AccountNumber} to {recipient.Username}'s account {toAccount.AccountNumber}.");
                }
                catch (InvalidOperationException ex)
                {
                    DisplayError(ex.Message);
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
                accountype = ValidateNonEmptyString("Account: ");

                if (accountype == "1" || accountype == "2")
                {
                    stringType = accountype == "1" ? "Account" : "Saving Account";

                    break;
                }

                DisplayError("Invalid account type. Please enter 1 or 2.\n");
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
            foreach (var transaction in user.Transactions)
            {
                Console.WriteLine(transaction.ToString());
            }
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

            Console.WriteLine("New user created successfully.");
            WaitForX();
        }

        static void Logout(BankProgram bankProgram) // Gotta check such that this works
        {
            Console.WriteLine("You have been logged out.");
            Thread.Sleep(2000);
            Console.Clear();
            bankProgram.MainMenu();
        }

        static void WaitForX()
        {
            // Set instruction text color
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nPress 'X' to go back.");

            while (true)
            {
                // Capture the key press
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true); // 'true' to prevent key output on the console

                // Check if the key pressed was 'X' or 'x'
                if (keyInfo.Key == ConsoleKey.X)
                {
                    Console.Clear();
                    Console.ResetColor(); // Reset color to default before returning
                    return;
                }
                else
                {
                    // Set color for invalid input message

                    DisplayError("Invalid input. Please press 'X' to go back.");

                    // Reset to instruction color for clarity
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
            }
        }

        static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red; // Set color for errors
            Console.WriteLine($"Error: {message}"); // Display the error message
            Console.ResetColor(); // Reset color to default
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
