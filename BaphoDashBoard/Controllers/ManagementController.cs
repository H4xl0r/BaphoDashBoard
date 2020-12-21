using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaphoDashBoard.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using BaphoDashBoard.DAL.Services;
using BaphoDashBoard.VueModels;

namespace BaphoDashBoard.Controllers
{
    public class ManagementController : Controller
    {
        private readonly ManagementService _service;
        public ManagementController(ManagementService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var list = await _service.GetRecordList();
            return View(list);
        }

        public async Task<ActionResult> Detail(int id)
        {
            var victimdetails = await _service.Details(id);
            return View(victimdetails);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            var result = await _service.Delete(id);
            return Json(result);
        }

    }
}