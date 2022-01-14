using DataSetBuilder.factories;
using DataSetBuilder.model;
using DataSetBuilder.user_controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DataSetBuilder.controller
{
    class ExpTabControlController
    {
        private MyExpTabControlModel myTabControlModel;
        private MyExpTabItemModel myExpTabItemModel;
        private MyExpTabItemFactory myExpTabItemFactory;
        private DepoTabControlController depoTabControlController;
        private TabControl mainTabControl;
        private string actualBasePath;

        public ExpTabControlController(TabControl tabControl, String basePath, MyExpTabItemModel myExpTabItemModel, DepoTabControlController depoTabControlController)
        {
            this.mainTabControl = tabControl;
            this.myTabControlModel = new MyExpTabControlModel(tabControl);
            this.myExpTabItemModel = myExpTabItemModel;
            this.myExpTabItemFactory = new MyExpTabItemFactory(basePath, depoTabControlController);
            this.depoTabControlController = depoTabControlController;
            //Si aggiunge l'evento della rilevazione della tab attiva al TabControl
            this.mainTabControl.SelectionChanged += TabControl_SelectionChanged;
        }

        //Funzione di controllo che verifica se il TabItem che si sta cercando di aggiungere esiste già
        private bool Contains(ListViewItem itemID)
        {
            if(myTabControlModel.Contains(itemID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void addItem(ListViewItem itemID, TabItem tabItem)
        {
            myTabControlModel.addItem(itemID, tabItem);
        }

        //Si crea il tabItem e si aggiunge al TabsBody passato come argomento, poi ritornato
        internal TabsBody createTabItem(TabsBody tabBody, ListViewItem listViewItem, string path)
        {
            //Controllo di aggiunta del TabItem, prosegue se non esiste ancora
            if (!Contains(listViewItem))
            {
                //Inizializzazione della struttura (user control) da aggiungere al tabItem dell'esperimento
                ExpItem expItem = new ExpItem();
                string tabheader = (string)listViewItem.Content;
                CloseableTab tabItem = myExpTabItemFactory.GetTabItem(extractName(tabheader), expItem.DepositionViewer, path);
                addItem(listViewItem, tabItem);
                //Creazione dell'user control che contiene il TabControl delle deposizioni (dove si aggiungeranno le tab delle varie deposizioni)
                //L'istanza della DepoTabItem viene poi inserita nella cella della griglia corretta (riga due, colonna uno e occupa due colonne)
                DepoTabItem depodataTabItem = new DepoTabItem();
                Grid.SetRow(depodataTabItem, 1);
                Grid.SetColumn(depodataTabItem, 0);
                Grid.SetColumnSpan(depodataTabItem, 2);
                //L'istanza della DepoTabItem viene aggiunta alla griglia della struttura della tabItem dell'esperimento, secondo le impostazioni sopra effettuate
                expItem.ExpGrid.Children.Add(depodataTabItem);
                //All'intestazione della tab si assegna il contenuto della listViewItem (il nome dell'esperimento)
                tabItem.Title = (string)listViewItem.Content;
                //Alla tabItem si assegna la struttura ExpItem
                tabItem.Content = expItem;
                //Al TabsControl si aggiunge l'item
                tabBody.TabsControl.Items.Add(tabItem);
                //Si aggiunge il TabControl delle deposizioni al dizionario di controllo con la rispettiva chiave (il nome dell'esperimento)
                this.myExpTabItemModel.addToDict((string)listViewItem.Content, depodataTabItem.DepoTabControl);
                //Si resetta il listviewitem, siccome è un riferimento
                listViewItem.Content = extractName((string)listViewItem.Content);
                //Si ritorna l'istanza tabBody debitamente aggiornata
                return tabBody;
            }
            else
            {
                //Se la tabItem esiste già, si restituisce l'argomento così come è stato passato
                return tabBody;
            }
        }

        //Funzione che rileva il cambiamento di selezione di tab
        //Preso Ctrl+C Ctrl+V dalla rete (FUNZIONA!)
        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                //Controllo dell'istanza, se si tratta di un CloseableTab allora si esegue il cast
                if (mainTabControl.SelectedItem is CloseableTab)
                {
                    CloseableTab tabItem = (CloseableTab)mainTabControl.SelectedItem;
                    //Mandatory check to avoid tabItem=null happened on drag&drop the tabItem
                    //Controllo obbligatorio per evitare che il drag&drop della tab produca un tabItem con riferimento null (ACCADEVA!)
                    if (tabItem != null)
                    {
                        //Mandatory to open the deposition in the correct tabControl
                        //Necessario affinché la deposizione sia aperta nel corretto TabControl
                        string header = (string)tabItem.Title;
                        if (header != null)
                        {
                            depoTabControlController.setActualTabControl(header);
                            header = extractName(header);
                            depoTabControlController.setDepoPath(header);
                        }
                    }
                }
            }
        }
        //Estrazione del nome --> si elimina la parte di copia (es (3))
        private string extractName(string header)
        {
            if (header.Contains("("))
            {
                int endIndex = header.IndexOf("(");
                string name = header.Substring(0, endIndex);
                return name;
            }
            else
            {
                return header;
            }
        }
    }
}