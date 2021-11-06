using DataSetBuilder.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataSetBuilder.controller
{
    class MyTabControlController
    {
        private MyTabControlModel myTabControlModel;

        public MyTabControlController(TabControl tabControl)
        {
            this.myTabControlModel = new MyTabControlModel(tabControl);
        }

        public TabControl addItem(Object itemID, TabItem tabItem)
        {
            myTabControlModel.addItem(itemID, tabItem);
            return myTabControlModel.GetTabControl();
        }

    }
}
