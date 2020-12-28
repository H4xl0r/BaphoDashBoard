using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaphoDashBoard.DTO
{
    public class NameAndValueDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class RsaKeysDTO
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
