using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BambooCards.Data;
using BambooCards.Services.Cards.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static BambooCards.Data.DataConstants;

namespace BambooCards.Services.Cards
{
    public class CardService : ICardService
    {
        private readonly IMemoryCache cache;

        public CardService(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public async Task<List<BrandServiceModel>> GetCatalog()
        {
            return await this.LoadCards();
        }

        public async Task<CardDetailsServiceModel> GetProductDetails(int id)
        {
            var brandWithItem = this.cache
                .Get<List<BrandServiceModel>>(LatestCardsCacheKey).FirstOrDefault(x => x.Products.FirstOrDefault(x => x.Id == id) != null);

            if (brandWithItem == null)
            {
                await LoadCards();
                brandWithItem = this.cache
                    .Get<List<BrandServiceModel>>(LatestCardsCacheKey).FirstOrDefault(x => x.Products.FirstOrDefault(x => x.Id == id) != null);
            }

            if (brandWithItem == null)
            {
                return null;
            }
                
            var product = brandWithItem.Products.FirstOrDefault(x => x.Id == id);

            return new CardDetailsServiceModel()
            {
                LogoUrl = brandWithItem.LogoUrl,
                Id = product.Id,
                Name = product.Name,
                MinFaceValue = product.MinFaceValue,
                MaxFaceValue = product.MaxFaceValue,
                Count = product.Count,
                Price = product.Price
            };
        }
        private async Task<List<BrandServiceModel>> LoadCards()
        {
            var lastLoadedCards = this.cache.Get<List<BrandServiceModel>>(LatestCardsCacheKey);

            if (lastLoadedCards == null)
            {
                HttpClient client = new HttpClient();
                var byteArray = Encoding.ASCII.GetBytes($"{DataConstants.AuthClientId}:{DataConstants.AuthSecret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                var response = await client.GetAsync(DataConstants.BambooApiMainEndpoint + DataConstants.GetCatalogEndpoint);
                string json = null;
                if (response.IsSuccessStatusCode)
                {
                    json = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return null;
                }

                var brandsArray = JsonConvert.DeserializeObject<BrandsArrayJsonModel>(json);
                var brands = brandsArray.Brands;

                lastLoadedCards =  brands.Select(x => new BrandServiceModel
                {
                    Description = x.Description,
                    LogoUrl = x.LogoUrl,
                    RedemptionInstructions = x.RedemptionInstructions,
                    Products = x.Products.Select(p => new ProductServiceModel
                    {
                        Price = new PriceServiceModel
                        {
                            Min = p.Price.Min,
                            Max = p.Price.Max,
                            CurrencyCode = p.Price.CurrencyCode
                        },
                        Count = p.Count,
                        Id = p.Id,
                        MaxFaceValue = p.MaxFaceValue,
                        MinFaceValue = p.MinFaceValue,
                        Name = p.Name
                    }).ToList()
                }).ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                this.cache.Set(LatestCardsCacheKey, lastLoadedCards, cacheOptions);
            }

            return lastLoadedCards;
        }
    }
}
