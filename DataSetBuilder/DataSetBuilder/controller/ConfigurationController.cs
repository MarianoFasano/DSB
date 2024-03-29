﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows;
using System.Collections.Specialized;

namespace DataSetBuilder.controller
{
    //Classe che gestisce il file di configurazione
    class ConfigurationController
    {
        private Configuration configuration;
        private OrderedDictionary recentExperiments;
        private short size = 10;

        public ConfigurationController()
        {
            //chiamata necessaria per l'istanziazione dell'oggetto Configuration
            this.configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //Istanziazione del dizionario ordinato che permette di mantenere gli esperimenti aperti di recenti, nr=size
            this.recentExperiments = getDictFromConfig(configuration);
        }
        //Metodo che ritorna un dizionario ordinato partendo dal file di configurazione
        private OrderedDictionary getDictFromConfig(Configuration configuration)
        {
            OrderedDictionary temp = new OrderedDictionary();
            //Si esegue un ciclo su ogni chiave contenura nel file di configurazione
            foreach(string key in ConfigurationManager.AppSettings)
            {
                //Si controlla che la chiave non sia legata ai percorsi: predefinito --> "path" e temporaneo --> "temppath"
                if (!key.Equals("path") && !key.Equals("temppath"))
                {
                    //Si recupera il valore associato alla chiave e si imposta nel dizionario ordinato insieme alla chiave
                    string value = ConfigurationManager.AppSettings[key];
                    temp.Add(key, value);
                }
            }
            //Ritorna il dizionario
            return temp;
        }

        //Aggiungere esperimenti aperti di recente oppure il percorso degli esperimenti
        public void addSettings(string key, string value)
        {
            //La verifica avviene sul dizionario ordinato, che rispecchia il file di configurazione per quanto concerne gli esperimenti aperti di recente
            if (!recentExperiments.Contains(key) && !key.Equals("path"))
            {
                if(recentExperiments.Count >= size)
                {
                    //Dapprima rimuovo il valore più vecchio
                    removeOldestKeyValuePair();
                    add(key, value);
                }
                else
                {
                    //Il valore non esiste per la determinata chiave e non superiamo il numero massimo indicato
                    add(key, value);
                }
            }
            else if (recentExperiments.Contains(key) && !key.Equals("path"))
            {
                //Dapprima rimuovo il valore contenuto
                removeSettings(key);
                add(key, value);

            }
            else
            {
                //Controllo se sto configurando il percorso, anche temporaneo
                if (key.Equals("path"))
                {
                    //Si chiede se si vuole modificare il percorso predefinito oppure no
                    System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Desideri impostare il percorso come predefinito?", "Imposta percorso", MessageBoxButton.YesNo);
                    //Il percorso predefinito si cambia se la risposta è affermativa
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        //Si cambia il percorso predefinito nel file di configurazione
                        removeKeyValue(key);
                        add(key, value);
                        //Se si cambia il percorso predefinito allora si toglie l'eventuale percorso non predefinito ancora salvato, onde evitare di riavviare l'applicativo su di esso
                        if (containsTemppath())
                        {
                            string tempkey = "temppath";
                            removeKeyValue(tempkey);
                        }
                    }
                    //Altrimenti si cambia il percorso temporaneo
                    else if (dialogResult == System.Windows.Forms.DialogResult.No)
                    {
                        //Impostazione del percorso temporaneo
                        string temp = "temppath";
                        removeKeyValue(temp);
                        add(temp, value);
                    }
                }
            }
        }
        //Funzione che rimuove il valore più "vecchio" dal file di configurazione sfruttando il dizionario ordinato
        private void removeOldestKeyValuePair()
        {
            //Recupero delle chiavi in un array
            string[] keys = new string[recentExperiments.Count];
            recentExperiments.Keys.CopyTo(keys, 0);
            //Si rimuove dal dizionario il primo elemento
            recentExperiments.RemoveAt(0);
            //Dal file di configurazione si rimuove l'elemento con la chiave più vecchia, ricavata dalla lista di chiavi (la posizione 0 è la più vecchia)
            configuration.AppSettings.Settings.Remove(keys[0]);
            //Si salva il file di configurazione
            configuration.Save(ConfigurationSaveMode.Minimal);
            MessageBox.Show("Hello");
        }
        //Funzione che rimuove l'elemento dal file di configurazione con la chiave passata come parametro
        private void removeKeyValue(string key)
        {
            //Si rimuove l'elemento e si salva la configurazione
            configuration.AppSettings.Settings.Remove(key);
            configuration.Save(ConfigurationSaveMode.Minimal);
        }
        //Funzione che aggiunge chiave-valore al dizionario e al file di configurazione --> chiave-valore legati agli ultimi esperimenti aperti
        private void add(string key, string value)
        {
            //Si aggiungono i parametri al dizionario e al file di configurazione, poi si salva il file
            recentExperiments.Add(key, value);
            configuration.AppSettings.Settings.Add(key, value);
            configuration.Save(ConfigurationSaveMode.Minimal);
        }
        //Getter per il dizionario
        public OrderedDictionary getRecenteExperiments()
        {
            return this.recentExperiments;
        }
        //Ritorna il percorso predefinito come stringa dal file di configurazione
        public string getConfigPath()
        {
            string path = ConfigurationManager.AppSettings["path"];
            return path;
        }
        //Ritorna il percorso temporaneo come stringa dal file di configurazione
        public string getTempPath()
        {
            string path = ConfigurationManager.AppSettings["temppath"];
            return path;
        }
        //Verifica se il file di configurazione contiene il percorso temporaneo
        public bool containsTemppath()
        {
            string key = "temppath";
            string value = ConfigurationManager.AppSettings[key];
            if (value != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Verifica se il file di configurazione contiene il percorso predefinito
        public bool containspath()
        {
            string key = "path";
            string value = ConfigurationManager.AppSettings[key];
            if (value != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Rimuove gli esperimenti aperti di recente, se si elimina la directory dell'esperimento, oppure il percorso temporaneo degli esperimenti
        public void removeSettings(string key)
        {
            //La verifica avviene sul dizionario ordinato, che rispecchia il file di configurazione per quanto concerne gli esperimenti aperti di recente
            if (recentExperiments.Contains(key) && !key.Equals("path"))
            {
                //Se la chiave è contenuta nel dizionario si toglie sia dal dizionario sia dal file di configurazione
                recentExperiments.Remove(key);
                removeKeyValue(key);
            }
            else
            {
                //Si sta rimuovendo il percorso temporaneo, quindi si rimuove unicamente dal file di configurazione
                removeKeyValue(key);
            }
        }
    }
}