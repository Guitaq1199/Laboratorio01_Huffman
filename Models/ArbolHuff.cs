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
    public class ArbolHuff
    {
        Nodo ArbolHuffman = new Nodo();
        public static double CantidadDeCaracteres = 0; // variable para la cantidad total de caracteres en el archivo
        public static Dictionary<char, double> dicContadorDeCaracteres = new Dictionary<char, double>();  //Diccionario de caracteres con probabilidad
        public static Dictionary<char, string> dicRecorridos = new Dictionary<char, string>(); // Diccionario de caracteres con recorrido del arbol
        const int bufferLenght = 500;   //lenght del buffer
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
                Leer(ruta);  //Enviar al metodo para extraer todos los caracteres y la cantidad de veces que se repita
                
            }
            catch(Exception er)
            {
                this.error = er;
            }
        }

        public void Leer(string RutaDeArchivo)
        {
            string ConvertidorByteAString; //convierte los bytes que provienen del archivo en cadenas de caracteres

            var buffer = new byte[bufferLenght];
            using (var file = new FileStream(RutaDeArchivo, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)  
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        ConvertidorByteAString = Encoding.UTF8.GetString(buffer, 0, buffer.Length);  //convierte bytes a una cadena
                        ArmarDiccionarios(ConvertidorByteAString);  //cada quinientos bytes envia al metodo de armar diccionario

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
                    dicContadorDeCaracteres[ArrayChar[i]] += 1;   // Si existe sumar uno al valor que acompaña la llave
                }
                else
                {
                    dicContadorDeCaracteres.Add(ArrayChar[i], 1);  //Si no existe la llave agregarla y poner un valor inicial de uno
                }
            }



        }
        public void AgregarNodos()
        {
            Nodo tmp;
            List<Nodo> lista = new List<Nodo>();
            Dictionary<char, double> auxiliarParaPorcentaje = new Dictionary<char, double>();
            dicContadorDeCaracteres = dicContadorDeCaracteres.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
           
           
            CantidadDeCaracteres = CantidadDeCaract(dicContadorDeCaracteres);
            foreach (var item in dicContadorDeCaracteres) //convertir el diccionario en uno con decimales
            {

                auxiliarParaPorcentaje.Add(item.Key, item.Value / CantidadDeCaracteres); //agrega la llave y el valor lo divide por el total de caracteres
                Nodo nodo = new Nodo();
                nodo.Caracter = item.Key; 
                nodo.Probabilidad = (Convert.ToDouble(item.Value) / CantidadDeCaracteres);
                lista.Add(nodo);  //agrega em la lista la llave y su probabilidad
            }
            dicContadorDeCaracteres.Clear();
            dicContadorDeCaracteres = auxiliarParaPorcentaje;
            while (lista.Count > 1) //mientras que la lista tenga una cantidad de elementos mayor a 1
            {

                tmp = new Nodo   //el temporal apunta a los dos hijos con valor mas pequeño
                {
                    NodoIzq = lista[0],
                    NodoDer = lista[1]
                };
                tmp.Probabilidad = tmp.NodoIzq.Probabilidad + tmp.NodoDer.Probabilidad;  //suma las probabilidades
                lista.RemoveRange(0, 2); //elimina los nodos a los que apunta el temporal
                lista.Add(tmp); //se añade el temporal
                tmp = null;
                lista = lista.OrderBy(p => p.Probabilidad).ToList(); //se ordena de nuevo la lista para que los mas pequeños queden en el inicio
            }
            ArbolHuffman = lista[0];  //Guardar el arbol en una variable universal
            ArmarDiccionarioDeRecorrido(ArbolHuffman, ""); 
        }

        public double CantidadDeCaract(Dictionary<char, double> ContarDiccionario)
        {
            double aux = 0;
            foreach (var item in ContarDiccionario)
            {
                aux += item.Value; //suma el valor de las probabilidades de cada llave en el diccionario para la cantidad total de caracteres
            }
            return aux;
        }
        public void ArmarDiccionarioDeRecorrido(Nodo temp, string recorrido)
        {
            if (VerificarHoja(temp) == true) //Si los apuntadores a la izquierda y derecha son nulas
            {
                dicRecorridos.Add(temp.Caracter, recorrido); //agregar el caracter con su recorrido
            }
            else
            {
                if (temp.NodoDer != null)
                {
                    ArmarDiccionarioDeRecorrido(temp.NodoDer, recorrido + 1); //Si la derecha no es null entonces enviar el hijo derecho y sumar al recorrido 1

                }
                if (temp.NodoIzq != null)
                {
                    ArmarDiccionarioDeRecorrido(temp.NodoIzq, recorrido + 0); //Si la izquierda no es null entonces enviar el hijo izquierdo y sumar al recorrido 0
                }

            }
        }

        public bool VerificarHoja(Nodo temp) //metodo bool que comprueba que apuntador a la izquierda y derecha sean null
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
        //
        public void LeerArchivoParaComprimir()
        {

            CreacionDeArchivo(); //Crea el archivo e inserta la cantidad de caracteres y diccionario
            string AuxCadenaParaEscribir = "", BuscarCadenaExacta = "";
            string Remanente = "";
            int BuscarModularExacto = 0;
            char[] aux, remanente;
            string auxiliarParaCaracteres;
            var buffer = new byte[bufferLenght];


            using (var file = new FileStream(Ruta, FileMode.Open)) //abre el archvio en la carpeta de archivos
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(bufferLenght);
                        auxiliarParaCaracteres = Encoding.UTF8.GetString(buffer, 0, buffer.Length); // convierte los caracteres a bytes
                        aux = auxiliarParaCaracteres.ToCharArray();  
                        AuxCadenaParaEscribir = "";
                        foreach (var item in aux)
                        {

                            AuxCadenaParaEscribir += BusquedaEnRecorrido(item);

                        }
                        AuxCadenaParaEscribir = Remanente + AuxCadenaParaEscribir;

                        if (AuxCadenaParaEscribir.Length % 8 == 0)
                        {
                            Remanente = "";
                            CreacionDeArchivoComprimido(AuxCadenaParaEscribir);

                        }
                        else
                        {
                            Remanente = "";
                            BuscarModularExacto = AuxCadenaParaEscribir.Length;
                            while (BuscarModularExacto % 8 != 0)
                            {
                                BuscarModularExacto -= 1;
                            }
                            BuscarCadenaExacta = AuxCadenaParaEscribir.Substring(0, BuscarModularExacto);
                            remanente = AuxCadenaParaEscribir.ToCharArray();
                            for (int i = BuscarModularExacto; i < remanente.Length; i++)
                            {
                                Remanente += remanente[i];

                            }
                            CreacionDeArchivoComprimido(BuscarCadenaExacta);



                        }


                    }


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
        public void CreacionDeArchivo()
        {
           
            string[] aux = this.NombreDelArchivo.Split('.');

            this.RutaDeDescarga += aux[0]+ ".huff";
            using (var file = new FileStream(RutaDeDescarga, FileMode.OpenOrCreate))
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
        }

        public void CreacionDeArchivoComprimido(string CadenaAComprimir)
        {

            using (var file = new FileStream(this.RutaDeDescarga, FileMode.Append))
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