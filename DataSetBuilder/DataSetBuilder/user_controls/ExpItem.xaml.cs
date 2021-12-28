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
    /// Logica di interazione per ExpItem.xaml
    /// </summary>
    /// 

    /*
    User control - file xaml con relativa classe cs integrabili in altre interfacce
    */
    public partial class ExpItem : UserControl
    {
        //Variabile legata al TabControl delle deposizioni
        private DepoTabItem depoTabItem;

        public ExpItem()
        {
            InitializeComponent();
        }
        //Settare la variabile DepoTabItem
        public void setDepoTabControl(DepoTabItem depoTabItem)
        {
            this.depoTabItem = depoTabItem;
        }
        //Torna il valore della depoTabItem
        public DepoTabItem getDepoTabItem()
        {
            return this.depoTabItem;
        }

        //TODO: Evento legato al menu item del commento
        private void ViewCommentMenu_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
