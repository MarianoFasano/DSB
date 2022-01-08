using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows;
using System.Collections.Specialized;

namespace DataSetBuilder.controller
{
    class ConfigurationController
    {
        private Configuration configuration;
        private OrderedDictionary recentExperiments;
        private short size = 10;

        public ConfigurationController()
        {
            this.configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            this.recentExperiments = getDictFromConfig(configuration);
        }

        private OrderedDictionary getDictFromConfig(Configuration configuration)
        {
            OrderedDictionary temp = new OrderedDictionary();
            foreach(string key in ConfigurationManager.AppSettings)
            {
                if (!key.Equals("path"))
                {
                    string value = ConfigurationManager.AppSettings[key];
                    temp.Add(key, value);
                }
            }
            return temp;
        }

        //Aggiungere esperimenti aperti di recente oppure il percorso degli esperimenti
        public void addSettings(string key, string value)
        {
            //configuration.AppSettings.Settings[key] == null
            if (!recentExperiments.Contains(key) && !key.Equals("path"))
            {
                if(recentExperiments.Count >= size)
                {
                    //Dapprima rimuovo il valore più vecchio
                    removeLast();
                    add(key, value);
                }
                else
                {
                    //Il valore non esiste per la determinata chiave e non superiamo il numero massimo indicato
                    add(key, value);
                }
            }
            else
            {
                //Controllo se sto configurando il percorso
                if (key.Equals("path"))
                {
                    System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Desideri impostare il percorso come predefinito?", "Imposta percorso", MessageBoxButton.YesNo);
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        //Si cambia il percorso predefinito nel file di configurazione
                        remove(key);
                        add(key, value);
                    }
                    else if (dialogResult == System.Windows.Forms.DialogResult.No)
                    {
                        //TODO: al momento rimane così poiché non è correttamente gestito il percorso ed è possibile aprire esperimenti da altri percorsi --> errori sui percorsi quando si selezionano altre tab
                        //Do Nothing
                        remove(key);
                        add(key, value);
                    }
                }
            }
        }

        private void removeLast()
        {

            List<string> keys = (List<string>)recentExperiments.Keys;
            recentExperiments.RemoveAt(0);
            configuration.AppSettings.Settings.Remove(keys[0]);
            configuration.Save(ConfigurationSaveMode.Minimal);
        }

        private void remove(string key)
        {
            configuration.AppSettings.Settings.Remove(key);
            configuration.Save(ConfigurationSaveMode.Minimal);
        }

        private void add(string key, string value)
        {
            recentExperiments.Add(key, value);
            configuration.AppSettings.Settings.Add(key, value);
            configuration.Save(ConfigurationSaveMode.Minimal);
        }

        public OrderedDictionary getRecenteExperiments()
        {
            return this.recentExperiments;
        }

        public string getConfigPath()
        {
            //string key = "path";
            string path = ConfigurationManager.AppSettings["path"];
            return path;
        }
    }
}