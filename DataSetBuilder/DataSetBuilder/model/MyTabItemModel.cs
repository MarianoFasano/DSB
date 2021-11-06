using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataSetBuilder.model
{
    
    class MyTabItemModel
    {
        private TabItem tabItem;

        public MyTabItemModel(TabItem tabitem)
        {
            this.tabItem = tabItem;
        }

        public TabItem GetTabItem()
        {
            return this.tabItem;
        }
    }
}
