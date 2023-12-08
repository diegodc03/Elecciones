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
            //Se ira haciendo mas grande conforme el bucle avance, si es 2 o mayor, si se crea implica que seria uno que no existe
            int escaniosMaximos = 0;
            //Bucle de todos los procesos para meterlos en el diccionario
            Dictionary<string, string> dicComprobacionColores = new Dictionary<string, string>();

            foreach(ProcesoElectoral proceso in procesos)
            {
                contadorDeProceso = contadorDeProceso + 1;
                //Lista con todos los partidos politicos del proceso electoral
                listaPartidosPoliticos = new List<Partido>(proceso.Partidos);

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

            //Calcular el ancho total para cada rectangulo y le quitamos el espacio entre los elementos
            double espacioNumPartidosTotal = dicPartidos.Count * contadorDeProceso;
            var val = canvasGrafica.ActualWidth;
            double anchoTotal = canvasGrafica.ActualWidth - ((espacioNumPartidosTotal+1) * espacioEntreRectangulos+15);

            //Numero de rectangulos
            double totalRectangulos = dicPartidos.Count * contadorDeProceso;

            //Tamaño para cada rectangulo
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
                    //Buscamos el Partido Politico que tenga el numero de proseo electoral actual
                    //FirstOrDefault busca el primer elemento que le indicamos, en este caso buscara el primer elemento que tenga i es decir, el proceso electoral Buscado
                    Partido partidoActual = listaPartido.FirstOrDefault(p => p.numProceso == i);
                    
                    //Escribimos una unica vez el nombre del partido politico y que se pueda poner en todo el ancho
                    if (flag == true)
                    {

                        double anchoEtiqueta = anchoRectangulo * 2 + espacioEntreRectangulos;
                        agregarEtiqueta(listaPartido[0].nombrePartido, anchoEtiqueta, left, bottom);
                        flag = false;
                    }


                    // Verificar si se encontró un partido para el número de proceso actual
                    if (partidoActual != null)
                    {
                        Console.WriteLine(partidoActual.nombrePartido);
                        double alturaRectangulo = (partidoActual.scanios * canvasGrafica.ActualHeight) / maxAltura;
                        int contNumProcesos = procesos.Count();
                        agregarRectangulo(left, bottom, anchoRectangulo, alturaRectangulo, partidoActual.color, partidoActual.nombrePartido, partidoActual.scanios, partidoActual.numProceso-1, contNumProcesos);
                        left = left + anchoRectangulo + espacioEntreRectangulos;
                    }
                    else
                    {
                        // Si no se encuentra un partido para el número de proceso actual, agregar espacio en blanco
                        Console.WriteLine("Espacio en blanco"); // Puedes ajustar esto según tus necesidades
                        left = left + anchoRectangulo + espacioEntreRectangulos;
                    }

                }
            }

            //Añadimos la linea que cuenta los partidos
            //Metemos las lineas de la iquierda del canvas para que podamos ver el numero de escaños que ha sacado cada partido
            // maxEscanios tiene ek numero maximo y le he añadido  10 para que se vea mejor en la pantalla --> esta guardado en la variable maxAltura
            //Pondremos cada 20 escaños para que se pueda ver bien los valores
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

                    // Agregar un TextBlock con el valor de 'i' junto a la línea
                    TextBlock texto = new TextBlock();
                    texto.Text = i.ToString();
                    Canvas.SetLeft(texto, 7); // Ajusta la posición horizontal según tus necesidades
                    Canvas.SetTop(texto, canvasGrafica.ActualHeight - valorCanvas - bottom); // Ajusta la posición vertical según tus necesidades
                    double factorEscala = 0.8;
                    texto.FontSize = factorEscala * texto.FontSize;
                    canvasGrafica.Children.Add(texto);
                }
            }

        }

        


        




        private void agregarRectangulo(double left, double bottom, double width, double height, string colorHex, string etiqueta, int escanios, int numProceso, int contNumProcesos)
        {
            Rectangle rectangulo = new System.Windows.Shapes.Rectangle();

            rectangulo.Width = width;
            rectangulo.Height = height;

            Color clr = (Color)ColorConverter.ConvertFromString(colorHex);
            //Si el numero de proceso es 1, entonces es el principal, le he restado uno para que asi el segundo proceso sea 1 y para multiplicar por el factor  de conversion sea mas facil
            //Como es 0 el numero de proceso 1 y no se tiene que cambiar, da igual
            if (numProceso >= 1)
            {
                double factorDeReduccion = 0.2; // Este factor determina cuánto se reduce la opacidad por cada proceso
                double val = 1.0 - (numProceso * factorDeReduccion);
                
                int alphaValor = 255 - (int)(val * 120);
                alphaValor = Math.Max(0, Math.Min(255, alphaValor)); // Limitar entre 0 y 255

                clr = Color.FromArgb((byte)alphaValor, clr.R, clr.G, clr.B);
                //Creo un nuevo color, este tendra la intensidad reducida dependiendo el numero de proceso que tenga
                //clr = (Color)Color.FromArgb((byte(255 - (int)(val * 255))), clr.R, clr.G, clr.B);
            }
            

            SolidColorBrush brocha = new SolidColorBrush(clr);

            rectangulo.Fill = brocha;
            
            Canvas.SetLeft(rectangulo, left);
            Canvas.SetBottom(rectangulo, bottom);

            rectangulo.ToolTip = new ToolTip { Content = escanios + " escaños" };

            canvasGrafica.Children.Add(rectangulo);

        }


        private void agregarEtiqueta(string nombrePartido, double anchoRectangulo, double left, double bottom)
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
