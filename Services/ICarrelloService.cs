using GestioneOrdini.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestioneOrdini.Services
{
    public interface ICarrelloService
    {
        // Metodi sincroni e asincroni
        List<CarrelloItem> GetCarrelloUtente(string userId);
        Task<Carrello> GetCarrelloUtenteAsync(string userId);
        
      
        Task AggiungiProdottoAsync(string userId, int prodottoId, int quantita);
        Task RimuoviProdottoAsync(string userId, int prodottoId);
        Task SvuotaCarrelloAsync(string userId);
    
        
        Task<int> GetConteggioCarrelloAsync(string userId);
        Task<List<CarrelloItem>> GetCarrelloItemsAsync(string userId);
    }
}