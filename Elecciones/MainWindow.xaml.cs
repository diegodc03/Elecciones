using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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

    

    public partial class MainWindow : Window
    {
        //Declaramos una lista ObservableCollection
        ObservableCollection<ProcesoElectoral> listaProcesos = new ObservableCollection<ProcesoElectoral>();

        //Instanciamos la ventana secundaria a null
        VentanaSecundaria wsec = null;

        ProcesoElectoral procesoSeleccionado = new ProcesoElectoral();     

        GraficoUnitario grafico;
        GraficoComparatorioEntreElecciones graficoComparativo;
        ProcesoElectoral proceso1;
        ProcesoElectoral proceso2;




        public MainWindow()
        {
            InitializeComponent();


            //Lectura de Fichero con los procesos Electorales
            LecturaDeFicheroProcesosElectorales lectura = new LecturaDeFicheroProcesosElectorales();
            listaProcesos = lectura.leerCSVPartidos("partidosAlPrincipio.csv");


            //Me suscribo eventos cambio en el tabItem
            canvasUnitaria.SizeChanged += canvasUnit_SizeChanged;
            canvasComparativa.SizeChanged += canvasComp_SizeChanged;

        }



        private void canvasUnit_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (canvasUnitaria.IsEnabled && e.NewSize.Width > 0 && procesoSeleccionado.Partidos != null)
            {
                canvasUnitaria.Children.Clear();
                GraficoUnitario graficoUnitario = new GraficoUnitario(canvasUnitaria);
                graficoUnitario.MostrarGrafico(procesoSeleccionado);
                procesoSeleccionado = null;
            }
        }

        private void canvasComp_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(canvasComparativa.IsEnabled && e.NewSize.Width > 0 && procesoSeleccionado.Partidos != null)
            {
                // Añadimos Grafica Comparatoria a su canvas
                canvasComparativa.Children.Clear();
                ProcesoElectoral p = procesoSeleccionado as ProcesoElectoral;
                List<ProcesoElectoral> aniadirGrafica = new List<ProcesoElectoral>();
                aniadirGrafica.Add(p);
                //Introducimos en una lista todos los valores que sean iguales
                foreach (ProcesoElectoral proc in listaProcesos)
                {
                    if (p.numeroDeEscanios == proc.numeroDeEscanios)
                    {
                        aniadirGrafica.Add(proc);
                    }
                }
                GraficoComparatorioEntreElecciones graficoomp = new GraficoComparatorioEntreElecciones(canvasComparativa);
                graficoomp.MostrarGrafico(aniadirGrafica);
                //procesoSeleccionado = null;

            }
        }


        private void listaPartidosPoliticos_SelectionChanger()
        {
            listaProcesos.Clear();
        }



        private void MenuConfig_Click(object sender, EventArgs e)
        {   
            if(wsec == null)
            {
                wsec = new VentanaSecundaria(this.listaProcesos);
            }
            
            if(listaProcesos != null)
            {
                wsec.DataGridPartidosPoliticos.ItemsSource = null;
                wsec.DataGridPartidosPoliticos.ItemsSource = listaProcesos;
            }
            
            wsec.Owner = this;



            

            //Nos suscribimos al actualizador de grafica
            wsec.ItemChanged += wsec_actualizarGrafica;
            //wsec.closed = Wsec_closed;
            
            wsec.Title = "Configuración";
            wsec.Show();

        }




        private void wsec_actualizarGrafica(object sender, ItemEventArgs e)
        {

            //Me suscribo un evento para que avise cuando el vanvas esta activo
            //canvasUnitaria.SizeChanged += metodoSizeChanged;

            //ObservableCollection<ProcesoElectoral> procesos = new ObservableCollection<ProcesoElectoral>();
            ProcesoElectoral proceso = new ProcesoElectoral();
            //e devuelve un ObservableCollection<ProcesoElectoral>, donde estan los elementos
            ItemEventArgs procesoDevuelto = e as ItemEventArgs;

            procesoSeleccionado = procesoDevuelto.procesoElectoral;

            if(procesoDevuelto != null)
            {
               proceso = procesoDevuelto.procesoElectoral;
            }

            if(proceso != null)
            {
                // Añadimos grafica Unitaria a su canvas
                if(canvasUnitaria.IsLoaded && canvasUnitaria.ActualWidth > 0)
                {
                    canvasUnitaria.Children.Clear();
                    GraficoUnitario graficoUnitario = new GraficoUnitario(canvasUnitaria);
                    graficoUnitario.MostrarGrafico(e.procesoElectoral);
                }
               
                if(canvasComparativa.IsLoaded && canvasComparativa.ActualWidth > 0)
                {
                    // Añadimos Grafica Comparatoria a su canvas
                    canvasComparativa.Children.Clear();
                    ProcesoElectoral p = e.procesoElectoral as ProcesoElectoral;
                    List<ProcesoElectoral> aniadirGrafica = new List<ProcesoElectoral>();
                    aniadirGrafica.Add(p);
                    //Introducimos en una lista todos los valores que sean iguales
                    foreach (ProcesoElectoral proc in listaProcesos)
                    {
                        if (p.numeroDeEscanios == proc.numeroDeEscanios)
                        {
                            aniadirGrafica.Add(proc);
                        }
                    }
                    GraficoComparatorioEntreElecciones graficoomp = new GraficoComparatorioEntreElecciones(canvasComparativa);
                    graficoomp.MostrarGrafico(aniadirGrafica);

                }


                //Añado la grafica de pactometro
            }


        }

        //private void metodoSizeChanged()

        private void Wsec_closed(object sender, EventArgs e)
        {
            wsec = null;
        }





        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (grafico != null)
            {
                canvasUnitaria.Children.Clear();
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







        // Metodos creacion de gráficas





    }
}
