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
        List<Partido> partidos = new List<Partido>();
        List<String> partidosPers = new List<string>();
        PartidosPersistentes partidosPersistentes = new PartidosPersistentes();
        

        public AgregarProcesoElectoral()
        {
            InitializeComponent();
            
            partidosPers = partidosPersistentes.getPartidos();
            PartidoComboBox.ItemsSource = partidosPers;

        }


        private void AniadirPartidoPolitico_Click(object sender, RoutedEventArgs e)
        {
            String nombrePartido = PartidoComboBox.Text;
            String numEscanios = NumEscaniosPartido.Text;
            String colorPartido = ColorPartido.Text;
            
            int numScanios;
            
            

            //Comprobamos si usuario ha introducido los dos valores pedidos
            if(string.IsNullOrWhiteSpace(nombrePartido) || string.IsNullOrWhiteSpace(numEscanios ) || string.IsNullOrWhiteSpace(colorPartido))
            {
                String mensajePorPantalla = "No ha introducido alguno de los tres campos";
                MessageBox.Show(mensajePorPantalla);
            }
            else
            {
                //Valores introducidos, comprobamos que el segundo valor es un Int32
                if(int.TryParse(numEscanios,out numScanios)){
                    
                    //Devuelve o una instancia de Partidos o null
                    if(partidos.Find(x => x.nombrePartido.Contains(nombrePartido))== null){
                        
                        numScanios = Int32.Parse(numEscanios);
                        nombrePartido = nombrePartido.ToUpper();
                        int indiceFila = DataGridPartidos.SelectedIndex;

                        //Se crea Instancia y se introduce en ListaDePartidos
                        Partido partidoPolitico = new Partido(nombrePartido, numScanios, colorPartido);
                        partidos.Add(partidoPolitico);



                        //Añadir a ListaPartidosPersistentes si no existe todavia
                        if (!partidosPers.Contains(nombrePartido))
                        {
                            partidosPersistentes.AgregarPartido(partidoPolitico.nombrePartido);

                            //Actualizar ComboBox
                            partidosPers = partidosPersistentes.getPartidos();
                            PartidoComboBox.ItemsSource = null;
                            PartidoComboBox.ItemsSource = partidosPers;
                        }


                        //Añadimos al dataGrid
                        var nuevoPartidoData = new { PARTIDO = nombrePartido, ESCAÑOS = numScanios };
                        //DataGridPartidos.ItemsSource = null;
                        //DataGridPartidos.ItemsSource = partidos;
                        DataGridPartidos.Items.Add(nuevoPartidoData);

                    }

                }
                else
                {
                    //Sacamos error por pantalla
                    String mensajePorPantalla = "Introduzca el valor de los escaños otra vez, valor incorrecto";
                    MessageBox.Show(mensajePorPantalla);
                }
                
            }
            

        }




        private void AniadirProcesoElectoral_Click(object sender, RoutedEventArgs e)
        {
            //Aqui tengo que añadir todo o así


        }

        private void nombreEleccion_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
