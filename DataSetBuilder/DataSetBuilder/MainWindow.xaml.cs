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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataSetBuilder
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DSB_Controller dsb_controller;
        public MainWindow()
        {
            InitializeComponent();
            this.dsb_controller = new DSB_Controller();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Resize experiment part
            Column.Width = dsb_controller.columnWidth(Column);        
        }
    }
}
