using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BambooCards.Services.Cards.Models
{
    public class CurrencyRateJsonModel
    {
        public string BaseCurrencyCode { get; init; }

        public List<RateJsonModel> Rates { get; set; }

        public class RateJsonModel
        {
            public decimal Value { get; init; }

            public string CurrencyCode { get; init; }
        }
    }
}
