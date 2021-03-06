using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BambooCards.Services.Cards.Models
{
    public class ProductServiceModel
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public decimal MinFaceValue { get; init; }

        public decimal MaxFaceValue { get; init; }

        public int? Count { get; init; }

        public PriceServiceModel Price { get; init; }
    }
}
