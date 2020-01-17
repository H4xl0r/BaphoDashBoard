using BaphoDashBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaphoDashBoard.DAL.Services
{
    public class ManagementService : BaseService
    {
        public ManagementService(MyLocalDatabase context) : base(context)
        {
        }

        public async Task<List<Administrators>> GetAdmin()
        {
            List<Administrators> result = new List<Administrators>();

            result = _context.Administrators.Select(x => new Administrators()
            {
                Id = x.Id,
                Username = x.Username,
                Password = x.Password,
                PasswordHash = x.PasswordHash
            }).ToList();

            return result;
        }
    }
}
