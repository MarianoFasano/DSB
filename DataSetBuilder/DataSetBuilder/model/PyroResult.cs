using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetBuilder.model
{
    //Classe usata per wrappare e tornare i valori del pirometro
    class PyroResult
    {
        //Variabili
        private String ms;              //stringa che contiene i ms
        private String temperature;     //string che contiene la temperatura

        //Costruttore
        public PyroResult(string pyroString)
        {
            if (pyroString.Equals("No value"))
            {
                this.ms = "No value";
                this.temperature = "No value";
            }
            else
            {
                this.ms = extractMsFromPyroString(pyroString);              //si imposta la stringa dei millisecondi estraendoli dalla stringa passata come parametro
                this.temperature = extractTempFromPyroString(pyroString);   //si imposta la stringa della temperatura estraendola dalla stringa passata come parametro
            }
        }

        //Ritorna la stringa dei millisecondi
        public String getTime()
        {
            return this.ms;
        }

        //Ritorna la stringa della tempaeratura
        public String getTemperature()
        {
            return this.temperature;
        }

        //La funzione estrae dalla stringa passata i ms e li restituisce come stringa --> dalla lista di stringhe del pirometro
        private string extractMsFromPyroString(string pyroString)
        {
            string local = pyroString;
            //Il +2 serve a "saltare" i caratteri "., L" poiché alcune righe contengono il punto ".", altre una lettera "C"...
            int start = local.IndexOf("Read:") + "Read:".Length + 2;
            //int start = "Read:.\t".Length;
            local = local.Substring(start);
            local = local.Substring(0, local.IndexOf("\t"));
            return local;
        }

        //Si estrae la temperatura dalla stringa passata come parametro
        private string extractTempFromPyroString(string pyroResult)
        {
            string temp = pyroResult;
            int start = temp.IndexOf("Read:.\t") + "Read:.\t".Length;
            temp = temp.Substring(start);
            string local = temp.Substring(0, temp.IndexOf("\t"));
            start = temp.IndexOf(local) + local.Length;
            temp = temp.Substring(start + "\t".Length);
            return temp;
        }
    }
}
