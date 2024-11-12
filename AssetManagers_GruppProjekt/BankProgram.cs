using System.Timers;

namespace AssetManagers_GruppProjekt
{
    internal class BankProgram
    {
        private static BankSystem _bankSystem = new();
        private static System.Timers.Timer? _transactionTimer;

        // Initializes the BankSystem
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

            // Run the transaction execution immediately, then wait for the timer each time
            OnTransactionTimerElapsed(null, null);

            // Ensure the timer is disposed of properly when the application exits
            AppDomain.CurrentDomain.ProcessExit += (s, e) => _transactionTimer.Dispose();
        }

        // Handles the transaction execution event
        private static void OnTransactionTimerElapsed(object? source, ElapsedEventArgs? e)
        {
            _bankSystem.ExecutePendingTransactions();
        }

        // Prints centered ASCII art to the console
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

        // Testing code that makes up some premade values for the bank system
        public void InitializeValues()
        {
            // Add more code here if you want to test it further, this is more of an example
            User user = new User("Jeff", "123");
            Admin admin = new Admin("Admin", "Admin");
            Admin rng = new Admin("m", "1");

            _bankSystem.Users.Add(user);
            _bankSystem.Users.Add(admin);
            _bankSystem.Users.Add(rng);

            Account account1 = user.OpenNewAccount("USD");
            Account account2 = user.OpenNewAccount("USD");
            Account account3 = user.OpenNewAccount("EUR");

            Account adminaccount1 = admin.OpenNewAccount("USD");
            Account adminaccount2 = admin.OpenNewAccount("EUR");
            SavingsAccount savingsAccount = user.OpenNewSavingAccount("USD", 0.05m);

            account1.Deposit(500);
            account2.Deposit(500);
            account3.Deposit(500);

            adminaccount1.Deposit(5000);
            adminaccount2.Deposit(5000);

            savingsAccount.Deposit(5000);

            decimal convertedAmount = _bankSystem.ConvertCurrency(100, account1.Currency, account2.Currency);

            _bankSystem.AddTransactionToPending(user.TransferFunds(user, user, account1, account2, 100, convertedAmount));
        }

        // Clears the console and prints out ASCII art
        static void ClearAndPrintAsciiArt()
        {
            Console.Clear();
            PrintAsciiArt();
        }

