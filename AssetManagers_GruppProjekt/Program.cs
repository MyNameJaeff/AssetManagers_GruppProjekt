namespace AssetManagers_GruppProjekt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BankProgram bankProgram = new();
            bankProgram.InitializeValues();
            bankProgram.MainMenu();
        }
    }
}