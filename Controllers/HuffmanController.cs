using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Laboratorio1_MarceloRosales_CristianAzurdia_Huffman.Models;
using System.IO;
using System.Linq;

namespace Laboratorio1_MarceloRosales_CristianAzurdia_Huffman.Controllers
{
    public class HuffmanController : Controller
    {
        // GET: Huffman
        public ActionResult Iniciar()
        {
            return View();
        }
    }
}