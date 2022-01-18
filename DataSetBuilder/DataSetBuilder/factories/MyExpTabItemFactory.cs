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

        public CloseableTab GetTabItem(string depoName, ExpItem expitem, string path)
        {
            CloseableTab tabItem = new CloseableTab { Title = depoName };
            expitem.initDepoList(depoName, path);
            return tabItem;
        }

        private bool isEmpty(int length)
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
    }
}
