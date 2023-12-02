using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Elecciones
{

    

    public partial class VentanaSecundaria : Window
    {
        //Creamos una lista observable, aqui pasamos la lista de la ventana "AgregarProcesoElectoral" consiguiendo tenerla y poder rellanar el DataGrid
        //public ObservableCollection<ProcesoElectoral> listaProcesoElectoral { get; } = new ObservableCollection<ProcesoElectoral>();
        public List<Partido> listaPartidos;
        List<ProcesoElectoral> procesosElectorales;




        public VentanaSecundaria()
        {
            InitializeComponent();
            
            
        }

        private void BotonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BotonCrearEleccion_Click(object sender, RoutedEventArgs e)
        {
            //DialogResult = true;

            AgregarProcesoElectoral agregar = new AgregarProcesoElectoral();

            //Me suscribo al avento
            agregar.ProcesosElectoralesActualizados += ListaProcesosElectorales;
            

            agregar.Title = "Agregar Proceso Electoral";
            agregar.ShowDialog();
            
        }
        
        private void ListaProcesosElectorales(object sender, ProcesosElectoralesEventArgs e)
        {
            procesosElectorales = e.ProcesoElectorales;


            //Actualizamos DataGrid
            foreach(ProcesoElectoral proceso in procesosElectorales)
            {
                DataGridProcesosElectorales.Items.Add(proceso);
            }

            

        }

        //Evcento al pulsar cada uno de los procesos Electorales
        //Este evento nos permite que dependiendo donde toquemos, tendremos los partidos politicos de cada proceso y solo será ir añadiendo al dataGrid
        private void DataGridProcesosElectorales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DataGridProcesosElectorales.SelectedItem != null)
            {

                ProcesoElectoral proceso = DataGridProcesosElectorales.SelectedItem as ProcesoElectoral;
                List<Partido> partidos = new List<Partido>(proceso.Partidos);
                
                foreach(Partido partido in partidos)
                {
                    if(partido != null)
                    {
                        //Añadimos el partido al DataGrid del procesoElectoral
                        DataGridPartidosPolíticos.Items.Add(partido);



                    }
                }


            }

            


        }

        /*//Se ira haciendo
        private void BotonEliminarEleccion_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void BotonEditarEleccion_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    */

    }
}
