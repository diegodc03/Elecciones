using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

       
        ObservableCollection<Partido> partidos = new ObservableCollection<Partido>();
        ObservableCollection<ProcesoElectoral> listaProcesosElectorales;

        ProcesoElectoral proceso;

        List<String> partidosPers = new List<string>();
        List<String> coloresPartidos = new List<string>();

        PartidosPersistentes partidosPersistentes = new PartidosPersistentes();
        LecturaDeFicheroTxt lecturaColores = new LecturaDeFicheroTxt();
        List<ProcesoElectoral> prueba = new List<ProcesoElectoral>();

        ProcesoElectoral aux = new ProcesoElectoral();
        Partido auxPartido = new Partido();

        int sumatorioNumeroEscanios = 0;
        int indiceFila = 0;
        int modificacionPartidos = 0;
        int modificacionProceso = 0;
        

       
        

        public AgregarProcesoElectoral(ObservableCollection<ProcesoElectoral> procesosActualizados)
        {
            InitializeComponent();

            introducirValoresComboBox();

            
            if(procesosActualizados.Count > 0)
            {
                ProcesoElectoral procesoAClonar;

                listaProcesosElectorales = new ObservableCollection<ProcesoElectoral>();
                //Añadimos procesos a una lista que no tenga referencia ObservableCollection
                foreach (ProcesoElectoral proc in procesosActualizados)
                {
                    procesoAClonar = new ProcesoElectoral();
                    listaProcesosElectorales.Add(procesoAClonar.ClonarProcesoElectoral(proc));

                }
            }
            else
            {
                listaProcesosElectorales = new ObservableCollection<ProcesoElectoral>();
            }
            
        }
        

        //Contructor para editar proceso Electoral
        public AgregarProcesoElectoral(ProcesoElectoral proceso, ObservableCollection<ProcesoElectoral> procesos)
        {

            InitializeComponent();

            ProcesoElectoral procesoAClonar;

            listaProcesosElectorales = new ObservableCollection<ProcesoElectoral>();
            //Añadimos procesos a una lista que no tenga referencia ObservableCollection
            foreach(ProcesoElectoral proc in procesos)
            {
                procesoAClonar = new ProcesoElectoral();
                listaProcesosElectorales.Add(procesoAClonar.ClonarProcesoElectoral(proc));
                
            }

            this.proceso = new ProcesoElectoral();

            procesoAClonar = new ProcesoElectoral();
            this.proceso = procesoAClonar.ClonarProcesoElectoral(proceso);

            //partidos = proceso.Partidos;
            introducirValoresComboBox();

            //Rellenamos los datos en este metodo
            EditarProcesoElectoral(this.proceso);
            modificacionProceso = 1;
        }


        


        //Metodos para editar el proceso a Editar
        private void EditarProcesoElectoral(ProcesoElectoral proceso)
        {   
            aux = proceso;

            nombreEleccion.Text = proceso.nombreProcesoElectoral;
            NumEscaniosTotal.Text = proceso.numeroDeEscanios.ToString();
            MayoriaAbsoluta.Text = proceso.mayoriaAbsoluta.ToString();
            FechaEleccion.Text = proceso.fechaProcesoElectoral.ToString();
            
            this.partidos.Clear();
            this.partidos = proceso.Partidos;

            DataGridPartidos.ItemsSource = partidos;

            foreach(Partido partido in partidos)
            {
                sumatorioNumeroEscanios = sumatorioNumeroEscanios + partido.scanios;
            }
      

        }

        private void introducirValoresComboBox()
        {
            //Se añade en la parte de los Partidos los elementos al COMBOBOX
            partidosPers = partidosPersistentes.getPartidos();
            PartidoComboBox.ItemsSource = partidosPers;

            coloresPartidos = lecturaColores.leerFichero();
            ColoresComboBox.ItemsSource = coloresPartidos;

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


                //Comprobamos si usuario ha introducido los dos valores pedidos ya que para poder coger el numero de partidos, se necesita el numero de escaños total
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

                            int.TryParse(numeroScaniosTotal, out escaniosPartidosComprobante);
                            if (modificacionPartidos == 0)
                            {
                                sumatorioNumeroEscanios += numScanios;
                                if (sumatorioNumeroEscanios <= escaniosPartidosComprobante)
                                {
                                   

                                    //Devuelve o una instancia de Partidos o null
                                    if (partidos.FirstOrDefault(x => x.nombrePartido.Contains(nombrePartido)) == null)
                                    {

                                        nombrePartido = nombrePartido.ToUpper();
                                        //indiceFila = DataGridPartidos.SelectedIndex;

                                        //Se crea Instancia y se introduce en ListaDePartidos
                                        Partido partidoPolitico = new Partido(nombrePartido, numScanios, colorPartido);
                                        partidos.Add(partidoPolitico);

                                        DataGridPartidos.ItemsSource = partidos;


                                        //Añadir a ListaPartidosPersistentes si no existe todavia
                                        if (!partidosPers.Contains(nombrePartido))
                                        {
                                            partidosPersistentes.AgregarPartido(partidoPolitico.nombrePartido);

                                            //Actualizar ComboBox
                                            partidosPers = partidosPersistentes.getPartidos();
                                            PartidoComboBox.ItemsSource = null;
                                            PartidoComboBox.ItemsSource = partidosPers;
                                        }

                                       

                                        PartidoComboBox.Text = "";
                                        NumEscaniosPartido.Text = "";
                                        ColoresComboBox.Text = "";
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
                                //En este momento sabemos que se quieren modificar los datos
                                nombrePartido = nombrePartido.ToUpper();
                                indiceFila = DataGridPartidos.SelectedIndex;


                                //Cogemos el Partido que esta en la lista
                                Partido partidoRemplazo = partidos.FirstOrDefault(x => x.nombrePartido.Contains(auxPartido.nombrePartido));
                                sumatorioNumeroEscanios = sumatorioNumeroEscanios - partidoRemplazo.scanios;
                                sumatorioNumeroEscanios = sumatorioNumeroEscanios + numScanios;

                                if (sumatorioNumeroEscanios <= escaniosPartidosComprobante)
                                {
                                    int indice = partidos.IndexOf(partidoRemplazo);

                                    //Se crea Instancia y se introduce en ListaDePartidos
                                    Partido partidoPolitico = new Partido(nombrePartido, numScanios, colorPartido);
                                    partidos[indice] = partidoPolitico;

                                    
                                    DataGridPartidos.ItemsSource = partidos;
                                    
                                    PartidoComboBox.Text = "";
                                    NumEscaniosPartido.Text = "";
                                    ColoresComboBox.Text = "";
                                    modificacionPartidos = 0;
                                }
                                else
                                {       
                                    String mensajePorPantalla = "Tiene que poner un valor menor que el número de escaños del proceso electoral";
                                    MessageBox.Show(mensajePorPantalla);
                                   sumatorioNumeroEscanios = sumatorioNumeroEscanios - numScanios;                   
                                }
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
                        
                        ObservableCollection<Partido> auxiliar = new ObservableCollection<Partido>(partidos.OrderByDescending(x => x.scanios));
                   
                        fechaSeleccionada = FechaEleccion.SelectedDate.Value;
                        ProcesoElectoral procesoNuevo = new ProcesoElectoral(nombreProceso, fechaSeleccionada, totalEscanios, mayoriaEscanios, auxiliar);

                        
                        if(modificacionProceso == 0)
                        {
                            listaProcesosElectorales.Add(procesoNuevo);

                            

                            List<ProcesoElectoral> aux = new List<ProcesoElectoral>();
                            List<ProcesoElectoral> ordenada = new List<ProcesoElectoral>();
                            //Creo nueva lista para ordenar los datos
                            ProcesoElectoral po = new ProcesoElectoral();
                            foreach(ProcesoElectoral procesoElec in listaProcesosElectorales)
                            {
                                aux.Add(po.ClonarProcesoElectoral(procesoElec));
                            }
                         
                            ordenada = aux.OrderByDescending(x => x.fechaProcesoElectoral).ToList();

                            listaProcesosElectorales.Clear();
                            foreach(ProcesoElectoral p in ordenada)
                            {
                                listaProcesosElectorales.Add(p);
                            }
                            

                        }
                        else
                        {
                            ProcesoElectoral procesoRemplazo = listaProcesosElectorales.FirstOrDefault(x => x.fechaProcesoElectoral.ToString().Contains(aux.fechaProcesoElectoral.ToString()));
                            if(procesoRemplazo != null)
                            {
                                int indice = listaProcesosElectorales.IndexOf(procesoRemplazo);
                                listaProcesosElectorales[indice] = procesoNuevo;
                                modificacionProceso = 0;
                            }
                        }
                   
                        //Se reinicia todos los textBox para introducir mas elementos
                        nombreEleccion.Text = "";
                        MayoriaAbsoluta.Text = "";
                        NumEscaniosTotal.Text = "";

                        DataGridPartidos.ItemsSource = null;
                        partidos = new ObservableCollection<Partido>();
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

        public event EventHandler<ObservableCollection<ProcesoElectoral>> listaActualizada;

        private void EnviarListaActualizada()
        {
            listaActualizada?.Invoke(this, listaProcesosElectorales);
        }




        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            EnviarListaActualizada();
            this.Close();
        }



        private void BotonEliminarPartido_Click(object sender, RoutedEventArgs e)
        {
            Partido partidoSeleccionado = DataGridPartidos.SelectedItem as Partido;

            if (partidoSeleccionado != null)
            {
                //Modfiica el numero de scanios total
                sumatorioNumeroEscanios = sumatorioNumeroEscanios - partidoSeleccionado.scanios;

                partidos.Remove(partidoSeleccionado);

                DataGridPartidos.ItemsSource = partidos;                
            }
        }

        private void BotonEditarPartidoPolitico_Click(object sender, RoutedEventArgs e)
        {
            Partido partidoSeleccionado = DataGridPartidos.SelectedItem as Partido;

            

            if (partidoSeleccionado != null)
            {
                this.auxPartido = partidoSeleccionado;
                PartidoComboBox.Text = partidoSeleccionado.nombrePartido;
                NumEscaniosPartido.Text = partidoSeleccionado.scanios.ToString();
                ColoresComboBox.Text = partidoSeleccionado.color;

                modificacionPartidos = 1;
            }
        }

        

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(ventana.ActualWidth < 550 || ventana.ActualHeight < 350)
            {
                String mensajePorPantalla = "No se puede hacer tan pequeño";
                MessageBox.Show(mensajePorPantalla);
            }
        }
    }
}
