using DataSetBuilder.factories;
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
    class MyTabItemController
    {
        private MyTabItemModel myTabItemModel;
        private MyTabItemFactory myTabItemFactory;

        public MyTabItemController(DepositionController depositionController, String basePath)
        {
            this.myTabItemFactory = new MyTabItemFactory(depositionController, basePath);
        }

        public TabItem getTabItem()
        {
            return myTabItemModel.GetTabItem();
        }
        public TabItem createTabItem(ListViewItem listViewItem, StackPanel stackPanel)
        {
            return myTabItemFactory.GetTabItem(listViewItem, stackPanel);
        }
    }
}
