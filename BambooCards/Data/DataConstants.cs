using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BambooCards.Data
{
    public class DataConstants
    {
        public const string BambooApiMainEndpoint = "https://api-stage.bamboocardportal.com/";
        public const string GetCatalogEndpoint = "api/integration/v1.0/catalog";
        public const string GetAccountsEndpoint = "api/integration/v1.0/accounts";
        public const string GetExchangeRatesEndpoint = "/api/integration/v1.0/exchange-rates?baseCurrency={0}&currency={1}";
        public const string PlaceAnOrderEndpoint = "/api/integration/v1.0/orders/checkout";
        public const string GetOrderEnpoint = "GET/api/integration/v1.0/orders/{0}";

        public const string AuthClientId = "TEST";
        public const string AuthSecret = "cd2544b5-2e75-4ec3-acba-8553ddbf40cc";


        public const string LatestCardsCacheKey = nameof(LatestCardsCacheKey);

    }
}
