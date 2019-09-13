using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq;
using System.IO;
using System.Text;

namespace Laboratorio1_MarceloRosales_CristianAzurdia_Huffman.Models
{
    public class ArbolHuff
    {
        Nodo ArbolHuffman = new Nodo();
        public static double CantidadDeCaracteres = 0;
        public static Dictionary<char, double> dicContadorDeCaracteres = new Dictionary<char, double>();
        public static Dictionary<char, string> dicRecorridos = new Dictionary<char, string>();
        const int bufferLenght = 500;

        public void Leer()
        {
            string aux;

            var buffer = new byte[bufferLenght];
            using (var file = new FileStream("C:/Users/mgrt9/desktop/PHuff.txt", FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        aux = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        ArmarDiccionarios(aux);

                    }
                }
            }
        }
        public void ArmarDiccionarios(string CadenaAComprimir)
        {

            char[] ArrayChar = CadenaAComprimir.ToCharArray();
            for (int i = 0; i < ArrayChar.Length; i++)
            {
                if (dicContadorDeCaracteres.ContainsKey(ArrayChar[i]))
                {
                    dicContadorDeCaracteres[ArrayChar[i]] += 1;
                }
                else
                {
                    dicContadorDeCaracteres.Add(ArrayChar[i], 1);
                }
            }



        }
        public void AgregarNodos()
        {
            Nodo tmp;
            List<Nodo> lista = new List<Nodo>();
            Dictionary<char, double> auxiliarParaPorcentaje = new Dictionary<char, double>();
            dicContadorDeCaracteres = dicContadorDeCaracteres.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            //Nodo padre = new Nodo();
            CantidadDeCaracteres = CantidadDeCaract(dicContadorDeCaracteres);
            foreach (var item in dicContadorDeCaracteres)
            {

                auxiliarParaPorcentaje.Add(item.Key, item.Value / CantidadDeCaracteres);
                Nodo nodo = new Nodo();
                nodo.Caracter = item.Key;
                nodo.Probabilidad = (Convert.ToDouble(item.Value) / CantidadDeCaracteres);
                lista.Add(nodo);
            }
            dicContadorDeCaracteres.Clear();
            dicContadorDeCaracteres = auxiliarParaPorcentaje;
            while (lista.Count > 1)
            {

                tmp = new Nodo
                {
                    NodoIzq = lista[0],
                    NodoDer = lista[1]
                };
                tmp.Probabilidad = tmp.NodoIzq.Probabilidad + tmp.NodoDer.Probabilidad;
                lista.RemoveRange(0, 2);
                lista.Add(tmp);
                tmp = null;
                lista = lista.OrderBy(p => p.Probabilidad).ToList();
            }
            ArbolHuffman = lista[0];
            ArmarDiccionarioDeRecorrido(ArbolHuffman, "");
        }

        public double CantidadDeCaract(Dictionary<char, double> ContarDiccionario)
        {
            double aux = 0;
            foreach (var item in ContarDiccionario)
            {
                aux += item.Value;
            }
            return aux;
        }
        public void ArmarDiccionarioDeRecorrido(Nodo temp, string recorrido)
        {
            if (VerificarHoja(temp))
            {
                dicRecorridos.Add(temp.Caracter, recorrido);
            }
            else
            {
                if (temp.NodoDer != null)
                {
                    ArmarDiccionarioDeRecorrido(temp.NodoDer, recorrido + 1);

                }
                if (temp.NodoIzq != null)
                {
                    ArmarDiccionarioDeRecorrido(temp.NodoIzq, recorrido + 0);
                }

            }
        }

        public bool VerificarHoja(Nodo temp)
        {
            if (temp.NodoDer == null && temp.NodoIzq == null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public void LeerArchivoParaComprimir()
        {
            string AuxCadenaParaEscribir = "";
            char[] aux;
            string auxiliarParaCaracteres;
            var buffer = new byte[bufferLenght];
            using (var file = new FileStream("C:/Users/mgrt9/desktop/PHuff.txt", FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        auxiliarParaCaracteres = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        aux = auxiliarParaCaracteres.ToCharArray();
                        foreach (var item in aux)
                        {

                            AuxCadenaParaEscribir += BusquedaEnRecorrido(item);
                        }


                    }
                    CreacionDeArchivoComprimido(AuxCadenaParaEscribir);

                }
            }
        }
        public string BusquedaEnRecorrido(char letraReccorrido)
        {
            if (dicRecorridos.ContainsKey(letraReccorrido))
            {
                return dicRecorridos[letraReccorrido];
            }
            else
            {
                return "";
            }

        }

        public void CreacionDeArchivoComprimido(string CadenaAComprimir)
        {

            //var buffer = new byte[bufferLenght];
            using (var file = new FileStream("C:/Users/mgrt9/desktop/archivo2.txt", FileMode.OpenOrCreate))
            {
                using (var texto = new StreamWriter(file))
                {
                    texto.Write(CantidadDeCaracteres.ToString() + "|");
                    foreach (var item in dicContadorDeCaracteres)
                    {
                        texto.Write(item.Key + "|" + item.Value + "|");
                    }
                }
            }


            using (var file = new FileStream("C:/Users/mgrt9/desktop/archivo2.txt", FileMode.Append))
            {

                using (var writer = new BinaryWriter(file))
                {




                    string octeto = "";
                    double x = Math.Ceiling(Convert.ToDouble(CadenaAComprimir.Length) / 8);
                    for (int i = 0; i < CadenaAComprimir.Length; i += 8)
                    {

                        if (i + 8 > CadenaAComprimir.Length)
                        {
                            int aux = i + 8 - CadenaAComprimir.Length;
                            octeto = CadenaAComprimir.Substring(i, 8 - aux);
                            if (octeto.Length < 8)
                            {
                                for (int j = 0; i < aux; j++)
                                {
                                    octeto += 0;
                                }
                            }
                            writer.Write(Convert.ToByte(octeto, 2));

                        }
                        else
                        {
                            octeto = CadenaAComprimir.Substring(i, 8);
                            writer.Write(Convert.ToByte(octeto, 2));


                        }


                    }
                }
            }
        }
       
    }
}