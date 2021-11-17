using DataSetBuilder.controller;
using DataSetBuilder.model;
using DataSetBuilder.user_controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DataSetBuilder
{
    class DSB_Controller
    {
        private MyExpTabControlController myExpTabControlController;
        private MyExpTabItemModel myExpTabItemModel = new MyExpTabItemModel();
        private DepoTabControlController depoTabControlController;

        public DSB_Controller(TabControl tabControl, String basePath)
        {            
            this.depoTabControlController = new DepoTabControlController(this.myExpTabItemModel, basePath);
            this.myExpTabControlController = new MyExpTabControlController(tabControl, basePath, this.myExpTabItemModel, this.depoTabControlController);
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

        internal TabsBody NewExpTabItem(TabsBody tabBody, ListViewItem listViewItem)
        {
            return myExpTabControlController.createTabItem(tabBody, listViewItem);
        }
    }
}
