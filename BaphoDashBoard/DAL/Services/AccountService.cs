using BaphoDashBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaphoDashBoard.DAL.Services
{
    public class AccountService : BaseService
    {
        public AccountService(MyLocalDatabase context) : base(context)
        {
        }
    }
}
