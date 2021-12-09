using DataSetBuilder.user_controls;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace DataSetBuilder.view
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    /// 
    
    /*
    Classe collegata all'interfaccia grafica principale.
    Ogni componente del file xaml può essere richiamato se nello xaml viene specificato con x:Name="Nome variabile".
    La visibilità di queste componenti è public.
     */

    public partial class MainWindow : Window
    {
        //Dichiarazione della classe DSB_Controller (il suo utilizzo è specificato nella classe stessa)
        DSB_Controller dsb_controller;

        //Percorso della cartella contenente gli esperimenti, anch'essi sono delle cartelle
        string expPath = @"J:\DTI\_DSB";    //fisso Mariano
        //string expPath = @"D:\_DSB";      //portatile Mariano

        //Dichiarazione della classe TabsBody, la classe di riferimento del file xaml con il medesimo nome
        private TabsBody tabBody;

        //Costruttore della MainWindow, nel quale sono inizializzati i componenti della finestra e altre classi e funzioni necessarie
        public MainWindow()
        {
            InitializeComponent();
            //Inizializza la lista degli esperimenti
            Init();
            initTabControl();
            this.dsb_controller = new DSB_Controller(this.tabBody.TabsControl, expPath);
            //Massimizza la finestra
            this.WindowState = System.Windows.WindowState.Normal;
        }

        private void Init()
        {
            //Il metodo di classe ritorna la lista dei nomi delle directories contenute nel percorso specificato quale argomento
            string[] expDirectories = Directory.GetDirectories(expPath);
            //TODO: Ciclo per inizializzare la lista di esperimenti, con listViewItem (da modificare)
            for(int i = 0; i < expDirectories.Length; i++)
            {
                var listItem = new ListViewItem();
                //Si estrae dal nome della folder dell'esperimento il percorso, lasciando unicamente il nome dell'esperimento
                listItem.Content = expDirectories[i].Remove(0, expPath.Length + 1);
                //Alla ListViewItem si aggiunge l'evento openExpDeps (l'evento che permette di aprire la tab dell'esperimento)
                listItem.MouseDoubleClick += openExpDeps;
                listItem.Selected += ListItem_Selected;
                //Si aggiunge l'elemento della lista appena creato alla viewer degli esperimenti
                ExperimentViewer.Items.Add(listItem);
                
            }
        }
        //Funzione che carica il commento dell'esperimento nel DocumentViewer
        private void ListItem_Selected(object sender, RoutedEventArgs e)
        {
            ListViewItem listViewItem = sender as ListViewItem;
            //ExpComment.Source = new Uri("J:\\DTI\\_DSB\\Experiment_2021_9_14__11_13_42\\Experiment_2021_9_14__11_13_42.txt");
            //ExpComment.Document = "J:\\DTI\\_DSB\\Experiment_2021_9_14__11_13_42\\Experiment_2021_9_14__11_13_42.txt";

        }

        //Funzione che inizializza la classe TabsBody
        private void initTabControl()
        {
            this.tabBody = new TabsBody();
            //Impostazione del posizionamento della classe nella griglia dell'interfaccia
            Grid.SetRow(tabBody, 1);
            Grid.SetColumn(tabBody, 2);
            //La classe viene aggiunta alla griglia principale dell'applicazione nella posizione specificata (1, seconda riga; 2, terza colonna)
            DSB_MainGrid.Children.Add(tabBody);
        }
        //Evento openExpDeps, che cattura il doppio click del mouse sul nome dell'esperimento in lista e delega la gestione al metodo NewExpTabItem della classe DSB_Controller
        private void openExpDeps(object sender, EventArgs e)
        {
            ListViewItem listViewItem = sender as ListViewItem;
            //La funzione della classe DSB_Controller ritorna un oggetto TabsBody
            tabBody = dsb_controller.NewExpTabItem(tabBody, listViewItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Ridimensionamento della colonna contenente la lista degli esperimenti (se visibile la nasconde, e viceversa; anche se non avviene tramite la proprietà Visibility, ma con numeri)
            Column.Width = dsb_controller.columnWidth(Column);        
        }
        //Evento legato al click del mouse sul menuitem del commento dell'esperimento
        private void ViewExpCommentMenu_Click(object sender, RoutedEventArgs e)
        {
            //TODO: dopo aver commentato il dsb_controller
            ExpComment.Visibility = dsb_controller.viewComment(ExpComment);
            ViewCommentMenu.Header = dsb_controller.commentText(ExpComment);
        }

    }
}
