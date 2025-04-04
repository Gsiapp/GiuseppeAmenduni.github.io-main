using GestioneOrdini.Data;
using GestioneOrdini.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GestioneOrdini.Services
{
    public class CarrelloService : ICarrelloService
    {
        private readonly AppDbContext _context;

        public CarrelloService(AppDbContext context)
        {
            _context = context;
        }

        public List<CarrelloItem> GetCarrelloUtente(string userId)
        {
            return _context.CarrelloItems
                .Include(c => c.Prodotto)
                .Where(c => c.UserId == userId)
                .ToList();
        }

        public async Task AggiungiProdottoAsync(string userId, int prodottoId, int quantita)
        {
            var prodotto = await _context.Prodotti.FindAsync(prodottoId);
            if (prodotto == null) return;

            var item = await _context.CarrelloItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProdottoId == prodottoId);

            if (item == null)
            {
                item = new CarrelloItem
                {
                    UserId = userId,
                    ProdottoId = prodottoId,
                    Quantita = quantita,
                    Prezzo = prodotto.Prezzo,
                    Prodotto = prodotto
                };
                await _context.CarrelloItems.AddAsync(item);
            }
            else
            {
                item.Quantita += quantita;
            }

            await _context.SaveChangesAsync();
        }
        public async Task<Carrello> GetCarrelloUtenteAsync(string userId)
        {
            var items = await _context.CarrelloItems
                .Include(c => c.Prodotto)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return new Carrello 
            { 
                UserId = userId, 
                Items = items 
            };
        }
        public async Task<bool> AggiornaQuantitaAsync(string userId, int prodottoId, int nuovaQuantita)
        {
            var item = await _context.CarrelloItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProdottoId == prodottoId);

            if (item == null) return false;

            item.Quantita = Math.Max(1, nuovaQuantita);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task RimuoviProdottoAsync(string userId, int prodottoId)
        {
            var item = await _context.CarrelloItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProdottoId == prodottoId);

            if (item != null)
            {
                _context.CarrelloItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetConteggioCarrelloAsync(string userId)
        {
            return await _context.CarrelloItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantita);
        }

        public async Task SvuotaCarrelloAsync(string userId)
        {
            var items = await _context.CarrelloItems
                .Where(c => c.UserId == userId)
                .ToListAsync();
            
            _context.CarrelloItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}