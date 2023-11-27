using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

            string numeroScaniosTotal = NumEscaniosTotal.Text;
            if(string.IsNullOrWhiteSpace(numeroScaniosTotal))
            {
                String mensajePorPantalla = "Primero completa los datos del proceso";
                MessageBox.Show(mensajePorPantalla);
            }
            else
            {


                //Comprobamos si usuario ha introducido los dos valores pedidos
                if (string.IsNullOrWhiteSpace(nombrePartido) || string.IsNullOrWhiteSpace(numEscanios) || string.IsNullOrWhiteSpace(colorPartido))
                {
                    String mensajePorPantalla = "No ha introducido alguno de los tres campos";
                    MessageBox.Show(mensajePorPantalla);
                }
                else
                {
                    //Valores introducidos, comprobamos que el segundo valor es un Int32
                    if (int.TryParse(numEscanios, out numScanios))
                    {

                        //Devuelve o una instancia de Partidos o null
                        if (partidos.Find(x => x.nombrePartido.Contains(nombrePartido)) == null)
                        {

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


                            //Añadimos al dataGrid //AQUI PROBLEMAS
                            var nuevoPartidoData = new { PARTIDO = nombrePartido, ESCAÑOS = numScanios };
                            //DataGridPartidos.ItemsSource = nuevoPartidoData;
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
        }




        private void AniadirProcesoElectoral_Click(object sender, RoutedEventArgs e)
        {
            string nombreProceso = nombreEleccion.Text;
            string mayoriaAbsoluta = MayoriaAbsoluta.Text;
            string numeroDeEscaniosTotal = NumEscaniosTotal.Text;
            DateTime? fechaSeleccionada = FechaEleccion.SelectedDate;

            int mayoriaEscanios;
            int totalEscanios;

            if (string.IsNullOrWhiteSpace(nombreProceso) || string.IsNullOrWhiteSpace(numeroDeEscaniosTotal) || fechaSeleccionada == null || string.IsNullOrWhiteSpace(mayoriaAbsoluta))
            {
                String mensajePorPantalla = "No ha introducido alguno de los cuatro campos";
                MessageBox.Show(mensajePorPantalla);
            }
            else
            {
                if (int.TryParse(numeroDeEscaniosTotal, out totalEscanios ) == false || (int.TryParse(mayoriaAbsoluta, out mayoriaEscanios) == false)){
                    String mensajePorPantalla = "Ha introducido el numero de escaños Incorrectamente";
                    MessageBox.Show(mensajePorPantalla);
                }
                else
                {
                    //Aqui significa que todo ha ido bien



                }
            }
        }

        private void nombreEleccion_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void NumEscaniosTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            string numero = NumEscaniosTotal.Text;
            int numeroInt;
            int numeroMayoriaAbsoluta;

            if (int.TryParse(numero, out numeroInt))
            {
                numeroMayoriaAbsoluta = numeroInt / 2 + 1;
                MayoriaAbsoluta.Text = numeroMayoriaAbsoluta.ToString();
            }

        }

        private void MayoriaAbsoluta_TextChanged(object sender, TextChangedEventArgs e)
        {
            string numero = MayoriaAbsoluta.Text;
            int numeroInt;
            int numeroTotal;

            if (int.TryParse(numero, out numeroInt))
            {
                numeroTotal = numeroInt * 2 - 1;
                NumEscaniosTotal.Text = numeroTotal.ToString();
            }
        }
    }
}
