using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    
    public class ProcesosElectoralesEventArgs : EventArgs
    {
        public ObservableCollection<ProcesoElectoral> ProcesoElectorales { get; set; }

        public ProcesosElectoralesEventArgs(ObservableCollection<ProcesoElectoral> procesoElectorals)
        {
            ProcesoElectorales = procesoElectorals;
        }
    }


    /// <summary>
    /// Lógica de interacción para AgregarProcesoElectoral.xaml
    /// </summary>
    public partial class AgregarProcesoElectoral : Window
    {

        //Declaro evento 
        public event EventHandler<ProcesosElectoralesEventArgs> ProcesosElectoralesActualizados;

        List<Partido> partidos = new List<Partido>();
        ObservableCollection<ProcesoElectoral> listaProcesosElectorales;

        List<String> partidosPers = new List<string>();
        List<String> coloresPartidos = new List<string>();

        PartidosPersistentes partidosPersistentes = new PartidosPersistentes();
        LecturaDeFicheroTxt lecturaColores = new LecturaDeFicheroTxt();
        List<ProcesoElectoral> prueba = new List<ProcesoElectoral>();

        int sumatorioNumeroEscanios = 0;
        int indiceFila = 0;


        public AgregarProcesoElectoral(ObservableCollection<ProcesoElectoral> procesosActualizados)
        {
            InitializeComponent();
            
            //Se añade en la parte de los Partidos los elementos al COMBOBOX
            partidosPers = partidosPersistentes.getPartidos();
            PartidoComboBox.ItemsSource = partidosPers;

            coloresPartidos = lecturaColores.leerFichero();
            ColoresComboBox.ItemsSource = coloresPartidos;

            if(procesosActualizados != null)
            {
                listaProcesosElectorales = new ObservableCollection<ProcesoElectoral>();
                listaProcesosElectorales = procesosActualizados;
            }
            else
            {
                listaProcesosElectorales = new ObservableCollection<ProcesoElectoral>();
            }
            

            

        }


        private void AniadirPartidoPolitico_Click(object sender, RoutedEventArgs e)
        {
            String nombrePartido = PartidoComboBox.Text;
            String numEscanios = NumEscaniosPartido.Text;
            String colorPartido = ColoresComboBox.Text;
            
            int numScanios;
            

            int escaniosPartidosComprobante;
            

            string numeroScaniosTotal = NumEscaniosTotal.Text;
            if(string.IsNullOrWhiteSpace(nombrePartido) || string.IsNullOrWhiteSpace(numEscanios) || string.IsNullOrWhiteSpace(colorPartido))
            {
                String mensajePorPantalla = "No ha introducido alguno de los campos";
                MessageBox.Show(mensajePorPantalla);
            }
            else
            {


                //Comprobamos si usuario ha introducido los dos valores pedidos
                if (string.IsNullOrWhiteSpace(numeroScaniosTotal))
                {
                    String mensajePorPantalla = "Primero completa los datos del proceso";
                    MessageBox.Show(mensajePorPantalla);
                }
                else
                {
                    //Valores introducidos, comprobamos que el segundo valor es un Int32
                    if (int.TryParse(numEscanios, out numScanios))
                    {

                        if(numScanios > 0)
                        {

                            
                            sumatorioNumeroEscanios += numScanios;
                            int.TryParse(numeroScaniosTotal, out escaniosPartidosComprobante);
                            if (sumatorioNumeroEscanios <= escaniosPartidosComprobante)
                            {
                                //MessageBox.Show("La cosa esta yendo biennnnnnnn");
                                
                                //Devuelve o una instancia de Partidos o null
                                if (partidos.Find(x => x.nombrePartido.Contains(nombrePartido)) == null)
                                {

                                    
                                    nombrePartido = nombrePartido.ToUpper();
                                    indiceFila = DataGridPartidos.SelectedIndex;

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



                                    Partido partido = ProcesoElectoralFactory.CrearPartido(nombrePartido, numScanios, colorPartido);
                                    //DataGridPartidos.ItemsSource = nuevoPartidoData;
                                    DataGridPartidos.Items.Add(partido);
                                    indiceFila = DataGridPartidos.SelectedIndex;
                                    //Vuelvo a dejar los DATABOX y COMBOBOX en blanco
                                    PartidoComboBox.Text = "";
                                    NumEscaniosPartido.Text = "";
                                    ColoresComboBox.Text = "Introduce un Color";


                                }
                                else
                                {
                                    String mensajePorPantalla = "Ya existe ese partido político";
                                    MessageBox.Show(mensajePorPantalla);
                                }
                            }
                            else
                            {
                                String mensajePorPantalla = "Tiene que poner un valor menor que el número de escaños del proceso electoral";
                                MessageBox.Show(mensajePorPantalla);
                                sumatorioNumeroEscanios = sumatorioNumeroEscanios - numScanios;

                            }
                            
                        }
                        else
                        {
                            String mensajePorPantalla = "Ha introducido un Valor 0 o negativo";
                            MessageBox.Show(mensajePorPantalla);
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

        //public event EventHandler<ProcesoElectoralEvenArgs> ProcesoElectoralEven;


        private void AniadirProcesoElectoral_Click(object sender, RoutedEventArgs e)
        {
            string nombreProceso = nombreEleccion.Text;
            string mayoriaAbsoluta = MayoriaAbsoluta.Text;
            string numeroDeEscaniosTotal = NumEscaniosTotal.Text;
            DateTime fechaSeleccionada;
            
            int mayoriaEscanios;
            int totalEscanios;

            if (string.IsNullOrWhiteSpace(nombreProceso) || string.IsNullOrWhiteSpace(numeroDeEscaniosTotal) || string.IsNullOrWhiteSpace(mayoriaAbsoluta) || FechaEleccion.SelectedDate.HasValue == false)
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

                    //Compruebo que el numero de Escaños es igual, a la suma de los escaños de todos los partidos
                    if(sumatorioNumeroEscanios == totalEscanios)
                    {
                        //MessageBox.Show("La cosa ha ido bien");
                        //OrdenaLista
                        
                        //partidos.OrderByDescending(s => s.scanios);
                        List<Partido> auxiliar = new List<Partido>();
                        auxiliar = partidos.OrderByDescending(x => x.scanios).ToList();


                        //Aqui significa que todo ha ido bien
                        //Instncia de la clase procesoElectoral
                        fechaSeleccionada = FechaEleccion.SelectedDate.Value;
                        ProcesoElectoral procesoNuevo = new ProcesoElectoral(nombreProceso, fechaSeleccionada, totalEscanios, mayoriaEscanios, auxiliar);

                        //Se añade cada vez que se pulsa
                        //Se añade al observableCollection, haciendo que pueda luego pasar a la ventana secundaria
                        
                        listaProcesosElectorales.Add(procesoNuevo);

                        //Se reinicia todos los textBox para introducir mas elementos
                        nombreEleccion.Text = "";
                        MayoriaAbsoluta.Text = "";
                        NumEscaniosTotal.Text = "";


                        //Tengo que eliminar los partidos del datagrid
                        //Hay que pasar todos los elementos de los partidos a 0, para que así no interfieran al añadir otro procesoElectoral
                        DataGridPartidos.Items.Clear();
                        partidos = new List<Partido>();
                        sumatorioNumeroEscanios = 0;
                    }
                    else
                    {
                        String mensajePorPantalla = "Introduza el número de escaños total de los partidos igual que el numero total de escaños";
                        MessageBox.Show(mensajePorPantalla);
                    }
                    
  
                }
            }
        }
        


        //Dos metodos para conseguir que si cambia un elemento se cambie el otro, asi conseguir que la mayoría absoluta y escaños total no puedan ser erroneos
        private void NumEscaniosTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            string numero = NumEscaniosTotal.Text;
            int numeroInt;
            int numeroMayoriaAbsoluta;
            
            if(NumEscaniosTotal.Text == "")
            {
                MayoriaAbsoluta.Text = "";
            }

            if (int.TryParse(numero, out numeroInt))
            {
                if(numeroInt > 0)
                {
                    numeroMayoriaAbsoluta = numeroInt / 2 + 1;
                    MayoriaAbsoluta.Text = numeroMayoriaAbsoluta.ToString();
                }
                else
                {
                    String mensajePorPantalla = "Valores Negativos o 0, vuelve a introducirlos";
                    MessageBox.Show(mensajePorPantalla);
                    MayoriaAbsoluta.Text = "";
                    NumEscaniosTotal.Text = "";
                }

               
            }

        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            ProcesosElectoralesActualizados?.Invoke(this, new ProcesosElectoralesEventArgs(listaProcesosElectorales));
            this.Close();
        }
    }
}
