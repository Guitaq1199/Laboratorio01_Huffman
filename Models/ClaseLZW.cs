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
        public static Dictionary<string, int> DiccionarioComplementarioDeCaracteres = new Dictionary<string, int>();
        public static Queue<string> Numeros = new Queue<string>();
        const int bufferLenght = 500;
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
                string[] auxNombre = file.FileName.Split('.');
                this.NombreDelArchivo = auxNombre[0];
              
             

            }
            catch (Exception er)
            {
                this.error = er;
            }
        }
        public Dictionary<string,int> Leer()
        {
            string AuxiliarDeconversion;
            Dictionary<string, int> dicContadorDeCaracteresOriginales = new Dictionary<string, int>();
            var buffer = new byte[bufferLenght];
            using (var file = new FileStream(this.Ruta, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        AuxiliarDeconversion = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        foreach (var item in AuxiliarDeconversion)
                        {
                            if (!dicContadorDeCaracteresOriginales.ContainsKey(item.ToString()))
                            {
                                dicContadorDeCaracteresOriginales.Add(item.ToString(), dicContadorDeCaracteresOriginales.Count() + 1);
                            }

                        }

                    }
                }
            }
            return dicContadorDeCaracteresOriginales;
        }

        public List<string> compresion(Dictionary<string, int> diccionarioOriginal) // Metodo para extraer cadenas en lista del archivo
        {
            DiccionarioComplementarioDeCaracteres = diccionarioOriginal;
            string AuxiliarDeconversion;
            var buffer = new byte[bufferLenght];
            List<string> CadenaAComprimir = new List<string>();
            using (var file = new FileStream(this.Ruta, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        AuxiliarDeconversion = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        CadenaAComprimir.Add(AuxiliarDeconversion);

                    }
                }
            }
            return CadenaAComprimir;
        }
        public string ArmarDiccionarioDeCaracteresComplementarios(string Cadena, ref string temporalSigBuffer, bool banderaFinal) // Metodo para generar los recorridos numericos
        {
            string aux = "";
            bool BanderaTemporal = false;
            string BuscarLenght = "";
            string valorDiccionario = "";
            string CadenaTotal = "";

            for (int i = 0; i < Cadena.Length; i++)
            {
                if (DiccionarioComplementarioDeCaracteres.ContainsKey(Cadena[i].ToString()))
                {
                    valorDiccionario = aux;
                    aux += Cadena[i].ToString();
                    if (i + 1 != Cadena.Length)
                    {
                        if (!DiccionarioComplementarioDeCaracteres.ContainsKey(aux))
                        {

                            DiccionarioComplementarioDeCaracteres.Add(aux, DiccionarioComplementarioDeCaracteres.Count() + 1);
                            aux = Cadena[i].ToString();
                            BuscarLenght = DiccionarioComplementarioDeCaracteres[valorDiccionario].ToString();
                            CadenaTotal += BuscarLenght.Length + DiccionarioComplementarioDeCaracteres[valorDiccionario].ToString();

                        }
                    }
                    else // cuando el indice +1 sea igual al lenght de la cadnea
                    {

                        if (!DiccionarioComplementarioDeCaracteres.ContainsKey(aux)) // verifica si no esta, entonces lo agrega
                        {
                            DiccionarioComplementarioDeCaracteres.Add(aux, DiccionarioComplementarioDeCaracteres.Count() + 1);

                            BuscarLenght = DiccionarioComplementarioDeCaracteres[valorDiccionario].ToString();
                            CadenaTotal += BuscarLenght.ToString() + DiccionarioComplementarioDeCaracteres[valorDiccionario].ToString();
                        }
                        else // si esta, etonces lo iguala al valor de retorno y retornará
                        {
                            if (banderaFinal)
                            {
                                BuscarLenght = DiccionarioComplementarioDeCaracteres[valorDiccionario].ToString();
                                CadenaTotal += BuscarLenght.ToString() + DiccionarioComplementarioDeCaracteres[valorDiccionario].ToString();
                            }
                            else
                            {
                                temporalSigBuffer = aux;
                                BanderaTemporal = true;
                            }

                        }
                    }
                }
            }
            if (!BanderaTemporal)
            {
                temporalSigBuffer = "";
            }


            return CadenaTotal;
        }


        public void EscribirDiccionario(Dictionary<string, int> dicContadorDeCaracteresOriginales)
        {

            this.RutaDeDescarga+= this.NombreDelArchivo + "_Comprimido.txt";
            using (var file = new FileStream(this.RutaDeDescarga, FileMode.Create))
            {
                using (var escritor = new StreamWriter(file))
                {
                    foreach (var item in dicContadorDeCaracteresOriginales)
                    {
                        escritor.Write(item.Key + "|" + item.Value + "|");
                    }
                    escritor.Write("┼");
                }

            }
        }
        public void Escritura(string cadena)
        {
            var buffer = new byte[cadena.Length];
            string aux = "";
            
            using (var file = new FileStream(this.RutaDeDescarga, FileMode.Append))
            {
                using (var escritor = new BinaryWriter(file))
                {

                    for (int i = 0; i < cadena.Length; i += 2)
                    {
                        if (i + 1 == cadena.Length)
                        {
                            aux = cadena[i].ToString();
                        }
                        else
                        {
                            aux = cadena[i].ToString() + cadena[i + 1].ToString();
                        }


                        escritor.Write(Convert.ToByte(aux));

                    }

                }
            }
        }
        public void Limpiar()
        {
            DiccionarioComplementarioDeCaracteres.Clear();
        }

    }
}