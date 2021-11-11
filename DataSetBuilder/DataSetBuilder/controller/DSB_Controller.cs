using DataSetBuilder.controller;
using DataSetBuilder.user_controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DataSetBuilder
{
    class DSB_Controller
    {
        private MyTabControlController myTabControlController;
        private MyTabItemController myTabItemController;
        private DepositionController depositionController;

        public DSB_Controller(TabControl tabControl, Button play, Button pause, Button prev, Button next, ComboBox speed, Image image, String basePath)
        {
            this.depositionController = new DepositionController(play, pause, prev, next, speed, image);
            this.myTabControlController = new MyTabControlController(tabControl);
            this.myTabItemController = new MyTabItemController(depositionController, basePath);
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
        /*internal TabControl NewDepTabItem(ListViewItem listViewItem)
        {
            return myTabControlController.addItem(listViewItem.Content, myTabItemController.createTabItem(listViewItem));
        }*/
        internal TabItem NewDepTabItem(ListViewItem listViewItem, StackPanel stackPanel)
        {
            return myTabItemController.createTabItem(listViewItem, stackPanel);
        }

        public DepositionController getdepoController()
        {
            return this.depositionController;
        }
    }
}
