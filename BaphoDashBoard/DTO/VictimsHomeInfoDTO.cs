using BaphoDashBoard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaphoDashBoard.DTO
{
    public class VictimsHomeInfoDTO
    {
        public int Machines { get; set; }
        public int Countries { get; set; }
        public int Cities { get; set; } 
        public int WindowsOS { get; set; }
        public int LinuxOs { get; set; }
        public List<VictimDetailsDTO> Alldata { get; set; }

        public List<VictimDetailsDTO> VictimDetails = new List<VictimDetailsDTO>();
   
    }
}
