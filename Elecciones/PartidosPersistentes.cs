﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace Elecciones
{
    internal class PartidosPersistentes
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
}
