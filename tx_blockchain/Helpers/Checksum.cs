using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace tx_blockchain.Helpers
{
    public static class Checksum
    {
        public static string CalculateMD5(byte[] document)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(document)).Replace("-", "").ToLower();
            }
        }
    }
}