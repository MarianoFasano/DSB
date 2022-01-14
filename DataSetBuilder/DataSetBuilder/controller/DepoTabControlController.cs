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
        private String depoPath;                //percorso della deposizione --> percorso base degli esperimenti + la cartella dell'esperimento
        private String dataPath;                //percorso dei dati --> percorso base degli esperimenti + la cartella dell'esperimento + la cartella della deposizione
        private String basePath;                //percorso base degli esperimenti

        //Dizionari che contengono i riferimenti ai dati e alla struttura grafica dell'interfaccia che viene caricata nel tabItem dell'esperimento
        //Riferimento ai dati 
        private IDictionary<String, MyDepoData> depoDatas = new Dictionary<String, MyDepoData>();
        //Riferimento alla struttura dell'interfaccia grafica
        private IDictionary<String, DepoItemBody> depoStructures = new Dictionary<String, DepoItemBody>();

        private MyExpTabItemModel myExpTabItemModel;
        //TabControl cui verrà assegnato il tabControl della deposizione di riferimento ogniqualvolta si seleziona la tab di un esperimento
        private TabControl actualTabControl;

        //Controllers di ricerca
        //Ricerca dell'immagine
        ImageSearcher imageSearcher = new ImageSearcher();
        //Ricerca della temperatura
        PyrometerSearcher pyrometerSearcher = new PyrometerSearcher();
        //Ricerca dei dai nel CN
        CNCSearcher cncSearcher = new CNCSearcher();

        public DepoTabControlController(MyExpTabItemModel myExpTabItemModel, String basePath)
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
        //Inizializzazione degli eventi dei controlli dell'interfaccia grafica
        private void initControlsAction()
        {
            //this.playButton.Click += PlayButton_Click;                      //Evento sul bottone di play
            //this.pauseButton.Click += PauseButton_Click;                    //Evento sul bottone di pausa
            //this.prevButton.Click += PrevButton_Click;                      //Evento sul bottone per l'immagine precedente
            //this.nextButton.Click += NextButton_Click;                      //Evento sul bottone per l'immagine successiva                
            //this.searchMs.KeyDown += SearchMs_KeyDown;                      //Evento sulla pressione del tasto enter
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
                initLists(listViewItem);                
                //initFirstImage((string)listViewItem.Content);
                initControlsAction();
            }
        }

        private Boolean allowAdding(ListViewItem listViewItem)
        {
            int copyindex = 0;
            string temp = (string)listViewItem.Content;
            try
            {
                while (this.depoStructures.ContainsKey(temp))
                {
                    copyindex++;
                    temp = (string)listViewItem.Content + "(" + copyindex.ToString() + ")";
                }
                listViewItem.Content = temp;
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        private void initLists(ListViewItem listViewItem)
        {
            TabControl tabControl = myExpTabItemModel.getTabControl(getExpName());
            string depofolder = extractName((string)listViewItem.Content);
            this.dataPath = basePath + @"\" + depoPath + @"\" + depofolder;
            MyDepoData myDepoData = new MyDepoData(dataPath);
            depoDatas.Add((string)listViewItem.Content, myDepoData);

            CloseableTab tabItem = new CloseableTab();
            tabItem.Title = (string)listViewItem.Content;
            DepoItemBody depoItemBody = new DepoItemBody(this, myDepoData, dataPath);
            tabItem.Content = depoItemBody;
            this.actualTabControl.Items.Add(tabItem);
            depoStructures.Add((string)listViewItem.Content, depoItemBody);
            listViewItem.Content = extractName((string)listViewItem.Content);
        }

        public String getDepoName()
        {
            String removeString = basePath + @"\" + depoPath + @"\";
            int index = dataPath.IndexOf(removeString);
            string cleanPath = (index < 0) ? dataPath : dataPath.Remove(index, removeString.Length);
            return cleanPath;
        }
        private String getExpName()
        {
            String removeString = basePath + @"\";
            int index = depoPath.IndexOf(removeString);
            string cleanPath = (index < 0) ? depoPath : depoPath.Remove(index, removeString.Length);
            return cleanPath;
        }
        //Getter del dizionario delle myDepoData
        public IDictionary<String, MyDepoData> getDepoDatas()
        {
            return this.depoDatas;
        }

        //Set this.tabControl based on a key value
        public void setActualTabControl(String key)
        {
            this.actualTabControl = this.myExpTabItemModel.getTabControl(key);
            this.actualTabControl.SelectionChanged += TabControl_SelectionChanged;

            //Richiama il TabItem selezionato
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

        private string extractMs(string msString)
        {
            string ms = msString;
            int start = "ms".Length;
            ms = ms.Substring(start, ms.IndexOf("_")-2);
            return ms;
        }
        //Estrazione del nome della deposizione/esperimento escludendo la parte di indicazione del numero di copia
        private string extractName(string header)
        {
            //Per essere considerata una copia, la stringa deve contenere il simbolo "("
            if (header.Contains("("))
            {
                //In caso affermativo si estra dalla stringa tutto ciò che arriva prima della parentesi aperta
                int endIndex = header.IndexOf("(");
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