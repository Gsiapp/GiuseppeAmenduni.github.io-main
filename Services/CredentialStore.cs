using System.Collections.Generic;

public static class CredentialsStore
{
    
    public static Dictionary<string, string> Admins = new Dictionary<string, string>
    {
        { "Gsiapp", BCrypt.Net.BCrypt.HashPassword("2080") }, 
        { "Amministratore1", BCrypt.Net.BCrypt.HashPassword("password2") }
    };

    public static Dictionary<string, string> Clienti = new Dictionary<string, string>
    {
        { "gamenduni08@gmail.com", BCrypt.Net.BCrypt.HashPassword("password1") }, 
        { "cliente2@example.com", BCrypt.Net.BCrypt.HashPassword("password3") }
    };
}
