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
        private IDictionary<string, TabControl> dataItems = new Dictionary<string, TabControl>();
        //Dizionario con chiave: nome esperimento, valore:stackpanel con la lista dei device
        private IDictionary<string, ListBox> onlineDeviceList = new Dictionary<string, ListBox>();

        //Funzione che aggiunge la coppia chiave-valore al dizionario dei TabControl, se non esiste
        public void addTabControlToDict(string key, TabControl value)
        {
            if (!this.dataItems.ContainsKey(key))
            {
                this.dataItems.Add(key, value);
            }
        }

        //Funzione che ritorna il TabControl in base alla chiave passata
        public TabControl getTabControl(string key)
        {
            if (dataItems.ContainsKey(key))
            {
                return this.dataItems[key];
            }
            return null;
        }

        //Funzione che aggiunge la coppia chiave-valore al dizionario dei device online, se non esiste
        public void addDeviceSetupToDict(string key, ListBox value)
        {
            if (!this.onlineDeviceList.ContainsKey(key))
            {
                this.onlineDeviceList.Add(key, value);
            }
        }

        //Funzione che ritorna lo Stackpanel con la lista dei setup dei device in base alla chiave passata
        public ListBox getDeviceSetupList(string key)
        {
            if (onlineDeviceList.ContainsKey(key))
            {
                return this.onlineDeviceList[key];
            }
            return null;
        }
    }
}
