using System;
using System.Collections.Generic;
using System.IO;
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
    class DSB_Controller
    {
        //TODO: refactoring, if needed
        internal GridLength columnWidth(ColumnDefinition column)
        {
            Double width = column.ActualWidth;
            if (width == 155)
            {
                width = 0;
            }
            else if (width < 155 && width > 0)
            {
                width = 0;
            }
            else
            {
                width = 155;
            }
            return new GridLength(width);
        }

        internal System.Windows.Visibility viewComment(TextBox ExpCommentBox)
        {
            if(ExpCommentBox.Visibility.Equals(System.Windows.Visibility.Collapsed))
            {
                return System.Windows.Visibility.Visible;
            }
            else
            {
                return System.Windows.Visibility.Collapsed;
            }            
        }

        internal string commentText(TextBox ExpCommentBox)
        {
            if (ExpCommentBox.Visibility.Equals(System.Windows.Visibility.Collapsed))
            {
                return "Vedi Commento";
            }
            else
            {
                return "Nascondi Commento";
            }
        }

        //Create a new tabItem with TODO: initialization for depo view
        internal TabItem NewDepTabItem(ListViewItem listViewItem)
        {
            TabItem tabItem = new TabItem { Header = listViewItem.Content };

            Grid tabGrid = new Grid();

            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(1, GridUnitType.Star);
            tabGrid.ColumnDefinitions.Add(column1);

            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(1, GridUnitType.Star);
            tabGrid.ColumnDefinitions.Add(column2);

            // Create a DockPanel    
            DockPanel itemDock = new DockPanel();
            // Create search text TextBox    
            TextBox searchText = new TextBox();
            searchText.Height = 20;
            searchText.Text = "Cerca...";
            searchText.TextWrapping = TextWrapping.Wrap;
            // Dock TextBox to top    
            DockPanel.SetDock(searchText, Dock.Top);
            // Add docked button to DockPanel    
            itemDock.Children.Add(searchText);


            //Create Menu with MenuItem
            Menu menu = createMenu("Edit");

            MenuItem menuItem = createMenuItem("Edit");
            var binding = new Binding("ActualWidth");
            binding.Source = menu;
            BindingOperations.SetBinding(menuItem, MenuItem.WidthProperty, binding);
            menu.Items.Add(menuItem);

            MenuItem menuItem1 = createMenuItem("Vedi commento");
            menuItem.Items.Add(menuItem1);

            MenuItem menuItem2 = createMenuItem("Directory");
            menuItem.Items.Add(menuItem2);

            MenuItem menuItem3 = createMenuItem("Cancella deposizione");
            menuItem.Items.Add(menuItem3);
            //Dock Menu to bottom
            DockPanel.SetDock(menu, Dock.Bottom);
            //Add docked Menu to DockPanel
            itemDock.Children.Add(menu);

            // Create comment text TextBox    
            TextBox commentText = new TextBox();
            commentText.Visibility = System.Windows.Visibility.Collapsed;
            commentText.Height = Double.NaN;
            // Dock TextBox to bottom    
            DockPanel.SetDock(commentText, Dock.Bottom);
            // Add docked TextBox to DockPanel    
            itemDock.Children.Add(commentText);

            // Create a ScrollView with nested StackPanel    
            ScrollViewer depoView = new ScrollViewer();
            StackPanel depoStack = initDepoList((string)listViewItem.Content);

            depoView.Content = depoStack;

            // Dock ScrollView to left    
            DockPanel.SetDock(depoView, Dock.Left);
            // Add docked ScrollView to DockPanel    
            itemDock.Children.Add(depoView);

            Grid.SetColumn(itemDock, 0);
            tabGrid.Children.Add(itemDock);

            tabItem.Content = tabGrid;

            return tabItem;
        }

        private Menu createMenu(String name)
        {
            Menu menu = new Menu();
            menu.Name = name;
            return menu;
        }

        private MenuItem createMenuItem(String name)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Header = name;
            return menuItem;
        }

        private StackPanel initDepoList(String subPath)
        {
            StackPanel depoList = new StackPanel();
            string depoPath = @"D:\_DSB" + @"\" + subPath;
            string[] depoDirectories = Directory.GetDirectories(depoPath);
            for (int i = 0; i < depoDirectories.Length; i++)
            {
                var listItem = new ListViewItem();
                listItem.Content = depoDirectories[i].Remove(0, depoPath.Length + 1);
                //listItem.MouseDoubleClick += openExpDeps;
                depoList.Children.Add(listItem);
            }
            return depoList;
        }
    }
}
