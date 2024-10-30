namespace AssetManagers_GruppProjekt
{
    internal class Admin : User
    {
        public Admin(string username, string password) : base(username, password)
        {
        }

        public User CreateUser(string username, string password)
        {
            return new User(username, password);
        }
    }
}
