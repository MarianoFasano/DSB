using DataSetBuilder.model;
using DataSetBuilder.user_controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DataSetBuilder.controller
{
    public class DepoTabControlController
    {
        /*Object from graphical interface*/
        //Generali
        private Button playButton;              //bottone play
        private Button pauseButton;             //bottone pausa
        private Button prevButton;              //bottone immagine precedente
        private Button nextButton;              //bottone immagine successiva
        private ComboBox imageSpeed;            //gestione "velocità" scorrimento immagini (play/pausa)
        private Image depoImage;                //immagine
        private StackPanel datasStackPanel;     //lista per i dati
        private TextBox searchMs;               //campo di testo per la ricerca dei ms

        //Versione estesa dei ms
        private TextBox extActualMs;            //campo di testo con i ms attuali
        private Label extMinMs;                 //etichetta co i ms minimi (ms della prima immagine)
        private Label extMaxMs;                 //etichetta con i ms totali (ms dell'ultima immagine)
        private Slider extSliderMs;             //slider dei ms

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

        private Boolean isAutomatic = false;
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
                            assignIControl(depoItemBody);
                            setDataPath(header);
                        }
                    }
                }
            }
        }
        //Inizializzazione degli eventi dei controlli dell'interfaccia grafica
        private void initControlsAction()
        {
            this.playButton.Click += PlayButton_Click;                      //Evento sul bottone di play
            this.pauseButton.Click += PauseButton_Click;                    //Evento sul bottone di pausa
            this.prevButton.Click += PrevButton_Click;                      //Evento sul bottone per l'immagine precedente
            this.nextButton.Click += NextButton_Click;                      //Evento sul bottone per l'immagine successiva                
            this.searchMs.KeyDown += SearchMs_KeyDown;                      //Evento sulla pressione del tasto enter
        }

        //Evento che avviene quando si schiaccia "enter" --> fa partire la ricerca del valore passato al box di ricerca
        private void SearchMs_KeyDown(object sender, KeyEventArgs e)
        {
            //Si deve premere "Enter"
            if(e.Key == Key.Return)
            {
                //Stringa in locale con il valore del box di ricerca
                string searchedValString = this.searchMs.Text;
                //La stringa deve contenere solo numeri
                if (searchedValString.All(char.IsDigit))
                {
                    //Cast a long della stringa e si richiama l'istanza che contiene i riferimenti correti ai files
                    long searchedValue = long.Parse(searchedValString);
                    MyDepoData myDepoData = depoDatas[getDepoName()];
                
                    //TODO:Si verifica che il valore passato si trovi nel corretto intervallo di valori --> slider min - slider max
                    //sistemare!
                    if (long.Parse(searchedValString) >= long.Parse(extractMs(myDepoData.getImages()[0])) && long.Parse(searchedValString) <= long.Parse(extractMs(myDepoData.getImages()[myDepoData.getImages().Count - 1])))
                    {
                        //Funzione di ricerca
                        msResearch(searchedValue, myDepoData);
                    }
                    else
                    {
                        //Altrimenti significa che il valore è fuori intervallo, si mostra quindi un popup che indica il corretto formato
                        String firstFormat = this.extSliderMs.Minimum.ToString() + "-" + this.extSliderMs.Maximum.ToString();
                        MessageBox.Show("I formati numerici associati sono i seguenti:\n\n" + firstFormat, "Formato errato");
                    }
                }
                else
                {
                    //Si ricorda all'utente che si devono cercare unicamente dei numeri
                    MessageBox.Show("Nel campo di ricerca devono essere scritti unicamente dei numeri", "Formato errato");
                }
                //Si pulisce la casella della ricerca
                this.searchMs.Clear();
            }
        }
        //Funzione di ricerca --> all'interno si ricerca immagine, temperatura e altri dati
        public void msResearch(long searchedValue, MyDepoData myDepoData)
        {
            //TODO: Ricerca l'immagine --> pare non andare
            searchImage(searchedValue, myDepoData);
            //Ricerca della temperatura --> file pirometro
            PyroResult pyroResult = searchTemperature(searchedValue, myDepoData);
            //Ricerca dei dati misurati e riportati nel CN --> file CN
            CncResult cncResult = searchCncDatas(searchedValue, myDepoData);        
            //Funzione che popola la colonna dei dati nell'interfaccia utente
            updateDatas(pyroResult, cncResult);
        }
        //Funzione per la ricerca/settaggio dell'immagine --> parametri: valore cercato, istanza contenente i vari dati/riferimenti
        private void searchImage(long searchedValue, MyDepoData myDepoData)
        {
            //Stringa del risultato della ricerca nella lista delle immagini
            string result = imageSearcher.longSearch(searchedValue, myDepoData);
            //Imposta l'immagine in base alla stringa risultante dalla ricerca
            setImage(result);
        }
        //Funzione per la ricerca della temperatura
        private PyroResult searchTemperature(long searchedValue, MyDepoData myDepoData)
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
        private CncResult searchCncDatas(long searchedValue, MyDepoData myDepoData)
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



        //Pulisce e popola la lista dei dati ricavati dalla ricerca dei valori
        private void updateDatas(PyroResult pyroResult, CncResult cncResult)
        {
            //Pulisce gli attuali valori della lista
            datasStackPanel.Children.Clear();

            //Etichette da aggiungere alla lista pulita
            Label Ms = new Label();
            Label Temperature = new Label();
            //Si imposta il testo dell'etichetta, sia esso il valore dei millisecondi sia esso "No value"
            Ms.Content = "Ms:\t" + pyroResult.getTime();
            //Si imposta il testo dell'etichetta, sia esso il valore della temperatura sia esso "No value"
            Temperature.Content = "Temperature:\t" + pyroResult.getTemperature();
            //Aggiunge le etichette alla lista
            datasStackPanel.Children.Add(Ms);
            datasStackPanel.Children.Add(Temperature);

            //Aggiunge un separatore alla lista
            Separator separator = new Separator();
            datasStackPanel.Children.Add(separator);

            //Liste locali dei risultati della ricerca sul CN
            List<String> measures = cncResult.getMeasures();
            List<String> values = cncResult.getValues();
            
            //Si cicla sulle liste e per ogni grandezza si crea un'etichetta con testo il nome della grandezza e il valore misurato
            for(int i = 0; i < measures.Count; i++)
            {
                //Si crea una nuova etichetta
                Label label = new Label();
                //Testo dell'etichetta
                label.Content = measures[i] + ":\t" + values[i];
                //Si aggiunge l'etichetta alla lista
                datasStackPanel.Children.Add(label);
            }
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
                initFirstImage((string)listViewItem.Content);
                initControlsAction();
            }
        }

        private Boolean allowAdding(ListViewItem listViewItem)
        {
            if (!this.depoStructures.ContainsKey((string)listViewItem.Content))
            {
                return true;
            }
            return false;
        }

        private void initLists(ListViewItem listViewItem)
        {
            TabControl tabControl = myExpTabItemModel.getTabControl(getExpName());

            CloseableTab tabItem = new CloseableTab();
            tabItem.Title = (string)listViewItem.Content;
            DepoItemBody depoItemBody = new DepoItemBody(this);
            depoItemBody.FileBrowser.Source = new Uri(basePath + @"\" + depoPath + @"\" + (string)listViewItem.Content);
            tabItem.Content = depoItemBody;
            tabControl.Items.Add(tabItem);
            depoStructures.Add((string)listViewItem.Content, depoItemBody);
            this.dataPath = basePath + @"\" + depoPath + @"\" + (string)listViewItem.Content;
            assignIControl(depoItemBody);
            depoDatas.Add((string)listViewItem.Content, new MyDepoData(dataPath));
            this.actualTabControl = tabControl;
        }
        /*
         Initialize the firts image.
         By the deposition name (the key), in origin listviewitem.Content in the experiment depositions, the method search the myDepoData value in the dictionary and return it.
         This myDepoData is used to get the followed informations: name of the image directory, the list of images name, the actual image by int index.
         The informations are used to open a new BitMapImage with the image correct Uri and assign this image to the source of the actual Image container showed by the interface.
         */
        private void initFirstImage(String depoName)
        {
            MyDepoData myDepoData = depoDatas[getDepoName()];
            BitmapImage bitmapImage;
            if (myDepoData.checkOldVersion())
            {
                bitmapImage = new BitmapImage(new Uri(dataPath + @"\"  + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
                this.depoImage.Source = bitmapImage;
            }
            else
            {
                bitmapImage = new BitmapImage(new Uri(dataPath + @"\" + myDepoData.getImageDirectory() + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
                this.depoImage.Source = bitmapImage;
            }
            

            //Extract actual and max ms
            initMsLabels(myDepoData);

            PyroResult pyroResult = searchTemperature(long.Parse(this.extActualMs.Text), myDepoData);
            CncResult cncResult = searchCncDatas(long.Parse(this.extActualMs.Text), myDepoData);
            updateDatas(pyroResult, cncResult);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            nextImage();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            MyDepoData myDepoData = depoDatas[getDepoName()];
            myDepoData.downActualImage();
            BitmapImage bitmapImage;
            if (myDepoData.checkOldVersion())
            {
                bitmapImage = new BitmapImage(new Uri(dataPath + @"\" + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
                this.depoImage.Source = bitmapImage;
            }
            else
            {
                bitmapImage = new BitmapImage(new Uri(dataPath + @"\" + myDepoData.getImageDirectory() + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
                this.depoImage.Source = bitmapImage;
            }
            setMsLabels(myDepoData);
            long actualMs = long.Parse(this.extActualMs.Text);
            PyroResult pyroResult = searchTemperature(actualMs, myDepoData);
            CncResult cncResult = searchCncDatas(actualMs, myDepoData);
            updateDatas(pyroResult, cncResult);

        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bottone pausa", "Bottone schiacciato");
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Bottone play", "Bottone schiacciato");
        }

        private void nextImage()
        {
            MyDepoData myDepoData = depoDatas[getDepoName()];
            myDepoData.upActualImage();
            BitmapImage bitmapImage;
            if (myDepoData.checkOldVersion())
            {
                bitmapImage = new BitmapImage(new Uri(dataPath + @"\" + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
                this.depoImage.Source = bitmapImage;
            }
            else
            {
                bitmapImage = new BitmapImage(new Uri(dataPath + @"\" + myDepoData.getImageDirectory() + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
                this.depoImage.Source = bitmapImage;
            }

            setMsLabels(myDepoData);
            long actualMs = long.Parse(this.extActualMs.Text);
            PyroResult pyroResult = searchTemperature(actualMs, myDepoData);
            CncResult cncResult = searchCncDatas(actualMs, myDepoData);
            updateDatas(pyroResult, cncResult);

        }

        private void setImage(string filename)
        {
            MyDepoData myDepoData = depoDatas[getDepoName()];
            BitmapImage bitmapImage;
            myDepoData.setActualImage(myDepoData.getImages().IndexOf(filename));

            if (myDepoData.checkOldVersion())
            {
                String imageName = myDepoData.getImages().ElementAt((int)myDepoData.getActualImage());
                String imagePath = dataPath + @"\" + imageName;
                bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                this.depoImage.Source = bitmapImage;
            }
            else
            {
                String imageName = myDepoData.getImages().ElementAt((int)myDepoData.getActualImage());
                String directoryName = myDepoData.getImageDirectory();
                String imagePath = dataPath + @"\" + directoryName + imageName;
                bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                this.depoImage.Source = bitmapImage;
            }
            
            setMsLabels(myDepoData);
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

        //Assign the correct buttons to the controller and re-init the button actions
        private void assignIControl(DepoItemBody depoItemBody)
        {
            //General
            this.playButton = depoItemBody.PlayImage;
            this.pauseButton = depoItemBody.PauseImage;
            this.prevButton = depoItemBody.PrevImage;
            this.nextButton = depoItemBody.NextImage;
            this.imageSpeed = depoItemBody.ImageSpeed;
            this.depoImage = depoItemBody.DepoImage;
            this.datasStackPanel = depoItemBody.DataList;
            //Significant version of ms
            this.searchMs = depoItemBody.SearchMs;
            //Extendend version of ms
            this.extActualMs = depoItemBody.ExtendActualMs;
            this.extMinMs = depoItemBody.ExtendMinMs;
            this.extMaxMs = depoItemBody.ExtendMaxMs;
            this.extSliderMs = depoItemBody.ExtendMsSlider;
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
                        assignIControl(depoItemBody);
                        setDataPath(header);
                    }
                }
            }
        }

        //responsabilità del controller o del mydepodata? --> spostare in mydepodata
        private void setMsLabels(MyDepoData myDepoData)
        {
            string maxMs = myDepoData.getImages()[myDepoData.getImages().Count - 1];
            string actualMs = myDepoData.getImages()[(int)myDepoData.getActualImage()];
            string minMs = myDepoData.getImages()[0];

            string maxString = extractMs(maxMs);
            string actualString = extractMs(actualMs);
            string minString = extractMs(minMs);

            long max = long.Parse(maxString);
            long actual = long.Parse(extractMs(actualMs));
            long min = long.Parse(minString);

            minString = extractDifferentDigit(minString, maxString);

            this.extActualMs.Text = actual.ToString();
        }
        private string extractDifferentDigit(String min, String max)
        {
            String minString = min;
            String maxString = max;

            var maxArray = maxString.ToArray();
            var minArray = minString.ToArray();

            for (int i = 0; i < maxString.Length; i++)
            {
                if ((maxArray[i] != minArray[i]))
                {
                    minString = minString.Substring(i);
                    break; ;
                }
            }
            return minString;
        }

        private void initMsLabels(MyDepoData myDepoData)
        {
            string maxMs = myDepoData.getImages()[myDepoData.getImages().Count - 1];
            string actualMs = myDepoData.getImages()[(int)myDepoData.getActualImage()];
            string minMs = myDepoData.getImages()[0];

            string maxString = extractMs(maxMs);
            string actualString = extractMs(actualMs);
            string minString = extractMs(minMs);

            long max = long.Parse(maxString);
            long actual = long.Parse(extractMs(actualMs));
            long min = long.Parse(minString);
/*
 *          Obsoleta, la funzionalità estraeva i ms significativi
 * 
            var maxArray = maxString.ToArray();
            var minArray = minString.ToArray();

            for (int i = 0; i < maxString.Length; i++)
            {
                if ((maxArray[i] != minArray[i]))
                {
                    minString = minString.Substring(i);
                    break;
                }
            }
*/
            this.extMaxMs.Content = maxString;                                              //settaggio del testo dell'etichetta extMaxMs con la stringa dei ms massimi estratti dall'ultima immagine
            this.extMinMs.Content = minString;                                              //settaggio del testo dell'etichetta extMinMs con la stringa dei ms minimi estratti dalla prima immagine
            this.extActualMs.Text = actual.ToString();                                      //settaggio del testo dell'etichetta extActualMs con la stringa dei ms attuali estratti dall'attuale immagine
            this.extSliderMs.Maximum = long.Parse(maxString);                               //settaggio del valore massimo dello slider con la stringa dei ms massimi estratti dall'ultima immagine "castati" come long              
            this.extSliderMs.Minimum = min;                                                 //settaggio del valore minimo dello slider con la stringa dei ms minimi estratti dalla prima immagine "castati" come long
        }

        private string extractMs(string msString)
        {
            string ms = msString;
            int start = "ms".Length;
            ms = ms.Substring(start, ms.IndexOf("_")-2);
            return ms;
        }
    }
}