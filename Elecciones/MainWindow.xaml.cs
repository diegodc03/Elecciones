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
        int comp = 1;

        //Lista partidos divididios en dos para la grafica pactometro
        List<Partido> partidosIzq = new List<Partido>();
        List<Partido> partidosDer = new List<Partido>();




        public MainWindow()
        {
            InitializeComponent();


            //Lectura de Fichero con los procesos Electorales
            LecturaDeFicheroProcesosElectorales lectura = new LecturaDeFicheroProcesosElectorales();
            listaProcesos = lectura.leerCSVPartidos("partidosAlPrincipio.csv");



        }



        private void canvasUnit_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (canvasUnitaria.IsEnabled && e.NewSize.Width > 0 && procesoSeleccionado.Partidos != null)
            {
                if (canvasUnitaria.ActualHeight > 175 || canvasUnitaria.ActualWidth > 225)
                {
                    canvasUnitaria.Children.Clear();
                    GraficoUnitario graficoUnitario = new GraficoUnitario(canvasUnitaria);
                    graficoUnitario.MostrarGrafico(procesoSeleccionado);
                    //procesoSeleccionado = null;
                }
                else
                {
                    String mensajePorPantalla = "No se puede hacer tan pequeño";
                    MessageBox.Show(mensajePorPantalla);
                }

            }
        }

        private void canvasComp_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(canvasComparativa.IsEnabled && e.NewSize.Width > 0 && procesoSeleccionado.Partidos != null)
            {
                if(canvasComparativa.ActualHeight > 175 && canvasComparativa.ActualWidth > 225)
                {
                    // Añadimos Grafica Comparatoria a su canvas
                    canvasComparativa.Children.Clear();

                    ProcesoElectoral p = procesoSeleccionado as ProcesoElectoral;

                    List<ProcesoElectoral> aniadirGrafica = new List<ProcesoElectoral>();
                    aniadirGrafica.Add(p);

                    //Introducimos en una lista todos los valores que sean iguales
                    foreach (ProcesoElectoral proc in listaProcesos)
                    {
                        if (p.numeroDeEscanios == proc.numeroDeEscanios && p.fechaProcesoElectoral != proc.fechaProcesoElectoral)
                        {
                            aniadirGrafica.Add(proc);
                        }
                    }

                    GraficoComparatorioEntreElecciones graficoomp = new GraficoComparatorioEntreElecciones(canvasComparativa);
                    graficoomp.MostrarGrafico(aniadirGrafica);
                }
                else
                {
                    String mensajePorPantalla = "No se puede hacer tan pequeño";
                    MessageBox.Show(mensajePorPantalla);
                }
                
                

            }
        }


        private void canvasPactometro_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(canvasPactometro.IsEnabled && canvasPactometro.ActualWidth > 0 )
            {
                if (canvasPactometro.ActualHeight > 175 && canvasPactometro.ActualWidth > 225 )
                {
                    if(partidosDer.Count() == 0 && partidosIzq.Count() == 0)
                    {
                        //Esto ocurre la primera vez que lo llame
                        canvasPactometro.Children.Clear();
                        ProcesoElectoral p = procesoSeleccionado as ProcesoElectoral;
                        if (p.Partidos != null)
                        {
                            graficaPactometro(p);
                        }		

                    }
                    else
                    {
                        //Cuando lo vuelva a llamar se que o una lista u otra o las dos tendran partiudos, por lo que quiero es que se vuelvan a escribir
                        actualizarGraficaPactometro(partidosIzq, partidosDer);
                    }
                    
                }
                else
                {
                    String mensajePorPantalla = "No se puede hacer tan pequeño";
                    MessageBox.Show(mensajePorPantalla);
                }
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
            comp = 1; 
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
                        if (p.numeroDeEscanios == proc.numeroDeEscanios && p.fechaProcesoElectoral != proc.fechaProcesoElectoral)
                        {
                            aniadirGrafica.Add(proc);
                        }
                    }

                    GraficoComparatorioEntreElecciones graficoomp = new GraficoComparatorioEntreElecciones(canvasComparativa);
                    graficoomp.MostrarGrafico(aniadirGrafica);

                }


                if(canvasPactometro.IsLoaded && canvasPactometro.ActualWidth > 0 && e.procesoElectoral.Partidos != null)
                {
                    //Hacemos la grafica
                    canvasPactometro.Children.Clear();
                    partidosIzq = new List<Partido>();
                    partidosDer = new List<Partido>();
                    comp = 1;
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
                      
            partidosIzq = p.Partidos.ToList();
            
            //Compruebo si acabo de pulsar, ya que deberé poner si hay el partido con mayoria absoluta a la derecha
            if (comp == 1)
            {
                Partido mayoria = partidosIzq[0];
                if(mayoria != null && mayoria.scanios >= p.mayoriaAbsoluta)
                {
                    partidosIzq.Remove(mayoria);
                    partidosDer.Add(mayoria);
                }
                
            }

            //Añadimos como está
            double contEscanios=0;
            if(partidosDer.Count() != 0)
            {
                  foreach (Partido partido in partidosDer)
                 {
                     contEscanios = contEscanios + partido.scanios;
                 }
                  textoMayoria.Text = contEscanios.ToString() + " / " + procesoSeleccionado.mayoriaAbsoluta.ToString();
            }

            if (partidosIzq.Count() != 0)
            {
                contEscanios = 0;
                foreach (Partido partido in partidosIzq)
                {
                    contEscanios = contEscanios + partido.scanios;
                }

                textoMayoria.Text = contEscanios.ToString() + " / " + p.mayoriaAbsoluta.ToString();
            }

         /*
         //Posicionar lado izquierdo y derecho --> Posicion centrada lado izquiedo y posicion centrada lado derecho
         double posIzq = ((canvasPactometro.ActualWidth/2)/2);
         double posDer = (canvasPactometro.ActualWidth / 2) + posIzq;


         //Coger Height del Canvas, para calcular
         double valorHeight = canvasPactometro.ActualHeight-(canvasPactometro.ActualHeight*0.1);
         double tamanioPorEscanio = valorHeight / p.numeroDeEscanios;
         double comienzoProxRectangulo = canvasPactometro.ActualHeight*0.05;
         double anchoRectangulo = canvasPactometro.ActualWidth * 0.25;

         //Dibujar Linea para si el pacto llega a mayoria absoluta o no
         double alturaLinea = tamanioPorEscanio * (p.mayoriaAbsoluta-1) + canvasPactometro.ActualHeight*0.05;
         introducirLinea(alturaLinea);
         double tamanioPartido;
         */

            //foreach (Partido partido in partidosIzq)
            //{
            //Tamaño de cada rectangulo, lo añadimos
            //tamanioPartido = tamanioPorEscanio * partido.scanios;
            //agregarRectanguloPact(posIzq, comienzoProxRectangulo, anchoRectangulo, tamanioPartido, partido.color, partido.scanios, partido, partidosIzq, partidosDer);
            //comienzoProxRectangulo += tamanioPartido;

            /*
            if(partidosDer != null)
            {
                comienzoProxRectangulo = canvasPactometro.ActualHeight * 0.05;
                foreach (Partido partido in partidosDer)
                {
                    tamanioPartido = tamanioPorEscanio * partido.scanios;
                    agregarRectanguloPact(posDer, comienzoProxRectangulo, anchoRectangulo, tamanioPartido, partido.color, partido.scanios, partido, partidosIzq, partidosDer);
                    comienzoProxRectangulo += tamanioPartido;
            }

            }*/


            actualizarGraficaPactometro(partidosIzq, partidosDer);
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
            int contEscanios=0;
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

            //Contamos escaños de la lista derecha y lo ponemos en el textBlock
            foreach(Partido partido in listaDer)
            {
                contEscanios = contEscanios + partido.scanios;
            }
            textoMayoria.Text = contEscanios.ToString() + " / " + procesoSeleccionado.mayoriaAbsoluta.ToString();
            contEscanios = 0;
            foreach (Partido partido in listaIzq)
            {
                contEscanios = contEscanios + partido.scanios;
            }

            textoMayoria2.Text = contEscanios.ToString() + " / " + procesoSeleccionado.mayoriaAbsoluta.ToString();
            actualizarGraficaPactometro(listaIzq, listaDer);

        }


        private void actualizarGraficaPactometro(List<Partido> partidosIzq, List<Partido> partidosDer)
        {

            canvasPactometro.Children.Clear();

            //Ordenamos listas para mejor visualización
            if(partidosIzq.Count() > 0)
            {
                partidosIzq = partidosIzq.OrderByDescending(x => x.scanios).ToList();
            }
            
            if(partidosDer.Count() > 0)
            {
                partidosDer = partidosDer.OrderByDescending(x => x.scanios).ToList();
            }

           


            //Posicionar lado izquierdo y derecho --> Posicion centrada lado izquiedo y posicion centrada lado derecho
            double posIzq = ((canvasPactometro.ActualWidth / 2) / 2);
            double posDer = (canvasPactometro.ActualWidth / 2) + posIzq;

            

            //Coger Height del Canvas, para calcular
            double valorHeight = canvasPactometro.ActualHeight - (canvasPactometro.ActualHeight * 0.1);
            double tamanioPorEscanio = valorHeight / procesoSeleccionado.numeroDeEscanios;
            double comienzoProxRectanguloIzq = canvasPactometro.ActualHeight * 0.05;
            double comienzoProxRectanguloDer = canvasPactometro.ActualHeight * 0.05;
            double anchoRectangulo = canvasPactometro.ActualWidth * 0.25;
            
            //Dibujar Linea para si el pacto llega a mayoria absoluta o no
            //Esto se hace pq en vez que de empezar por arriba las lineas, se empiezan por arriba, lo que hace que, si quiero empezar por abajo, tenga que poner la altura con la mayoria absoluta - 1, ya que en la parte de arriba, habra 40
            // y en la parte de abajo 41, que  es lo que exactamente quiero
            double alturaLinea = tamanioPorEscanio * procesoSeleccionado.mayoriaAbsoluta-1 + canvasPactometro.ActualHeight * 0.05; 
            introducirLinea(alturaLinea);
            if(partidosIzq != null)
            {
                foreach (Partido partido in partidosIzq)
            {
                //Tamaño de cada rectangulo, lo añadimos
                double tamanioPartido = tamanioPorEscanio * partido.scanios;
                agregarRectanguloPact(posIzq, comienzoProxRectanguloIzq, anchoRectangulo, tamanioPartido, partido.color, partido.scanios, partido, partidosIzq, partidosDer);
                comienzoProxRectanguloIzq += tamanioPartido;
            }
            }
            if(partidosDer.Count() > 0)
            {
                foreach (Partido partido in partidosDer)
                {
                    //Tamaño de cada rectangulo, lo añadimos
                    double tamanioPartido = tamanioPorEscanio * partido.scanios;
                    agregarRectanguloPact(posDer, comienzoProxRectanguloDer, anchoRectangulo, tamanioPartido, partido.color, partido.scanios, partido, partidosIzq, partidosDer);
                    comienzoProxRectanguloDer += tamanioPartido;
                }
            }

            this.partidosDer = partidosDer;
            this.partidosIzq = partidosIzq;

            comp = 0;
        }



        private void Wsec_closed(object sender, EventArgs e)
        {
            wsec = null;
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
