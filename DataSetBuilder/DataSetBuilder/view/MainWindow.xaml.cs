using DataSetBuilder.user_controls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataSetBuilder.view
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DSB_Controller dsb_controller;
        //string expPath = @"J:\DTI\_DSB";    //fisso Mariano
        string expPath = @"D:\_DSB";      //portatile Mariano

        private TabsBody tabBody;
        private DepoItemBody depoItemBody = new DepoItemBody();
        
        public MainWindow()
        {
            InitializeComponent();
            Init();
            initTabControl();
            this.dsb_controller = new DSB_Controller(this.tabBody.TabsControl, expPath);
        }

        private void Init()
        {
            //TODO extra initialize
            string[] expDirectories = Directory.GetDirectories(expPath);
            for(int i = 0; i < expDirectories.Length; i++)
            {
                var listItem = new ListViewItem();
                listItem.Content = expDirectories[i].Remove(0, expPath.Length + 1);
                listItem.MouseDoubleClick += openExpDeps;
                ExperimentViewer.Children.Add(listItem);
            }
        }
        private void initTabControl()
        {
            this.tabBody = new TabsBody();
            Grid.SetRow(tabBody, 1);
            Grid.SetColumn(tabBody, 2);
            DSB_MainGrid.Children.Add(tabBody);
        }
        private void openExpDeps(object sender, EventArgs e)
        {
            ListViewItem listViewItem = sender as ListViewItem;
            tabBody = dsb_controller.NewExpTabItem(tabBody, listViewItem);
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
}
