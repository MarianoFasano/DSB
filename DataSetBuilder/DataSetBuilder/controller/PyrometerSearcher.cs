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
    //La classe si occupa della ricerca della temperatura in base ai ms passati
    class PyrometerSearcher
    {
        //Counter che viene incrementato a ogni chiamata, gestito tramite la variabile bigO
        private short count = 0;
        //Counter che viene incrementato ogniqualvolta il limite sinistro è maggiore del destro
        private ulong LRCounter = 0;
        //Variabile di controllo del counter, calcolata in base alla "O grande" della binary search per liste sopra 1'000'000 elementi
        private short bigO = 23;
        //Variabile aggiunta al valore cercato quando non viene trovato
        private short offset = -1;

        //Funzione di ingresso che chiama le altre funzioni necessarie alla ricerca della temperatura
        internal string pyroSearch(long searchedValue, MyDepoData myDepoData)
        {
            List<string> pyroLines;
            //TODO: prima dell'assegnazione si verifica la versione del file system --> probabilmente da cambiare o eliminare come controllo
            if (myDepoData.checkOldVersion())
            {
                //pyroLines = File.ReadAllLines(myDepoData.getDirectory() + @"\" + myDepoData.getPyrometerList()[0]).Cast<string>().ToList();
                pyroLines = myDepoData.getPyrometerList();
            }
            else
            {
                //pyroLines = File.ReadAllLines(myDepoData.getPyroFileDirectory() + @"\" + myDepoData.getPyrometerList()[0]).Cast<string>().ToList();
                pyroLines = myDepoData.getPyrometerList();
            }

            //La lista viene ridotta togliendo le parti non utili ai fini della ricerca
            pyroLines = extractFromPyroList(pyroLines);
            //Stringa con i ms minimi, estratti dal nome della prima immagine
            string minMsValue = extractMs(myDepoData.getImages()[0]);
            //Richiamare la funzione di ricerca della stringa all'interno del file (versione lista di stringhe) del pirometro
            return pyroLongBS(pyroLines, searchedValue, minMsValue);
        }
        //Funzione di ingresso alla ricorsione
        private string pyroLongBS(List<string> pyroLines, long searchedMs, string minMsValue)
        {
            //Reset del counter di "O grande" e assegnazione dell'offset a -1
            resetCounter();
            resetLRCount();
            offset = -1;
            return pyroLongBS(pyroLines, searchedMs, 0, pyroLines.Count() - 1, minMsValue);
        }
        //Funzione ricorsiva --> ritorna quando si trova i ms cercati, se non lo sono questi sono aumentati dell'offset
        private string pyroLongBS(List<string> pyroLines, long searchedMs, int left, int right, string minMsValue)
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
                    return pyroLongBS(pyroLines, searchedMs + (offset * 7), 0, pyroLines.Count() - 1, minMsValue);
                }
                return pyroLongBS(pyroLines, searchedMs + offset, 0, pyroLines.Count() - 1, minMsValue);
            }
            //Indice mediano, su cui si basa la binary search
            int middle = (left + right) / 2;
            string originalElement = pyroLines.ElementAt(middle);
            string element = extractFromPyroLine(originalElement);

            //Se il valore cercato è minore del minimo si inverte l'offset --> il valore cercato aumenta anziché diminuire
            if ((searchedMs - long.Parse(minMsValue)) < 0)
            {
                //Cambiamento dell'offset (positivo), reset e ricorsione
                offset = 1;
                resetCounter();
                return pyroLongBS(pyroLines, searchedMs + offset, 0, pyroLines.Count() - 1, minMsValue);
            }
            //Verifica se il conteggio è maggiore di "O grande"
            if (count > bigO)
            {
                resetCounter();
                return pyroLongBS(pyroLines, searchedMs + offset, 0, pyroLines.Count() - 1, minMsValue);
            }
            //Se il valore cercato è uguale a quello analizzato si ritorna la stringa completa di riferimento
            if ((long.Parse(element) - long.Parse(minMsValue)) == (searchedMs - long.Parse(minMsValue)))
            {
                return originalElement;
            }
            //Verifica se il valore passato si trova nella parte sinistra o destra della lista
            else if ((long.Parse(element) - long.Parse(minMsValue)) > (searchedMs - long.Parse(minMsValue)))
            {
                return pyroLongBS(pyroLines, searchedMs, left, middle - 1, minMsValue);
            }
            else
            {
                return pyroLongBS(pyroLines, searchedMs, middle + 1, right, minMsValue);
            }
        }
        //La funzione estrae dalla stringa passata i ms e li restituisce come stringa --> dalla lista di stringhe del pirometro
        private string extractFromPyroLine(string element)
        {
            string local = element;
            //Il +2 serve a "saltare" i caratteri "., L" poiché alcune righe contengono il punto ".", altre una lettera "C"...
            int start = local.IndexOf("Read:") + "Read:".Length + 2;
            //int start = "Read:.\t".Length;
            local = local.Substring(start);
            local = local.Substring(0, local.IndexOf("\t"));
            return local;
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

        //Si estrae la temperatura dalla stringa passata come parametro
        private string extractTemp(string pyroResult)
        {
            string temp = pyroResult;
            int start = temp.IndexOf("Read:.\t") + "Read:.\t".Length;
            temp = temp.Substring(start);
            string local = temp.Substring(0, temp.IndexOf("\t"));
            start = temp.IndexOf(local) + local.Length;
            temp = temp.Substring(start + "\t".Length);
            return temp;
        }

        //TODO: La funzione toglie gli elementi non utili durante la ricerca --> al momento hard-coded
        //APPUNTO: Eventualmente creare una copia del parametro
        private List<string> extractFromPyroList(List<string> original)
        {
            //Copia del parametro per evitare di modificare la lista originale
            List<String> pyroLines = new List<string>(original);

            //Hard coded, need refactoring
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            return pyroLines;
        }
    }
}