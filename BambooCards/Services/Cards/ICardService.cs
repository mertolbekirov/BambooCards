using BambooCards.Services.Cards.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BambooCards.Services.Cards
{
    public interface ICardService
    {
        public Task<List<BrandServiceModel>> GetCatalog();

        public Task<CardDetailsServiceModel> GetProductDetails(int id);
        Task<bool> BuyCard(int id, int quantity);

    }
}
