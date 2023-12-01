using System;
using System.Collections.Generic;
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
    public partial class MainWindow : Window
    {
        //Instanciamos la ventana secundaria a null
        VentanaSecundaria wsec = null;

        GraficoUnitario grafico;
        GraficoComparatorioEntreElecciones graficoComparativo;
        ProcesoElectoral proceso1;
        ProcesoElectoral proceso2;
        public MainWindow()
        {
            InitializeComponent();
            /*
            //proceso1 = new ProcesoElectoral("Elecciones 2022", "01/01/2022", 410);

            proceso1.Partidos.Add(new Partido("PP", 150, "Blue"));
            proceso1.Partidos.Add(new Partido("PSOE", 120, "Red"));
            proceso1.Partidos.Add(new Partido("VOX", 60, "Green"));
            proceso1.Partidos.Add(new Partido("Podemos", 40, "Purple"));
            proceso1.Partidos.Add(new Partido("SUMAR", 30, "Pink"));
            proceso1.Partidos.Add(new Partido("Junts", 20, "Brown"));
            proceso1.Partidos.Add(new Partido("CAGAR", 10, "Yellow"));
            proceso1.Partidos.Add(new Partido("Ciudadanos", 5, "Black"));
            proceso1.Partidos.Add(new Partido("Junts", 20, "Brown"));
            proceso1.Partidos.Add(new Partido("CAGAR", 10, "Yellow"));
            proceso1.Partidos.Add(new Partido("Ciudadanos", 5, "Black"));
            proceso1.Partidos.Add(new Partido("Podemos", 40, "Purple"));
            proceso1.Partidos.Add(new Partido("SUMAR", 30, "Pink"));
            proceso1.Partidos.Add(new Partido("Junts", 20, "Brown"));
            proceso1.Partidos.Add(new Partido("CAGAR", 10, "Yellow"));
            proceso1.Partidos.Add(new Partido("Ciudadanos", 5, "Black"));
            proceso1.Partidos.Add(new Partido("Junts", 20, "Brown"));
            proceso1.Partidos.Add(new Partido("CAGAR", 10, "Yellow"));
            proceso1.Partidos.Add(new Partido("Ciudadanos", 5, "Black"));



            proceso2 = new ProcesoElectoral("Elecciones 2026", "01/01/2026", 410);
            proceso2.Partidos.Add(new Partido("PSOE", 160, "Red"));
            proceso2.Partidos.Add(new Partido("PP", 100, "Blue"));
            proceso2.Partidos.Add(new Partido("SUMAR", 70, "Pink"));
            proceso2.Partidos.Add(new Partido("VOX", 40, "Green"));
            proceso2.Partidos.Add(new Partido("CAGAR", 20, "Yellow"));
            proceso2.Partidos.Add(new Partido("Podemos", 10, "Purple"));



            //Tenemos el tamaño del canvas real
            //canvasGrafica.Width = xrealmax - xrealmin;
            //canvasGrafica.Height = yrealmax - yrealmin;


            */

        }


        private void GraficaSoloUnaEleccion_Click(object sender, EventArgs e)
        {

            List<Partido> partidos = proceso1.Partidos; // Obtén la lista de partidos de la elección que deseas graficar
            
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
        /*
        private void GraficaPactometroUnaEleccion_Click(object sender, SizeChangedEventArgs e)
        {

        }
*/
        private void MenuConfig_Click(object sender, EventArgs e)
        {
            
            if(wsec == null)
            {
                wsec = new VentanaSecundaria();
            }

            wsec.Owner = this;

           
            wsec.Title = "Configuración";
            wsec.Show();

            
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
