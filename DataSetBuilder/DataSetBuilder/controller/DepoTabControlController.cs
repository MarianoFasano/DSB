using DataSetBuilder.model;
using DataSetBuilder.user_controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DataSetBuilder.controller
{
    public class DepoTabControlController
    {

        //Altre variabili
        //Percorsi
        private string depoPath;                //percorso della deposizione --> percorso base degli esperimenti + la cartella dell'esperimento
        private string dataPath;                //percorso dei dati --> percorso base degli esperimenti + la cartella dell'esperimento + la cartella della deposizione
        private string basePath;                //percorso base degli esperimenti

        private string errorMessage = "Possibili problemi:\n\n" +
                                      " - la deposizione non contiene file/directory nel corretto formato\n\n" +
                                      " - la deposizione contiene i simboli proibiti '(' o ')' nel nome";

        //I caratteri "proibiti" nel nome dell'esperimento
        private char forbiddenSymbol = '(';
        private char forbiddenSymbol2 = ')';

        //Dizionari che contengono i riferimenti ai dati e alla struttura grafica dell'interfaccia che viene caricata nel tabItem dell'esperimento
        //Riferimento ai dati 
        private IDictionary<String, MyDepoData> depoDatas = new Dictionary<String, MyDepoData>();
        //Riferimento alla struttura dell'interfaccia grafica
        private IDictionary<String, DepoItemBody> depoStructures = new Dictionary<String, DepoItemBody>();

        private ExpTabControlModel myExpTabItemModel;
        //TabControl cui verrà assegnato il tabControl della deposizione di riferimento ogniqualvolta si seleziona la tab di un esperimento
        private TabControl actualTabControl;

        //Controllers di ricerca
        //Ricerca dell'immagine
        ImageSearcher imageSearcher = new ImageSearcher();
        //Ricerca della temperatura
        PyrometerSearcher pyrometerSearcher = new PyrometerSearcher();
        //Ricerca dei dai nel CN
        CNCSearcher cncSearcher = new CNCSearcher();

        public DepoTabControlController(ExpTabControlModel myExpTabItemModel, String basePath)
        {
            //Nel costruttore si inietta l'istanza di myExpTabItemModel, creata all'apertura dell'applicativo
            this.myExpTabItemModel = myExpTabItemModel;
            //Percorso base passato come parametro
            this.basePath = basePath;            
        }
        //Evento legato alla tab selezionata dell'esperimento, quando ciò avviene sono caricati alcuni dati relativi alle deposizioni dell'esperimento selezionato
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (actualTabControl.SelectedItem is CloseableTab)
                {           
                    CloseableTab tabItem = (CloseableTab)actualTabControl.SelectedItem;
                    String header = (string)tabItem.Title;

                    //Mandatory check to avoid tabItem=null happened on drag&drop the tabItem
                    if (tabItem != null && header != null)
                    {
                        //Si richiama la depoItemBody di riferimento dalla struttura dati per l'esperimento selezionato
                        DepoItemBody depoItemBody = this.depoStructures[header];
                        //Verifica che essa non sia null
                        if (depoItemBody != null)
                        {
                            //Si assegnano i relativi controlli (bottoni, slider, ecc...) e il percorso dell'esperimento nel file system
                            //assignIControl(depoItemBody);
                            setDataPath(header);
                        }
                    }
                }
            }
        }

        //Funzione per la ricerca/settaggio dell'immagine --> parametri: valore cercato, istanza contenente i vari dati/riferimenti
        public string searchImage(long searchedValue, MyDepoData myDepoData)
        {
            //Stringa del risultato della ricerca nella lista delle immagini
            string result = imageSearcher.longSearch(searchedValue, myDepoData);
            //Imposta l'immagine in base alla stringa risultante dalla ricerca
            //setImage(result);
            return result;
        }
        //Funzione per la ricerca della temperatura
        public PyroResult searchTemperature(long searchedValue, MyDepoData myDepoData)
        {
            PyroResult pyroResult;
            //Se la lista di file relativa al pirometro contiene elementi, si ricerca e si estrae la temperatura
            if (myDepoData.getPyrometerList().Any())
            {
                //Si ricava la riga del file in corrispondenza dei ms passati
                string pyroString = pyrometerSearcher.pyroSearch(searchedValue, myDepoData);
                //Si estrae la temperatura dalla riga
                pyroResult = new PyroResult(pyroString);
            }
            else
            {
                //Nel caso la lista controllata non contiene alcun elemento, la stringa della temperatura viene impostata su "No value"
                pyroResult = new PyroResult("No value");
            }
            return pyroResult;
        }
        //Funzione per la ricerca dei dati nel file CN --> ritorna un'istanza della classe CncResult che contiene le liste con le grandezze misurate e quella con i rispettivi valori misurati
        public CncResult searchCncDatas(long searchedValue, MyDepoData myDepoData)
        {
            CncResult cncResult = new CncResult();
            //Verifica della presenza di elementi nella lista dei file CN (una lista di liste di stringhe)
            if (myDepoData.getCNList().Any())
            {
                //TODO: sostituire non appena si esegue il refactoring del problema I/O
                String measureString;
                if (myDepoData.checkOldVersion())
                {
                    //measureString = File.ReadAllLines(myDepoData.getDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList().ElementAt(0);
                    measureString = myDepoData.getCNList().ElementAt(0).ElementAt(0);
                }
                else
                {
                    //measureString = File.ReadAllLines(myDepoData.getCNCFileDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList().ElementAt(0);
                    measureString = myDepoData.getCNList().ElementAt(0).ElementAt(0);
                }
                //La funzione imposta la lista delle grandezze di misura estrapolate dalla prima riga del file del CN
                cncResult.settingMeasure(measureString);
                //Si ricerca la stringa che si riferisce al valore passato come parametro
                string stringCncResult = cncSearcher.cncSearch(searchedValue, myDepoData);
                //La funzione imposta la lista dei valori misurati estrapolati dalla stringa risultante della ricerca
                cncResult.settingValues(stringCncResult);
                
            }
            else
            {
                //Nel caso la lista dei file del CN fosse vuota si impostano delle grandezze e dei valori su "No values"
                cncResult.getMeasures().Add("No Cnc File");
                cncResult.getValues().Add("No values");
            }
            return cncResult;
        }


        //Imposta la variabile del percorso della deposizione in base alla stringa passata come parametro
        public void setDepoPath(String path)
        {
            this.depoPath = path;
        }
        //Imposta la variabile del percorso dei dati in base alla stringa passata come parametro
        public void setDataPath(String path)
        {
            //La stringa del percorso si basa sul percorso di base e su quello della deposizione
            this.dataPath = basePath + @"\" + depoPath + @"\" + path;
        }

        internal void openDepsData(object sender, MouseButtonEventArgs e)
        {
                ListViewItem listViewItem = sender as ListViewItem;
                init(listViewItem);            
        }

        private void init(ListViewItem listViewItem)
        {
            if (allowAdding(listViewItem))
            {
                try
                {
                    initLists(listViewItem);
                }
                catch (DirectoryNotFoundException e)
                {
                    MessageBox.Show(errorMessage, e.Message);
                }
                catch(ArgumentOutOfRangeException e)
                {
                    MessageBox.Show(errorMessage, e.Message);
                }
            }
        }
        //Verifica se è possibile aggiungere/aprire la deposizione
        private bool allowAdding(ListViewItem listViewItem)
        {
            //Indice che indica il numero di copie
            int copyindex = 0;
            //La stringa rappresenta il contenuto testuale, il nome, dell'item della deposizione che si cerca di aprire
            string temp = (string)listViewItem.Content;
            try
            {
                //Fintanto che il dizionario delle strutture contiene la chiave, ossia nome deposizione + eventuale postfisso a indicarne il numero aperto
                while (this.depoStructures.ContainsKey(temp))
                {
                    //Si incrementa l'indice e si riassegna alla variabile temp il nome della deposizione con l'indice della copia come postfisso
                    copyindex++;
                    temp = (string)listViewItem.Content + "(" + copyindex.ToString() + ")";
                }
                //Necessario riassegnarlo come contenuto dell'item per la successiva inizializzazione dei dati: altrimenti il contenuto dell'item sarebbe già impiegato come chiave di riferimento
                listViewItem.Content = temp;
                return true;
            }
            //Se vi fossero eccezioni di qualsiasi tipo, ritornerà false
            catch (Exception exception)
            {
                return false;
            }
        }
        //Inizializza i dizionari
        private void initLists(ListViewItem listViewItem)
        {
            try
            {
                //Ritorna il TabControl corretto
                TabControl tabControl = myExpTabItemModel.getTabControl(getExpName());
                //Si assegna il percorso
                string depofolder = extractName((string)listViewItem.Content);
                this.dataPath = basePath + @"\" + depoPath + @"\" + depofolder;
                //Si inizializzano i dati passando il percorso in cui sono memorizzati
                MyDepoData myDepoData = new MyDepoData(dataPath);
                //Si aggiunge il modello dati al dizionario
                depoDatas.Add((string)listViewItem.Content, myDepoData);
                //Si crea un Tab chiudibile, cui si assegna lo stesso nome dell'item della lista che ha scaturito l'evento iniziale
                CloseableTab tabItem = new CloseableTab();
                tabItem.Title = (string)listViewItem.Content;
                //Si crea la struttura grafica che contiene i valori passati dal modello dati
                DepoItemBody depoItemBody = new DepoItemBody(this, myDepoData, dataPath);
                //La struttura grafica è assegnata alla tab chiudibile creata in precedenza
                tabItem.Content = depoItemBody;
                //Si aggiunge la tab al tabcontrol
                this.actualTabControl.Items.Add(tabItem);
                //Si aggiunge la struttura grafica al dizionario dedicato
                depoStructures.Add((string)listViewItem.Content, depoItemBody);
                //L'item selezionato riprende il nome originale, senza postfissi dovuti alla copia
                listViewItem.Content = extractName((string)listViewItem.Content);
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show("Sono presenti alcuni errori: controllare il contenuto della deposizione che si sta aprendo.\nVerificare il numero di immagini provino presenti, Ground Control se ne aspetta una!", exception.Message);
            }
        }
        //Rimuove le parti del percorso in modo da ritornare il nome della deposizione
        public string getDepoName()
        {
            //stringa da rimuovere
            string removeString = basePath + @"\" + depoPath + @"\";
            int index = dataPath.IndexOf(removeString);
            string cleanPath = (index < 0) ? dataPath : dataPath.Remove(index, removeString.Length);
            return cleanPath;
        }
        //Ritornare le parti del percoso in modo da ritornare il nome dell'esperimento
        private string getExpName()
        {
            //stringa da rimuovere
            string removeString = basePath + @"\";
            int index = depoPath.IndexOf(removeString);
            string cleanPath = (index < 0) ? depoPath : depoPath.Remove(index, removeString.Length);
            return cleanPath;
        }
        //Getter del dizionario delle myDepoData
        public IDictionary<String, MyDepoData> getDepoDatas()
        {
            return this.depoDatas;
        }

        //Assegna this.tabControl in base al valore della chiave passata
        public void setActualTabControl(string key)
        {
            this.actualTabControl = this.myExpTabItemModel.getTabControl(key);
            this.actualTabControl.SelectionChanged += TabControl_SelectionChanged;

            //Richiama il TabItem selezionato
            if (actualTabControl.SelectedItem is CloseableTab)
            {
                CloseableTab tabItem = (CloseableTab)actualTabControl.SelectedItem;
                string header = (string)tabItem.Title;

                //Mandatory check to avoid tabItem=null happened on drag&drop the tabItem
                if (tabItem != null && header != null)
                {
                    //Si richiama la depoItemBody di riferimento dalla struttura dati per l'esperimento selezionato
                    DepoItemBody depoItemBody = this.depoStructures[header];
                    //Verifica che essa non sia null
                    if (depoItemBody != null)
                    {
                        //Si assegnano i relativi controlli (bottoni, slider, ecc...) e il percorso dell'esperimento nel file system
                        //assignIControl(depoItemBody);
                        setDataPath(header);
                    }
                }
            }
        }

        //Estrazione del nome della deposizione/esperimento escludendo la parte di indicazione del numero di copia
        private string extractName(string header)
        {
            //Per essere considerata una copia, la stringa deve contenere il simbolo "(" e il simbolo ")"
            if (header.Contains(forbiddenSymbol) && header.Contains(forbiddenSymbol2))
            {
                //In caso affermativo si estra dalla stringa tutto ciò che arriva prima della parentesi aperta
                int endIndex = header.IndexOf(forbiddenSymbol);
                string name = header.Substring(0, endIndex);
                return name;
            }
            else
            {
                return header;
            }
        }
    }
}