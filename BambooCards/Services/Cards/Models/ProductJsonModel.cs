using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BambooCards.Services.Cards.Models
{
    public class ProductJsonModel
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public decimal MinFaceValue { get; init; }
        public decimal MaxFaceValue { get; init; }

        public int? Count { get; init; }

        public PriceJsonModel Price { get; init; }

        public DateTime ModifiedDate { get; init; }
    }

    public class PriceJsonModel
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }

        public string CurrencyCode { get; init; }
    }
}
