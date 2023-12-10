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

            double espacioEntreRectangulos = canvasGrafica.ActualWidth * 0.02;

            double anchoTotal = canvasGrafica.ActualWidth - ((proceso.Partidos.Count+1) * espacioEntreRectangulos + 15);

            double anchoRectangulo = anchoTotal / proceso.Partidos.Count;

            double maxEscanios = proceso.Partidos.Max(p => p.scanios);
            double maxAltura = maxEscanios + maxEscanios * 0.25;

            double leftInicio = espacioEntreRectangulos + 15;
            double left = leftInicio;
            double bottom = 30;

            foreach (Partido partido in proceso.Partidos)
            {
                double alturaRectangulo = (partido.scanios * canvasGrafica.ActualHeight ) / maxAltura;

                AgregarRectangulo(left, bottom, anchoRectangulo, alturaRectangulo, partido.color, partido.nombrePartido, partido.scanios);

                AgregarEtiqueta(partido.nombrePartido, anchoRectangulo, left, bottom);

                left += anchoRectangulo + espacioEntreRectangulos;
            }


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

        private void AgregarRectangulo(double left, double bottom, double width, double height, string colorHex, string etiqueta, int escanios)
        {
            
            Rectangle rectangulo = new System.Windows.Shapes.Rectangle();
            
            rectangulo.Width = width;
            rectangulo.Height = height;
            
            Color clr = (Color) ColorConverter.ConvertFromString(colorHex);
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
