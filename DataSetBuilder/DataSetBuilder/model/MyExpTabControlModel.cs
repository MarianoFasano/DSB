using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataSetBuilder.model
{
    class MyExpTabControlModel
    {
        private TabControl tabControl;
        private IDictionary items = new Dictionary<Object, TabItem>();

        public MyExpTabControlModel(TabControl tabControl)
        {
            this.tabControl = tabControl;
        }

        public Boolean addItem(Object itemID, TabItem tabItem)
        {
            if (!items.Contains(itemID))
            {
                items.Add(itemID, tabItem);
                //tabControl.Items.Add(tabItem);
                return true;
            }
            return false;

        }
        public TabControl GetTabControl()
        {
            return this.tabControl;
        }
    }
}
