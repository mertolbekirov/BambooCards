using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BambooCards.Data;
using BambooCards.Services.Accounts.Models;
using Newtonsoft.Json;

namespace BambooCards.Services.Accounts
{
    public class AccountsService : IAccountService
    {
        public async Task<List<AccountServiceModel>> GetAccounts()
        {
            HttpClient client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes($"{DataConstants.AuthClientId}:{DataConstants.AuthSecret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            var response = await client.GetAsync(DataConstants.BambooApiMainEndpoint + DataConstants.GetAccountsEndpoint);
            string json = null;
            if (response.IsSuccessStatusCode)
            {
                json = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }

            var accountsArray = JsonConvert.DeserializeObject<AccountsJsonModel>(json);
            if (accountsArray == null)
            {
                return new List<AccountServiceModel>();
            }
            var accounts = accountsArray.Accounts;

            return accounts;
        }
    }
}
