using DataSetBuilder.controller;
using DataSetBuilder.user_controls;
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
    class MyExpTabItemFactory
    {
        private MenuFactory menuFactory ;
        private MenuItemFactory menuItemFactory ;
        private ColumnFactory columnFactory;
        private DepoTabItemController depositionController;
        private String basePath;

        public MyExpTabItemFactory(DepoTabItemController depositionController, String basePath)
        {
            this.menuFactory = new MenuFactory();
            this.menuItemFactory = new MenuItemFactory();
            this.columnFactory = new ColumnFactory();
            this.depositionController = depositionController;
            this.basePath = basePath;
        }

        public TabItem GetTabItem(ListViewItem listViewItem, StackPanel stackPanel)
        {
            TabItem tabItem = new TabItem { Header = listViewItem.Content };
            stackPanel = initDepoList((string)listViewItem.Content, stackPanel);
            return tabItem;
        }

        private StackPanel initDepoList(String subPath, StackPanel stackPanel)
        {
            StackPanel depoList = stackPanel;
            string depoPath = basePath + @"\" + subPath;
            //string depoPath = basePath + @"\" + subPath;
            depositionController.setPath(depoPath);
            string[] depoDirectories = Directory.GetDirectories(depoPath);

            if (isEmpty(depoDirectories.Length))
            {
                depoList.Children.Add(emptyMessage());
            }

            for (int i = 0; i < depoDirectories.Length; i++)
            {
                var listItem = new ListViewItem();
                listItem.Content = depoDirectories[i].Remove(0, depoPath.Length + 1);
                listItem.MouseDoubleClick += depositionController.openDepsData;
                depoList.Children.Add(listItem);
            }

            return depoList;
        }
        private Boolean isEmpty(int length)
        {
            if (length==0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private Label emptyMessage()
        {
            var label = new Label();
            label.Content = "Nessuna deposizione trovata";
            label.HorizontalAlignment = HorizontalAlignment.Center;
            return label;
        }
    }
}
