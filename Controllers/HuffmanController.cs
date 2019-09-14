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
        public ActionResult CargaDeArchivo()
        {
            return View();
        }
        [HttpPost]
        public FileResult CargaDeArchivo(HttpPostedFileBase file)
        {
            ArbolHuff modelo = new ArbolHuff();
            //if (file != null)
            //{
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
                
                return File(rutaDeDescarga, "application/huff","Comprimido"+ aux[0]+".huff");
               //  return RedirectToAction("/Descargar",modelo.NombreDelArchivo);

            //}
            //else
            //{
            //    return RedirectToAction("/inicio");
            //}

            //return View();
        }
        //public FileResult Descargar(string nombreDescarga)
        //{
        //    string RutaDescarga = Server.MapPath("~/Comprimidos/");
        //    RutaDescarga += nombreDescarga;
        //    return File(RutaDescarga, "application/txt", nombreDescarga);
        //}
    }
}