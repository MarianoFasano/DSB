using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DataSetBuilder.model
{
    //Classe di controllo usata per verificare che il TabItem sia o meno esistente prima di aggiungerlo al TabControl di riferimento
    class MyExpTabControlModel
    {
        private TabControl tabControl;
        private IDictionary items = new Dictionary<string, TabItem>();

        //TODO: costruttore probabilmente da rivedere poiché l'attributo TabControl non è utilizzato
        public MyExpTabControlModel(TabControl tabControl)
        {
            this.tabControl = tabControl;
        }

        //In contemporanea è aggiunta al dizionario presente in questa classe
        public void addItem(ListViewItem itemID, TabItem tabItem)
        {
            items.Add((string)itemID.Content, tabItem);
        }
        //La funzione si occupa di ritornare un valore boolean che indica se la TabItem è aggiungibile o meno al TabControl
        public bool Contains(ListViewItem itemID)
        {
            int copyindex = 0;
            string temp = (string)itemID.Content;
            try
            {
                while (items.Contains(temp))
                {
                    copyindex++;
                    temp = (string)itemID.Content + "(" + copyindex.ToString() + ")";
                }
                itemID.Content = temp;
                return false;
            }
            catch(Exception exception)
            {
                return true;
            }
        }
    }
}
