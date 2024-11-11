using AssetManagers_GruppProjekt;
using System.Diagnostics;

public class BankSystem
{
    public List<User> Users { get; set; }
    public List<Account> Accounts { get; set; }
    private List<Transaction> _pendingTransactions { get; set; }
    public Dictionary<string, decimal> ExchangeRates { get; set; }
    private int FailedLoginAttempts { get; set; }

    // Initializes the banksystem with some default values and empty lists
    public BankSystem()
    {
        Users = new List<User>();
        Accounts = new List<Account>();
        _pendingTransactions = new List<Transaction>();
        ExchangeRates = new Dictionary<string, decimal>
            {
                { "USD", 1.0m },  // Example exchange rates
                { "EUR", 0.85m },
                { "SEK", 9.0m }
            };
    }

    // Displays all of the exchange rates in the bank system and returns as a string that gets centered in the BankProgram
    public string DisplayExchangeRates()
    {
        string[] exchangeRates = new string[]
        {
            "Exchange Rates:",
            "--------------------------------"
        };
        foreach (var rate in ExchangeRates)
        {
            exchangeRates = exchangeRates.Concat(new[] { $"| {rate.Key,-5} | {rate.Value,8:F2} |" }).ToArray();
        }
        exchangeRates = exchangeRates.Concat(new[] { "--------------------------------" }).ToArray();
        return string.Join(Environment.NewLine, exchangeRates);
    }

    // Checks if the user is locked out
    public bool IsLockedOut()
    {
        return FailedLoginAttempts >= 3;
    }

    // Logs in the user and returns the user if the login is successful, otherwise returns null
    public User? Login(string username, string password)
    {
        if (IsLockedOut())
        {
            throw new InvalidOperationException("Account is locked out due to too many failed attempts.");
        }

        User? foundUser = Users.Find(u => u.Username == username && u.Password == password);

        if (foundUser != null)
        {
            Console.Clear();
            string userType = foundUser is Admin ? "Admin" : "Regular User";
            Console.WriteLine($"{userType} - Welcome, {foundUser.Username}!");
            FailedLoginAttempts = 0; // Reset on successful login
            return foundUser;
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Invalid username or password.");
            FailedLoginAttempts++;

            if (IsLockedOut())
            {
                throw new InvalidOperationException("Account is locked out due to too many failed attempts.");
            }

            return null; // Return null instead of false
        }
    }

    // Adds a transaction to the pending list of transactions
    public void AddTransactionToPending(Transaction transaction)
    {
        _pendingTransactions.Add(transaction);
    }

    // Executes all pending transactions in the list and logs the results to the debug window
    public void ExecutePendingTransactions()
    {
        // Ensure the _pendingTransactions list is initialized
        if (_pendingTransactions == null)
        {
            _pendingTransactions = new List<Transaction>();
        }

        // Log the start of the transaction execution process to the debug window
        Debug.WriteLine("Executing pending transactions...");

        // Iterate through pending transactions and process each one
        foreach (var transaction in _pendingTransactions.ToList()) // Use ToList() to avoid modifying the collection while iterating
        {
            try
            {
                Debug.WriteLine("\n-------------------------------");
                transaction.FromAccount.Withdraw(transaction.Amount);
                if (transaction.ConvertedAmount != transaction.Amount)
                {
                    transaction.ToAccount.Deposit(transaction.ConvertedAmount);
                }
                else
                {
                    transaction.ToAccount.Deposit(transaction.Amount);
                }
                Debug.WriteLine($"Transaction executed: {transaction}");
                Debug.WriteLine("-------------------------------\n");

                // Remove the transaction from the pending list after successful execution
                transaction.FromUser.Transactions.Add(transaction);
                _pendingTransactions.Remove(transaction);
            }
            catch (Exception ex)
            {
                // Log the error and continue with the next transaction
                Debug.WriteLine($"Failed to execute transaction: {transaction}. Error: {ex.Message}");
                Transaction failedTransaction = new Transaction(transaction.FromUser, transaction.ToUser, transaction.FromAccount, transaction.ToAccount, transaction.Amount, transaction.ConvertedAmount, transaction.Currency, ex.Message);
                transaction.FromUser.Transactions.Add(failedTransaction);
                _pendingTransactions.Remove(transaction);
            }
        }

        Debug.WriteLine("All Transactions have been completed!\n");
    }

    // Converts an amount from one currency to another using the exchange rates
    public decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency)
            return amount;

        // Check if exchange rates for both currencies exist
        if (ExchangeRates.TryGetValue(fromCurrency, out decimal fromRate) &&
            ExchangeRates.TryGetValue(toCurrency, out decimal toRate))
        {
            // Convert from the original currency to USD
            decimal amountInUSD = amount / fromRate;  // Correct conversion to USD

            // Then convert from USD to the target currency
            decimal convertedAmount = amountInUSD * toRate;

            // Return the converted amount
            return convertedAmount;
        }
        else
        {
            throw new Exception("Exchange rate not available for the specified currencies.");
        }
    }
}