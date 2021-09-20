using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BambooCards.Services.Accounts.Models
{
    public class AccountServiceModel
    {
        public int Id { get; init; }

        public string Currency { get; init; }

        public decimal Balance { get; init; }

        public bool IsActive { get; init; }
    }
}