        // Handles the main menu of the program and user interaction
        public void MainMenu()
        {
            PrintAsciiArt();
            var checkLogin = Login();
            if (checkLogin == null) // Checks if the user is logged in, breaks the loop if not
            {
                return;
            }
            User? user = (User)checkLogin;
            bool userIsAdmin = user is Admin; // Used to create the admin menu if the user is an admin

            string errorMessage = string.Empty;

            while (true) // Loops the main menu until the user logs out or exits the program
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
                    "-----------------------------",
                    "9.      Update Exchange Rate ",
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

                if (!string.IsNullOrEmpty(errorMessage)) // Display the error message if it exists
                {
                    DisplayError(errorMessage);
                }

                // Dictionary to map the menu options to actions 
                var menuActions = new Dictionary<ConsoleKey, Action>
                {
                    { ConsoleKey.D1, () => { ClearAndPrintAsciiArt(); ViewAccounts(user, true); WaitForX(); } },
                    { ConsoleKey.D2, () => { ClearAndPrintAsciiArt(); Deposit(user); } },
                    { ConsoleKey.D3, () => { ClearAndPrintAsciiArt(); Withdraw(user); } },
                    { ConsoleKey.D4, () => { ClearAndPrintAsciiArt(); TransferFunds(user); } },
                    { ConsoleKey.D5, () => { ClearAndPrintAsciiArt(); CheckTransactions(user); } },
                    { ConsoleKey.D6, () => { ClearAndPrintAsciiArt(); OpenNewAccount(user); } },
                    { ConsoleKey.D7, () => { ClearAndPrintAsciiArt(); Loan(user); } },
                    { ConsoleKey.D8, () => { if (userIsAdmin) { ClearAndPrintAsciiArt(); CreateNewUser(); } } },
                    { ConsoleKey.D9, () => { if (userIsAdmin) { ClearAndPrintAsciiArt(); UpdateExchangeRate(_bankSystem); } } },
                    { ConsoleKey.D0, () => { ClearAndPrintAsciiArt(); Logout(this); } },
                    { ConsoleKey.X, () => { ConfirmExit(); } }
                };

                var key = WaitForReadkey();
                if (menuActions.ContainsKey(key)) // Check if the key is a valid menu option
                {
                    menuActions[key].Invoke();
                    errorMessage = string.Empty; // Clear the error message after a valid action
                }
                else
                {
                    errorMessage = "Invalid option. Please try again."; // Set the error message if the option is invalid
                }
            }
        }

        // Method to check if the user really wants to exit the program such that they dont accidentaly close it
        private void ConfirmExit()
        {
            DisplayError("Are you sure? If so press X again to exit!");
            if (WaitForReadkey() == ConsoleKey.X)
            {
                PrintCenteredText("Bye!");
                Environment.Exit(0);
            }
        }

        // Handles the login process for the user
        static User? Login()
        {
            bool userIsLoggedIn = false;
            Console.Clear();

            while (!userIsLoggedIn) // Loop until the user is logged in or until they get locked out
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

        // Method to allow the user to take out loans from the bank
        static void Loan(User user)
        {
            if (user.Accounts.Count == 0) // Checks such that the user has an account to loan to
            {
                DisplayError("You have no accounts to deposit to.");
                WaitForX();
                return;
            }
            while (true) // Loops through options untill the user selects a valid option
            {
                // Get valid menu option
                string loanOption = ValidateNonEmptyString("Select 1 to view all your loans, select 2 to take a loan:");
                if (loanOption != "1" && loanOption != "2") // Checks if the input is valid
                {
                    DisplayError("Invalid option. Please try again.");
                    continue;
                }
                else if (loanOption == "1") // Displays all the loans the user has taken
                {
                    int LoanLenght = user.Loans.Count;
                    if (LoanLenght == 0) // If they have no loans then print that
                    {
                        PrintCenteredText("You currently have no loans");
                        WaitForX();
                        return;
                    }

                    user.Loans.ForEach(loan => PrintCenteredText(loan.ToString()));
                    WaitForX();
                    return;
                }

                // Get valid selection of account to loan to
                string selectedIndex;
                while (true) // Loops through options untill the user selects a valid option
                {
                    ViewAccounts(user, false);
                    selectedIndex = ValidateNonEmptyString($"Enter the account number to loan money to (1-{user.Accounts.Count()}):");
                    if (int.TryParse(selectedIndex, out int accountIndex) && accountIndex - 1 >= 0 && accountIndex - 1 < user.Accounts.Count)
                    {
                        break;  // If valid input, break out of the loop
                    }
                    else
                    {
                        DisplayError("Invalid account number. Please try again.");
                    }
                }
                var userTotalBalanceConverted = 0.0m;

                foreach (Account accountBalance in user.Accounts)
                {
                    userTotalBalanceConverted += _bankSystem.ConvertCurrency(accountBalance.Balance, accountBalance.Currency, "USD");
                }

                var account = user.Accounts[int.Parse(selectedIndex) - 1]; // Selects the user's account based on their input

                // Get valid loan amount
                decimal loanedMoney;
                while (true) // Loops through options untill the user selects a valid option
                {
                    string moneyCount = ValidateNonEmptyString("How much money would you like to borrow?");
                    if (decimal.TryParse(moneyCount, out loanedMoney) && loanedMoney > 0)
                    {
                        if (loanedMoney > (userTotalBalanceConverted * 5)) // Checks such that the user cant loan more than 5 times their account balance
                        {
                            if (account.Balance < 5)
                            {
                                DisplayError($"You have to little money to loan anything! Minimum of 5 {account.Currency} in any of your accounts to make a loan!");
                                WaitForX();
                                return;
                            }
                            DisplayError($"You can't loan more than 5 times your current total balance: {userTotalBalanceConverted:F2} {account.Currency}, your max is {userTotalBalanceConverted * 5:F2} {account.Currency}");
                            continue;
                        }
                        else
                        {
                            break;  // Valid input, break out of the loop
                        }
                    }
                    else
                    {
                        DisplayError("Invalid input. Please enter a valid loan amount greater than 0.");
                    }
                }

                // Get valid loan period
                int loanPeriod;
                while (true) // Loops through options untill the user selects a valid option
                {
                    string monthCount = ValidateNonEmptyString("Over how many months do you plan to repay the loan? (1-240)");
                    if (int.TryParse(monthCount, out loanPeriod) && loanPeriod > 0 && loanPeriod <= 240)
                    {
                        break;  // Valid input, break out of the loop
                    }
                    else
                    {
                        DisplayError("Invalid input. Please enter a valid loan period between 1 and 240.");
                    }
                }

                // Apply loan to the selected account
                try
                {
                    user.Loan(0.05m, loanedMoney, loanPeriod);
                    account.Deposit(loanedMoney);
                    PrintCenteredText($"Loan of {loanedMoney} {account.Currency} approved. You have {loanPeriod} months to repay.");
                    PrintCenteredText($"Loan start date: {DateTime.Now}, end date: {DateTime.Now.AddMonths(loanPeriod)}");
                }
                catch (Exception ex)
                {
                    DisplayError(ex.Message);
                }

                WaitForX();
                return;
            }
        }

        // Method to print all of the accounts that a user has, with their balance, currency and account type
        static void ViewAccounts(User user, bool detailed)
        {
            var userAccounts = user.Accounts;

            if (user.Accounts.Count == 0) // Checks such that the user has an account
            {
                DisplayError("You have no accounts yet, go make one!");
                return;
            }

            PrintCenteredText("Accounts:", true, ConsoleColor.White);
            PrintCenteredText("---------------------------------------------------------------------------------------------", true, ConsoleColor.Gray);
            PrintCenteredText("| #   | Account ID                           | Balance      | Currency | Account Type    |", true, ConsoleColor.Gray);
            PrintCenteredText("---------------------------------------------------------------------------------------------", true, ConsoleColor.Gray);

            int accountNumber = 1;
            foreach (var account in userAccounts)
            {
                // Check if the account is a SavingsAccount
                if (account is SavingsAccount savingsAccount)
                {
                    // Print the details of the SavingsAccount with specific column widths and alignment
                    PrintCenteredText(
                        $"| {accountNumber,-3} | {account.AccountNumber,-36} | {account.Balance.ToString("F2"),-12} | {account.Currency,-8} | Savings Account |", true, ConsoleColor.White);

                    // Print detailed information if requested
                    if (detailed)
                    {
                        PrintCenteredText($"  Interest Income: {savingsAccount.Balance * savingsAccount.InterestRate:C}, Interest Rate: {savingsAccount.InterestRate * 100}%", true, ConsoleColor.DarkYellow);
                    }

                    PrintCenteredText("---------------------------------------------------------------------------------------------", true, ConsoleColor.Gray);
                }
                else
                {
                    // Print the details of a non-SavingsAccount (private account)
                    PrintCenteredText(
                        $"| {accountNumber,-3} | {account.AccountNumber,-36} | {account.Balance.ToString("F2"),-12} | {account.Currency,-8} | Private Account |", true, ConsoleColor.White);

                    PrintCenteredText("---------------------------------------------------------------------------------------------", true, ConsoleColor.Gray);
                }

                // Increment accountNumber for the next account
                accountNumber++;
            }

            Console.WriteLine();
        }

        // Lets the user deposit money into an account (Money comes from like an ATM machine)
        static void Deposit(User user)
        {
            if (user.Accounts.Count == 0) // Checks such that the user has an account
            {
                DisplayError("You have no accounts to deposit to.");
                WaitForX();
                return;
            }
            // Prints all of the users accounts to make selection easier
            ViewAccounts(user, false);

            // Get valid account selection
            string accountIndexInput;
            while (true) // Loops through options untill the user selects a valid option
            {
                // ask user to choose an account to deposit to.
                accountIndexInput = ValidateNonEmptyString($"\nEnter the account number you want to deposit funds to (1-{user.Accounts.Count()}):");
                // Validate the user's input to ensure it's a valid account number within the allowed range
                if (int.TryParse(accountIndexInput, out int accountIndex) && accountIndex - 1 >= 0 && accountIndex - 1 < user.Accounts.Count)
                {
                    break;  // Valid input, break out of the loop
                }
                else
                {
                    DisplayError("Invalid account number. Please try again.");
                }
            }

            // Select account based on the user's validated input (adjusting for zero-based index)
            var account = user.Accounts[int.Parse(accountIndexInput) - 1];

            decimal amount;
            // ask user for the amount they want to deposit, and makes sure it's type is decimal
            while (true)
            {
                string depositAmount = ValidateNonEmptyString("Enter the amount you want to deposit:");
                if (decimal.TryParse(depositAmount, out amount) && amount > 0)
                {
                    break;  // Valid input, break out of the loop
                }
                else
                {
                    DisplayError("Invalid amount. Please enter a valid deposit amount greater than 0.");
                }
            }

            // Perform the deposit operation
            try
            {
                // deposits the selected amount
                account.Deposit(amount);
                // writes the success message
                PrintCenteredText($"Successfully deposited {amount} {account.Currency} to account {account.AccountNumber}.");
                PrintCenteredText($"The account's new balance is: {account.Balance} {account.Currency}.");
                WaitForX();
            }
            catch (Exception ex)
            {
                DisplayError($"An error occurred during the deposit: {ex.Message}");
            }
        }

        // Method to allow the user to withdraw their money from the bank (Money goes to like an ATM machine)
        static void Withdraw(User user)
        {
            // check if user has accounts
            if (user.Accounts.Count == 0)
            {
                DisplayError("You have no accounts to deposit to.");
                WaitForX();
                return;
            }
            // prints the users available accounts
            ViewAccounts(user, false);

            string accountIndexInput;
            while (true)
            {
                // ask user to choose an account to withdraw from
                accountIndexInput = ValidateNonEmptyString($"\nEnter the account number you want to withdraw funds from (1-{user.Accounts.Count()}):");
                // Validate the user's input to ensure it's a valid account number within the allowed range
                if (int.TryParse(accountIndexInput, out int accountIndex) && accountIndex - 1 >= 0 && accountIndex - 1 < user.Accounts.Count)
                {
                    break;
                }
                else
                {
                    DisplayError("Invalid account number. Please try again.");
                }
            }

            // Select account based on the user's validated input (adjusting for zero-based index)
            var account = user.Accounts[int.Parse(accountIndexInput) - 1];

            // Get valid withdrawal amount
            decimal amount;

            // Ask the user for the amount they want to withdraw, and make sure it the correct type
            while (true)
            {
                string amountInput = ValidateNonEmptyString("Enter the amount you want to withdraw:");
                if (decimal.TryParse(amountInput, out amount) && amount > 0 && amount <= account.Balance)
                {
                    break;
                }
                else
                {
                    if (amount <= 0)
                    {
                        DisplayError("Invalid amount. Please enter a valid withdrawal amount greater than 0.");
                    }
                    else
                    {
                        DisplayError($"Insufficient funds. You can only withdraw up to {account.Balance} {account.Currency}.");
                    }
                }
            }

            // Perform the withdrawal operation
            try
            {
                Console.WriteLine();
                // withdraws the selected amount from the account
                account.Withdraw(amount);
                // writes the success message
                PrintCenteredText($"Successfully withdrew {amount} {account.Currency} from account {account.AccountNumber}.");
                PrintCenteredText($"The account's new balance is: {account.Balance} {account.Currency}.");
                WaitForX();
            }
            catch (Exception ex)
            {
                DisplayError($"An error occurred during withdrawal: {ex.Message}");
            }
        }

        // A static method to transfer funds between accounts
        static void TransferFunds(User user)
        {
            // if the user has no active accounts this givesa an error
            if (user.Accounts.Count == 0)
            {
                DisplayError("You have no accounts to transfer from!");
                WaitForX();
                return;
            }
            while (true)
            {

                // Gives and option to transfer within your their accounts or to other users

                PrintCenteredText("Choose transfer option:\n");
                PrintCenteredText("1. Transfer to another account (within your own accounts)", false, ConsoleColor.White);
                string transferOption = ValidateNonEmptyString("2. Transfer to another user's account");

                // if the user input is not 1 or 2 then give error
                if (transferOption != "1" && transferOption != "2")
                {
                    DisplayError("Invalid option. Please try again.");
                    continue;
                }

                // When user chooses to transfer within their own accounts
                if (transferOption == "1")
                {
                    ClearAndPrintAsciiArt();
                    // call a method that prints user's accounts
                    ViewAccounts(user, false);

                    int fromAccountIndex;
                    while (true)
                    {
                        // ask user to choose an account to transfer from.
                        string fromAccountIndexInput = ValidateNonEmptyString($"\nEnter the account number you want to transfer funds from (1-{user.Accounts.Count()}):");
                        // Validate the user's input to ensure it's a valid account number within the allowed range
                        if (int.TryParse(fromAccountIndexInput, out fromAccountIndex) && fromAccountIndex - 1 >= 0 && fromAccountIndex - 1 < user.Accounts.Count)
                        {
                            break;
                        }
                        else
                        {
                            DisplayError("Invalid account number. Please try again.");
                        }
                    }

                    // Select the 'from' account based on the user's validated input (adjusting for zero-based index)
                    var fromAccount = user.Accounts[fromAccountIndex - 1];


                    int toAccountIndex;
                    while (true)
                    {
                        // ask user to choose an account to transfer to.
                        string toAccountIndexInput = ValidateNonEmptyString($"Enter the account number you want to transfer funds to (1-{user.Accounts.Count()}):");
                        // Validate the user's input to ensure it's a valid account number within the allowed range
                        if (int.TryParse(toAccountIndexInput, out toAccountIndex) && toAccountIndex - 1 >= 0 && toAccountIndex - 1 < user.Accounts.Count && toAccountIndex - 1 != fromAccountIndex - 1)
                        {
                            break;
                        }
                        else
                        {
                            DisplayError("Invalid account number. Please try again.");
                        }
                    }

                    // Select the 'to' account based on the user's validated input (adjusting for zero-based index)
                    var toAccount = user.Accounts[toAccountIndex - 1];

                    // if both account do not have the same currency
                    if (fromAccount.Currency != toAccount.Currency)
                    {
                        PrintCenteredText($"You are trying to transfer {fromAccount.Currency} to {toAccount.Currency}. The current conversion rate is: ");
                        PrintCenteredText(_bankSystem.DisplayExchangeRates());
                    }

                    decimal amount;
                    while (true)
                    {
                        string transferAmount = ValidateNonEmptyString("Enter the amount you want to transfer:");
                        // validates that the input is of decimal type, if not it gives an error
                        if (decimal.TryParse(transferAmount, out amount) && amount > 0)
                        {
                            break;
                        }
                        else
                        {
                            DisplayError("Invalid amount. Please enter a valid amount greater than 0.");
                        }
                    }

                    // call a method in banksystem that converts the currency
                    decimal convertedAmount = _bankSystem.ConvertCurrency(amount, fromAccount.Currency, toAccount.Currency);

                    // tries to create the Transaction
                    try
                    {
                        // add the Transaction to pending 
                        _bankSystem.AddTransactionToPending(user.TransferFunds(user, user, fromAccount, toAccount, amount, convertedAmount));
                        // give a success message
                        PrintCenteredText($"Successfully transferred {amount} {fromAccount.Currency} from account {fromAccount.AccountNumber} to account {toAccount.AccountNumber}. {convertedAmount:F2} {toAccount.Currency}.");
                        WaitForX();
                        return;
                    }
                    catch (InvalidOperationException ex)
                    {
                        // give error message
                        DisplayError(ex.Message);
                        continue;
                    }
                }
                else if (transferOption == "2")
                {
                    // Transfer to another user's account
                    User? recipient;
                    while (true)
                    {
                        // input for the username of another user
                        string recipientUsername = ValidateNonEmptyString("Enter the username of the recipient:");
                        // Find the recipient user by their username, ignoring case sensitivity
                        recipient = _bankSystem.Users.FirstOrDefault(u => u.Username.Equals(recipientUsername, StringComparison.OrdinalIgnoreCase));

                        // actions based on the users existens
                        if (recipient != null)
                        {
                            break;
                        }
                        else
                        {
                            DisplayError("Recipient not found. Please try again.");
                        }
                    }

                    // if the chosen user has no active accounts
                    if (recipient.Accounts.Count == 0)
                    {
                        DisplayError("The recipient does not have any accounts.");
                        WaitForX();
                        return;
                    }

                    ClearAndPrintAsciiArt();
                    // print the users available accounts
                    ViewAccounts(user, false);

                    int fromAccountIndex;
                    while (true)
                    {
                        // ask user to choose an account to transfer from.
                        string fromAccountIndexInput = ValidateNonEmptyString($"\nEnter the account number you want to transfer funds from (1-{user.Accounts.Count()}):");
                        // Validate the user's input to ensure it's a valid account number within the allowed range
                        if (int.TryParse(fromAccountIndexInput, out fromAccountIndex) && fromAccountIndex - 1 >= 0 && fromAccountIndex - 1 < user.Accounts.Count)
                        {
                            break;
                        }
                        else
                        {
                            DisplayError("Invalid account number. Please try again.");
                        }
                    }

                    // Select the 'from' account based on the user's validated input (adjusting for zero-based index)
                    var fromAccount = user.Accounts[fromAccountIndex - 1];

                    // writes down the others user accounts for selection
                    PrintCenteredText($"Available accounts for {recipient.Username}:");
                    for (int i = 0; i < recipient.Accounts.Count; i++)
                    {
                        var account = recipient.Accounts[i];
                        PrintCenteredText($"{i + 1}. Account number: {account.AccountNumber}, Currency: {account.Currency}");
                    }

                    int toAccountIndex;
                    while (true)
                    {
                        // ask user to choose an account to transfer to.
                        string toAccountIndexInput = ValidateNonEmptyString($"Enter the account number you want to transfer funds to (1-{recipient.Accounts.Count()}):");
                        // Validate the user's input to ensure it's a valid account number within the allowed range
                        if (int.TryParse(toAccountIndexInput, out toAccountIndex) && toAccountIndex - 1 >= 0 && toAccountIndex - 1 < recipient.Accounts.Count)
                        {
                            break;
                        }
                        else
                        {
                            DisplayError("Invalid account number. Please try again.");
                        }
                    }

                    // Select the 'to' account based on the user's validated input (adjusting for zero-based index)
                    var toAccount = recipient.Accounts[toAccountIndex - 1];

                    // if the selected account do not share the same currency
                    if (fromAccount.Currency != toAccount.Currency)
                    {
                        PrintCenteredText($"You are trying to transfer {fromAccount.Currency} to {toAccount.Currency}. The current conversion rate is: ");
                        _bankSystem.DisplayExchangeRates();
                    }

                    decimal amount;

                    // ask user what amount they want to transfer
                    while (true)
                    {
                        string transferAmount = ValidateNonEmptyString("Enter the amount you want to transfer:");
                        if (decimal.TryParse(transferAmount, out amount) && amount > 0)
                        {
                            break;
                        }
                        else
                        {
                            DisplayError("Invalid amount. Please enter a valid amount greater than 0.");
                        }
                    }

                    // calls the bankSystem method to convert the currency
                    decimal convertedAmount = _bankSystem.ConvertCurrency(amount, fromAccount.Currency, toAccount.Currency);
                    // tries do create the transaction
                    try
                    {
                        _bankSystem.AddTransactionToPending(user.TransferFunds(user, recipient, fromAccount, toAccount, amount, convertedAmount));
                        PrintCenteredText($"Successfully transferred {amount} {fromAccount.Currency} from account {fromAccount.AccountNumber} to account {toAccount.AccountNumber}. {convertedAmount:F2} {toAccount.Currency}.");
                        WaitForX();
                        return;
                    }
                    catch (InvalidOperationException ex)
                    {
                        // give error on fail
                        DisplayError(ex.Message);
                        continue;
                    }
                }
            }
        }

        // A static method to give the user an option to create new account
        static void OpenNewAccount(User user)
        {
            // a list with all possible currencies
            var availableCurrencies = new HashSet<string> { "USD", "EUR", "SEK" };
            string stringType = "";

            // Account type selection with validation
            PrintCenteredText("Choose type of account:");
            PrintCenteredText("1. Private Account");
            PrintCenteredText("2. Saving Account");

            string? accountype;
            while (true)
            {
                accountype = ValidateNonEmptyString("Account: ");

                // checks the user choice of account type
                if (accountype == "1" || accountype == "2")
                {
                    stringType = accountype == "1" ? "Private Account" : "Saving Account";
                    break;
                }

                // calls the error method
                DisplayError("Invalid account type. Please enter 1 or 2.");
            }

            // Currency selection with validation
            string userCurrency;
            while (true)
            {
                // Change text color to green
                Console.ForegroundColor = ConsoleColor.Green;
                PrintCenteredText("Available Currencies: USD, EUR, SEK");
                // reset text color
                Console.ResetColor();

                // ReadLine for users choice of currency, make the readline to upper case to prevent errors
                userCurrency = ValidateNonEmptyString("Choose Currency:").ToUpper();

                // checks if user currency exists in the list of allowed currencies
                if (availableCurrencies.Contains(userCurrency))
                {
                    PrintCenteredText($"{stringType} Created!");
                    WaitForX();
                    break;
                }

                // calls the error method
                DisplayError("Invalid currency. Please enter one of the following: USD, EUR, SEK.");
            }

            // Account creation based on type
            if (accountype == "1")
            {
                user.OpenNewAccount(userCurrency);
            }
            else
            {
                user.OpenNewSavingAccount(userCurrency, 0.05m);
            }
        }

        // A void method to print the users Transactions
        void CheckTransactions(User user)
        {
            if (user.Transactions.Count == 0)
            {
                DisplayError("No transactions to display.");
            }
            else
            {
                PrintCenteredText("Transactions:", false, ConsoleColor.White);
                PrintCenteredText("-------------------------------------------------------------------------------------------------------------------------", false, ConsoleColor.White);
                PrintCenteredText("|    Date and Time    | Amount | Currency |               Sender ID              |               Receiver ID            |", false, ConsoleColor.White);
                PrintCenteredText("-------------------------------------------------------------------------------------------------------------------------", false, ConsoleColor.White);

                foreach (var transaction in user.Transactions)
                {
                    // Construct the transaction row and store it in a variable
                    string transactionRow = $"| {transaction.Timestamp} | {transaction.Amount,-6} | {transaction.Currency,-6}   | {transaction.FromAccount.AccountNumber,-10} | {transaction.ToAccount.AccountNumber,-28} |";

                    if (transaction.ErrorMsg != "")
                    {
                        // Print the transaction row
                        PrintCenteredText(transactionRow, false, ConsoleColor.Red);

                        // Get the width of the transaction row for centering the error message
                        int rowWidth = transactionRow.Length;

                        // Construct the error message and calculate the padding
                        string errorMsgContent = $"Error: {transaction.ErrorMsg}";
                        int innerWidth = rowWidth - 4; // Subtract 4 for the "| " on each side
                        int padding = (innerWidth - errorMsgContent.Length) / 2;

                        // Pad the error message content with spaces to center it
                        string centeredErrorMsg = $"| {errorMsgContent.PadLeft(errorMsgContent.Length + padding).PadRight(innerWidth)} |";

                        // Print the centered error message with aligned "|"
                        PrintCenteredText(centeredErrorMsg, false, ConsoleColor.Red);
                    }
                    else
                    {
                        PrintCenteredText(transactionRow, false, ConsoleColor.White);
                    }
                }
                PrintCenteredText("-------------------------------------------------------------------------------------------------------------------------", false, ConsoleColor.White);
            }
            // Pauses execution to allow user to view transactions
            WaitForX();
        }

        // A method to create a new user
        static void CreateNewUser()
        {
            string username;

            // Loop until a valid username is entered that doesn't already exist
            while (true)
            {
                // Prompt user to enter a non-empty username
                username = ValidateNonEmptyString("Enter the username of the new user:");

                // Check if the username already exists in the system (ignores case)
                if (!_bankSystem.Users.Any(user => user.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    break;  // Valid username, exit the loop
                }

                // Display an error if the username is already taken
                DisplayError("Username already exists. Please choose a different username.");
            }

            string password;
            string password2;

            // Loop until the user successfully confirms the password
            while (true)
            {
                // Prompt user to enter a password and confirm it
                password = ReadPasswordWithValidation("Enter the password for the new user: ");
                string prompt = "Confirm Password: ";
                PrintCenteredText(prompt, false, ConsoleColor.White);
                Console.SetCursorPosition((int)((Console.WindowWidth - prompt.Length + Math.Round(prompt.Length * 0.1)) / 2), Console.CursorTop); // Centering input cursor
                password2 = ReadPassword();

                // Check if passwords match
                if (password != password2)
                {
                    // Display an error if passwords do not match
                    DisplayError("Passwords do not match. Please try again.\n");
                }
                else
                {
                    break;  // Passwords match, exit the loop
                }
            }

            // Variable to store the type of user (Regular or Admin)
            string typeOfUser;

            // Loop until a valid user type is entered
            while (true)
            {
                // Prompt user to enter user type (1 for Regular, 2 for Admin)
                typeOfUser = ValidateAccountTypeInput("Enter the type of user (1: Regular, 2: Admin):");

                // Check if the input is valid (1 or 2)
                if (typeOfUser == "1" || typeOfUser == "2")
                {
                    break;  // Valid input, exit the loop
                }

                // Display an error if input is invalid
                DisplayError("Invalid user type specified. Please enter 1 for Regular or 2 for Admin.");
            }

            // Create the new user based on the type
            if (typeOfUser == "1")
            {
                _bankSystem.Users.Add(new User(username, password));  // Add Regular user
            }
            else if (typeOfUser == "2")
            {
                _bankSystem.Users.Add(new Admin(username, password));  // Add Admin user
            }

            // Display success message after user creation
            PrintCenteredText("New user created successfully.");

            // Pause the screen briefly to allow user to see the success message
            WaitForX();
        }

        // Lets an admin change the exchange rate of a currency
        public void UpdateExchangeRate(BankSystem bankSystem)
        {
            while (true)
            {
                // Display all exchange rates
                foreach (var cur in bankSystem.ExchangeRates)
                {
                    PrintCenteredText($"{cur.Key}: {cur.Value}");
                }

                string currencyInput;

                // Loop until the user enters a valid currency
                while (true)
                {
                    // Prompt user for the currency they want to update
                    currencyInput = ValidateNonEmptyString("What currency would you like to change the rate of?").ToUpper().Trim();

                    // Check if the currencyInput exists in ExchangeRates
                    if (bankSystem.ExchangeRates.ContainsKey(currencyInput))
                    {
                        PrintCenteredText($"Current exchange rate for {currencyInput}: {bankSystem.ExchangeRates[currencyInput]}");
                        break; // Exit loop if valid currency is found
                    }
                    else
                    {
                        DisplayError($"Currency {currencyInput} not found in the exchange rates. Please try again.");
                    }
                }

                decimal newRate;

                // Loop until the user enters a valid exchange rate
                while (true)
                {
                    // Prompt for a new exchange rate
                    string rateInput = ValidateNonEmptyString("Enter the new exchange rate: ");
                    rateInput = rateInput.Replace(".", ",");
                    // Attempt to parse the input
                    if (decimal.TryParse(rateInput, out newRate) && newRate > 0)
                    {
                        // Update the exchange rate for the given currency
                        bankSystem.ExchangeRates[currencyInput] = newRate;

                        // Display the updated exchange rate (without forcing two decimals)
                        PrintCenteredText($"Exchange rate for {currencyInput} updated to {newRate}");
                        WaitForX();
                        return; // Exit the method after successful update
                    }
                    else
                    {
                        DisplayError("Invalid exchange rate. Please enter a positive decimal value.");
                    }
                }
            }
        }

        // A method to log the user out of the bank program
        static void Logout(BankProgram bankProgram)
        {
            PrintCenteredText("You have been logged out.");
            // Pause for 4 seconds to give the user time to read the message
            Thread.Sleep(4000);
            Console.Clear();
            // Call the MainMenu method to bring the user back to the main menu
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


        // A static method to wait for a key press and return the pressed key
        static ConsoleKey WaitForReadkey()
        {
            // Reads a key press from the console without displaying it
            // (intercept: true hides the key from appearing on the console)
            return Console.ReadKey(intercept: true).Key;
        }
        // method for reusable error
        static void DisplayError(string message)
        {
            PrintCenteredText(message, false, ConsoleColor.Red, ConsoleColor.White); // Display the error message
        }

        // a method to center the given text, also can change text color with input
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
                PrintCenteredText(prompt, false, ConsoleColor.White);
                Console.SetCursorPosition((int)((Console.WindowWidth - prompt.Length + Math.Round(prompt.Length * 0.1)) / 2), Console.CursorTop); // Centering input cursor
                string password = ReadPassword();

                // if IsValidPassword return as true then the method leaves the loop
                if (IsValidPassword(password))
                {
                    return password;
                }

                DisplayError("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit.");
            }
        }

        // Function that validates password
        static bool IsValidPassword(string password)
        {
            // if password is shorter than 8
            if (password.Length < 8)
                return false;

            // if password has any uppper case character
            bool hasUpperCase = password.Any(char.IsUpper);
            // if password has any lower case character
            bool hasLowerCase = password.Any(char.IsLower);
            // if password has any numeric character
            bool hasDigit = password.Any(char.IsDigit);

            return hasUpperCase && hasLowerCase && hasDigit;
        }

        // This function creates a ReadLine and makes sure it is not empty, and gives an appropriate error otherwise.
        static string ValidateNonEmptyString(string prompt, ConsoleColor color1 = ConsoleColor.White)
        {
            while (true)
            {
                PrintCenteredText(prompt, false, color1);
                // Centering input cursor
                Console.SetCursorPosition((int)((Console.WindowWidth - prompt.Length + Math.Round(prompt.Length * 0.1)) / 2), Console.CursorTop);
                string? input = Console.ReadLine();
                // if the sring is not empty it leaves the loop
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                DisplayError("Input cannot be empty. Please enter a valid value.");
            }
        }

        // A static method to validate the account type
        static string ValidateAccountTypeInput(string prompt)
        {
            while (true)
            {
                string? input = ValidateNonEmptyString(prompt);
                // makes sure it is either 1 or 2 otherwise you get error
                if (input == "1" || input == "2")
                {
                    return input;
                }
                DisplayError("Invalid account type. Please enter 1 or 2.");
            }
        }
    }
}
