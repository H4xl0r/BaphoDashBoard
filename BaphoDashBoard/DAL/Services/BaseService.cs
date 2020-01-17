using BaphoDashBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaphoDashBoard.DAL.Services
{
    public class BaseService
    {
        protected readonly MyLocalDatabase _context;
        public BaseService(MyLocalDatabase context)
        {
            _context = context;
        }
    }
}
