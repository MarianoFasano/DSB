using DataSetBuilder.model;
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
    /// Logica di interazione per DepoTabItem.xaml
    /// </summary>
    /// 

    /*
    User control - file xaml con relativa classe cs integrabili in altre interfacce
    */
    public partial class DepoTabItem : UserControl
    {
        public DepoTabItem()
        {
            InitializeComponent();
        }

        public TabControl getDepoTabControl()
        {
            return DepoTabControl;
        }

        //Le prossime due funzioni servono per poter muovere di ordine i tab e mostrare la preview di dove finirà la tab in movimento
        //https://stackoverflow.com/questions/10738161/is-it-possible-to-rearrange-tab-items-in-tab-control-in-wpf
        //Prese Ctrl+C Ctrl+V
        //Con i CloseableTab non sembrano funzionare
        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!(e.Source is CloseableTab tabItem))
            {
                return;
            }

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
            }
        }

        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            if (e.Source is TabItem tabItemTarget &&
                e.Data.GetData(typeof(CloseableTab)) is TabItem tabItemSource &&
                !tabItemTarget.Equals(tabItemSource) &&
                tabItemTarget.Parent is TabControl tabControl)
            {
                int targetIndex = tabControl.Items.IndexOf(tabItemTarget);

                tabControl.Items.Remove(tabItemSource);
                tabControl.Items.Insert(targetIndex, tabItemSource);
                tabItemSource.IsSelected = true;
            }
        }
    }
}
