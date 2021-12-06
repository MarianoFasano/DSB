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

namespace DataSetBuilder.user_controls
{
    /// <summary>
    /// Logica di interazione per DepoItemBody.xaml
    /// </summary>
    /// 

    /*
    User control - file xaml con relativa classe cs integrabili in altre interfacce
    */
    public partial class DepoItemBody : UserControl
    {
        public DepoItemBody()
        {
            InitializeComponent();
        }

        //Evento collegato al click del bottone "<<" (indietro)
        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            //Se la componente FileBrowser (WebBrowser) è in grado di navigare indietro allora lo fa (massimo fino al percorso specificato in fase di inizializzazione)
            if (FileBrowser.CanGoBack)
            {
                FileBrowser.GoBack();
            }
        }

        //Evento collegato al click del bottone ">>" (avanti)
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            //Se la componente FileBrowser (WebBrowser) è in grado di navigare in avanti allora lo fa
            //(unicamente se si ha già navigato in avanti rispetto al percorso base, es. entrare in una cartella)
            if (FileBrowser.CanGoForward)
            {
                FileBrowser.GoForward();
            }
        }
    }
}
