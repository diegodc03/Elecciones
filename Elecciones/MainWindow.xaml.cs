using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace Elecciones
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    /// 

    public enum TipoGrafica
    {
        Unitaria,
        Comparatoria,
        Pactometro
    }



    public partial class MainWindow : Window
    {
        //Declaramos una lista ObservableCollection
        ObservableCollection<ProcesoElectoral> listaProcesos = new ObservableCollection<ProcesoElectoral>();

        //Instanciamos la ventana secundaria a null
        VentanaSecundaria wsec = null;

        private TipoGrafica tipoGrafica;


        GraficoUnitario grafico;
        GraficoComparatorioEntreElecciones graficoComparativo;
        ProcesoElectoral proceso1;
        ProcesoElectoral proceso2;




        public MainWindow()
        {
            InitializeComponent();

            //flota.CollectionChanged += Flota_CollectionChanged;

            /*
            //Tenemos el tamaño del canvas real
            //canvasGrafica.Width = xrealmax - xrealmin;
            //canvasGrafica.Height = yrealmax - yrealmin;

            */


        }
        private void listaPartidosPoliticos_SelectionChanger()
        {
            listaProcesos.Clear();
        }


        private void graficaUnitaria_Click(object sender, RoutedEventArgs e)
        {
            tipoGrafica = TipoGrafica.Unitaria;
        }

        private void graficaComparatoria_Click(object sender, RoutedEventArgs e)
        {
            tipoGrafica = TipoGrafica.Comparatoria;
        }

        private void graficaPactometro_Click(object sender, RoutedEventArgs e)
        {
            tipoGrafica = TipoGrafica.Pactometro;
        }










        /*
        private void GraficaSoloUnaEleccion_Click(object sender, EventArgs e)
        {

            ObservableCollection<Partido> partidos = proceso1.Partidos; // Obtén la lista de partidos de la elección que deseas graficar
            
            //Tenemos el tamaño del canvas real
            //canvasGrafica.Width = xrealmax - xrealmin;
            //canvasGrafica.Height = yrealmax - yrealmin;

            //Creamos una instancia  de la clase Grafico Unitario
            grafico = new GraficoUnitario(canvasGrafica);
            grafico.MostrarGrafico(proceso1);
        }




        private void graficaComparatoriaEntreElecciones_Click(object sender, EventArgs e)
        {
            //Introduciremos la lista con todos los procesos


            graficoComparativo = new GraficoComparatorioEntreElecciones(canvasGrafica);
            List<ProcesoElectoral> procesos = new List<ProcesoElectoral>();
            procesos.Add(proceso1);
            procesos.Add(proceso2);

            graficoComparativo = new GraficoComparatorioEntreElecciones(canvasGrafica);
            graficoComparativo.MostrarGrafico(procesos);


        }
        
        private void GraficaPactometroUnaEleccion_Click(object sender, SizeChangedEventArgs e)
        {

        }
*/
        private void MenuConfig_Click(object sender, EventArgs e)
        {
            if(wsec == null)
            {
                wsec = new VentanaSecundaria(tipoGrafica);
            }

            wsec.Owner = this;
            
            wsec.DataGridProcesosElectorales.ItemsSource = listaProcesos;

            //Nos suscribimos al actualizador de grafica
            wsec.ItemChanged += wsec_actualizarGrafica;
            //wsec.closed = Wsec_closed;

            wsec.Title = "Configuración";
            wsec.Show();

        }

        

        private void wsec_actualizarGrafica(object sender, ItemEventArgs e)
        {
            ObservableCollection<ProcesoElectoral> procesos = new ObservableCollection<ProcesoElectoral>();
            //e devuelve un ObservableCOllection<ProcesoElectoral> donde estan los elementos
            ItemEventArgs listaDevuelta = e as ItemEventArgs;

            if(listaDevuelta != null)
            {
                procesos = listaDevuelta.procesoElectoral;
            }



            switch (tipoGrafica)
            {
                case TipoGrafica.Unitaria:
                    //Aqui va la logica de la grafica Unitaria
                    GraficoUnitario graficoUnitario = new GraficoUnitario(canvasGrafica);
                    //Sabemos que este es un unico elemento, entonces
                    foreach(ProcesoElectoral proceso in procesos)
                    graficoUnitario.MostrarGrafico(proceso);
                    break;

                case TipoGrafica.Comparatoria:
                    //Aqui va la logica de la grafica Comparatoria
                    graficoComparativo = new GraficoComparatorioEntreElecciones(canvasGrafica);
                    graficoComparativo.MostrarGrafico(procesos);
                    break;

                case TipoGrafica.Pactometro:
                    //Aqui va la logica de la grafica Pactometro
                    break;
            }



        }

        private void Wsec_closed(object sender, EventArgs e)
        {
            wsec = null;
        }





        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (grafico != null)
            {
                canvasGrafica.Children.Clear();
                grafico.MostrarGrafico(proceso1);
            }


            if (e.NewSize.Width < 500 || e.NewSize.Height <300)
            {
                mostarCuadroTamanioMinimo();
            }

        }

        private void mostarCuadroTamanioMinimo()
        {

            string msg = "La aplicación no puede hacerse más pequeña";
            string titulo = "Elecciones";

            MessageBoxButton boton = MessageBoxButton.OK;
            MessageBoxImage imagen = MessageBoxImage.Error;

            MessageBox.Show(msg, titulo, boton, imagen);


        }

       



    }




}
