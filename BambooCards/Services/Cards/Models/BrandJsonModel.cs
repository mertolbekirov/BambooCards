using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BambooCards.Services.Cards.Models
{
    public class BrandJsonModel
    {
        public string Name { get; init; }

        public string CountryCode { get; init; }

        public string CurrencyCode { get; init; }

        public string Description { get; init; }

        public string Disclaimer { get; init; }

        public string RedemptionInstructions { get; init; }

        public string Terms { get; init; }

        public string LogoUrl { get; init; }

        public DateTime ModifiedDate { get; init; }

        public List<ProductJsonModel> Products { get; set; } = new List<ProductJsonModel>();

    }
}
