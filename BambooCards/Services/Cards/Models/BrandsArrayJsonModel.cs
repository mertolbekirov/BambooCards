using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BambooCards.Services.Cards.Models
{
    public class BrandsArrayJsonModel
    {
        public List<BrandJsonModel> Brands { get; set; } = new List<BrandJsonModel>();
    }
}
