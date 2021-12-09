using DataSetBuilder.controller;
using DataSetBuilder.model;
using DataSetBuilder.user_controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DataSetBuilder
{
    //Classe di controllo della finestra principale
    //Alcune funzionalità le delega per evitare di sovraccaricarsi
    class DSB_Controller
    {
        private MyExpTabControlController myExpTabControlController;
        private MyExpTabItemModel myExpTabItemModel = new MyExpTabItemModel();
        private DepoTabControlController depoTabControlController;
        private double width = 0;

        //Costruttore della classe cui sono passati alcuni argomenti iniettati in seguito in altri costruttori
        public DSB_Controller(TabControl tabControl, String basePath)
        {            
            this.depoTabControlController = new DepoTabControlController(this.myExpTabItemModel, basePath);
            this.myExpTabControlController = new MyExpTabControlController(tabControl, basePath, this.myExpTabItemModel, this.depoTabControlController);
        }

        //Gestione della larghezza della colonna della lista degli esperimenti
        //La colonna assume in maniera predefinita la larghezza del contenuto (lunghezza della stringa)
        //Se la larghezza è 0 assume la larghezza iniziale, e viceversa
        //Ritorna quindi un oggetto GridLength che va a impostare la larghezza della colonna dell'interfaccia
        internal GridLength columnWidth(ColumnDefinition column)
        {
            //Variabile locale che assume il valore dell'attributo "width"
            Double width = column.ActualWidth;
            if (this.width == 0)
            {
                //Se tale attributo è ancora di valore zero, esso assume l'attuale valore della larghezza
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

        //Gestione della visibilità del commento sulla base del click del mouse sul menu dedicato
        internal System.Windows.Visibility viewComment(FrameworkElement ExpCommentBox)
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

        //Cambia il testo del menuitem legato al commento in base al fatto che sia attualmente visibile o meno
        internal string commentText(FrameworkElement ExpCommentBox)
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

        //La funzione chiamata nella classe MainWindow è delegata alla classe myExpTabControlController
        //Ritorna un oggetto TabsBody (l'argomento passato viene tornato modificato)
        internal TabsBody NewExpTabItem(TabsBody tabBody, ListViewItem listViewItem)
        {
            return myExpTabControlController.createTabItem(tabBody, listViewItem);
        }
    }
}
