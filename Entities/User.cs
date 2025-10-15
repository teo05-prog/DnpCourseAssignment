namespace Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    
    public User()
    {
        UserName = string.Empty;
        Password = string.Empty;
    }
    
    public User(string userName, string password)
    {
        UserName = userName;
        Password = password;
    }
}