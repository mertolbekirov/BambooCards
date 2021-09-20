using System.Collections.Generic;
using System.Threading.Tasks;
using BambooCards.Services.Accounts.Models;
namespace BambooCards.Services.Accounts
{
    public interface IAccountService
    {
        public Task<List<AccountServiceModel>> GetAccounts();
    }
}
