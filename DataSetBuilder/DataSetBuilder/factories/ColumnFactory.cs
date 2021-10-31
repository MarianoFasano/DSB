using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DataSetBuilder.factories
{
    class ColumnFactory
    {
        public ColumnDefinition getNewColumn()
        {
            return createColumn();
        }

        private ColumnDefinition createColumn()
        {
            ColumnDefinition column = new ColumnDefinition();
            column.Width = new GridLength(1, GridUnitType.Star);
            return column;
        }
    }
}
