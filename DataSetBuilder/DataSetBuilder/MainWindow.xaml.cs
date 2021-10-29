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
            tabGrid.Background = Brushes.Gray;

            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(1, GridUnitType.Star);
            tabGrid.ColumnDefinitions.Add(column1);

            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(1, GridUnitType.Star);
            tabGrid.ColumnDefinitions.Add(column2);

            Label colonna1 = new Label();
            colonna1.Content = "colonnA 1";

            Label colonna2 = new Label();
            colonna2.Content = "colonna 2";

            //Grid.SetColumn(colonna1, 0);
            //Grid.SetRow(colonna1, 0);
            //Grid.SetColumn(colonna2, 1);
            //Grid.SetRow(colonna2, 0);
/*
            //SearchBox
            TextBox searchText = new TextBox();
            searchText.Height = 20;
            searchText.Text = "Cerca...";

            //Menu
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

            //CommentBox
            TextBox commentText = new TextBox();
            commentText.Visibility = System.Windows.Visibility.Collapsed;
            commentText.Height = Double.NaN;

            //Viewer and panel
            ScrollViewer depoView = new ScrollViewer();
            StackPanel depoStack = new StackPanel();
            ListViewItem depoItem = new ListViewItem();

            depoItem.Content = "Deposition 2";
            depoStack.Children.Add(depoItem);
            depoView.Content = depoStack;

            DockPanel itemDock = new DockPanel();
            itemDock.MinWidth = 0;
            itemDock.LastChildFill = true;
            DockPanel.SetDock(searchText, Dock.Top);
            DockPanel.SetDock(menu, Dock.Bottom);
            DockPanel.SetDock(commentText, Dock.Bottom);
            DockPanel.SetDock(depoView, Dock.Left);
            DockPanel.SetDock(colonna1, Dock.Top);

            Grid.SetColumn(itemDock, 0);
            tabGrid.Children.Add(itemDock);
*/
            //tabGrid.Children.Add(colonna1);
            //tabGrid.Children.Add(colonna2);

            DockPanel itemDock = new DockPanel();
            itemDock.Background = Brushes.LightCyan;

            TextBox searchText = new TextBox();
            searchText.Height = 20;
            searchText.Text = "Cerca...";
            searchText.TextWrapping = TextWrapping.Wrap;
            searchText.Background = Brushes.Black;

            DockPanel.SetDock(searchText, Dock.Left);

            Grid.SetColumn(itemDock, 0);
            tabGrid.Children.Add(itemDock);

            tabItem.Content = tabGrid;

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
