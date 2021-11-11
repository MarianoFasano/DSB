using DataSetBuilder.model;
using DataSetBuilder.user_controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataSetBuilder.controller
{
    class MyExpTabControlController
    {
        private MyExpTabControlModel myTabControlModel;
        private MyExpTabItemController myTabItemController;

        public MyExpTabControlController(TabControl tabControl, MyExpTabItemController myTabItemController)
        {
            this.myTabControlModel = new MyExpTabControlModel(tabControl);
            this.myTabItemController = myTabItemController;
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
            TabItem tabItem = myTabItemController.createTabItem(listViewItem, expItem.DepositionViewer);
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
                DepoTabItem depoTabItem = new DepoTabItem();
                Grid.SetRow(depoTabItem, 1);
                Grid.SetColumn(depoTabItem, 0);
                tabBody.TabBody_Grid.Children.Add(depoTabItem);
                return tabBody;
            }
            else
            {
                return tabBody;
            }
        }
    }
}
