using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataSetBuilder.model
{
    //Classe che gestisce la memorizzazione i tabcontrol degli esperimenti in base alla stringa chiave (che altro non è che il nome dell'esperimento)
    //In questo modo è possibile ritornare il TabControl correlato al rispettivo esperimento
    public class MyExpTabItemModel
    {
        //Dizionario con chiave: nome esperimento, valore:TabControl
        private IDictionary<String, TabControl> dataItems = new Dictionary<String, TabControl>();

        //Funzione che aggiunge la coppia chiave-valore al dizionario, se non esiste
        public void addToDict(String key, TabControl value)
        {
            if (!this.dataItems.ContainsKey(key))
            {
                this.dataItems.Add(key, value);
            }
        }

        //Funzione che ritorna il TabControl in base alla chiave passata
        public TabControl getTabControl(String key)
        {
            if (dataItems.ContainsKey(key))
            {
                return this.dataItems[key];
            }
            return null;
        }
    }
}
