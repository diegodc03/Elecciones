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
        public String nombreProcesoElectoral 
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

        public ProcesoElectoral(String nombreEleccion, DateTime fechaProceso, int numEscanios, int mayoria, ObservableCollection<Partido> partidos)
        {
            this.nombreProcesoElectoral = nombreEleccion;
            this.fechaProcesoElectoral = fechaProceso;
            this.numeroDeEscanios = numEscanios;
            this.mayoriaAbsoluta = mayoria;
           
            //Se crea una lista nueva de partidos para que se vayan introduciendo
            ObservableCollection<Partido> Partidos = new ObservableCollection<Partido>();
            this.Partidos = partidos;

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
        }

        public Partido()
        {
            
        }



    }


    public static class ProcesoElectoralFactory
    {
        public static ProcesoElectoral CrearProcesoElectoral(String nombreEleccion, DateTime fechaProceso, int numEscanios, int mayoria, ObservableCollection<Partido> partidos)
        {
            return new ProcesoElectoral(nombreEleccion, fechaProceso, numEscanios, mayoria, partidos);
        }


        public static Partido CrearPartido(String nombreProceso, int numEscanios, string color)
        {
            return new Partido(nombreProceso, numEscanios, color);
        }

    }

    
}
