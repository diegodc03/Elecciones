using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
//using NumeroProcesoElectoral;

namespace Elecciones
{
    internal class GraficoComparatorioEntreElecciones
    {

        private Canvas canvasGrafica;
        public GraficoComparatorioEntreElecciones(Canvas canvas) 
        {
            this.canvasGrafica = canvas;
        }


        public void CrearListaParaModificarColores(List<ProcesoElectoral> procesos)
        {
            List<ProcesoElectoral> procesosModificados = new List<ProcesoElectoral>();

            foreach(ProcesoElectoral proceso in procesos)
            {
                ProcesoElectoral procesoCopia = new ProcesoElectoral
                {
                    nombreProcesoElectoral = proceso.nombreProcesoElectoral,
                    fechaProcesoElectoral = proceso.fechaProcesoElectoral,
                    numeroDeEscanios = proceso.numeroDeEscanios,
                    mayoriaAbsoluta = proceso.mayoriaAbsoluta,
                    Partidos = new ObservableCollection<Partido>()
                };

                foreach (Partido partido in proceso.Partidos)
                {
                    // Crear una copia del partido con un color modificado
                    Partido partidoCopia = new Partido
                    {
                        nombrePartido = partido.nombrePartido,
                        scanios = partido.scanios,
                        color = partido.color
                    };

                    procesoCopia.Partidos.Add(partidoCopia);
                }

                procesosModificados.Add(procesoCopia);
            }

        }


        public void MostrarGrafico(List<ProcesoElectoral> procesos)
        {

            double espacioEntreRectangulos = canvasGrafica.ActualWidth * 0.01;
            double maxEscanios = 0;

            //Aqui comprobamos quien es mayor para luego hacer la altura
            foreach(ProcesoElectoral proc in procesos)
            {
                
                if( maxEscanios < proc.Partidos.Max(p => p.scanios))
                {
                    maxEscanios = proc.Partidos.Max(p => p.scanios);
                }
            }

            
            // Crear un diccionario con clave de tipo String y valor de tipo List tipo int para guardar los escaños
            Dictionary<string, List<Partido>> dicPartidos = new Dictionary<string, List<Partido>>();

            
            List<Partido> listaPartidosPoliticos = new List<Partido>();
           
            int contadorDeProceso = 0;
          
            int escaniosMaximos = 0;
        
            Dictionary<string, string> dicComprobacionColores = new Dictionary<string, string>();

            foreach(ProcesoElectoral proceso in procesos)
            {
                contadorDeProceso = contadorDeProceso + 1;
                //Lista con todos los partidos politicos del proceso electoral
                listaPartidosPoliticos = new List<Partido>();
              
                //Creo una lista diferente para asi no tengan referencia los partidos con la lista que paso a mostrarGrafico, ya que se pasa por refrencia
                foreach (Partido partido in proceso.Partidos)
                {
                    Partido partidoCopia = new Partido
                    {
                        nombrePartido = partido.nombrePartido,
                        scanios = partido.scanios,
                        color = partido.color
                    };
                    listaPartidosPoliticos.Add(partidoCopia);
                }


                //Recorremos todos los valores de la lista metiendolos en el diccionario
                foreach(Partido partido in listaPartidosPoliticos)
                {
                    if (dicComprobacionColores.ContainsKey(partido.nombrePartido) == true)
                    {
                        string color = dicComprobacionColores[partido.nombrePartido];
                        partido.color = color;
                    }
                    else
                    {
                        dicComprobacionColores.Add(partido.nombrePartido, partido.color);

                    }
                    //////////
                    if (dicPartidos.ContainsKey(partido.nombrePartido))
                    {
                        // La clave existe en el diccionario
                        //Añado a la lista del diccionario
                        
                        partido.numProceso = contadorDeProceso;
                        if (escaniosMaximos < partido.scanios)
                        {
                            escaniosMaximos = partido.scanios;
                        }
                        
                        dicPartidos[partido.nombrePartido].Add(partido);
                    }
                    else
                    {
                        //La clave no está en el diccionario
                        //Creo una nueva clave en el diccionario
                        
                        partido.numProceso = contadorDeProceso;
                        if (escaniosMaximos < partido.scanios)
                        {
                            escaniosMaximos = partido.scanios;
                        }
                        dicPartidos.Add(partido.nombrePartido, new List<Partido> { partido });
                    }   
                }

            }
            
            double maxAltura = maxEscanios + maxEscanios * 0.25;

            double espacioNumPartidosTotal = dicPartidos.Count * contadorDeProceso;
            var val = canvasGrafica.ActualWidth;
            double anchoTotal = canvasGrafica.ActualWidth - ((espacioNumPartidosTotal+1) * espacioEntreRectangulos+15);


            double totalRectangulos = dicPartidos.Count * contadorDeProceso;

            double anchoRectangulo = anchoTotal / totalRectangulos;
            



            double leftInicio = espacioEntreRectangulos+15;
            double left = leftInicio;
            double bottom = 30;


            foreach (var par in dicPartidos)
            {
                List<Partido> listaPartido = par.Value.ToList();
                bool flag = true;
                for (int i = 1; i <= contadorDeProceso; i++)
                {
                    
                    Partido partidoActual = listaPartido.FirstOrDefault(p => p.numProceso == i);
                    
                    if (flag == true)
                    {
                        double anchoEtiqueta = anchoRectangulo * listaPartido.Count() + espacioEntreRectangulos;
                        AgregarEtiqueta(listaPartido[0].nombrePartido, anchoEtiqueta, left, bottom);
                        flag = false;
                    }

                    if (partidoActual != null)
                    {
                        Console.WriteLine(partidoActual.nombrePartido);
                        double alturaRectangulo = (partidoActual.scanios * canvasGrafica.ActualHeight) / maxAltura;
                        int contNumProcesos = procesos.Count();
                        AgregarRectangulo(left, bottom, anchoRectangulo, alturaRectangulo, partidoActual.color, partidoActual.nombrePartido, partidoActual.scanios, partidoActual.numProceso-1, contNumProcesos);
                        left = left + anchoRectangulo + espacioEntreRectangulos;
                    }
                    else
                    {
                        //Console.WriteLine("Espacio en blanco"); 
                        left = left + anchoRectangulo + espacioEntreRectangulos;
                    }

                }
            }

        
            for (int i = 0; i <= maxAltura; i = i + 20)
            {

                Line linea = new Line();
                double valorCanvas = (i * canvasGrafica.ActualHeight) / maxAltura;


                if (valorCanvas + 30 < canvasGrafica.ActualHeight)
                {
                    linea.Stroke = Brushes.Red;
                    linea.X1 = 0;
                    linea.X2 = 10;
                    linea.Y1 = canvasGrafica.ActualHeight - valorCanvas - bottom;
                    linea.Y2 = canvasGrafica.ActualHeight - valorCanvas - bottom;

                    canvasGrafica.Children.Add(linea);

                    TextBlock texto = new TextBlock();
                    texto.Text = i.ToString();
                    Canvas.SetLeft(texto, 7);
                    Canvas.SetTop(texto, canvasGrafica.ActualHeight - valorCanvas - bottom); 
                    double factorEscala = 0.8;
                    texto.FontSize = factorEscala * texto.FontSize;
                    canvasGrafica.Children.Add(texto);
                }
            }

        }

       


