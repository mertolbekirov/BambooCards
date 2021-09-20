using BambooCards.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using BambooCards.Models.Cards;
using BambooCards.Services.Cards;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace BambooCards.DControllers
{
    public class HomeController : Controller
    {
        private readonly ICardService cards;

        public HomeController(ILogger<HomeController> logger, ICardService cards, IMemoryCache cache)
        {
            this.cards = cards;
        }

        public async Task<IActionResult> Index()
        {
            var lastLoadedCards = await this.cards.GetCatalog();

            return View(lastLoadedCards);
        }

       
        public async Task<IActionResult> CardBuyDetails(int id)
        {
            var product = await this.cards.GetProductDetails(id);

            if (product == null)
            {
                return BadRequest();
            }

            return View(new CardFormModel()
            {
                CurrencyCode = product.Price.CurrencyCode,
                LogoUrl = product.LogoUrl,
                MaxFaceValue = product.MaxFaceValue,
                MinFaceValue = product.MinFaceValue,
                Name = product.Name,
                Price = product.Price.Min,
                QuantityAvailable = product.Count,
            });
        }

        [HttpPost]
        public IActionResult CardBuyDetails(int id, CardFormModel card)
        {
            if (!ModelState.IsValid)
            {
                return View(card);
            }

            return null;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
    }
}
