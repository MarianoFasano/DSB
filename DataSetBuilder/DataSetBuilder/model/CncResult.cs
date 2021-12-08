using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataSetBuilder.model
{
    //Classe usata per wrappare e tornare i valori del cnc
    class CncResult
    {
        //Lista che contiene le grandezze misurate durante l'esperimento
        private List<String> measures = new List<string>();
        //Lista che contiene i valori delle misure
        private List<String> values = new List<string>();

        //Funzione che, passata la stringa risultato, estrae i valori e li aggiunge alla lista
        public void settingValues(String result)
        {
            String temp = result;
            String value;
            int endValue;

            do
            {
                //Indice che indica la fine del valore (carattere tab)
                endValue = temp.IndexOf("\t") + "\t".Length;
                if (endValue <= 0)
                {
                    endValue = temp.Length;
                }
                //Si estrae il valore, in formato stringa, dalla stringa risultato passata alla funzione
                value = temp.Substring(0, endValue);
                //Si toglie la stringa del valore appena estratto dalla stringa di risultato passata alla funzione
                temp = temp.Substring(endValue);

                if (value.Any(char.IsLetterOrDigit))
                {
                    values.Add(value);
                }
                if (temp.Equals(value))
                {
                    break;
                }
                //Verificare se la string a contiene ancora numeri o lettere, in caso affermativo si continua a estrarre valori
            } while (temp.Any(char.IsDigit) || temp.Any(char.IsLetter));;
        }

        public void settingMeasure(String result)
        {
            String temp = result;
            String value;
            int endValue;

            do
            {
                //Indice che indica la fine del valore (carattere tab)
                endValue = temp.IndexOf("\t") + "\t".Length;
                if (endValue <= 0)
                {
                    endValue = temp.Length;
                }
                //Si estrae il valore, in formato stringa, dalla stringa risultato passata alla funzione
                value = temp.Substring(0, endValue);
                //Si toglie la stringa del valore appena estratto dalla stringa di risultato passata alla funzione
                temp = temp.Substring(endValue);

                if (value.Any(char.IsLetter))
                {
                    measures.Add(value);
                }
                if (temp.Equals(value))
                {
                    break;
                }
                //Verificare se la string a contiene ancora numeri o lettere, in caso affermativo si continua a estrarre valori
            } while (temp.Any(char.IsLetter));
        }
        //Ritorna la lista con le grandezze misurate
        public List<String> getMeasures()
        {
            return this.measures;
        }
        //Ritorna la lista con i valori
        public List<String> getValues()
        {
            return this.values;
        }

    }
}