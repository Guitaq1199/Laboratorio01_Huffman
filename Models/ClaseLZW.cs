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
        public void compresion()
        {
            string AuxiliarDeconversion;
            string temporalSigBuffer = "";
            var buffer = new byte[bufferLenght];
            using (var file = new FileStream("C:/Users/mgrt9/desktop/PHuff.txt", FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        AuxiliarDeconversion = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                        temporalSigBuffer = ArmarDiccionarioDeCaracteresComplementarios(temporalSigBuffer + AuxiliarDeconversion);
                    }
                }
            }
        }
        public string ArmarDiccionarioDeCaracteresComplementarios(string Cadena)
        {
            string aux = "";
            string temporalSigBuffer = "";
            Queue<int> CombinacionBytes = new Queue<int>();
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
                            CadenaTotal += DiccionarioComplementarioDeCaracteres[valorDiccionario];

                        }
                    }
                    else // cuando el indice +1 sea igual al lenght de la cadnea
                    {

                        if (!DiccionarioComplementarioDeCaracteres.ContainsKey(aux)) // verifica si no esta, entonces lo agrega
                        {
                            DiccionarioComplementarioDeCaracteres.Add(aux, DiccionarioComplementarioDeCaracteres.Count() + 1);
                            CadenaTotal += DiccionarioComplementarioDeCaracteres[valorDiccionario];
                        }
                        else // si esta, etonces lo iguala al valor de retorno y retornará
                        {
                            temporalSigBuffer = aux;
                        }
                    }
                }
            }

            Escritura(CadenaTotal);
            return temporalSigBuffer;
        }


        public void EscribirDiccionario()
        {
            string rut = RutaDeDescarga + "/" + NombreDelArchivo + "_Comprimido.txt";
            using (var file = new FileStream(rut, FileMode.Create))
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
        void Escritura(string cadena)
        {
            var buffer = new byte[cadena.Length];
            string aux = "";
            string rut = RutaDeDescarga + "/" + NombreDelArchivo + "_Comprimido.txt";
            using (var file = new FileStream(rut, FileMode.Append))
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
            dicContadorDeCaracteresOriginales.Clear();
        }

    }
}