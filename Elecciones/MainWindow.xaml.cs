using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Channels;
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
        ProcesoElectoral aux = new ProcesoElectoral();
        //Instanciamos la ventana secundaria a null
        VentanaSecundaria wsec = null;

        ProcesoElectoral procesoSeleccionado = new ProcesoElectoral();     





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


        private void canvasPactometro_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(canvasPactometro.IsEnabled && canvasPactometro.ActualWidth > 0)
            {
                canvasPactometro.Children.Clear();
                ProcesoElectoral p = procesoSeleccionado as ProcesoElectoral;
                graficaPactometro(p);

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
            wsec.Closed += Wsec_Closed;
            wsec.Show();

        }

         
        private void Wsec_Closed(Object sender, EventArgs e)
        {
            wsec = null;
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

                if(canvasPactometro.IsLoaded && canvasPactometro.ActualWidth > 0 )
                {
                    //Hacemos la grafica
                    canvasPactometro.Children.Clear();
                    //Meto en P el procesoElectral que he pulsado en el DataGrid
                    ProcesoElectoral p = new ProcesoElectoral();
                    p = e.procesoElectoral;

                    graficaPactometro(p);

                    


                }
            }
        }

        private void introducirLinea(double altura)
        {
            Polyline polilinea = new Polyline();

            polilinea.Stroke = Brushes.Black;
            Point[] puntos = new Point[2];

            puntos[0].Y = altura;
            puntos[0].X = 0;
            puntos[1].Y = altura;
            puntos[1].X = canvasPactometro.ActualWidth;

            polilinea.Points = new PointCollection(puntos);
            canvasPactometro.Children.Add(polilinea);
        }


        private void graficaPactometro(ProcesoElectoral p)
        {
            //Declaro Lista partidos izquierda y derecha
            List<Partido> partidosIzq = new List<Partido>();
            List<Partido> partidosDer = new List<Partido>();
            aux = p;
            //Tengo todos los partidos a la izquierda
            partidosIzq = p.Partidos.ToList();


            //Posicionar lado izquierdo y derecho --> Posicion centrada lado izquiedo y posicion centrada lado derecho
            double posIzq = ((canvasPactometro.ActualWidth/2)/2);
            double posDer = (canvasPactometro.ActualWidth / 2) + posIzq;


            //Coger Height del Canvas, para calcular
            double valorHeight = canvasPactometro.ActualHeight-(canvasPactometro.ActualHeight*0.1);
            double tamanioPorEscanio = valorHeight / p.numeroDeEscanios;
            double comienzoProxRectangulo = 10;
            double anchoRectangulo = canvasPactometro.ActualWidth * 0.25;

            //Dibujar Linea para si el pacto llega a mayoria absoluta o no
            double alturaLinea = tamanioPorEscanio * p.mayoriaAbsoluta +20;
            introducirLinea(alturaLinea);

            foreach(Partido partido in partidosIzq)
            {
                //Tamaño de cada rectangulo, lo añadimos
                double tamanioPartido = tamanioPorEscanio * partido.scanios;
                agregarRectanguloPact(posIzq, comienzoProxRectangulo, anchoRectangulo, tamanioPartido, partido.color, partido.scanios, partido, partidosIzq, partidosDer);
                comienzoProxRectangulo += tamanioPartido;
            }
        }



        private void agregarRectanguloPact(double left, double bottom, double width, double height, string colorHex, int escanios, Partido p, List<Partido> listaIzq, List<Partido> listaDer)
        {

            Rectangle rectangulo = new System.Windows.Shapes.Rectangle();

            //Asignino un evento a el rectangulo, en el cual, cuando se pulse el click derecho del raton se haga, eso implica que seria como un click
            rectangulo.MouseLeftButtonDown += (sender, e) => cambiarRectanguloPosicion_Click(sender, e, p, listaIzq, listaDer);

            rectangulo.Width = width;
            rectangulo.Height = height;

            Color clr = (Color)ColorConverter.ConvertFromString(colorHex);
            SolidColorBrush brocha = new SolidColorBrush(clr);

            rectangulo.Fill = brocha;

            double movimiento = width / 2;
            Canvas.SetLeft(rectangulo, left-movimiento);

            Canvas.SetBottom(rectangulo, bottom);

            //Agrego ToolTip al rectangulo para que cuando pase por el van los escaños
            rectangulo.ToolTip = new ToolTip { Content = escanios + " escaños" };

            canvasPactometro.Children.Add(rectangulo);


        }


        private void cambiarRectanguloPosicion_Click(Object sender, MouseButtonEventArgs e, Partido p, List<Partido> listaIzq, List<Partido> listaDer)
        {

            if (listaIzq.Contains(p))
            {
                listaIzq.Remove(p);
                listaDer.Add(p);
            }
            else if(listaDer.Contains(p))
            {
                listaDer.Remove(p);
                listaIzq.Add(p);
            }

            actualizarGraficaPactometro(listaIzq, listaDer);

        }

        private void actualizarGraficaPactometro(List<Partido> partidosIzq, List<Partido> partidosDer)
        {

            canvasPactometro.Children.Clear();

            //Ordenamos listas para mejor visualización
            partidosIzq = partidosIzq.OrderByDescending(x => x.scanios).ToList();
            partidosDer = partidosDer.OrderByDescending(x => x.scanios).ToList();


            //Posicionar lado izquierdo y derecho --> Posicion centrada lado izquiedo y posicion centrada lado derecho
            double posIzq = ((canvasPactometro.ActualWidth / 2) / 2);
            double posDer = (canvasPactometro.ActualWidth / 2) + posIzq;

            

            //Coger Height del Canvas, para calcular
            double valorHeight = canvasPactometro.ActualHeight - (canvasPactometro.ActualHeight * 0.1);
            double tamanioPorEscanio = valorHeight / aux.numeroDeEscanios;
            double comienzoProxRectanguloIzq = 10;
            double comienzoProxRectanguloDer = 10;
            double anchoRectangulo = canvasPactometro.ActualWidth * 0.25;

            double altura = tamanioPorEscanio * aux.mayoriaAbsoluta + 20;
            introducirLinea(altura);
            foreach (Partido partido in partidosIzq)
            {
                //Tamaño de cada rectangulo, lo añadimos
                double tamanioPartido = tamanioPorEscanio * partido.scanios;
                agregarRectanguloPact(posIzq, comienzoProxRectanguloIzq, anchoRectangulo, tamanioPartido, partido.color, partido.scanios, partido, partidosIzq, partidosDer);
                comienzoProxRectanguloIzq += tamanioPartido;
            }
            foreach(Partido partido in partidosDer)
            {
                //Tamaño de cada rectangulo, lo añadimos
                double tamanioPartido = tamanioPorEscanio * partido.scanios;
                agregarRectanguloPact(posDer, comienzoProxRectanguloDer, anchoRectangulo, tamanioPartido, partido.color, partido.scanios, partido, partidosIzq, partidosDer);
                comienzoProxRectanguloDer += tamanioPartido;
            }

        }



        private void Wsec_closed(object sender, EventArgs e)
        {
            wsec = null;
        }



        /*
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
        }*/

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
