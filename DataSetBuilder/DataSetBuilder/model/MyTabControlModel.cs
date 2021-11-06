using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataSetBuilder.model
{
    class MyTabControlModel
    {
        private TabControl tabControl;
        private IDictionary items = new Dictionary<Object, TabItem>();

        public MyTabControlModel(TabControl tabControl)
        {
            this.tabControl = tabControl;
        }

        public void addItem(Object itemID, TabItem tabItem)
        {
            if (items.Contains(itemID))
            {

            } else if (!items.Contains(itemID))
            {
                items.Add(itemID, tabItem);
                tabControl.Items.Add(tabItem);
            }

        }
        public TabControl GetTabControl()
        {
            return this.tabControl;
        }
    }
}
