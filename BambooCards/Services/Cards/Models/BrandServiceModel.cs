using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BambooCards.Services.Cards.Models
{
    public class BrandServiceModel
    {
        public string Description { get; init; }

        public string LogoUrl { get; init; }

        public string RedemptionInstructions { get; init; }

        public List<ProductServiceModel> Products { get; set; } = new List<ProductServiceModel>();
    }
}
