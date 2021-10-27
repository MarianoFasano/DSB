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
            Init();
            this.dsb_controller = new DSB_Controller();
        }

        private void Init()
        {
            //TODO extra initialize
            string expPath = @"J:\DTI\_DSB";
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
            ListViewItem listView = sender as ListViewItem;
            //MessageBox.Show("Hello, ciao", "There is an error", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);

            var tabItem = new TabItem { Header = listView.Content };
            
            Grid tabGrid = new Grid();
            tabItem.Content = tabGrid;

            ColumnDefinition column0 = new ColumnDefinition();
            ColumnDefinition column1 = new ColumnDefinition();

            tabGrid.ColumnDefinitions.Add(column0);
            tabGrid.ColumnDefinitions.Add(column1);

            DockPanel itemDock = new DockPanel();
            itemDock.MinWidth = 0;
            itemDock.LastChildFill = true;
            TextBox searchText = new TextBox();
            searchText.Height = 20;
            searchText.Text = "Cerca...";

            Menu menu = new Menu();
            menu.Name = "Edit";
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Vedi commento";
            menu.Items.Add(menuItem);

            MenuItem menuItem1 = new MenuItem();
            menuItem1.Header = "Directory";
            menu.Items.Add(menuItem1);

            MenuItem menuItem2 = new MenuItem();
            menuItem2.Header = "Cancella deposizione";
            menu.Items.Add(menuItem2);

            TextBox commentText = new TextBox();
            commentText.Visibility = System.Windows.Visibility.Collapsed;
            commentText.Height = Double.NaN;

            ScrollViewer depoView = new ScrollViewer();
            StackPanel depoStack = new StackPanel();
            ListViewItem depoItem = new ListViewItem();

            depoItem.Content = "Deposition 2";
            depoStack.Children.Add(depoItem);
            depoView.Content = depoStack;
           
            DockPanel.SetDock(searchText, Dock.Top);
            DockPanel.SetDock(menu, Dock.Bottom);
            DockPanel.SetDock(commentText, Dock.Bottom);
            DockPanel.SetDock(depoView, Dock.Left);

            tabGrid.Children.Add(itemDock);
            Grid.SetColumn(itemDock, 0);

            TabsControl.Items.Add(tabItem);
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
