using DataSetBuilder.controller;
using DataSetBuilder.user_controls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DataSetBuilder.factories
{
    class MyExpTabItemFactory
    {
        private DepoTabControlController depoTabControlController;
        private String basePath;

        public MyExpTabItemFactory(String basePath, DepoTabControlController depoTabControlController)
        {
            this.depoTabControlController = depoTabControlController;
            this.basePath = basePath;
        }

        public TabItem GetTabItem(ListViewItem listViewItem, ListBox listBox)
        {
            TabItem tabItem = new TabItem { Header = listViewItem.Content };
            listBox = initDepoList((string)listViewItem.Content, listBox);
            return tabItem;
        }

        private ListBox initDepoList(String subPath, ListBox listBox)
        {
            ListBox depoList = listBox;
            string depoPath = basePath + @"\" + subPath;
            //string depoPath = basePath + @"\" + subPath;
            string[] depoDirectories = Directory.GetDirectories(depoPath);

            if (isEmpty(depoDirectories.Length))
            {
                depoList.Items.Add(emptyMessage());
            }

            for (int i = 0; i < depoDirectories.Length; i++)
            {
                var listItem = new ListViewItem();
                listItem.Content = depoDirectories[i].Remove(0, depoPath.Length + 1);
                listItem.MouseDoubleClick += depoTabControlController.openDepsData;
                depoList.Items.Add(listItem);
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
