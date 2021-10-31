using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DataSetBuilder.factories
{
    class TabItemFactory
    {
        private MenuFactory menuFactory = new MenuFactory();
        private MenuItemFactory menuItemFactory = new MenuItemFactory();
        private ColumnFactory columnFactory = new ColumnFactory();

        public TabItem GetTabItem(ListViewItem listViewItem)
        {
            TabItem tabItem = new TabItem { Header = listViewItem.Content };

            Grid tabGrid = new Grid();

            tabGrid.ColumnDefinitions.Add(columnFactory.getNewColumn());
            tabGrid.ColumnDefinitions.Add(columnFactory.getNewColumn());

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
            Menu menu = menuFactory.getNewMenu("Edit");

            MenuItem menuItem = menuItemFactory.getNewMenuItem("Edit");
            var binding = new Binding("ActualWidth");
            binding.Source = menu;
            BindingOperations.SetBinding(menuItem, MenuItem.WidthProperty, binding);
            menu.Items.Add(menuItem);

            MenuItem menuItem1 = menuItemFactory.getNewMenuItem("Vedi commento");
            menuItem.Items.Add(menuItem1);

            MenuItem menuItem2 = menuItemFactory.getNewMenuItem("Directory");
            menuItem.Items.Add(menuItem2);

            MenuItem menuItem3 = menuItemFactory.getNewMenuItem("Cancella deposizione");
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

        private StackPanel initDepoList(String subPath)
        {
            StackPanel depoList = new StackPanel();
            string depoPath = @"J:\DTI\_DSB" + @"\" + subPath;
            //string depoPath = @"D:\_DSB" + @"\" + subPath;
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
