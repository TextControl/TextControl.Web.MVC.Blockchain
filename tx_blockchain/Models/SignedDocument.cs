using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tx_blockchain.Models
{
    public class SignedDocument
    {
        public string Checksum { get; set; }
        public string AddedBy { get; set; }
    }
}