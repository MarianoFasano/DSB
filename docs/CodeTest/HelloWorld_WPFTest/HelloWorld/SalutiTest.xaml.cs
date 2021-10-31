using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace cSharpeTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ThreadTest taskTest = new ThreadTest();
        RadioButtons radioButtons;
        public MainWindow()
        {
            taskTest.ErrorEvent += ManageError;
            InitializeComponent();
            this.radioButtons = new RadioButtons();
            this.gridMain.Children.Add(radioButtons);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (radioButtons.HelloButton.IsChecked == true)
            {
                MessageBox.Show("Benvenuto/a!");
            }
            else if (radioButtons.GoodByeButton.IsChecked == true)
            {
                MessageBox.Show("Arrivederci!!");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            taskTest.start();
        }

        public void ManageError(object sender, EventArgs args)
        {
            String number = "Data Manage TODO";
            MessageBox.Show($"Error time {number}", "There is an error", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
        }
    }
}
