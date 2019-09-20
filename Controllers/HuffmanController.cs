using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Laboratorio1_MarceloRosales_CristianAzurdia_Huffman.Models;
using System.IO;


namespace Laboratorio1_MarceloRosales_CristianAzurdia_Huffman.Controllers
{
    public class HuffmanController : Controller
    {
        // GET: Huffman
        public ActionResult Iniciar()
        {
            return View();
        }
        public ActionResult CargaDeArchivo()
        {
            return View();
        }
        public ActionResult ArchivoLZW()
        {
            return View();
        }
        public FileResult Descargar(string nombreDescarga)
        {
            string RutaDescarga = Server.MapPath("~/Comprimidos/");
            RutaDescarga += nombreDescarga;
            return File(RutaDescarga, "application/huff", nombreDescarga);
        }
        [HttpPost]
        public ActionResult CargaDeArchivo(HttpPostedFileBase file)
        {
            ArbolHuff modelo = new ArbolHuff();
            if (file != null)
            {
                string ruta = Server.MapPath("~/Archivos/");
                string rutaDeDescarga = Server.MapPath("~/Comprimidos/");
                ruta += file.FileName;
                modelo.CargarArchivo(ruta, file,rutaDeDescarga);
                ViewBag.Correcto = modelo.Exito;
                ViewBag.Error = modelo.error;
                modelo.AgregarNodos();
                modelo.LeerArchivoParaComprimir();
                string[] aux = modelo.NombreDelArchivo.Split('.');
                rutaDeDescarga += aux[0]+ ".huff";

         
                Descargar(aux[0] + ".huff");
               
              

            }
                       
                return View();
            

         
        }

        [HttpPost]
        public ActionResult ArchivoLZW(HttpPostedFileBase fileLZW)
        {
            ClaseLZW modelo = new ClaseLZW();

            if(fileLZW != null)
            {
                string ruta = Server.MapPath("~/Archivos/");
                string RutaDescarga = Server.MapPath("~/Comprimidos");
                ruta += fileLZW.FileName;
                modelo.CargarArchivo(ruta, fileLZW,RutaDescarga);
                ViewBag.Correcto = modelo.Exito;
                ViewBag.Error = modelo.error;

            }
            return View();
        }
        
    }
}