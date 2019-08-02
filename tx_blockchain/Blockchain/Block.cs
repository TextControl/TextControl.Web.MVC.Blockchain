using System;
using System.Security.Cryptography;
using System.Text;

// this class contains the structure of a single block in the
// blockchain and stores the document MD5 hash in the data property.
public class Block
{
    public int Index { get; set; }
    public DateTime TimeStamp { get; set; }
    public string PreviousBlockHash { get; set; }
    public string BlockHash { get; set; }
    public string Data { get; set; }
    public int Nonce { get; set; } = 0;

    // constructor
    public Block (DateTime timeStamp, string previousBlockHash, string data)
    {
        Index = 0;
        TimeStamp = timeStamp;
        PreviousBlockHash = previousBlockHash;
        Data = data;
        BlockHash = GenerateBlockHash();
    }

    // generates a new hash for the block including the time stamp,
    // the previous block has, the data and the nonce.
    internal string GenerateBlockHash()
    {
        SHA256 sha256 = SHA256.Create();

        byte[] bInput = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousBlockHash ?? ""}-{Data}-{Nonce}");
        byte[] bOutput = sha256.ComputeHash(bInput);

        return Convert.ToBase64String(bOutput);
    }

    // this method is generating hashes until a new hash with a specific
    // number of leading zeros is created
    public void Mine(int difficulty)
    {
        string sLeadingZeros = new string('0', difficulty);

        while (this.BlockHash == null || this.BlockHash.Substring(0, difficulty) != sLeadingZeros)
        {
            this.Nonce++;
            this.BlockHash = this.GenerateBlockHash();
        }
    }
}
