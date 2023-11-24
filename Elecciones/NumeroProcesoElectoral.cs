using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elecciones
{
    internal class NumeroProcesoElectoral
    {
        //Almacenará los procesos electorales para que asi sea más facil de poner
        public int numProceso;

        //Almacena el numero de escaños del partido
        public int numEscanios;


        public NumeroProcesoElectoral(int numProceso, int numEscanios) 
        { 
            this.numProceso = numProceso;
            this.numEscanios = numEscanios;
        } 





    }
}
