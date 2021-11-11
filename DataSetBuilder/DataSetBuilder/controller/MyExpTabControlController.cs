using DataSetBuilder.factories;
using DataSetBuilder.model;
using DataSetBuilder.user_controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DataSetBuilder.controller
{
    class MyExpTabControlController
    {
        private MyExpTabControlModel myTabControlModel;
        private MyExpTabItemModel myExpTabItemModel;
        private MyExpTabItemFactory myTabItemFactory;
        private DepoTabControlController depoTabControlController;
        private TabControl mainTabControl;

        public MyExpTabControlController(TabControl tabControl, String basePath, MyExpTabItemModel myExpTabItemModel, DepoTabControlController depoTabControlController)
        {
            this.mainTabControl = tabControl;
            this.myTabControlModel = new MyExpTabControlModel(tabControl);
            this.myExpTabItemModel = myExpTabItemModel;
            this.myTabItemFactory = new MyExpTabItemFactory(basePath, depoTabControlController);
            this.depoTabControlController = depoTabControlController;
            this.mainTabControl.SelectionChanged += TabControl_SelectionChanged;
        }

        private Boolean addItem(Object itemID, TabItem tabItem)
        {
            if(myTabControlModel.addItem(itemID, tabItem))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal TabsBody createTabItem(TabsBody tabBody, ListViewItem listViewItem)
        {
            ExpItem expItem = new ExpItem();
            TabItem tabItem = myTabItemFactory.GetTabItem(listViewItem, expItem.DepositionViewer);
            if (addItem(listViewItem, tabItem))
            {
                DepoTabItem depodataTabItem = new DepoTabItem();
                Grid.SetRow(depodataTabItem, 1);
                Grid.SetColumn(depodataTabItem, 0);
                Grid.SetColumnSpan(depodataTabItem, 2);
                expItem.ExpGrid.Children.Add(depodataTabItem);
                tabItem.Header = listViewItem.Content;
                tabItem.Content = expItem;

                tabBody.TabsControl.Items.Add(tabItem);

                this.myExpTabItemModel.addToDict((string)listViewItem.Content, depodataTabItem.DepoTabControl);
                return tabBody;
            }
            else
            {
                return tabBody;
            }
        }

        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabItem tabItem = (TabItem)mainTabControl.SelectedItem;
                depoTabControlController.setPath((string)tabItem.Header);
            }
        }
    }
}
