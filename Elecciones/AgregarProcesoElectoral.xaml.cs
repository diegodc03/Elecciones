using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Elecciones
{
    /// <summary>
    /// Lógica de interacción para AgregarProcesoElectoral.xaml
    /// </summary>
    public partial class AgregarProcesoElectoral : Window
    {
        public AgregarProcesoElectoral()
        {
            InitializeComponent();
        }


        private void AniadirPartidoPolitico_Click(object sender, RoutedEventArgs e)
        {
            String nombrePartido = PartidoPolitico.Text;
            String numEscanios = NumEscaniosPartido.Text;
            int numScanios;
            
            //Comprobamos si usuario ha introducido los dos valores pedidos
            if(string.IsNullOrWhiteSpace(nombrePartido) || string.IsNullOrWhiteSpace(numEscanios))
            {
                String mensajePorPantalla = "No ha introducido uno de los dos campos";
                MessageBox.Show(mensajePorPantalla);
            }
            else
            {
                //Valores introducidos, comprobamos que el segundo valor es un Int32
                try
                {
                    numScanios = Int32.Parse(numEscanios);
                }
                catch (FormatException)
                {
                    //Sacamos error por pantalla
                    String mensajePorPantalla = "Introduzca el valor de los escaños otra vez, valor incorrecto";
                    MessageBox.Show(mensajePorPantalla);
                }

                //Si salimos del tryCatch implica que tenemos que podemos añadir a la lista el partido


            }
            
            

            
        
        }

        private void AniadirProcesoElectoral_Click(object sender, RoutedEventArgs e)
        {
            //Aqui tengo que añadir todo o así


        }

    }
}
