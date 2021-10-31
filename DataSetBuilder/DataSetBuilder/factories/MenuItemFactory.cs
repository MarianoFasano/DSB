using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataSetBuilder.factories
{
    class MenuItemFactory
    {
        //Public method to create a menu item
        public MenuItem getNewMenuItem(String name)
        {
            return createMenuItem(name);
        }

        //Private method called by the public method
        private MenuItem createMenuItem(String name)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Header = name;
            return menuItem;
        }
    }
}
