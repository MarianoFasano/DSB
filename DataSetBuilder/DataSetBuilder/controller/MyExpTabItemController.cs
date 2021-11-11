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
    class MyExpTabItemController
    {
        private MyExpTabItemModel myTabItemModel;
        private MyExpTabItemFactory myTabItemFactory;

        public MyExpTabItemController(DepoTabItemController depositionController, String basePath)
        {
            this.myTabItemFactory = new MyExpTabItemFactory(depositionController, basePath);
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
