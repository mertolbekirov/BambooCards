using System.Threading.Tasks;
using BambooCards.Services.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace BambooCards.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountService accounts;

        public AccountsController(IAccountService accounts)
        {
            this.accounts = accounts;
        }

        public async Task<IActionResult> GetAccounts()
        {
            var accountsToReturn = await this.accounts.GetAccounts();
            if (accountsToReturn == null)
            {
                return BadRequest();
            }

            return View(accountsToReturn);
        }
    }
}
