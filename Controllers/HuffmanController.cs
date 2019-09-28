using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Laboratorio1_MarceloRosales_CristianAzurdia_Huffman.Models;
using System.IO;
using PruebaHuffman;


namespace Laboratorio1_MarceloRosales_CristianAzurdia_Huffman.Controllers
{
    public class HuffmanController : Controller
    {
        
        // GET: Huffman
        public ActionResult Iniciar()
        {
            LZW objeto = new LZW();
            
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
        public ActionResult DescomprimirLZW()
        {
            return View();
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

         
                
               
              

            }
                       
                return View();
            

         
        }

        [HttpPost]
        public FileResult ArchivoLZW(HttpPostedFileBase fileLZW)
        {
            ClaseLZW modelo = new ClaseLZW();

            string sobrante = "";
            List<string> RecorridoDeCadenas = new List<string>();
            bool BanderaFinal = false;
            int ContarBuffer = 0;
            string ruta = Server.MapPath("~/Archivos/");
                string RutaDescarga = Server.MapPath("~/Comprimidos/");
                ruta += fileLZW.FileName;
                modelo.CargarArchivo(ruta, fileLZW,RutaDescarga);
                ViewBag.Correcto = modelo.Exito;
                ViewBag.Error = modelo.error;

            Dictionary<string, int> diccionarioOriginal = modelo.Leer();
            modelo.EscribirDiccionario(diccionarioOriginal);
            List<string> CadenaAComprimir = modelo.compresion(diccionarioOriginal);

            Dictionary<string, int> DiccionarioComplementario = diccionarioOriginal;

            foreach (var item in CadenaAComprimir)
            {
                ContarBuffer++;
                if (ContarBuffer == CadenaAComprimir.Count)
                {
                    BanderaFinal = true;
                }
                RecorridoDeCadenas.Add(modelo.ArmarDiccionarioDeCaracteresComplementarios(sobrante + item, ref sobrante, BanderaFinal));
            }
            foreach (var item in RecorridoDeCadenas)
            {
                modelo.Escritura(item);
            }
            modelo.Limpiar();
                string[] auxiliarNombre = fileLZW.FileName.Split('.');
               
           
                RutaDescarga += auxiliarNombre[0] + ".lzw";
                return File(RutaDescarga, "application/txt", auxiliarNombre[0]);

     
        }
        [HttpPost]
        public FileResult DescomprimirLZW(HttpPostedFileBase fileDesLZW)
        {
            //ClaseLZW modelo = new ClaseLZW();
            LZW PruebaLZW = new LZW();
            string ruta = Server.MapPath("~/Archivos/");
            string RutaDescarga = Server.MapPath("~/Descomprimidos/");
            ruta += fileDesLZW.FileName;
            //modelo.CargarArchivoDescomprimido(ruta,fileDesLZW,RutaDescarga);
            //modelo.LecturaDesc();
            /*List<int> NumerosCompletos = modelo.Desencolar();
            modelo.GuardarLista(NumerosCompletos);
            modelo.LeerDescompresionParaDiccionario();*/
            string[] auxiliarNombre = fileDesLZW.FileName.Split('.'); 
            PruebaLZW.Leer();
            PruebaLZW.EscribirDiccionario();
            PruebaLZW.compresion();
            // modelo.Mostrar();
            PruebaLZW.LecturaDesc();
            List<int> NumerosCompletos = PruebaLZW.Desencolar();
            PruebaLZW.GuardarLista(NumerosCompletos);
            PruebaLZW.LeerDescompresionParaDiccionario();
            RutaDescarga += auxiliarNombre[0] + ".txt";
            return File(RutaDescarga, "application/txt", auxiliarNombre[0]);

        }
       

    }
}