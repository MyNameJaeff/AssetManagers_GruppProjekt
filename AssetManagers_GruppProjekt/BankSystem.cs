using AssetManagers_GruppProjekt;

public class BankSystem
{
    // Properties
    public List<User> Users { get; set; }
    public List<Account> Accounts { get; set; }
    public Dictionary<string, decimal> ExchangeRates { get; set; }
    private int FailedLoginAttempts { get; set; }

    // Constructor
    public BankSystem()
    {
        Users = new List<User>();
        Accounts = new List<Account>();
        ExchangeRates = new Dictionary<string, decimal>
            {
                { "USD", 1.0m },  // Example exchange rates
                { "EUR", 0.85m },
                { "SEK", 9.0m }
            };
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


    public void ExecutePendingTransactions()
    {
        // Code to execute pending transactions
        Console.WriteLine("Executing pending transactions...");
        // Example: iterate through pending transactions and process each one
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