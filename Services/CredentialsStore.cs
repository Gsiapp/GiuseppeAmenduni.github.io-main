using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GestioneOrdini.Data;
using GestioneOrdini.Models;

namespace GestioneOrdini.Services
{
    public class CredentialsStore : ICredentialsStore
    {
        private readonly AppDbContext _context;
        private static readonly Dictionary<string, string> Admins = new()
        {
            { "admin@example.com", BCrypt.Net.BCrypt.HashPassword("admin123") }
        };

        public CredentialsStore(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Cliente?> GetClienteByEmailAsync(string email)
        {
            return await _context.Clienti.FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Clienti.AnyAsync(c => c.Email.ToLower() == email.ToLower());
        }

        public async Task AddClienteAsync(Cliente cliente)
        {
            await _context.Clienti.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public bool VerifyAdminCredentials(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            var adminKey = Admins.Keys.FirstOrDefault(k => k.ToLower() == username.ToLower());
            if (adminKey == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, Admins[adminKey]);
        }

        public async Task<bool> VerifyClienteCredentialsAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return false;

            var cliente = await _context.Clienti
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());

            if (cliente == null || string.IsNullOrEmpty(cliente.PasswordHash))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, cliente.PasswordHash);
        }
    }
} 