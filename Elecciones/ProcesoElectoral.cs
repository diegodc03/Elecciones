using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elecciones
{

    public class ProcesoElectoral
    {
        public String nombreProcesoElectoral { get; set; }
        public String fechaProcesoElectoral { get; set; }
        public double numeroDeEscanios { get; set; }
        public double mayoriaAbsoluta { get; set; }
        
        public List<Partido> Partidos { get; set; } // Lista de partidos asociados al proceso

        public ProcesoElectoral(String nombreEleccion, String fechaProceso, int numEscanios)
        {
            this.nombreProcesoElectoral = nombreEleccion;
            this.fechaProcesoElectoral = fechaProceso;
            this.numeroDeEscanios = numEscanios;
            this.mayoriaAbsoluta = (numEscanios / 2) + 1;
           
            //Se crea una lista nueva de partidos para que se vayan introduciendo
            Partidos = new List<Partido>();

        }
    }


    public class Partido
    {
        public String nombrePartido { get; set; }
        public int scanios { get; set; }
        public String color { get; set; }
        public int numProceso { get; set; }

        public Partido(String part, int scani, String colorr)
        {
            this.nombrePartido = part;
            this.scanios = scani;
            this.color = colorr;
            this.numProceso = 0;
        }



    }

    
}
