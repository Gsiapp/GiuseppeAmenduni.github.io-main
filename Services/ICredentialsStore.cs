using System.Threading.Tasks;
using GestioneOrdini.Models;

namespace GestioneOrdini.Services
{
    public interface ICredentialsStore
    {
        Task<Cliente?> GetClienteByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task AddClienteAsync(Cliente cliente);
        bool VerifyAdminCredentials(string username, string password);
        Task<bool> VerifyClienteCredentialsAsync(string email, string password);
    }
} 