using DataSetBuilder.controller;
using DataSetBuilder.model;
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

        public CloseableTab GetTabItem(ListViewItem listViewItem, ListBox listBox)
        {
            CloseableTab tabItem = new CloseableTab { Title = (string)listViewItem.Content };
            listBox = initDepoList((string)listViewItem.Content, listBox);
            return tabItem;
        }

        private ListBox initDepoList(String subPath, ListBox listBox)
        {
            ListBox depoList = listBox;
            string depoPath = basePath + @"\" + subPath;
            string[] depoDirectories = Directory.GetDirectories(depoPath);

            if (isEmpty(depoDirectories.Length))
            {
                depoList.Items.Add(emptyMessage());
            }

            for (int i = 0; i < depoDirectories.Length; i++)
            {
                var listItem = new ListViewItem();
                string depoName = depoDirectories[i].Remove(0, depoPath.Length + 1);
                if (depoName.Contains("Deposition"))
                {
                    listItem.Content = depoName;
                    listItem.MouseDoubleClick += depoTabControlController.openDepsData;
                    depoList.Items.Add(listItem);
                }

            }
            if (depoList.Items.Count == 0)
            {
                depoList.Items.Add(emptyMessage());
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
