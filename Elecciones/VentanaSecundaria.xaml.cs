using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Lógica de interacción para VentanaSecundaria.xaml
    /// </summary>
    public partial class VentanaSecundaria : Window
    {
        public VentanaSecundaria()
        {
            InitializeComponent();
        }

        private void BotonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BotonCrearEleccion_Click(object sender, RoutedEventArgs e)
        {
            //DialogResult = true;
           
            AgregarProcesoElectoral agregar = new AgregarProcesoElectoral();
            agregar.Title = "Agregar Proceso Electoral";
            agregar.ShowDialog();
            
        }
        
        /*//Se ira haciendo
        private void BotonEliminarEleccion_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void BotonEditarEleccion_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    */
    
    }
}
