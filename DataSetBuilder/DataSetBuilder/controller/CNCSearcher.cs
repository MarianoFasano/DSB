using DataSetBuilder.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataSetBuilder.controller
{
    class CNCSearcher
    {
        //Counter che viene incrementato a ogni chiamata, gestito tramite la variabile bigO
        private short count = 0;
        //Counter che viene incrementato ogniqualvolta il limite sinistro è maggiore del destro
        private ulong LRCounter = 0;
        //Variabile di controllo del counter, calcolata in base alla "O grande" della binary search per liste sopra 1'000'000 elementi
        private short bigO = 23;
        //Variabile aggiunta al valore cercato quando non viene trovato
        private short offset = -1;

        //Funzione di ingresso che chiama le altre funzioni necessarie alla ricerca delle misurazioni nei file CN
        internal string cncSearch(long searchedValue, MyDepoData myDepoData)
        {
            List<string> CNCFileLines;
            //TODO: prima dell'assegnazione si verifica la versione del file system --> probabilmente da cambiare o eliminare come controllo
            if (myDepoData.checkOldVersion())
            {
                //CNCLines = File.ReadAllLines(myDepoData.getDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList();
                CNCFileLines = myDepoData.getCNList().ElementAt(0);
            }
            else
            {
                //CNCLines = File.ReadAllLines(myDepoData.getCNCFileDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList();
                CNCFileLines = myDepoData.getCNList().ElementAt(0);
            }
            //Si copia la lista per evitare sorprese --> modifiche del file originale
            List<String> cncReduce = new List<string>(CNCFileLines);
            //La lista viene ridotta togliendo le parti non utili ai fini della ricerca
            cncReduce = extractFromCNCList(cncReduce);
            //Stringa con i ms minimi, estratti dal nome della prima immagine
            string minMsValue = extractMs(myDepoData.getImages()[0]);
            //Richiamare la funzione ricorsiva di ricerca della stringa all'interno del file (versione lista di stringhe) del CN
            return cncLongBS(cncReduce, searchedValue, minMsValue);
        }
        //Funzione di ingresso alla ricorsione
        private string cncLongBS(List<string> cNCLines, long searchedValue, string minMsValue)
        {
            //Reset del counter di "O grande" e assegnazione dell'offset a -1
            resetCounter();
            resetLRCount();
            offset = -1;
            return cncLongBS(cNCLines, searchedValue, 0, cNCLines.Count() - 1, minMsValue);
        }
        //Funzione ricorsiva --> ritorna quando si trova i ms cercati, se non lo sono questi sono aumentati dell'offset
        private string cncLongBS(List<string> cNCLines, long searchedMs, int left, int right, string minMsValue)
        {
            //Aumento del conteggio, ogniqualvolta si esegue la ricorsione
            count++;
            //Verifica se il limite inferiore è maggiore di quello superiore
            if (left > right)
            {
                LRCounter++;
                resetCounter();
                //Verifica se il controllo left>right è stato verificato 100 volte
                if (LRCounter > 100)
                {
                    //Reset counter LR
                    resetLRCount();
                    //TODO: Ricorsione --> dopo varie prove l'offset è stato moltiplicato per un fattore 7, ma rimane da verificare il come mai si presenta questa necessità
                    return cncLongBS(cNCLines, searchedMs + (offset * 7), 0, cNCLines.Count() - 1, minMsValue);
                }
                return cncLongBS(cNCLines, searchedMs + offset, 0, cNCLines.Count() - 1, minMsValue);
            }

            int middle = (left + right) / 2;
            string originalElement = cNCLines.ElementAt(middle);
            string element = extractFromCNCLine(originalElement);

            //Se il valore cercato è minore del minimo si inverte l'offset --> il valore cercato aumenta anziché diminuire
            if ((searchedMs - long.Parse(minMsValue)) < 0)
            {
                //Cambiamento dell'offset (positivo), reset e ricorsione
                offset = 1;
                resetCounter();
                return cncLongBS(cNCLines, searchedMs + offset, 0, cNCLines.Count() - 1, minMsValue);
            }
            //Verifica se il conteggio è maggiore di "O grande"
            if (count > bigO)
            {
                resetCounter();
                return cncLongBS(cNCLines, searchedMs + offset, 0, cNCLines.Count() - 1, minMsValue);
            }

            if ((long.Parse(element) - long.Parse(minMsValue)) == (searchedMs - long.Parse(minMsValue)))
            {
                return originalElement;
            }
            //Verifica se il valore passato si trova nella parte sinistra o destra della lista
            else if ((long.Parse(element) - long.Parse(minMsValue)) > (searchedMs - long.Parse(minMsValue)))
            {
                return cncLongBS(cNCLines, searchedMs, left, middle - 1, minMsValue);
            }
            else
            {
                return cncLongBS(cNCLines, searchedMs, middle + 1, right, minMsValue);
            }
        }
        //La funzione estrae dalla stringa passata i ms e li restituisce come stringa --> dal nome dell'immagine
        private string extractMs(string msString)
        {
            //Copia locale del parametro
            string ms = msString;
            //Indice di inizio per l'estrazione dei ms
            int start = "ms".Length;
            //Estrazione, si sottrae 2 all'indice finale poiché in seguito ai test ho notato che il risultato conteneva due caratteri in più del voluto
            ms = ms.Substring(start, ms.IndexOf("_") - 2);
            return ms;
        }
        //La funzione estrae dalla stringa passata i ms e li restituisce come stringa --> dalla lista di stringhe del CN
        private string extractFromCNCLine(string originalElement)
        {
            string local = originalElement;
            int start = local.IndexOf("\t") + "\t".Length + 1;      //+1, jump the first line char
            local = local.Substring(start);
            local = local.Substring(0, local.IndexOf("\t"));
            return local;
        }

        //Esegue il reset del counter di comparazione con "O grande"
        private void resetCounter()
        {
            this.count = 0;
        }
        //Esegue il reset del counter LR --> quello che si occupa di contare quante volte il limite sinistro supera il destro
        private void resetLRCount()
        {
            this.LRCounter = 0;
        }
        //TODO: La funzione toglie gli elementi non utili durante la ricerca --> al momento hard-coded
        private List<string> extractFromCNCList(List<string> cNCLines)
        {
            //Copia del parametro per evitare di modificare la lista originale

            List<String> cncLines = new List<string>(cNCLines);
            cncLines.RemoveAt(0);
            return cncLines;
        }
    }
}
