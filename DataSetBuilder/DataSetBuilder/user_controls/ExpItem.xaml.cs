using DataSetBuilder.controller;
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

namespace DataSetBuilder.user_controls
{
    /// <summary>
    /// Logica di interazione per ExpItem.xaml
    /// </summary>
    /// 

    /*
    User control - file xaml con relativa classe cs integrabili in altre interfacce
    */
    public partial class ExpItem : UserControl
    {
        //Variabile legata al TabControl delle deposizioni
        private DepoTabItem depoTabItem;

        private DepoTabControlController depoTabControlController;

        //Riferimenti ai percorsi delle deposizioni
        string basePath;
        string depoName;


        public ExpItem(DepoTabControlController depoTabControlController)
        {
            InitializeComponent();
            this.depoTabControlController = depoTabControlController;
        }
        //Settare la variabile DepoTabItem
        public void setDepoTabControl(DepoTabItem depoTabItem)
        {
            this.depoTabItem = depoTabItem;
        }
        //Torna il valore della depoTabItem
        public DepoTabItem getDepoTabItem()
        {
            return this.depoTabItem;
        }
        //Chiamata all'inizializzazione della lista delle deposizioni, memorizzando percorso fino alla deposizione e nome della deposizione
        internal void initDepoList(string depoName, string path)
        {
            this.basePath = path;
            this.depoName = depoName;
            Init(0);
        }
        //Inizializzazione della lista delle deposizioni
        private void Init(int number)
        {
            //Si ripulisce la listbox --> vale per l'aggiornamento
            DepositionViewer.Items.Clear();
            //Si ottiene la stringa della deposizione --> percorso base + nome della deposizione
            string depoPath = basePath + @"\" + depoName;
            //Si ottengono le cartelle nella directory in formato stringa indicata al metodo statico
            string[] depoDirectories = Directory.GetDirectories(depoPath);
            
            //Controllo se la lista di cartelle ne contiene, contenere cartelle significa che ci sono delle deposizioni
            if (isEmpty(depoDirectories.Length))
            {
                //Se l'array risulta vuoto si aggiunge il messaggio che indica all'utente che non sono presenti deposizioni per l'esperimento aperto
                DepositionViewer.Items.Add(emptyMessage());
            }
            else
            {
                //In caso di presenza di deposizioni, si cicla su di esse andando a creare un item per la lista e gli si aggiunge l'evento di apertura della deposizione gestita nella classe DepoTabControlController
                for (int i = 0; i < depoDirectories.Length; i++)
                {
                    var listItem = new ListViewItem();
                    string depoName = depoDirectories[i].Remove(0, depoPath.Length + 1);

                    //Istruzione switch che discerne fra la ricarica completa della lista e la ricarica dovuta alla ricerca dinamica di una deposizione
                    switch (number)
                    {
                        //Caso associato all'inizializzazione
                        case 0:
                            if (depoName.Contains("Deposition"))
                            {
                                listItem.Content = depoName;
                                listItem.MouseDoubleClick += depoTabControlController.openDepsData;
                                DepositionViewer.Items.Add(listItem);
                            }
                            break;
                        //Caso associato all'aggiornamento della lista dovuto alla ricerca nel campo di testo
                        case 1:
                            if (depoName.Contains("Deposition") && depoName.Contains(DepoSearchBox.Text))
                            {
                                listItem.Content = depoName;
                                listItem.MouseDoubleClick += depoTabControlController.openDepsData;
                                DepositionViewer.Items.Add(listItem);
                            }
                            break;
                    }
                }
            }            
        }
        //Controllo booleano se l'array contiene o meno elementi --> vuoto se lungo zero
        private bool isEmpty(int length)
        {
            if (length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Costruzione della label contenente il messaggio vuoto
        private Label emptyMessage()
        {
            var label = new Label();
            label.Content = "Nessuna deposizione trovata";
            label.HorizontalAlignment = HorizontalAlignment.Center;
            return label;
        }
        //Evento che si verifica quando si modifica il testo nella searchbox degli esperimenti --> in cima alla lista
        private void DepoSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Aggiornamento della lista
            Init(1);
        }
        //Aggiornamento della lista delle deposizioni, evento legato al clicksulla voce "Aggiorna" del menu "opzioni"
        private void UpdateDepoView_Click(object sender, RoutedEventArgs e)
        {
            Init(0);
        }
    }
}
