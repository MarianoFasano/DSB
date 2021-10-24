﻿using System;
using System.Collections.Generic;
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
    }
}