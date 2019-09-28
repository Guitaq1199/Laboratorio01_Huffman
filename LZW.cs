using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;


namespace PruebaHuffman
{
    public class LZW
    {
        public static double CantidadDeCaracteres = 0;
        public static Dictionary<string, int> dicContadorDeCaracteresOriginales = new Dictionary<string, int>();
        public static Dictionary<string, int> DiccionarioComplementarioDeCaracteres = new Dictionary<string, int>();
        public Dictionary<int, string> dicRecorridosDescompresion = new Dictionary<int, string>();
        public static Queue<string> Numeros = new Queue<string>();
        const int bufferLenght = 500;
        public void Leer()
        {
            string AuxiliarDeconversion;

            var buffer = new byte[bufferLenght];
            using (var file = new FileStream("C:/Users/Azurdia/desktop/PruebaDiegolzw.txt", FileMode.Open))
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

            DiccionarioComplementarioDeCaracteres = dicContadorDeCaracteresOriginales;

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
            using (var file = new FileStream("C:/Users/Azurdia/desktop/PruebaDiegolzw.txt", FileMode.Open))
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
                            CadenaTotal += BuscarLenght.Length + DiccionarioComplementarioDeCaracteres[valorDiccionario].ToString();
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

        public void Mostrar()
        {
            foreach (var item in DiccionarioComplementarioDeCaracteres)
            {
                Console.WriteLine(item.Key + "  " + item.Value);
            }
        }
        public void EscribirDiccionario()
        {
            using (var file = new FileStream("C:/Users/Azurdia/desktop/archivo2.lzw", FileMode.Create))
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
            int x;
            var buffer = new byte[cadena.Length];
            string aux = "";
            using (var file = new FileStream("C:/Users/Azurdia/desktop/archivo2.lzw", FileMode.Append))
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
                            if (cadena[i] == '0')
                            {
                                aux = cadena[i].ToString();
                                i -= 1;
                            }
                            else
                            {
                                aux = cadena[i].ToString() + cadena[i + 1].ToString();
                            }
                        }


                        escritor.Write(Convert.ToByte(aux));

                    }

                }
            }
        }
        public void LecturaDesc()
        {
            string AuxiliarDeconversion;
            string[] CadenaSplit;

            bool bandera = true;
            var buffer = new byte[bufferLenght];
            using (var file = new FileStream("C:/Users/Azurdia/desktop/archivo2.lzw", FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        AuxiliarDeconversion = Encoding.UTF8.GetString(buffer, 0, buffer.Length);

                        if (AuxiliarDeconversion.Contains("┼") == true || bandera == false)
                        {
                            bandera = false;
                            if (AuxiliarDeconversion.Contains("┼"))
                            {
                                CadenaSplit = AuxiliarDeconversion.Split('┼');
                                conversor(CadenaSplit[1]);

                            }
                            else
                            {
                                conversor(AuxiliarDeconversion);
                            }

                        }

                    }
                }
            }
        }
        List<int> lista = new List<int>();
        public void conversor(string cadena)
        {
            string cad = "";
            int x;
            byte[] z = new byte[1];
            int y, w;
            string auxiliarDeconversion;
            for (int i = 0; i < cadena.Length; i++)
            {
                x = Convert.ToInt32(cadena[i]);
                cad += x.ToString();

            }
            for (int i = 0; i < cad.Length; i++)
            {

                y = Convert.ToInt32(cad[i]);
                z[0] = Convert.ToByte(cad[i]);
                auxiliarDeconversion = Encoding.UTF8.GetString(z, 0, z.Length);
                w = Convert.ToInt32(auxiliarDeconversion);
                añadir(w);
            }

        }
        public void añadir(int x)
        {
            Numeros.Enqueue(x.ToString());

        }
        public List<int> Desencolar()
        {
            int indice;
            List<int> NumerosCompletos = new List<int>();
            string cadena = "";
            while (Numeros.Count != 0)
            {
                indice = Convert.ToInt32(Numeros.Dequeue());
                for (int i = 0; i < indice; i++)
                {
                    cadena += Numeros.Dequeue();

                }
                NumerosCompletos.Add(Convert.ToInt32(cadena));
                cadena = "";
            }
            return NumerosCompletos;
        }

        //Comenzar descompresion

        public void LeerDescompresionParaDiccionario()
        {
            string aux;

            var buffer = new byte[bufferLenght];
            bool bandera = true;
            using (var file = new FileStream("C:/Users/Azurdia/Desktop/archivo2.lzw", FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {

                        buffer = reader.ReadBytes(bufferLenght);
                        aux = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        EscribirTextoDescomprimido(aux);
                    }
                }
            }
        }

        public Dictionary<int, string> ArmarDiccionario(string cadena)
        {
            string[] arrayString = cadena.Split('|');
            // char[] arrayChar = cadena.ToCharArray();
            int pos1 = 1;
            int pos2 = 0;
            Dictionary<int, string> dicAux = new Dictionary<int, string>();
            for (int i = 0; i < arrayString.Length; i++)
            {

                if (pos2 != arrayString.Length - 1)
                {
                    dicAux.Add(Convert.ToInt16(arrayString[pos1]), arrayString[pos2].ToString());
                    pos1 += 2;
                    pos2 += 2;
                }
                else
                {
                    break;
                }

            }
            return dicAux;
        }

        public List<int> listaAuxiliar = new List<int>();
        public void GuardarLista(List<int> lista)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                listaAuxiliar.Add(lista[i]);
            }
        }

        public string Descomprimir(string texto)
        {
            Dictionary<int, string> dicAux = ArmarDiccionario(texto);
            dicRecorridosDescompresion = dicAux;
            string[] arrayString = texto.Split('|');
            string salida = "";
            for (int i = 0; i < arrayString.Length; i++)
            {
                if (arrayString[i].Contains("┼") == true)
                {
                    List<int> listaNumeros = listaAuxiliar;
                    int codigoViejo, codigoNuevo;
                    string cadena;
                    string caracter;
                    codigoViejo = listaNumeros[0];
                    caracter = BuscarEnDiccionario(codigoViejo);
                    salida = caracter;
                    foreach (var item in listaNumeros)
                    {
                        codigoNuevo = item;
                        if (dicAux.ContainsKey(codigoNuevo) && codigoNuevo != 1)
                        {
                            cadena = BuscarEnDiccionario(codigoNuevo);
                            salida += cadena;
                            caracter = cadena[0].ToString();
                            dicAux.Add(dicAux.Count + 1, BuscarEnDiccionario(codigoViejo) + caracter);
                            codigoViejo = codigoNuevo;
                        }
                    }
                }
            }
            return salida;
        }

        public List<int> Convertidor(string Numeros)
        {
            List<int> listaAux = new List<int>();
            for (int i = 0; i < Numeros.Length; i++)
            {
                listaAux.Add(Convert.ToInt32(Numeros[i].ToString()));
            }
            return listaAux;
        }

        public string BuscarEnDiccionario(int UnNumero)
        {
            var Cadena = string.Empty;
            if (dicRecorridosDescompresion.ContainsKey(UnNumero))
            {
                var a = dicRecorridosDescompresion.First(x => x.Key == UnNumero);//funcion que debe ir a buscar al diccionario la llave UnNumero para igualar cadena a el valor
                Cadena = a.Value;
            }
            return Cadena;
        }
        public string Descomprimido;
        public void EscribirTextoDescomprimido(string texto)
        {
            Descomprimido = Descomprimir(texto);


            using (var file = new FileStream("C:/Users/Azurdia/desktop/ArchivoDescomprimido.txt", FileMode.OpenOrCreate))
            {
                using (var escritor = new StreamWriter(file))
                {
                    escritor.Write(Descomprimido);
                }
            }
        }


    }
}

