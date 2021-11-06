using DataSetBuilder.factories;
using DataSetBuilder.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataSetBuilder.controller
{
    class MyTabItemController
    {
        private MyTabItemModel myTabItemModel;
        private MyTabItemFactory myTabItemFactory;

        public MyTabItemController()
        {
            this.myTabItemFactory = new MyTabItemFactory();
        }

        public TabItem getTabItem()
        {
            return myTabItemModel.GetTabItem();
        }
        public TabItem createTabItem(ListViewItem listViewItem)
        {
            return myTabItemFactory.GetTabItem(listViewItem);
        }
    }
}
