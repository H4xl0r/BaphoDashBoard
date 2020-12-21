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
using static BaphoDashBoard.CustomModels.ChartComponents;

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
            var result = await _service.GetHostData();
            return Json(result);
        }

        public async Task<IActionResult> GetVictimData()
        {
            var result = await _service.GetVictimData();

            return Json(result);
        }

        public async Task<IActionResult> GetOSData()
        {
            AppResult<VMChart<string>> result = new AppResult<VMChart<string>>();

            try
            {
                var list = await _service.GetmachinesOs();
                if(list.Count != 0)
                {
                    VMChartDataset<string> vmChartDataset = new VMChartDataset<string>();
                    vmChartDataset.Data.AddRange(list.Select(x => x.Porcent.ToString()).ToList());
                    result.MRObject.Labels = list.Select(x => x.Name).ToList();
                    result.MRObject.DataSet.Add(vmChartDataset);
                    result.MRObject.MultipleDataset = false;
                }
            }
            catch(Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }

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
