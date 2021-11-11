using DataSetBuilder.controller;
using DataSetBuilder.user_controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DataSetBuilder
{
    class DSB_Controller
    {
        private MyExpTabControlController myTabControlController;
        private DepoTabItemController depositionController;
        private MyExpTabItemController myTabItemController;
        public DSB_Controller(TabControl tabControl, Button play, Button pause, Button prev, Button next, ComboBox speed, Image image, String basePath)
        {
            this.depositionController = new DepoTabItemController(play, pause, prev, next, speed, image);
            MyExpTabItemController myTabItemController = new MyExpTabItemController(depositionController, basePath);
            this.myTabItemController = myTabItemController;
            this.myTabControlController = new MyExpTabControlController(tabControl, myTabItemController);
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

        internal TabsBody NewDepTabItem(TabsBody tabBody, ListViewItem listViewItem)
        {
            return myTabControlController.createTabItem(tabBody, listViewItem);
        }

        public DepoTabItemController getdepoController()
        {
            return this.depositionController;
        }
    }
}
