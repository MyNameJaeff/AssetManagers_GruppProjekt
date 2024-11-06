using AssetManagers_GruppProjekt;

public class BankSystem
{
    // Properties
    public List<User> Users { get; set; }
    public List<Account> Accounts { get; set; }
    private List<Transaction> _pendingTransactions { get; set; }
    public Dictionary<string, decimal> ExchangeRates { get; set; }
    private int FailedLoginAttempts { get; set; }

    // Constructor
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

    public bool IsLockedOut()
    {
        return FailedLoginAttempts >= 3;
    }

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

    public void AddTransactionToPending(Transaction transaction)
    {
        _pendingTransactions.Add(transaction);
    }

    public void ExecutePendingTransactions()
    {
        // Ensure the _pendingTransactions list is initialized
        if (_pendingTransactions == null)
        {
            _pendingTransactions = new List<Transaction>();
        }

        // Log the start of the transaction execution process
        Console.WriteLine("Executing pending transactions...");

        // Iterate through pending transactions and process each one
        foreach (var transaction in _pendingTransactions.ToList()) // Use ToList() to avoid modifying the collection while iterating
        {
            try
            {
                transaction.FromAccount.Withdraw(transaction.Amount);
                transaction.ToAccount.Deposit(transaction.Amount);
                Console.WriteLine($"Transaction executed: {transaction}");

                // Remove the transaction from the pending list after successful execution
                _pendingTransactions.Remove(transaction);
            }
            catch (Exception ex)
            {
                // Log the error and continue with the next transaction
                Console.WriteLine($"Failed to execute transaction: {transaction}. Error: {ex.Message}");
            }
        }
    }

    public decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency)
            return amount;

        // Check if exchange rates for both currencies exist
        if (ExchangeRates.TryGetValue(fromCurrency, out decimal fromRate) &&
            ExchangeRates.TryGetValue(toCurrency, out decimal toRate))
        {
            decimal convertedAmount = (amount / fromRate) * toRate;
            return convertedAmount;
        }
        else
        {
            throw new Exception("Exchange rate not available for the specified currencies.");
        }
    }
}