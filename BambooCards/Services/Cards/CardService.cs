using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BambooCards.Data;
using BambooCards.Services.Accounts;
using BambooCards.Services.Cards.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static BambooCards.Data.DataConstants;

namespace BambooCards.Services.Cards
{
    public class CardService : ICardService
    {
        private readonly IAccountService accounts;
        private readonly IMemoryCache cache;

        public CardService(IMemoryCache cache, IAccountService accounts)
        {
            this.cache = cache;
            this.accounts = accounts;
        }

        public async Task<List<BrandServiceModel>> GetCatalog()
        {
            return await this.LoadCards();
        }

        public async Task<CardDetailsServiceModel> GetProductDetails(int id)
        {
            var brandWithItem = await GetBrandWithItemById(id);

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


        public async Task<bool> BuyCard(int id, int quantity)
        {
            if (quantity <= 0)
            {
                return false;
            }
            var brandWithProduct = await GetBrandWithItemById(id);
            if (brandWithProduct == null)
            {
                return false;
            }

            var product = brandWithProduct.Products.FirstOrDefault(x => x.Id == id);
            var availableAccounts = await this.accounts.GetAccounts();
            var accountToUse =
                availableAccounts.FirstOrDefault(x => x.IsActive && x.Currency == product.Price.CurrencyCode);
            if (accountToUse == null)
            {
                //search for account with enough balance
                foreach (var account in availableAccounts)
                {
                    var exchangeRate = await GetExchangeRates(product.Price.CurrencyCode, account.Currency);
                    if (product.Price.Min * exchangeRate.Rates.First().Value <= account.Balance)
                    {
                        accountToUse = account;
                        break;
                    }
                }
                //if no account is found return false
                if (accountToUse == null)
                {
                    return false;
                }
            }

            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(DataConstants.AuthClientId + ":" + DataConstants.AuthSecret));
            //var requestObject = new PlaceProductOrderJsonModel()
            //{
            //    AccountId = accountToUse.Id,
            //};
            //requestObject.Products.Add(new ProductBuyJsonModel()
            //{
            //    ProductId = product.Id,
            //    Quantity = quantity,
            //    Value = product.MaxFaceValue

            //});
            //var jsonObj = JsonConvert.SerializeObject(requestObject);
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create(DataConstants.BambooApiMainEndpoint + DataConstants.PlaceAnOrderEndpoint);
            //
            //httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
            //httpWebRequest.PreAuthenticate = true;
            //httpWebRequest.ContentType = "application/json";
            //httpWebRequest.Method = "POST";

            //await using var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            //await streamWriter.WriteAsync(jsonObj);
            var requestObject = new PlaceProductOrderJsonModel()
            {
                AccountId = accountToUse.Id,
            };
            requestObject.Products.Add(new ProductBuyJsonModel()
            {
                ProductId = product.Id,
                Quantity = quantity,
                Value = product.MaxFaceValue
            });

            var jsonObj = JsonConvert.SerializeObject(requestObject);
            var url = DataConstants.BambooApiMainEndpoint + DataConstants.PlaceAnOrderEndpoint;

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.Accept = "application/json";
            httpRequest.Headers["Authorization"] = "Basic " + encoded;
            httpRequest.ContentType = "application/json";


            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonObj);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }

            return true;
        }

        private async Task<CurrencyRateJsonModel> GetExchangeRates(string baseCurrency, string wantedCurrency)
        {

            using HttpClient client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes($"{DataConstants.AuthClientId}:{DataConstants.AuthSecret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var response = await client.GetAsync(DataConstants.BambooApiMainEndpoint + DataConstants.GetExchangeRatesEndpoint + $"?baseCurrency={baseCurrency}&currency={wantedCurrency}");
            string json = null;
            if (response.IsSuccessStatusCode)
            {
                json = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }
            return JsonConvert.DeserializeObject<CurrencyRateJsonModel>(json);
        }

        private async Task<BrandServiceModel?> GetBrandWithItemById(int id)
        {
            var brandWithItem = this.cache
                .Get<List<BrandServiceModel>>(LatestCardsCacheKey)
                .FirstOrDefault(x => x.Products.FirstOrDefault(x => x.Id == id) != null);

            if (brandWithItem == null)
            {
                await LoadCards();
                brandWithItem = this.cache
                    .Get<List<BrandServiceModel>>(LatestCardsCacheKey)
                    .FirstOrDefault(x => x.Products.FirstOrDefault(x => x.Id == id) != null);
            }

            return brandWithItem;
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
