using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataSetBuilder.model
{
    
    class MyExpTabItemModel
    {
        private TabItem tabItem;

        public MyExpTabItemModel(TabItem tabitem)
        {
            this.tabItem = tabItem;
        }

        public TabItem GetTabItem()
        {
            return this.tabItem;
        }
    }
}
