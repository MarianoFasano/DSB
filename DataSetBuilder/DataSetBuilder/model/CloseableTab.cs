using DataSetBuilder.user_controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DataSetBuilder.model
{
    //Classe TabItem su misura per il bottone di chiusura Tab
    class CloseableTab : TabItem
    {
        //Variabili
        private CloseableHeader closeableTabHeader;
        // Costruttore
        public CloseableTab()
        {
            // Crea un'istanza dello usercontrol
            closeableTabHeader = new CloseableHeader();
            //Assegna l'usercontrol creato all'header della tab
            this.Header = closeableTabHeader;

            // Attach to the CloseableHeader events
            // (Mouse Enter/Leave, Button Click, and Label resize)
            closeableTabHeader.button_close.MouseEnter += new MouseEventHandler(button_close_MouseEnter);
            closeableTabHeader.button_close.MouseLeave += new MouseEventHandler(button_close_MouseLeave);
            closeableTabHeader.button_close.Click += new RoutedEventHandler(button_close_Click);
            closeableTabHeader.label_TabTitle.SizeChanged += new SizeChangedEventHandler(label_TabTitle_SizeChanged);
        }

        /// <span class="code-SummaryComment"><summary>
        //</span>/// Property - Set the Title of the Tab
        /// <span class="code-SummaryComment"></summary>
        //</span>
        public string Title
        {
            set
            {
                ((CloseableHeader)this.Header).label_TabTitle.Content = value;
            }
            get
            {
                return (string)((CloseableHeader)this.Header).label_TabTitle.Content;
            }
        }

        // Override OnSelected - Show the Close Button
        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Visible;
        }
        // Override OnUnSelected - Hide the Close Button
        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Hidden;
        }
        // Override OnMouseEnter - Show the Close Button
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Visible;
        }
        // Override OnMouseLeave - Hide the Close Button (If it is NOT selected)
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!this.IsSelected)
            {
                ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Hidden;
            }
        }

        // Button MouseEnter - When the mouse is over the button - change color to Red
        void button_close_MouseEnter(object sender, MouseEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Foreground = Brushes.Red;
        }
        // Button MouseLeave - When mouse is no longer over button - change color back to black
        void button_close_MouseLeave(object sender, MouseEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Foreground = Brushes.Black;
        }
        // Button Close Click - Remove the Tab - (or raise
        // an event indicating a "CloseTab" event has occurred)
        //TODO: gestire la rimozione della tab, sia esperimento sia essa deposizione --> es. un dispatcher (o qualche pattern)
        void button_close_Click(object sender, RoutedEventArgs e)
        {
            ((TabControl)this.Parent).Items.Remove(this);
        }
        // Label SizeChanged - When the Size of the Label changes
        // (due to setting the Title) set position of button properly
        void label_TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Margin = new Thickness(
               ((CloseableHeader)this.Header).label_TabTitle.ActualWidth + 5, 3, 4, 0);
        }
    }
}
