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
      

    public partial class MainWindow : Window
    {
        ObservableCollection<ProcesoElectoral> listaProcesos = new ObservableCollection<ProcesoElectoral>();
        ProcesoElectoral aux = new ProcesoElectoral();

        VentanaSecundaria wsec = null;

        ProcesoElectoral procesoSeleccionado = new ProcesoElectoral();
        int comp = 1;
        int flagEliminado = 0;

        //Lista partidos divididios en dos para la grafica pactometro
        List<Partido> partidosIzq = new List<Partido>();
        List<Partido> partidosDer = new List<Partido>();


        public MainWindow()
        {
            InitializeComponent();

            //Lectura de Fichero con los procesos Electorales
            LecturaDeFicheroProcesosElectorales lectura = new LecturaDeFicheroProcesosElectorales();
            listaProcesos = lectura.leerCSProcesos("partidosAlPrincipio.csv");

        }


        private void CanvasUnit_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (canvasUnitaria.IsEnabled && e.NewSize.Width > 0 && procesoSeleccionado.Partidos != null)
            {
                if (canvasUnitaria.ActualHeight > 200 && canvasUnitaria.ActualWidth > 300)
                {
                    if(flagEliminado == 0)
                    {
                        canvasUnitaria.Children.Clear();
                        GraficoUnitario graficoUnitario = new GraficoUnitario(canvasUnitaria);
                        NombreEleccionLabelGraficaUnitaria.Text = procesoSeleccionado.nombreProcesoElectoral;
                        graficoUnitario.MostrarGrafico(procesoSeleccionado);
                    }
                }
                else
                {
                    String mensajePorPantalla = "No se puede hacer tan pequeño";
                    MessageBox.Show(mensajePorPantalla);
                }

            }
        }

        private void CanvasComp_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(canvasComparativa.IsEnabled && e.NewSize.Width > 0 && procesoSeleccionado.Partidos != null)
            {
                if(canvasComparativa.ActualHeight > 200 && canvasComparativa.ActualWidth > 300)
                {
                    if(flagEliminado == 0)
                    {
                        // Añadimos Grafica Comparatoria a su canvas
                        canvasComparativa.Children.Clear();

                        ProcesoElectoral p = procesoSeleccionado as ProcesoElectoral;

                        List<ProcesoElectoral> aniadirGrafica = new List<ProcesoElectoral>();
                        aniadirGrafica.Add(p);
                        NombreEleccionLabelGraficComparativo.Text = p.nombreProcesoElectoral;
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
                   
                }
                else
                {
                    String mensajePorPantalla = "No se puede hacer tan pequeño";
                    MessageBox.Show(mensajePorPantalla);
                }
                
                

            }
        }


        private void CanvasPactometro_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(canvasPactometro.IsEnabled && canvasPactometro.ActualWidth > 0 )
            {
                if (canvasPactometro.ActualHeight > 250 && canvasPactometro.ActualWidth > 300 )
                {
                    if(partidosDer.Count() == 0 && partidosIzq.Count() == 0)
                    {
                        if(flagEliminado == 0)
                        {
                            //Esto ocurre la primera vez que lo llame
                            canvasPactometro.Children.Clear();
                            ProcesoElectoral p = procesoSeleccionado as ProcesoElectoral;
                            NombreEleccionLabelGraficaPactometro.Text = procesoSeleccionado.nombreProcesoElectoral;
                            if (p.Partidos != null)
                            {
                                GraficaPactometro(p);
                            }
                        }	
                    }
                    else
                    {
                        //Cuando lo vuelva a llamar se que o una lista u otra o las dos tendran partiudos, por lo que quiero es que se vuelvan a escribir
                        ActualizarGraficaPactometro(partidosIzq, partidosDer);
                    }
                }
                else
                {
                    String mensajePorPantalla = "No se puede hacer tan pequeño";
                    MessageBox.Show(mensajePorPantalla);
                }
            }
        }



        private void MenuConfig_Click(object sender, EventArgs e)
        {   
            if(wsec == null)
            {
                if(listaProcesos != null)
                    wsec = new VentanaSecundaria(this.listaProcesos);
            }
            
            if(listaProcesos != null)
            {
                wsec.DataGridPartidosPoliticos.ItemsSource = null;
                wsec.DataGridPartidosPoliticos.ItemsSource = listaProcesos;
            }
            
            wsec.Owner = this;

            //Nos suscribimos al actualizador de grafica
            wsec.ItemChanged += Wsec_actualizarGrafica;
            //Nos suscribimos al limpiador de canvas
            wsec.LimpiarCanvas += limpiarCanvas;
           
            
            wsec.Title = "Configuración";
            wsec.Closed += Wsec_Closed;
            wsec.Show();

        }


        private void limpiarCanvas(object sender, EventArgs e)
        {
            flagEliminado = 1;
            canvasComparativa.Children.Clear();
            canvasPactometro.Children.Clear();
            canvasUnitaria.Children.Clear();
            textoMayoria.Text = " ";
            textoMayoria2.Text = " ";
            NombreEleccionLabelGraficaUnitaria.Text = "";
            NombreEleccionLabelGraficComparativo.Text = "";
            NombreEleccionLabelGraficaPactometro.Text = "";
        }


         
        private void Wsec_Closed(Object sender, EventArgs e)
        {
            wsec = null;
        }


        private void Wsec_actualizarGrafica(object sender, ItemEventArgs e)
        {
            comp = 1;
            flagEliminado = 0;

            ProcesoElectoral proceso = new ProcesoElectoral();

            ItemEventArgs procesoDevuelto = e as ItemEventArgs;

            procesoSeleccionado = procesoDevuelto.procesoElectoral;

            if(procesoDevuelto != null)
            {
               proceso = procesoDevuelto.procesoElectoral;
            }

            NombreEleccionLabelGraficaUnitaria.Text = e.procesoElectoral.nombreProcesoElectoral;
            NombreEleccionLabelGraficComparativo.Text = e.procesoElectoral.nombreProcesoElectoral;
            NombreEleccionLabelGraficaPactometro.Text = e.procesoElectoral.nombreProcesoElectoral;

            if (proceso != null)
            {
                // Añadimos grafica Unitaria a su canvas
                if(canvasUnitaria.ActualWidth > 0)
                {
                    canvasUnitaria.Children.Clear();
                    GraficoUnitario graficoUnitario = new GraficoUnitario(canvasUnitaria);
                    graficoUnitario.MostrarGrafico(e.procesoElectoral);
                    
                }

                if( canvasComparativa.ActualWidth > 0)
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


                if( canvasPactometro.ActualWidth > 0 && e.procesoElectoral.Partidos != null)
                {
                    //Hacemos la grafica
                    canvasPactometro.Children.Clear();

                    partidosIzq = new List<Partido>();
                    partidosDer = new List<Partido>();

                    textoMayoria2.Text = "";
                    textoMayoria.Text = "";

                    comp = 1;
                    ProcesoElectoral p = new ProcesoElectoral();
                    p = e.procesoElectoral;

                    GraficaPactometro(p);
                    
                                        
                }
            }
        }

        private void GraficaPactometro(ProcesoElectoral p)
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

                textoMayoria2.Text = contEscanios.ToString() + " / " + p.mayoriaAbsoluta.ToString();
            }

            ActualizarGraficaPactometro(partidosIzq, partidosDer);
        }


        private void ActualizarGraficaPactometro(List<Partido> partidosIzq, List<Partido> partidosDer)
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

            double posIzq = ((canvasPactometro.ActualWidth / 2) / 2);
            double posDer = (canvasPactometro.ActualWidth / 2) + posIzq;


            double valorHeight = canvasPactometro.ActualHeight - (canvasPactometro.ActualHeight * 0.1);
            double tamanioPorEscanio = valorHeight / procesoSeleccionado.numeroDeEscanios;
            double comienzoProxRectanguloIzq = canvasPactometro.ActualHeight * 0.05;
            double comienzoProxRectanguloDer = canvasPactometro.ActualHeight * 0.05;
            double anchoRectangulo = canvasPactometro.ActualWidth * 0.25;
            
            
            double alturaLinea = tamanioPorEscanio * procesoSeleccionado.mayoriaAbsoluta-1 + canvasPactometro.ActualHeight * 0.05; 
            IntroducirLinea(alturaLinea);


            if(partidosIzq != null)
            {
                foreach (Partido partido in partidosIzq)
            {
                //Tamaño de cada rectangulo, lo añadimos
                double tamanioPartido = tamanioPorEscanio * partido.scanios;
                AgregarRectanguloPact(posIzq, comienzoProxRectanguloIzq, anchoRectangulo, tamanioPartido, partido, partidosIzq, partidosDer);
                comienzoProxRectanguloIzq += tamanioPartido;
            }
            }
            if(partidosDer.Count() > 0)
            {
                foreach (Partido partido in partidosDer)
                {
                    //Tamaño de cada rectangulo, lo añadimos
                    double tamanioPartido = tamanioPorEscanio * partido.scanios;
                    AgregarRectanguloPact(posDer, comienzoProxRectanguloDer, anchoRectangulo, tamanioPartido, partido, partidosIzq, partidosDer);
                    comienzoProxRectanguloDer += tamanioPartido;
                }
            }

            this.partidosDer = partidosDer;
            this.partidosIzq = partidosIzq;

            comp = 0;
        }


        private void AgregarRectanguloPact(double left, double bottom, double width, double height,  Partido p, List<Partido> listaIzq, List<Partido> listaDer)
        {

            Rectangle rectangulo = new System.Windows.Shapes.Rectangle();

            //Asigno un evento a el rectangulo, en el cual, cuando se pulse el click derecho del raton se haga, eso implica que seria como un click
            rectangulo.MouseLeftButtonDown += (sender, e) => CambiarRectanguloPosicion_Click(sender, e, p, listaIzq, listaDer);

            rectangulo.Width = width;
            rectangulo.Height = height;

            Color clr = (Color)ColorConverter.ConvertFromString(p.color);
            SolidColorBrush brocha = new SolidColorBrush(clr);

            rectangulo.Fill = brocha;

            double movimiento = width / 2;
            Canvas.SetLeft(rectangulo, left-movimiento);

            Canvas.SetBottom(rectangulo, bottom);

            //Agrego ToolTip al rectangulo para que cuando pase por el van los escaños
            rectangulo.ToolTip = new ToolTip { Content =p.nombrePartido+" "+ p.scanios + " escaños" };

            canvasPactometro.Children.Add(rectangulo);


        }

        private void IntroducirLinea(double altura)
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


        private void CambiarRectanguloPosicion_Click(Object sender, MouseButtonEventArgs e, Partido p, List<Partido> listaIzq, List<Partido> listaDer)
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
            ActualizarGraficaPactometro(listaIzq, listaDer);

        }



    }
}