        private void AgregarRectangulo(double left, double bottom, double width, double height, string colorHex, string etiqueta, int escanios, int numProceso, int contNumProcesos)
        {
            Rectangle rectangulo = new System.Windows.Shapes.Rectangle();

            rectangulo.Width = width;
            rectangulo.Height = height;

            Color clr = (Color)ColorConverter.ConvertFromString(colorHex);
            
            if (numProceso >= 1)
            {
                double factorDeReduccion = 0.2; 
                double val = 1.0 - (numProceso * factorDeReduccion);

                clr = Color.FromArgb((byte)(255 - (80 *val)), clr.R, clr.G, clr.B);
                
            }
            
            SolidColorBrush brocha = new SolidColorBrush(clr);

            rectangulo.Fill = brocha;
            
            Canvas.SetLeft(rectangulo, left);
            Canvas.SetBottom(rectangulo, bottom);

            rectangulo.ToolTip = new ToolTip { Content = escanios + " escaños" };

            canvasGrafica.Children.Add(rectangulo);

        }


        private void AgregarEtiqueta(string nombrePartido, double anchoRectangulo, double left, double bottom)
        {
            TextBlock etiquetaText = new TextBlock();
            etiquetaText.Text = nombrePartido;
            etiquetaText.Foreground = Brushes.Black;
            etiquetaText.TextAlignment = TextAlignment.Left;
            etiquetaText.Width = anchoRectangulo;

            Canvas.SetLeft(etiquetaText, left);
            Canvas.SetBottom(etiquetaText, bottom - 15);

            canvasGrafica.Children.Add(etiquetaText);

        }
    }
}
