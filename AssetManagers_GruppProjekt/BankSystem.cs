using AssetManagers_GruppProjekt;
using System.Diagnostics;

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

    public bool UpdateExchangeRate()
    {

        foreach (var cur in ExchangeRates)
        {
            Console.WriteLine($"{cur.Key}: {cur.Value}");
        }
        // Prompt user for the currency they want to update
        Console.WriteLine("What currency would you like to change the rate of?");
        string currencyInput = Console.ReadLine()?.ToUpper().Trim();

        // Check if the currencyInput exists in ExchangeRates
        if (ExchangeRates.ContainsKey(currencyInput))
        {
            Console.WriteLine($"Current exchange rate for {currencyInput}: {ExchangeRates[currencyInput]}");

            // Prompt for a new exchange rate
            Console.Write("Enter the new exchange rate: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal newRate) && newRate > 0)
            {
                // Update the exchange rate for the given currency
                ExchangeRates[currencyInput] = newRate;
                Console.WriteLine($"Exchange rate for {currencyInput} updated to {newRate}.");
                Console.WriteLine("Press X to go back to the main menu");
                while (true)
                {
                    // Capture the key press
                    ConsoleKey keyInfo = Console.ReadKey(intercept: true).Key;

                    // Check if the key pressed was 'X' or 'x'
                    if (keyInfo == ConsoleKey.X)
                    {
                        Console.Clear();
                        Console.ResetColor(); // Reset color to default before returning
                        return true;
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid exchange rate. Please enter a valid decimal value.");
                return false;
            }
        }
        else
        {
            // Currency not found in ExchangeRates
            Console.WriteLine($"Currency {currencyInput} not found in the exchange rates.");
            return false;
        }
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
                _pendingTransactions.Remove(transaction);
            }
            catch (Exception ex)
            {
                // Log the error and continue with the next transaction
                Debug.WriteLine($"Failed to execute transaction: {transaction}. Error: {ex.Message}");
            }
        }

        Debug.WriteLine("All Transactions have been completed!\n");
    }

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