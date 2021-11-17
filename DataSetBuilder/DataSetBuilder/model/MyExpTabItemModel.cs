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
        private IDictionary<String, TabControl> dataItems = new Dictionary<String, TabControl>();

        public void addToDict(String key, TabControl value)
        {
            if (!this.dataItems.ContainsKey(key))
            {
                this.dataItems.Add(key, value);
            }
        }

        public TabControl getTabControl(String key)
        {
            if (dataItems.ContainsKey(key))
            {
                return this.dataItems[key];
            }
            return null;
        }
    }
}
