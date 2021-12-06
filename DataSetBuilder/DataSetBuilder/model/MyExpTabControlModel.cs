using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataSetBuilder.model
{
    //Classe di controllo usata per verificare che il TabItem sia o meno esistente prima di aggiungerlo al TabControl di riferimento
    class MyExpTabControlModel
    {
        private TabControl tabControl;
        private IDictionary items = new Dictionary<Object, TabItem>();

        //TODO: costruttore probabilmente da rivedere poiché l'attributo TabControl non è utilizzato
        public MyExpTabControlModel(TabControl tabControl)
        {
            this.tabControl = tabControl;
        }

        //La funzione si occupa di ritornare un valore boolean che indica se la TabItem è aggiungibile o meno al TabControl
        //In contemporanea è aggiunta al dizionario presente in questa classe
        public Boolean addItem(Object itemID, TabItem tabItem)
        {
            if (!items.Contains(itemID))
            {
                items.Add(itemID, tabItem);
                //tabControl.Items.Add(tabItem);
                return true;
            }
            return false;
        }
    }
}
