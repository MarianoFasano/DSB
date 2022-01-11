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
        //Variabili per la visione in automatico delle immagini
        private Boolean isAutomatic = false;
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        private CancellationToken token;

        public DepoItemBody(DepoTabControlController depoTabControlController)
        {
            InitializeComponent();
            //Assegnazione dell'istanza depotabcontrolController passata come parametro
            this.depoTabControlController = depoTabControlController;
            //Inizializzazione del token per l'interruzione del task per lo scorrimento automatico delle immagini
            this.token = cancelTokenSource.Token;
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
            this.depoTabControlController.msResearch(searchedValue, myDepoData);
        }

        private void PrevImage_Click(object sender, RoutedEventArgs e)
        {
                depoTabControlController.PrevButton_Click();         
        }

        private void NextImage_Click(object sender, RoutedEventArgs e)
        {
                depoTabControlController.NextButton_Click();
            
        }

        private void PlayImage_Click(object sender, RoutedEventArgs e)
        {
            depoTabControlController.PlayButton_Click();            
        }

        private void PauseImage_Click(object sender, RoutedEventArgs e)
        {
                depoTabControlController.PauseButton_Click();
           
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
    }
}
