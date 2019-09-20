using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Laboratorio1_MarceloRosales_CristianAzurdia_Huffman.Models;
using System.IO;
using System.Text;

namespace Laboratorio1_MarceloRosales_CristianAzurdia_Huffman.Models
{
    public class ClaseLZW
    {
        const int bufferLenght = 500;
        public static Dictionary<string, int> dicContadorDeCaracteresOriginales = new Dictionary<string, int>();
        public static Dictionary<string, int> DiccionarioComplementarioDeCaracteres = new Dictionary<string, int>();
        public string Exito { get; set; }
        public Exception error { get; set; }
        public string Ruta { get; set; }   //Ruta de ubicacion en la carpeta archivos
        public string NombreDelArchivo { get; set; }  //Nombre del archivo
        public string RutaDeDescarga { get; set; }  //Ruta de ubicacion en la carpeta comprimidos

        public void CargarArchivo(string ruta, HttpPostedFileBase file, string rutaDeDescarga)
        {
            try
            {
                file.SaveAs(ruta);
                this.Exito = "Se ha subido el archivo";
                this.Ruta = ruta;
                this.RutaDeDescarga = rutaDeDescarga;
                this.NombreDelArchivo = file.FileName;
                Leer(ruta);
             

            }
            catch (Exception er)
            {
                this.error = er;
            }
        }
        public void Leer(string ruta)
        {
            string AuxiliarDeconversion;

            var buffer = new byte[bufferLenght];
            using (var file = new FileStream(ruta, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        AuxiliarDeconversion = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        foreach (var item in AuxiliarDeconversion)
                        {
                            ArmarDiccionarios(item);
                        }

                    }
                }
            }
           
            dicContadorDeCaracteresOriginales = dicContadorDeCaracteresOriginales.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }
        void ArmarDiccionarios(char caracterAVerificar)
        {
            string verificador = caracterAVerificar.ToString();

            if (dicContadorDeCaracteresOriginales.ContainsKey(verificador))
            {
                return;
            }
            else
            {
                dicContadorDeCaracteresOriginales.Add(verificador, dicContadorDeCaracteresOriginales.Count() + 1);
            }


        }

    }
}