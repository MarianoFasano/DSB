using DataSetBuilder.controller;
using DataSetBuilder.factories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataSetBuilder
{
    class DSB_Controller
    {
        private MyTabItemFactory myTabItemFactory;
        private MyTabControlController myTabControlController;
        private MyTabItemController myTabItemController;

        public DSB_Controller(TabControl tabControl)
        {
            this.myTabItemFactory = new MyTabItemFactory();
            this.myTabControlController = new MyTabControlController(tabControl);
            this.myTabItemController = new MyTabItemController();
        }

        //TODO: refactoring, if needed
        internal GridLength columnWidth(ColumnDefinition column)
        {
            Double width = column.ActualWidth;
            if (width == 155)
            {
                width = 0;
            }
            else if (width < 155 && width > 0)
            {
                width = 0;
            }
            else
            {
                width = 155;
            }
            return new GridLength(width);
        }

        internal System.Windows.Visibility viewComment(TextBox ExpCommentBox)
        {
            if(ExpCommentBox.Visibility.Equals(System.Windows.Visibility.Collapsed))
            {
                return System.Windows.Visibility.Visible;
            }
            else
            {
                return System.Windows.Visibility.Collapsed;
            }            
        }

        internal string commentText(TextBox ExpCommentBox)
        {
            if (ExpCommentBox.Visibility.Equals(System.Windows.Visibility.Collapsed))
            {
                return "Vedi Commento";
            }
            else
            {
                return "Nascondi Commento";
            }
        }

        //Create a new tabItem with TODO: initialization for depo view
        internal TabControl NewDepTabItem(ListViewItem listViewItem)
        {
            //TabControl
            //return myTabItemFactory.GetTabItem(listViewItem);
            return myTabControlController.addItem(listViewItem.Content, myTabItemController.createTabItem(listViewItem));

        }
    }
}
