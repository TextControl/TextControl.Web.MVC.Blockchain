using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using tx_blockchain.Helpers;
using tx_blockchain.Models;

namespace tx_blockchain.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sign()
        {
            return View();
        }

        public ActionResult Validate(string documentId)
        {
            ValidationModel validationModel = new ValidationModel()
            {
                DocumentId = documentId
            };

            return View(validationModel);
        }

    }
}