using Newtonsoft.Json;
using System;
using System.Web.Http;
using System.Web.Mvc;
using tx_blockchain.Helpers;
using tx_blockchain.Models;

namespace tx_blockchain.Controllers
{
    public class DocumentController : Controller
    {
        // this method stores the document hash in the blockchain
        [System.Web.Http.HttpPost]
        public ActionResult StoreDocument(
            [FromBody] string Document,
            [FromBody] string UniqueId,
            [FromBody] string SignerName)
        {
            byte[] bPDF;

            // create temporary ServerTextControl
            using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl())
            {
                tx.Create();

                // load the document
                tx.Load(Convert.FromBase64String(Document),
                    TXTextControl.BinaryStreamType.InternalUnicodeFormat);

                TXTextControl.SaveSettings saveSettings = new TXTextControl.SaveSettings()
                {
                    CreatorApplication = "TX Text Control Sample Application",
                };

                // save the document as PDF
                tx.Save(out bPDF, TXTextControl.BinaryStreamType.AdobePDF, saveSettings);
            }

            // calculate the MD5 checksum of the binary data
            // and store in SignedDocument object
            SignedDocument document = new SignedDocument()
            {
                Checksum = Checksum.CalculateMD5(bPDF)
            };

            // define a Blockchain object
            Blockchain bcDocument;

            // if the blockchain exists, load it
            if (System.IO.File.Exists(
                Server.MapPath("~/App_Data/Blockchains/" + UniqueId + ".bc")))
            {
                string bc = System.IO.File.ReadAllText(
                    Server.MapPath("~/App_Data/Blockchains/" + UniqueId + ".bc"));

                bcDocument = JsonConvert.DeserializeObject<Blockchain>(bc);
            }
            else
                bcDocument = new Blockchain(true); // otherwise create a new blockchain

            // add a new block to the blockchain and store the SignedDocument object
            bcDocument.AddBlock(new Block(DateTime.Now, null, JsonConvert.SerializeObject(document)));

            if (System.IO.Directory.Exists(Server.MapPath("~/App_Data/Blockchains/")) == false)
                System.IO.Directory.CreateDirectory(Server.MapPath("~/App_Data/Blockchains/"));

            // store the blockchain as a file
            System.IO.File.WriteAllText(
                Server.MapPath("~/App_Data/Blockchains/" + UniqueId + ".bc"),
                JsonConvert.SerializeObject(bcDocument));

            // create and return a view model with the PDF and the unique document ID
            StoredDocument storedDocument = new StoredDocument()
            {
                PDF = Convert.ToBase64String(bPDF),
                DocumentId = UniqueId
            };

            return new JsonResult() { Data = storedDocument,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // this method validates a document with the last block on the blockchain
        [System.Web.Http.HttpPost]
        public bool ValidateDocument([FromBody] string Document, [FromBody] string UniqueId)
        {
            if (Document == null || UniqueId == null)
                return false;

            // calculate the MD5 of the uploaded document
            string sChecksum = Checksum.CalculateMD5(Convert.FromBase64String(Document));

            Blockchain newDocument;

            // load the associated blockchain
            if (System.IO.File.Exists(
                Server.MapPath("~/App_Data/Blockchains/" + UniqueId + ".bc")))
            {
                string bc = System.IO.File.ReadAllText(
                    Server.MapPath("~/App_Data/Blockchains/" + UniqueId + ".bc"));

                newDocument = JsonConvert.DeserializeObject<Blockchain>(bc);

                // get the SignedDocument object from the block
                SignedDocument signedDocument = 
                    JsonConvert.DeserializeObject<SignedDocument>(newDocument.GetCurrentBlock().Data);

                // compare the checksum in the stored block
                // with the checksum of the uploaded document
                if (signedDocument.Checksum == sChecksum)
                    return true; // document valid
                else
                    return false; // not valid
            }
            else
                return false; // blockchain doesn't exist
        }
    }
}