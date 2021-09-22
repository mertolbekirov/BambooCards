using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BambooCards.Services.Cards.Models
{
    public class PlaceProductOrderJsonModel
    {
        public string RequestId { get; set; } = Guid.NewGuid().ToString();

        public int AccountId { get; set; }

        public List<ProductBuyJsonModel> Products { get; set; } = new List<ProductBuyJsonModel>();
    }

        public class ProductBuyJsonModel
        {
            public int ProductId { get; set; }

            public int Quantity { get; set; }

            public decimal Value { get; set; }
        }
    
}
