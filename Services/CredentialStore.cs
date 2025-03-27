using System.Collections.Generic;

public static class CredentialsStore
{
    // Sostituisci gli hash con quelli generati dinamicamente
    public static Dictionary<string, string> Admins = new Dictionary<string, string>
    {
        { "Gsiapp", BCrypt.Net.BCrypt.HashPassword("2080") }, // Genera nuovo hash
        { "Amministratore1", BCrypt.Net.BCrypt.HashPassword("password2") }
    };

    public static Dictionary<string, string> Clienti = new Dictionary<string, string>
    {
        { "gamenduni08@gmail.com", BCrypt.Net.BCrypt.HashPassword("password1") }, // Genera nuovo hash
        { "cliente2@example.com", BCrypt.Net.BCrypt.HashPassword("password3") }
    };
}
