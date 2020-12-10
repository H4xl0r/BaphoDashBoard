using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BaphoDashBoard.Models;
using System.Text;
using System.Security.Cryptography;
using BaphoDashBoard.DAL.Services;

namespace BaphoDashBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly BaseService _service;
        private readonly ILogger<HomeController> _logger;
        public HomeController(BaseService service, ILogger<HomeController> logger)
        {
            _service = service;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetHostData()
        {
            var result = _service.GetHostData();
            return Json(result.Result);
        }

        public async Task<IActionResult> GetVictimData()
        {
            var result = _service.GetVictimData();

            return Json(result);
        }

        //public async Task<IActionResult> InitUser()
        //{
        //    var result = _service.StartUser();
        //    ViewBag.Message = result.message;
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
