using System;
using System.IO;
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

namespace DataSetBuilder.view
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
            Init();
            this.dsb_controller = new DSB_Controller(TabsControl);
        }

        private void Init()
        {
            //TODO extra initialize
            //string expPath = @"J:\DTI\_DSB";
            string expPath = @"D:\_DSB";
            string[] expDirectories = Directory.GetDirectories(expPath);
            for(int i = 0; i < expDirectories.Length; i++)
            {
                var listItem = new ListViewItem();
                listItem.Content = expDirectories[i].Remove(0, expPath.Length + 1);
                listItem.MouseDoubleClick += openExpDeps;
                ExperimentViewer.Children.Add(listItem);
            }
        }

        private void openExpDeps(object sender, EventArgs e)
        {
            ListViewItem listViewItem = sender as ListViewItem;
            //TabsControl.Items.Add(dsb_controller.NewDepTabItem(listViewItem));
            TabsControl = dsb_controller.NewDepTabItem(listViewItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Resize experiment section
            Column.Width = dsb_controller.columnWidth(Column);        
        }

        private void ViewExpCommentMenu_Click(object sender, RoutedEventArgs e)
        {
            //Manage experiment comment section
            ExpComment.Visibility = dsb_controller.viewComment(ExpComment);
            ViewCommentMenu.Header = dsb_controller.commentText(ExpComment);
        }
    }

    //add from stackoverflow
    /*class ContentVM
    {
        public ContentVM(string name, int index)
        {
            Name = name;
            Index = index;
        }
        public string Name { get; set; }
        public int Index { get; set; }
    }

    private void OnTabCloseClick(object sender, RoutedEventArgs e)
    {
        var tab = (sender as Button).DataContext as TabVM;
        if (Tabs.Count > 2)
        {
            var index = Tabs.IndexOf(tab);
            if (index == Tabs.Count - 2)//last tab before [+]
            {
                MyTabControl.SelectedIndex--;
            }
            Tabs.RemoveAt(index);
        }
    }*/
}
