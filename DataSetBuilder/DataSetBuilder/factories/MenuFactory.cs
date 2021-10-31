using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataSetBuilder.factories
{
    class MenuFactory
    {
        //Public method to create a new menu
        public Menu getNewMenu(String name)
        {
            return createMenu(name);
        }

        //Private method called by the public method
        private Menu createMenu(String name)
        {
            Menu menu = new Menu();
            menu.Name = name;
            return menu;
        }
    }
}
