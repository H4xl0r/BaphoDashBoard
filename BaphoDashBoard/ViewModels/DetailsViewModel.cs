using BaphoDashBoard.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BaphoDashBoard.ViewModels
{
    public class DetailsViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Machine Name")]
        public string MachineName { get; set; }
        [Display(Name = "Machine OS version")]
        public string Machine_OS { get; set; }
        [Display(Name = "Host Name")]
        public string HostName { get; set; }
        [Display(Name = "IP")]
        public string Ip { get; set; }
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        [Display(Name = "Region")]
        public string Region { get; set; }
        public string OsVersionInfo { get; set; }
        public List<String> Vulns { get; set; }
        public byte Photo { get; set; }
        public List<NameAndValueDTO> CveInfo = new List<NameAndValueDTO>();
    }
}
