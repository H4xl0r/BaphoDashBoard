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

namespace BaphoDashBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        protected readonly MyLocalDatabase _context;
        public HomeController(MyLocalDatabase context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult StartUser()
        {
            try
            {
                var hash = "";
                using (MD5 mD5 = MD5.Create())
                {
                    hash = GetMd5Hash(mD5, "admin");
                }

                var result = new Administrators()
                {
                    Username = "admin",
                    Password = "admin",
                    PasswordHash = hash
                };
                _context.Administrators.Add(result);
                _context.SaveChanges();

                ViewBag.Message = "Your administrator account has been created successfully";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex;
            }
            
            return View();
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
