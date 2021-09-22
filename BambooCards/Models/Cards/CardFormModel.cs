using System.ComponentModel.DataAnnotations;

namespace BambooCards.Models.Cards
{
    public class CardFormModel
    {
        public string LogoUrl { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal MinFaceValue { get; set; }
        public decimal MaxFaceValue { get; set; }

        public decimal Price { get; set; }

        [Required]
        public string CurrencyCode { get; set; }

        public int? QuantityAvailable { get; set; }

        [Range(1, int.MaxValue)]
        public int QuantityToBuy { get; set; }
    }
}
