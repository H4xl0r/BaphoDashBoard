using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BaphoDashBoard.Models
{
    public class VictimDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Ip { get; set; }
        public string Hostname { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string PostalCode { get; set; }
        public string MachinName { get; set; }
        public string MachineOS { get; set; }
    }
}
