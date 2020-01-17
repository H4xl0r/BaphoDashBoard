using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaphoDashBoard.DAL.Services;
using Microsoft.AspNetCore.Mvc;

namespace BaphoDashBoard.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _service;
        public AccountController(AccountService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}