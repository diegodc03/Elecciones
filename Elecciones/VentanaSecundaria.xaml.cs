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
    public class ItemEventArgs : EventArgs
    {
        public ProcesoElectoral procesoElectoral
        {
            get;
            set;
        }

        public ItemEventArgs(ProcesoElectoral proceso)
        {
            procesoElectoral = proceso;
        }
    }


   



    public partial class VentanaSecundaria : Window
    {
        //Creamos una lista observable, aqui pasamos la lista de la ventana "AgregarProcesoElectoral" consiguiendo tenerla y poder rellanar el DataGrid
        //public ObservableCollection<ProcesoElectoral> listaProcesoElectoral { get; } = new ObservableCollection<ProcesoElectoral>();
        ObservableCollection<Partido> listaPartidos;
        ObservableCollection<ProcesoElectoral> procesosElectorales = new ObservableCollection<ProcesoElectoral>();
        ObservableCollection<Partido> partidosPoliticos = new ObservableCollection<Partido>();
        ObservableCollection<ProcesoElectoral> procesosGraficas = new ObservableCollection<ProcesoElectoral> ();
        
        //Evento que salta cuando cambiamos los elementos de la grafica
        //public event Action<ObservableCollection<ProcesoElectoral>> OnDatosActualizados;


        

        public VentanaSecundaria(ObservableCollection<ProcesoElectoral> procesos)
        {
            InitializeComponent();

            this.procesosElectorales = procesos;
            DataGridProcesosElectorales.ItemsSource = procesosElectorales;
        }



        private void BotonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BotonCrearEleccion_Click(object sender, RoutedEventArgs e)
        {
            //DialogResult = true;

            AgregarProcesoElectoral agregar = new AgregarProcesoElectoral(procesosElectorales);

            //Me suscribo al avento
            agregar.ProcesosElectoralesActualizados += ListaProcesosElectorales;
            
            agregar.Title = "Agregar Proceso Electoral";
            agregar.ShowDialog();
            
        }
        
        private void ListaProcesosElectorales(object sender, ProcesosElectoralesEventArgs e)
        {
            //Como paso todos los elementos otra vez a la ventanaProcesoElectorral, lo que pasa es que luego al devolverlos, se me pasan todos otra vez, y se añaden todos al dataGrid
            // como no quiero que pase eso, elimino todos los elementos y asi no tengo problema
            
            
            
            //DataGridProcesosElectorales.Items.Clear();
            procesosElectorales = e.ProcesoElectorales;
            DataGridProcesosElectorales.ItemsSource = procesosElectorales;
            //Actualizamos DataGrid
            //foreach(ProcesoElectoral proceso in procesosElectorales)
            //{
            //    DataGridProcesosElectorales.Items.Add(proceso);
            //} 

        }


        //Evcento al pulsar cada uno de los procesos Electorales
        //Este evento nos permite que dependiendo donde toquemos, tendremos los partidos politicos de cada proceso y solo será ir añadiendo al dataGrid
        private void DataGridProcesosElectorales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {  
            
            
          //  if(tipoGrafica == TipoGrafica.Unitaria || tipoGrafica == TipoGrafica.Pactometro || tipoGrafica == TipoGrafica.Pactometro)
           // {
               // procesosGraficas.Clear();
                if(DataGridProcesosElectorales.SelectedItem != null && DataGridProcesosElectorales.SelectedItem is ProcesoElectoral)
                {
                //Tenemos el proceso
                    ProcesoElectoral proceso = DataGridProcesosElectorales.SelectedItem as ProcesoElectoral;
                    ActualizarGrafica(proceso);
                    //O hacemos un evento o metodo para llamar
                }
            //}

            DataGridPartidosPoliticos.ItemsSource=null;

            if (DataGridProcesosElectorales.SelectedItem != null)
            {
                ProcesoElectoral proceso = DataGridProcesosElectorales.SelectedItem as ProcesoElectoral;

                if (proceso != null)
                {     
                    partidosPoliticos = proceso.Partidos;
                    DataGridPartidosPoliticos.ItemsSource = partidosPoliticos;   
                }
            }
        }

        /*
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //Si se cambia y no estamos en la grafica comparativa no cambiamos nada ya que no queremos nada
            if(tipoGrafica == TipoGrafica.Comparatoria)
            {
                var checkBox = sender as CheckBox; 
                ProcesoElectoral proc = checkBox.DataContext as ProcesoElectoral;
                if (!procesosGraficas.Contains(proc)) {
                    procesosGraficas.Add(proc);
                    ActualizarGrafica(procesosGraficas);
                }
            }
        }




        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //Si se cambia y no estamos en la grafica comparativa no cambiamos nada ya que no queremos nada
            if (tipoGrafica == TipoGrafica.Comparatoria)
            {
                var checkBox = sender as CheckBox;
                ProcesoElectoral proc = checkBox.DataContext as ProcesoElectoral;
                if (procesosGraficas.Contains(proc))
                {
                    procesosGraficas.Remove(proc);
                    ActualizarGrafica(procesosGraficas);
                }
            }
        }
        */


        public event EventHandler<ItemEventArgs> ItemChanged;

        public void OnItemChanged(ItemEventArgs e)
        {
            if (ItemChanged != null)
            {
                ItemChanged(this, e);
            }
        }



        private void ActualizarGrafica(ProcesoElectoral datos)
        {
            //ItemChanged?.Invoke(this, new ItemEventArgs(datos));

            //Estamos aqui para pasar a la ventana principal
            //Datos es la collection donde he metido los valores para pasar a la ventana prinipal
            if (procesosGraficas != null)
            {
                OnItemChanged(new ItemEventArgs((ProcesoElectoral)datos));
            }
        }




        private void BotonEliminarEleccion_Click(object sender, RoutedEventArgs e)
        {
            ProcesoElectoral procesoElectoralSeleccionado = DataGridProcesosElectorales.SelectedItem as ProcesoElectoral;

            if(procesoElectoralSeleccionado != null)
            {
                procesosElectorales.Remove(procesoElectoralSeleccionado);

                //DataGridProcesosElectorales.Items.Clear();
                //Añadimos otra vez todo correctamente sin el elemento seleccionado
                //foreach(ProcesoElectoral proceso in procesosElectorales)
                //{
                    DataGridProcesosElectorales.ItemsSource = procesosElectorales;
                //}
            }
        }

        
        private void BotonEditarEleccion_Click(object sender, RoutedEventArgs e)
        {
            ProcesoElectoral procesoElectoralSeleccionado = DataGridProcesosElectorales.SelectedItem as ProcesoElectoral;
            if(procesoElectoralSeleccionado != null)
            {
                AgregarProcesoElectoral agregar = new AgregarProcesoElectoral(procesoElectoralSeleccionado);
                agregar.Show();
            }

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
