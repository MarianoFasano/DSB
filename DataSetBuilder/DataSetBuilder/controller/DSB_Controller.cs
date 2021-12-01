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
        private double width = 0;

        public DSB_Controller(TabControl tabControl, String basePath)
        {            
            this.depoTabControlController = new DepoTabControlController(this.myExpTabItemModel, basePath);
            this.myExpTabControlController = new MyExpTabControlController(tabControl, basePath, this.myExpTabItemModel, this.depoTabControlController);
        }

        //TODO: refactoring, if needed
        internal GridLength columnWidth(ColumnDefinition column)
        {
            Double width = column.ActualWidth;
            if (this.width == 0)
            {
                this.width = width;
            }
            if (width == this.width)
            {
                width = 0;
                return new GridLength(width);
            }
            else if (width < this.width && width > 0)
            {
                width = 0;
                return new GridLength(width);
            }
            else
            {
                width = this.width;
                return new GridLength(width);
            }
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
