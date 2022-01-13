using DataSetBuilder.controller;
using DataSetBuilder.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DataSetBuilder.user_controls
{
    /// <summary>
    /// Logica di interazione per DepoItemBody.xaml
    /// </summary>
    /// 

    /*
    User control - file xaml con relativa classe cs integrabili in altre interfacce
    */
    public partial class DepoItemBody : UserControl
    {
        //Istanza necessaria per chiamare la funzione di ricerca, essa è passata nel costruttore della classe DepoItemBody
        private DepoTabControlController depoTabControlController;
        private MyDepoData myDepoData;
        private string path;
        //Variabili per la visione in automatico delle immagini
        private Boolean isAutomatic = false;
        private int timeMs = 1000;

        public DepoItemBody(DepoTabControlController depoTabControlController, MyDepoData myDepoData, string path)
        {
            InitializeComponent();
            //Assegnazione dell'istanza depotabcontrolController passata come parametro
            this.depoTabControlController = depoTabControlController;
            //L'interfaccia grafica, view, riceve il suo modello di riferimento
            this.myDepoData = myDepoData;
            this.path = path;
            this.FileBrowser.Source = new Uri(path);

            initFirstImage(path);
        }

        private void initFirstImage(String depoName)
        {
            BitmapImage bitmapImage;
            if (myDepoData.checkOldVersion())
            {
                bitmapImage = new BitmapImage(new Uri(path + @"\" + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
                SetImageSource(bitmapImage);
            }
            else
            {
                bitmapImage = new BitmapImage(new Uri(path + @"\" + myDepoData.getImageDirectory() + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
                SetImageSource(bitmapImage);
            }


            //Extract actual and max ms
            initMsLabels(myDepoData);

            PyroResult pyroResult = depoTabControlController.searchTemperature(long.Parse(this.ExtendActualMs.Text), myDepoData);
            CncResult cncResult = depoTabControlController.searchCncDatas(long.Parse(this.ExtendActualMs.Text), myDepoData);
            updateDatas(pyroResult, cncResult);
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


            SetMaxLabel(maxString);                                            //settaggio del testo dell'etichetta extMaxMs con la stringa dei ms massimi estratti dall'ultima immagine
            SetMinLabel(minString);                                            //settaggio del testo dell'etichetta extMinMs con la stringa dei ms minimi estratti dalla prima immagine
            SetActualMs(actual.ToString());                               //settaggio del testo dell'etichetta extActualMs con la stringa dei ms attuali estratti dall'attuale immagine
            SetSliderMax(long.Parse(maxString));                               //settaggio del valore massimo dello slider con la stringa dei ms massimi estratti dall'ultima immagine "castati" come long              
            SetSliderMin(min);                                                 //settaggio del valore minimo dello slider con la stringa dei ms minimi estratti dalla prima immagine "castati" come long
        }
        private string extractMs(string msString)
        {
            string ms = msString;
            int start = "ms".Length;
            ms = ms.Substring(start, ms.IndexOf("_") - 2);
            return ms;
        }
        //Pulisce e popola la lista dei dati ricavati dalla ricerca dei valori
        private void updateDatas(PyroResult pyroResult, CncResult cncResult)
        {
            //Pulisce gli attuali valori della lista
            DataList.Children.Clear();

            //Etichette da aggiungere alla lista pulita
            Label Ms = new Label();
            Label Temperature = new Label();
            //Si imposta il testo dell'etichetta, sia esso il valore dei millisecondi sia esso "No value"
            Ms.Content = "Ms:\t" + pyroResult.getTime();
            //Si imposta il testo dell'etichetta, sia esso il valore della temperatura sia esso "No value"
            Temperature.Content = "Temperature:\t" + pyroResult.getTemperature();
            //Aggiunge le etichette alla lista
            DataList.Children.Add(Ms);
            DataList.Children.Add(Temperature);

            //Aggiunge un separatore alla lista
            Separator separator = new Separator();
            DataList.Children.Add(separator);

            //Liste locali dei risultati della ricerca sul CN
            List<String> measures = cncResult.getMeasures();
            List<String> values = cncResult.getValues();

            //Si cicla sulle liste e per ogni grandezza si crea un'etichetta con testo il nome della grandezza e il valore misurato
            for (int i = 0; i < measures.Count; i++)
            {
                //Si crea una nuova etichetta
                Label label = new Label();
                //Testo dell'etichetta
                label.Content = measures[i] + ":\t" + values[i];
                //Si aggiunge l'etichetta alla lista
                DataList.Children.Add(label);
            }
        }

        //Evento collegato al click del bottone "<<" (indietro)
        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            //Se la componente FileBrowser (WebBrowser) è in grado di navigare indietro allora lo fa (massimo fino al percorso specificato in fase di inizializzazione)
            if (FileBrowser.CanGoBack)
            {
                FileBrowser.GoBack();
            }
        }

        //Evento collegato al click del bottone ">>" (avanti)
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            //Se la componente FileBrowser (WebBrowser) è in grado di navigare in avanti allora lo fa
            //(unicamente se si ha già navigato in avanti rispetto al percorso base, es. entrare in una cartella)
            if (FileBrowser.CanGoForward)
            {
                FileBrowser.GoForward();
            }
        }

        //Evento collegato al cursore dello slider, che richiama una funzione della classe DepoTabControlController
        private void ExtendMsSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //Variabile locale che ottiene il valore dallo slider --> cast a long poiché i valori predefiniti dello slider sono double
            long searchedValue = (long)ExtendMsSlider.Value;
            //Variabile locale che ottiene il nome della deposizione, utile per richiamare l'istanza MyDepoData da passare alla funzione di ricerca
            String depositionName = this.depoTabControlController.getDepoName();
            //Variabile locale che ottiene l'istanza myDepoData di riferimento
            MyDepoData myDepoData = this.depoTabControlController.getDepoDatas()[depositionName];
            //Ricerca del valore tramite l'istanza depoTabControlController
            msResearch(searchedValue);
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
            myDepoData.downActualImage();
            BitmapImage bitmapImage;
            if (myDepoData.checkOldVersion())
            {
                bitmapImage = new BitmapImage(new Uri(path + @"\" + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
                SetImageSource(bitmapImage);
            }
            else
            {
                bitmapImage = new BitmapImage(new Uri(path + @"\" + myDepoData.getImageDirectory() + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
                SetImageSource(bitmapImage);
            }
            setMsLabels();
            long actualMs = long.Parse(this.ExtendActualMs.Text);
            PyroResult pyroResult = depoTabControlController.searchTemperature(actualMs, myDepoData);
            CncResult cncResult = depoTabControlController.searchCncDatas(actualMs, myDepoData);
            updateDatas(pyroResult, cncResult);
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
            nextImage();            
        }
        //Avvio della riproduzione automatica delle immagini
        private async void PlayImage_Click(object sender, RoutedEventArgs e)
        {                        
            isAutomatic = true;
            //Loop di aggiornamento dell'immagine
            while(isAutomatic)
            {
                int ratio = getSpeed();             //velocità ricavata dal combobox
                nextImage();
                await Task.Delay(timeMs/ratio);
                if (areImageEnd())
                {
                    isAutomatic = false;
                }
            }
        }
        //Stop alla riproduzione automatica delle immagini
        private void PauseImage_Click(object sender, RoutedEventArgs e)
        {
            isAutomatic = false;
        }
        //Controllo se le immagini da mostrare sono terminate oppure no
        private bool areImageEnd()
        {
            if(myDepoData.getActualImage() == myDepoData.getMaxNrImage())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        internal void SetImageSource(BitmapImage bitmapImage)
        {
            DepoImage.Source = bitmapImage;
        }
        //Funzione per estrarre il valore numerico dal combobox: 2x --> 2
        public int getSpeed()
        {
            ComboBoxItem item = (ComboBoxItem)ImageSpeed.SelectedItem;
            string value = item.Content.ToString();
            int endIndex = value.IndexOf("x");
            int ratio = Int32.Parse(value.Substring(0, endIndex));
            return ratio;
        }

        internal void SetMaxLabel(string maxString)
        {
            ExtendMaxMs.Content = maxString;
        }

        internal void SetMinLabel(string minString)
        {
            ExtendMinMs.Content = minString;
        }

        internal void SetActualMs(string actualvalue)
        {
            ExtendActualMs.Text = actualvalue;
        }

        internal void SetSliderMax(long maxvalue)
        {
            ExtendMsSlider.Maximum = maxvalue;
        }

        internal void SetSliderMin(long minvalue)
        {
            ExtendMsSlider.Minimum = minvalue;
        }
        private void nextImage()
        {
            myDepoData.upActualImage();
            BitmapImage bitmapImage;
            if (myDepoData.checkOldVersion())
            {
                //Ritorna il percorso al file partendo dal path
                string filename = myDepoData.getImages().ElementAt((int)myDepoData.getActualImage());
                bitmapImage = new BitmapImage(new Uri(path + @"\" + filename, UriKind.RelativeOrAbsolute));
                SetImageSource(bitmapImage);
            }
            else
            {
                //Ritorna il percorso al file della versione vecchia partendo dal path, c'è un cartella in più da superare
                string filename = myDepoData.getImageDirectory() + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage());
                bitmapImage = new BitmapImage(new Uri(path + @"\" + filename, UriKind.RelativeOrAbsolute));
                SetImageSource(bitmapImage);
            }

            setMsLabels();
            long actualMs = long.Parse(this.ExtendActualMs.Text);
            PyroResult pyroResult =depoTabControlController.searchTemperature(actualMs, myDepoData);
            CncResult cncResult = depoTabControlController.searchCncDatas(actualMs, myDepoData);
            updateDatas(pyroResult, cncResult);
        }
        private void setMsLabels()
        {
            string actualMs = myDepoData.getImages()[(int)myDepoData.getActualImage()];
            long actual = long.Parse(extractMs(actualMs));

            SetActualMs(actual.ToString());
        }
        //Evento che avviene quando si schiaccia "enter"
        private void SearchMs_KeyDown(object sender, KeyEventArgs e)
        {
            //Si deve premere "Enter"
            if (e.Key == Key.Return)
            {
                //Stringa in locale con il valore del box di ricerca
                string searchedValString = this.SearchMs.Text;
                //La stringa deve contenere solo numeri
                if (searchedValString.All(char.IsDigit))
                {
                    //Cast a long della stringa e si richiama l'istanza che contiene i riferimenti correti ai files
                    long searchedValue = long.Parse(searchedValString);

                    //TODO:Si verifica che il valore passato si trovi nel corretto intervallo di valori --> slider min - slider max
                    //sistemare!
                    if (long.Parse(searchedValString) >= long.Parse(extractMs(myDepoData.getImages()[0])) && long.Parse(searchedValString) <= long.Parse(extractMs(myDepoData.getImages()[myDepoData.getImages().Count - 1])))
                    {
                        //Funzione di ricerca
                        msResearch(searchedValue);
                    }
                    else
                    {
                        //Altrimenti significa che il valore è fuori intervallo, si mostra quindi un popup che indica il corretto formato
                        String firstFormat = this.ExtendMsSlider.Minimum.ToString() + "-" + this.ExtendMsSlider.Maximum.ToString();
                        MessageBox.Show("I formati numerici associati sono i seguenti:\n\n" + firstFormat, "Formato errato");
                    }
                }
                else
                {
                    //Si ricorda all'utente che si devono cercare unicamente dei numeri
                    MessageBox.Show("Nel campo di ricerca devono essere scritti unicamente dei numeri", "Formato errato");
                }
                //Si pulisce la casella della ricerca
                this.SearchMs.Clear();
            }
        }
        //Funzione di ricerca --> all'interno si ricerca immagine, temperatura e altri dati
        public void msResearch(long searchedValue)
        {
            //TODO: Ricerca l'immagine --> pare non andare
            string resultImage = depoTabControlController.searchImage(searchedValue, myDepoData);
            setImage(resultImage);
            //Ricerca della temperatura --> file pirometro
            PyroResult pyroResult = depoTabControlController.searchTemperature(searchedValue, myDepoData);
            //Ricerca dei dati misurati e riportati nel CN --> file CN
            CncResult cncResult = depoTabControlController.searchCncDatas(searchedValue, myDepoData);
            //Funzione che popola la colonna dei dati nell'interfaccia utente
            updateDatas(pyroResult, cncResult);
        }

        private void setImage(string filename)
        {
            BitmapImage bitmapImage;
            myDepoData.setActualImage(myDepoData.getImages().IndexOf(filename));

            if (myDepoData.checkOldVersion())
            {
                String imageName = myDepoData.getImages().ElementAt((int)myDepoData.getActualImage());
                String imagePath = path + @"\" + imageName;
                bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                SetImageSource(bitmapImage);
            }
            else
            {
                String imageName = myDepoData.getImages().ElementAt((int)myDepoData.getActualImage());
                String directoryName = myDepoData.getImageDirectory();
                String imagePath = path + @"\" + directoryName + imageName;
                bitmapImage = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                SetImageSource(bitmapImage);
            }

            setMsLabels();
        }
    }
}
