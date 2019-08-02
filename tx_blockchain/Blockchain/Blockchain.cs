using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// this class implements the actual linked list
public class Blockchain
{
    public IList<Block> Chain { set; get; }
    public int Difficulty { set; get; } = 2;

    public Blockchain()
    {
        InitializeChain();
    }

    public Blockchain(bool newBlockChain)
    {
        InitializeChain();
        AddGenesisBlock(); // add the first block
    }

    public void InitializeChain()
    {
        Chain = new List<Block>();
    }

    public Block CreateGenesisBlock()
    {
        return new Block(DateTime.Now, null, "{}");
    }

    public void AddGenesisBlock()
    {
        Chain.Add(CreateGenesisBlock());
    }

    // return the last block in the list
    public Block GetCurrentBlock()
    {
        return Chain[Chain.Count - 1];
    }

    // adds a new block to the chain
    public void AddBlock(Block block)
    {
        // check blockchain consistency first
        if (this.IsValid() == true) 
        {
            Block latestBlock = GetCurrentBlock();
            block.Index = latestBlock.Index + 1;
            block.PreviousBlockHash = latestBlock.BlockHash;
            
            // mine for a valid hash
            block.Mine(this.Difficulty);

            // add the block to the chain
            Chain.Add(block);
        }
    }

    // checks, if the blockchain is consistent by
    // re-generating and comparing the hashes in each block
    public bool IsValid()
    {
        for (int i = 1; i < Chain.Count; i++)
        {
            Block currentBlock = Chain[i];
            Block previousBlock = Chain[i - 1];

            if (currentBlock.BlockHash != currentBlock.GenerateBlockHash())
            {
                return false;
            }

            if (currentBlock.PreviousBlockHash != previousBlock.BlockHash)
            {
                return false;
            }
        }

        return true;
    }
}
