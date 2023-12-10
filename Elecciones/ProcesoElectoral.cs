using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elecciones
{

    public class ProcesoElectoral
    {
        public string nombreProcesoElectoral 
        { 
            get; 
            set; 
        }

        public DateTime fechaProcesoElectoral 
        {
            get; 
            set; 
        }

        public double numeroDeEscanios 
        { 
            get; 
            set; 
        }

        public double mayoriaAbsoluta 
        { 
            get; 
            set; 
        }

        // Lista de partidos asociados al proceso
        public ObservableCollection<Partido> Partidos 
        {
            get; 
            set; 
        }

        public ProcesoElectoral()
        {
           
        }

        public ProcesoElectoral(string nombreEleccion, DateTime fechaProceso, int numEscanios, int mayoria, ObservableCollection<Partido> partidos)
        {
            this.nombreProcesoElectoral = nombreEleccion;
            this.fechaProcesoElectoral = fechaProceso;
            this.numeroDeEscanios = numEscanios;
            this.mayoriaAbsoluta = mayoria;
           
            //Se crea una lista nueva de partidos para que se vayan introduciendo
            List<Partido> Partidos = new List<Partido>();
            this.Partidos = partidos;

        }


        public ProcesoElectoral ClonarProcesoElectoral(ProcesoElectoral procesoOriginal)
        {
            ProcesoElectoral procesoClonado = new ProcesoElectoral
            {
                nombreProcesoElectoral = procesoOriginal.nombreProcesoElectoral,
                fechaProcesoElectoral = procesoOriginal.fechaProcesoElectoral,
                numeroDeEscanios = procesoOriginal.numeroDeEscanios,
                mayoriaAbsoluta = procesoOriginal.mayoriaAbsoluta
            };

            procesoClonado.Partidos = new ObservableCollection<Partido>();
            foreach (Partido partido in procesoOriginal.Partidos)
            {
                procesoClonado.Partidos.Add(new Partido
                {
                    nombrePartido = partido.nombrePartido,
                    scanios = partido.scanios,
                    color = partido.color
                });
            }

            return procesoClonado;
        }


    }


    public class Partido
    {
        public string nombrePartido { get; set; }
        public int scanios { get; set; }
        public string color { get; set; }
        public int numProceso { get; set; }

        public Partido(string part, int scani, string colorr)
        {
            this.nombrePartido = part;
            this.scanios = scani;
            this.color = colorr;
        }

        public Partido()
        {
            
        }
    }
    
}
