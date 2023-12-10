using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.Globalization;

namespace Elecciones
{ 
    public class PartidosPersistentes
    {

        private List<String> listaPartidosPoliticos = new List<String>();
        private string fichero = "partidos.xml";

        public PartidosPersistentes()
        {
            CargarPartidos();

        }

      
        
        public List<String> getPartidos()
        {
            return listaPartidosPoliticos;
        }


        public void AgregarPartido(String partido)
        {
            listaPartidosPoliticos.Add(partido);
            GuardarPartidos();
        }

        public void CargarPartidos()
        {
            if (File.Exists(fichero))
            {
                using (FileStream fs = new FileStream(fichero, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<String>));
                    listaPartidosPoliticos = (List<String>)serializer.Deserialize(fs);
                }
            }

        }

        private void GuardarPartidos()
        {
            using (FileStream fs = new FileStream(fichero, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<String>));
                serializer.Serialize(fs, listaPartidosPoliticos);
            }
        }




    }

    public class LecturaDeFicheroTxt
    {

        public List<string> leerFichero()
        {
            string nombreFichero = "coloresTexto.txt";
            List<string> lineas = new List<string>();

            if (File.Exists(nombreFichero))
            {
                using(StreamReader sr = new StreamReader(nombreFichero))
                {
                    string linea;
                    while((linea = sr.ReadLine()) != null)
                    {
                        lineas.Add(linea);
                    }
                }
            }


            return lineas;
        }

    }


    public class LecturaDeFicheroProcesosElectorales { 
    
        public ObservableCollection<ProcesoElectoral>  leerCSProcesos(String nombreFich)
        {

            ObservableCollection<ProcesoElectoral> procesos = new ObservableCollection<ProcesoElectoral>();
            if (File.Exists(nombreFich))
            {
                using (StreamReader reader = new StreamReader(nombreFich))
                {
                    while (!reader.EndOfStream)
                    {
                        //Lee toda la linea
                        String linea = reader.ReadLine();
                        //Dividimos la linea en ','
                        string[] fila = linea.Split(',');
                        int cont = 0;

                        ProcesoElectoral proceso = new ProcesoElectoral
                        {
                            nombreProcesoElectoral = fila[0],
                            fechaProcesoElectoral = DateTime.ParseExact(fila[1], "d/M/yyyy", CultureInfo.InvariantCulture),

                        };

                        ObservableCollection<Partido> partidos = new ObservableCollection<Partido>();

                        for (int i = 2; i < fila.Length; i += 3)
                        {
                            Partido partido = new Partido();
                            partido.nombrePartido = fila[i];
                            partido.scanios = Int32.Parse(fila[i + 1]);
                            cont = cont + partido.scanios;
                            partido.color = fila[i + 2];
                            partidos.Add(partido);
                        }
                        proceso.Partidos = partidos;
                        proceso.numeroDeEscanios = cont;
                        proceso.mayoriaAbsoluta = cont / 2 + 1;

                        procesos.Add(proceso);
                    }
                }
            
            }

            return procesos;
     
        }
    }






}
