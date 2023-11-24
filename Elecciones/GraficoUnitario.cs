using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Elecciones
{
    internal class GraficoUnitario
    {
        private Canvas canvasGrafica;
       

        public GraficoUnitario(Canvas canvas) {
            this.canvasGrafica = canvas;
        }

        public void MostrarGrafico(ProcesoElectoral proceso)
        {

            double espacioEntreRectangulos = 10;
            double porcentajeAltura = 0.8;

            //Calcular el ancho total para cada rectangulo y le quitamos el ancho de cada lado
            double anchoTotal = canvasGrafica.ActualWidth - ((proceso.Partidos.Count+1) * espacioEntreRectangulos);

            //Tamaño para cada rectangulo
            double anchoRectangulo = anchoTotal / proceso.Partidos.Count;

            // Encontrar la cantidad máxima de escaños entre los partidos
            double maxEscanios = proceso.Partidos.Max(p => p.scanios);
            
            double maxAltura = maxEscanios + 30;

            //Dejamos espacio entre el canvas para hacerlo mas "bonito"
            //Donde lo vamos a colocar en el canvas
            double leftInicio = espacioEntreRectangulos;
            double left = leftInicio;
            double bottom = 30;




            foreach (Partido partido in proceso.Partidos)
            {
                double alturaRectangulo = (partido.scanios * canvasGrafica.ActualHeight ) / maxAltura;

                agregarRectangulo(left, bottom, anchoRectangulo, alturaRectangulo, partido.color, partido.nombrePartido, partido.scanios);

                agregarEtiqueta(partido.nombrePartido, anchoRectangulo, left, bottom);

                left += anchoRectangulo + espacioEntreRectangulos;
            }


            //Metemos las lineas de la iquierda del canvas para que podamos ver el numero de escaños que ha sacado cada partido
            // maxEscanios tiene ek numero maximo y le he añadido  10 para que se vea mejor en la pantalla --> esta guardado en la variable maxAltura
            //Pondremos cada 20 escaños para que se pueda ver bien los valores
            for (int i = 0; i <= maxAltura; i = i + 20)
            {
                
                
                Line linea = new Line();
                double valorCanvas = (i * canvasGrafica.ActualHeight ) / maxAltura;


                if (valorCanvas+30 < canvasGrafica.ActualHeight)
                {
                    linea.Stroke = Brushes.Red;
                    linea.X1 = 0;
                    linea.X2 = 10;
                    linea.Y1 = canvasGrafica.ActualHeight - valorCanvas-bottom;
                    linea.Y2 = canvasGrafica.ActualHeight - valorCanvas-bottom;

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

        private void agregarRectangulo(double left, double bottom, double width, double height, string colorHex, string etiqueta, int escanios)
        {
            
            Rectangle rectangulo = new System.Windows.Shapes.Rectangle();
            
            rectangulo.Width = width;
            rectangulo.Height = height;
            
            Color clr = (Color) ColorConverter.ConvertFromString(colorHex);
            SolidColorBrush brocha = new SolidColorBrush(clr);

            rectangulo.Fill = brocha;
            //rectangulo.Stroke = brocha; rectangulo.StrokeThickness = 1;
            Canvas.SetLeft(rectangulo, left);
            
            Canvas.SetBottom(rectangulo, bottom);

            //Agrego ToolTip al rectangulo para que cuando pase por el van los escaños
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
