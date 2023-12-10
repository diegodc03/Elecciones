using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
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
        ObservableCollection<ProcesoElectoral> procesosElectorales = new ObservableCollection<ProcesoElectoral>();
        ObservableCollection<Partido> partidosPoliticos = new ObservableCollection<Partido>();

        
        public event EventHandler LimpiarCanvas;

        protected virtual void OnLimpiarCanvas()
        {
            LimpiarCanvas?.Invoke(this, EventArgs.Empty);
        }


        public VentanaSecundaria(ObservableCollection<ProcesoElectoral> procesos)
        {
            InitializeComponent();

            this.procesosElectorales = procesos;
            if (procesosElectorales.Count > 0)
            {
                DataGridProcesosElectorales.ItemsSource = procesosElectorales;
            }
        }


        private void BotonCrearEleccion_Click(object sender, RoutedEventArgs e)
        {

            AgregarProcesoElectoral agregar = new AgregarProcesoElectoral(procesosElectorales);

            //Me suscribo al avento
            //agregar.ProcesosElectoralesActualizados += ListaProcesosElectorales;
            agregar.listaActualizada += actualizarProcesoElectoral;
            agregar.Title = "Agregar Proceso Electoral";
            agregar.ShowDialog();
            
        }
        


        private void DataGridProcesosElectorales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {  
            
            if(DataGridProcesosElectorales.SelectedItem != null && DataGridProcesosElectorales.SelectedItem is ProcesoElectoral)
            {
                ProcesoElectoral proceso = DataGridProcesosElectorales.SelectedItem as ProcesoElectoral;
                ActualizarGrafica(proceso);
            }
            

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
            if (datos != null)
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

                DataGridProcesosElectorales.ItemsSource = procesosElectorales;

                //Invocamos el evento 
                OnLimpiarCanvas();
               
            }
            
        }

        
        private void BotonEditarEleccion_Click(object sender, RoutedEventArgs e)
        {

            BotonEditar.IsEnabled = false;
            BotonEliminar.IsEnabled = false;
            BotonCrear.IsEnabled = false;


            ProcesoElectoral procesoElectoralSeleccionado = DataGridProcesosElectorales.SelectedItem as ProcesoElectoral;
            if(procesoElectoralSeleccionado != null)
            {
                AgregarProcesoElectoral agregar = new AgregarProcesoElectoral(procesoElectoralSeleccionado, procesosElectorales);
                agregar.Title = "Modifcar Proceso Electoral";
                agregar.listaActualizada += actualizarProcesoElectoral;


                agregar.Show(); 
            }
        }

        private void actualizarProcesoElectoral(object sender , ObservableCollection<ProcesoElectoral> procesosActualizados)
        {
            BotonEditar.IsEnabled = true;
            BotonEliminar.IsEnabled = true;
            BotonCrear.IsEnabled = true;
            this.procesosElectorales.Clear();
            ProcesoElectoral p = new ProcesoElectoral();

            foreach(ProcesoElectoral procesoElectoral in procesosActualizados)
            {
                procesosElectorales.Add(p.ClonarProcesoElectoral(procesoElectoral));
            }
           
            DataGridProcesosElectorales.ItemsSource = this.procesosElectorales;
            //Invocamos el evento 
            OnLimpiarCanvas();

        }


        private void SecondWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (segundaVentana.ActualWidth < 600 || segundaVentana.ActualHeight < 400)
            {
                String mensajePorPantalla = "No se puede hacer tan pequeño";
                MessageBox.Show(mensajePorPantalla);
            }
            
        }
    }
}
