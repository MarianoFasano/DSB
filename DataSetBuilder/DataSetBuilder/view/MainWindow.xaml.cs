using DataSetBuilder.controller;
using DataSetBuilder.model;
using DataSetBuilder.user_controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace DataSetBuilder.view
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    /// 
    
    /*
    Classe collegata all'interfaccia grafica principale.
    Ogni componente del file xaml può essere richiamato se nello xaml viene specificato con x:Name="Nome variabile".
    La visibilità di queste componenti è public.
     */

    public partial class MainWindow : Window
    {
        //Variabili
        ConfigurationController configurationController = new ConfigurationController();
        private ExpTabControlController myExpTabControlController;
        private ExpTabControlModel myExpTabItemModel = new ExpTabControlModel();
        private DepoTabControlController depoTabControlController;

        //Stringa del percorso root degli esperimenti
        string expPath;
        //Percorso immagine predefinita
        string defaultImage = @"/Immagini/GC_addImage.png";
        //Larghezza per la colonna degli esperimenti
        private double width = 0;
        //Dichiarazione della classe TabsBody, la classe di riferimento del file xaml con il medesimo nome
        private TabsBody tabBody;

        //Domanda cambiamento immagine
        string questionDialog = "Desideri cambiare immagine di \"Provino\"?";

        //Testo in caso di percorso esperimenti non consono
        string noExpDetected = "Non sono presenti esperimenti nella lista! \nSeleziona il percorso degli esperimenti dal menu file.\nNota: in lista appariranno unicamente le cartelle che contengono la parola 'Experiment' nel nome.";

        //Costruttore della MainWindow, nel quale sono inizializzati i componenti della finestra e altre classi e funzioni necessarie
        public MainWindow()
        {
            InitializeComponent();
            ConfigurationInit();
            //Inizializza la lista degli esperimenti
            Init(0);
            initTabControl();
            this.depoTabControlController = new DepoTabControlController(this.myExpTabItemModel, expPath);
            this.myExpTabControlController = new ExpTabControlController(this.tabBody.TabsControl, expPath, this.myExpTabItemModel, this.depoTabControlController);
            //NON Massimizza la finestra
            this.WindowState = System.Windows.WindowState.Normal;
        }
        //Inizializzazione dal file di configurazione
        private void ConfigurationInit()
        {
            //Configurazione percorso
            GetPathFromConfig();
            //Configurazione esperimenti recenti
            GetRecentExperimentFromConfig();
        }
        //Ripresa dei dati sugli esperimenti aperti di recente dal file di configurazione
        private void GetRecentExperimentFromConfig()
        {
            updateRecentExpMenu();
        }

        //Ripresa dei dati sul percorso dal file di configurazione
        private void GetPathFromConfig()
        {
            //Verifica se il file di configurazione contiene un percorso segnato come temporaneo
            if (configurationController.containsTemppath())
            {
                //Imposta il percorso dell'applicativo al percorso temporaneo di configurazione 
                this.expPath = configurationController.getTempPath();
            }
            //Verifica se il file di configurazione contiene un percorso segnato come predefinito
            else if (configurationController.containspath())
            {
                //Imposta il percorso dell'applicativo al percorso predefinito di configurazione 
                this.expPath = configurationController.getConfigPath();
            }
            else
            {
                //Richiama la funzione per impostare un nuovo percorso se non sono trovati ne quello predefinito ne quello temporaneo
                changeExpPath();
            }
        }

        private void Init(int number)
        {
            //Pulizia della lista dei listviewitems, altrimenti rimangono presenti i precedenti items
            ExperimentViewer.Items.Clear();
            string[] expDirectories;
            //Il metodo di classe ritorna la lista dei nomi delle directories contenute nel percorso specificato quale argomento
            try
            {
                expDirectories = Directory.GetDirectories(expPath);
                //TODO: Ciclo per inizializzare la lista di esperimenti, con listViewItem (da modificare)
                for (int i = 0; i < expDirectories.Length; i++)
                {
                    var listItem = new ListViewItem();
                    //Si estrae dal nome della folder dell'esperimento il percorso, lasciando unicamente il nome dell'esperimento
                    string folderName = expDirectories[i].Remove(0, expPath.Length + 1);

                    //Switch per gestire l'inizializzazione iniziale e l'aggiornamento dinamica della listbox degli esperimenti digitando nella casella di ricerca
                    switch (number)
                    {
                        //Inizializzazione
                        case 0:
                            //Si verifica che il nome contenga la parola "Experiment"
                            if (folderName.Contains("Experiment"))
                            {
                                //Si assegna il nome al content del ListViewItem
                                listItem.Content = folderName;
                                //Alla ListViewItem si aggiunge l'evento openExpDeps (l'evento che permette di aprire la tab dell'esperimento)
                                listItem.MouseDoubleClick += openExpDeps;
                                //Si aggiunge l'elemento della lista appena creato alla viewer degli esperimenti
                                ExperimentViewer.Items.Add(listItem);
                            }
                            break;
                        //Aggiornamento tramite searchbox
                        case 1:
                            string textsearched = ExpSearchBox.Text;
                            //Si verifica che il nome contenga la parola "Experiment"
                            if (folderName.Contains("Experiment") && folderName.Contains(textsearched))
                            {
                                //Si assegna il nome al content del ListViewItem
                                listItem.Content = folderName;
                                //Alla ListViewItem si aggiunge l'evento openExpDeps (l'evento che permette di aprire la tab dell'esperimento)
                                listItem.MouseDoubleClick += openExpDeps;
                                //Si aggiunge l'elemento della lista appena creato alla viewer degli esperimenti
                                ExperimentViewer.Items.Add(listItem);
                            }
                            break;
                    }
                }
                defaultInitIfExpListIsEmpty(ExperimentViewer);
            }
            catch(DirectoryNotFoundException dirEx)
            {
                //Se si verificasse la citata eccezione si chiederebbe all'utente di cambiare il percorso
                changeExpPath();
            }
            catch (ArgumentException argEx)
            {
                //Se si verificasse la citata eccezione si chiederebbe all'utente di cambiare il percorso
                changeExpPath();
            }
        }
        //Controllo se la lista degli esperimenti contiene elementi --> se no, inserisce la label di comunicazione
        private void defaultInitIfExpListIsEmpty(ListBox experimentViewer)
        {
            bool hasElement;
            //Controllo se la cartella indicata contiene cartelle relative agli esperimenti
            if (experimentViewer.Items.Count > 0)
            {
                hasElement = true;
            }                
            else
            {
                hasElement = false;
            }

            if (!hasElement)
            {
                //Se la lista non contiene elementi, ossia nessuna cartella di esperimenti, sarà mostrata la scritta di avviso
                Label label = new Label();
                label.Content = noExpDetected;
                experimentViewer.Items.Add(label);
            }
        }
        //Funzione che carica i dettagli negli appositi controlli a ogni selezione dell'item
        private void ExperimentViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem listViewItem = sender as ListViewItem;

            //Recupero dei dati necessari, file di testo, immagine e aggiornamento dei dettagli
            string fileName = getExpCommentPath();
            Provino.Source = getProvinoImage();
            updateDetailsValues(fileName);
            updateDetailsSize();
        }
        //Funzione che ritorna l'immagine di Provino, se presente, oppure mostra quella predefinita
        private ImageSource getProvinoImage()
        {
            //Controllo se l'item selezionato dalla lista sia effettivamente un listviewitem
            if (checkItemType())
            {
                var icon = Resources.Source;
                string imagepath;
                BitmapImage tmpImage;
                ListViewItem listViewItem = (ListViewItem)ExperimentViewer.SelectedItem;    //Si recupare l'item selezionato

                //Verifica che sia effettivamente selezionato un item della lista
                //Se così non fosse si apre la directory degli esperimenti
                if (listViewItem != null)
                {
                    string itemName = (string)listViewItem.Content;                         //Si recupera il nome dell'item selezionato (nome della cartella dell'esperimento)
                    string path = expPath + @"\" + itemName;
                    string[] files = Directory.GetFiles(path);                              //Si ottengono i files presenti nella cartella sottoforma di array di stringhe
                    List<string> fileNames = new List<string>();

                    //Si verifica che la lista contenga degli elementi
                    if (files.Length > 0)
                    {
                        //Si estraggono in una lista le stringhe che contengono la parola Experiment, Provino e che sono dei file jpeg
                        for (int i = 0; i < files.Length; i++)
                        {
                            //Si controlla che il nome del file sia quello auspicato e in caso affermativo si aggiunge il percorso alla lista dei risultati corretti, che ci aspettiamo sia uno o il primo
                            string filename = getExpName(files[i]);
                            if (filename.Contains(".jpeg") && filename.Contains("Experiment") && filename.Contains("Provino"))
                            {
                                fileNames.Add(files[i]);
                            }
                        }
                        //Si verifica che la lista contenga degli elementi
                        if (fileNames.Count > 0)
                        {
                            imagepath = fileNames[0];
                            tmpImage = new BitmapImage((new Uri(imagepath)));
                        }                            
                        else
                        {
                            tmpImage = new BitmapImage(new Uri(defaultImage, UriKind.RelativeOrAbsolute));
                        }                            
                    }
                    else
                    {
                        tmpImage = new BitmapImage(new Uri(defaultImage, UriKind.RelativeOrAbsolute));
                    }
                }
                else
                {
                   tmpImage = new BitmapImage(new Uri(defaultImage, UriKind.RelativeOrAbsolute));
                }
                return tmpImage;
            }
            else
            {
                BitmapImage tmpImage = new BitmapImage(new Uri(defaultImage, UriKind.RelativeOrAbsolute));
                return tmpImage;
            }
        }

        //Adattamento delle dimensioni dell'area dei dettagli dell'esperimento
        private void updateDetailsSize()
        {
            ExpDetails.Height = this.ActualHeight / 2;
            Provino.Height = ExpDetails.ActualHeight / 2;
            ExpComment.Height = ExpDetails.ActualHeight / 2;
        }
        //Aggiornamento dei dettagli dell'esperimento
        private void updateDetailsValues(string filepath)
        {
            //Aggiornamento del commento in formato txt
            if (Path.GetExtension(filepath).Equals(".txt"))
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(System.IO.File.ReadAllText(filepath));
                FlowDocument document = new FlowDocument(paragraph);
                ExpComment.Document = document;
            }            
        }
        //Funzione che inizializza la classe TabsBody
        private void initTabControl()
        {
            this.tabBody = new TabsBody();
            //Impostazione del posizionamento della classe nella griglia dell'interfaccia
            Grid.SetRow(tabBody, 1);
            Grid.SetColumn(tabBody, 2);
            //La classe viene aggiunta alla griglia principale dell'applicazione nella posizione specificata (1, seconda riga; 2, terza colonna)
            DSB_MainGrid.Children.Add(tabBody);
        }
        //Evento openExpDeps, che cattura il doppio click del mouse sul nome dell'esperimento in lista e delega la gestione al metodo NewExpTabItem della classe DSB_Controller
        private void openExpDeps(object sender, EventArgs e)
        {
            ListViewItem listViewItem = sender as ListViewItem;
            //La funzione della classe ExpTabControlController ritorna un oggetto TabsBody
            tabBody = myExpTabControlController.createTabItem(tabBody, listViewItem, expPath);

            //Parte relativa all'aggiunta del esperimento appena aperto all'elenco degli esperimenti aperti di recente --> chiave: nome esperimento, valore: percorso esperimento
            string expname = (string)listViewItem.Content;
            string singleExpPath = expPath + @"\" + expname;
            //Aggiunta di chiave-valore al file di configurazione
            configurationController.addSettings(expname, singleExpPath);
            //Aggiornamento degli esperimenti recenti
            updateRecentExpMenu();
        }
        //Visibilità della colonna della lista degli esperimenti
        //Gestione della larghezza della colonna della lista degli esperimenti
        //La colonna assume in maniera predefinita la larghezza del contenuto (lunghezza della stringa)
        //Se la larghezza è 0 assume la larghezza iniziale, e viceversa
        //Si istanzia quindi un oggetto GridLength che va a impostare la larghezza della colonna dell'interfaccia
        private void HideExp_Click(object sender, RoutedEventArgs e)
        {
            //Ridimensionamento della colonna contenente la lista degli esperimenti (se visibile la nasconde, e viceversa; anche se non avviene tramite la proprietà Visibility, ma con numeri)
            //Variabile locale che assume il valore dell'attributo "width"
            Double width = Column.ActualWidth;
            if (this.width == 0)
            {
                //Se tale attributo è ancora di valore zero, esso assume l'attuale valore della larghezza
                this.width = width;
            }
            if (width == this.width)
            {
                width = 0;
                Column.Width = new GridLength(width);
            }
            else if (width < this.width && width > 0)
            {
                width = 0;
                Column.Width = new GridLength(width);
            }
            else
            {
                width = this.width;
                Column.Width = new GridLength(width);
            }
        }
        //Evento legato al click del mouse sul menuitem del commento dell'esperimento
        private void ViewExpCommentMenu_Click(object sender, RoutedEventArgs e)
        {
            string fileName =getExpCommentPath();
            ExpDetails.Visibility = viewComment();
            ViewCommentMenu.Header = ChangeExpDetailsMenuText();
            updateDetailsValues(fileName);
            updateDetailsSize();
        }
        //Cambia il testo del menuitem legato al commento in base al fatto che sia attualmente visibile o meno
        private object ChangeExpDetailsMenuText()
        {
            if (ExpDetails.Visibility.Equals(System.Windows.Visibility.Collapsed))
            {
                return "Vedi dettagli";
            }
            else
            {
                return "Nascondi Dettagli";

            }
        }
        //Gestione della visibilità del commento sulla base del click del mouse sul menu dedicato
        private Visibility viewComment()
        {
            if (ExpDetails.Visibility.Equals(System.Windows.Visibility.Collapsed))
            {
                return System.Windows.Visibility.Visible;
            }
            else
            {
                return System.Windows.Visibility.Collapsed;
            }
        }

        //Funzione per recuperare il percorso del commento dell'esperimento
        private string getExpCommentPath()
        {
            //Percorso commento predefinito
            string defaultComment = "";

            try
            {
                //Si ottiene il percorso corrente di esecuzione, ma è nella cartella /bin/Debug e non nella "root" del progetto
                string temppath = Directory.GetCurrentDirectory();           
                //perciò taglio il percorso alla corrispondenza di /bin in modo da essere nella directory con la folder "Testi" e le altre cartelle del progetto
                int endindex = temppath.IndexOf(@"\bin");
                defaultComment = temppath.Substring(0, endindex) + @"/Testi/Comment not found.txt";
            }
            catch(Exception exception)
            {
                //In caso di eccezioni si visualizza un messaggio e 
                MessageBox.Show(exception.Message);
            }
            

            //Controllo se l'item selezionato dalla lista sia effettivamente un listviewitem
            if (checkItemType())
            {

                string commentPath;
                ListViewItem listViewItem = (ListViewItem)ExperimentViewer.SelectedItem;    //Si recupare l'item selezionato

                //Verifica che sia effettivamente selezionato un item della lista
                //Se così non fosse si apre la directory degli esperimenti
                if (listViewItem != null)
                {

                    string itemName = (string)listViewItem.Content;                         //Si recupera il nome dell'item selezionato (nome della cartella dell'esperimento)
                    string path = expPath + @"\" + itemName;
                    string[] files = Directory.GetFiles(path);                              //Si ottengono i files presenti nella cartella sottoforma di array di stringhe
                    List<string> fileNames = new List<string>();
                    //Si verifica che la lista contenga degli elementi
                    if (files.Length > 0)
                    {
                        //Si estraggono in una lista le stringhe che contengono la parola Experiment e che sono dei file txt
                        for (int i = 0; i < files.Length; i++)
                        {
                            //Si recupera unicamente il nome del file e si verifica che sia un txt con Experiment nel nome
                            string filename = getExpName(files[i]);
                            if (filename.Contains(".txt") && filename.Contains("Experiment"))
                            {
                                //In caso affermativo, quel percoso è aggiunta alla lista
                                fileNames.Add(files[i]);
                            }
                        }

                        //Si verifica che la lista contenga degli elementi e si assegna il primo alla stringa relativa al percorso del commento, altrimenti si assegna il percorso al commento predefinito
                        if (fileNames.Count > 0)
                            commentPath = fileNames[0];
                        else
                            commentPath = defaultComment;
                    }
                    else
                    {
                        commentPath = defaultComment;
                    }
                }
                else
                {
                    commentPath = defaultComment;
                }
                return commentPath;
            }
            else
            {
                return "";
            }
        }
        //Funzione di drag&drop per l'immagine
        private void Provino_Drop(object sender, DragEventArgs e)
        {
            string[] files = null;
            
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //Le stringhe salvate sono il percorso del file rilasciato sulla finestra
                files = (string[])e.Data.GetData(DataFormats.FileDrop, true);

                //Siccome qualsiasi file può essere "droppato", la condizione verifica che:
                // - la lista non sia vuota
                // - la lista contenga un'immagine tramite la verifica dell'estensione
                if (files.Length>0 && isImage(files[0]))
                {
                    System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show(questionDialog, "Immagine \"Provino\"", MessageBoxButton.YesNo);
                    if(dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        //Si assegna l'immagine
                        BitmapImage tmpImage = new BitmapImage((new Uri(files[0])));
                        Provino.Source = tmpImage;
                        SaveImage(tmpImage);
                    }
                    else if (dialogResult == System.Windows.Forms.DialogResult.No)
                    {
                        //Do Nothing
                    }
                }                
            }
        }
        //Funzione di salvataggio dell'immagine di Provino dell'esperimento
        private void SaveImage(BitmapImage tmpImage)
        {
            //Variabili necessarie per il salvataggio dell'immagine
            Bitmap bitmap = convertBitmapImageToBitmap(tmpImage);
            ImageCodecInfo imageCodecInfo;
            Encoder encoder;
            EncoderParameter encoderParameter;
            EncoderParameters encoderParameters;

            //Parametrizzazione per il salvataggio
            imageCodecInfo = GetEncoderInfo("image/jpeg");
            encoder = Encoder.Quality;
            encoderParameters = new EncoderParameters(1);

            //Salvataggio dell'immagine
            ListViewItem listViewItem = (ListViewItem)ExperimentViewer.SelectedItem;
            string imagename = "_Provino.jpeg";
            string expname = (string)listViewItem.Content;
            string filename = (string)listViewItem.Content + imagename;
            string imagepath = expPath + @"\" + expname + @"\" + filename;

            encoderParameter = new EncoderParameter(encoder, 100L);
            encoderParameters.Param[0] = encoderParameter;
            Bitmap image = convertBitmapImageToBitmap(tmpImage);

            image.Save(imagepath, imageCodecInfo, encoderParameters);

        }
        //Convertitore di BitmapImage in Bitmap
        private Bitmap convertBitmapImageToBitmap(BitmapImage tmpImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(tmpImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
        //Funzione copiata da https://docs.microsoft.com/en-us/dotnet/api/system.drawing.image.save?view=dotnet-plat-ext-6.0
        private ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        //Image Drag_Enter
        private void Provino_DragEnter(object sender, DragEventArgs e)
        {
            {
                if (e.Data.GetDataPresent(DataFormats.Bitmap))
                    e.Effects = DragDropEffects.Copy;
                else
                    e.Effects = DragDropEffects.None;
            }
        }
        //Funzione di verifica dell'estensione del file
        private bool isImage(String filename)
        {
            if (Path.GetExtension(filename).Equals(".jpg") || Path.GetExtension(filename).Equals(".bmp") || Path.GetExtension(filename).Equals(".png"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Evento legato al click dell'apertura della directory degli esperimenti
        private void ShowExpsDir_Click(object sender, RoutedEventArgs e)
        {
            string path = expPath;
            openFolder(path);
        }
        //Evento legato al click dell'apertura della directory dell'esperimento selezionato
        private void ShowExpDir_Click(object sender, RoutedEventArgs e)
        {
            if (checkItemType())
            {
                string path;
                ListViewItem listViewItem = (ListViewItem)ExperimentViewer.SelectedItem;    //Si recupare l'item selezionato

                //Verifica che sia effettivamente selezionato un item della lista
                //Se così non fosse si apre la directory degli esperimenti
                if (listViewItem != null)
                {
                    string itemName = (string)listViewItem.Content;                         //Si recupera il nome dell'item selezionato (nome della cartella dell'esperimento)
                    path = expPath + @"\" + itemName;
                }
                else
                {
                    path = expPath;
                }
                openFolder(path);
            }            
        }
        //Verifica se l'item selezionato della lista è un listviewitem --> se sì torna true
        private bool checkItemType()
        {
            if(ExperimentViewer.SelectedItem is ListViewItem)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Funzione che apre in Windows il file explorer con il percorso passato come stringa
        private void openFolder(string path)
        {
            if (Directory.Exists(path))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = path,
                    FileName = "explorer.exe"
                };
                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} La directory non esiste!", path));
            }
        }
        //Comando di refresh che ricarica l'intero applicativo
        private void RefreshCmd_Click(object sender, RoutedEventArgs e)
        {
            //Inizializza nuovamente la lista degli esperimenti
            Init(0);            
        }
        //Comando di chiusura dell'applicazione
        private void QuitCmd_Click(object sender, RoutedEventArgs e)
        {
            //Si chiede la conferma per la chiusura dell'applicazione
            System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Si desidera chiudere Ground Control?", "Chiusura Ground Control", MessageBoxButton.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                //Si chiude l'applicazione e si toglie il riferimento al percorso temporaneo
                if (configurationController.containsTemppath())
                {
                    string key = "temppath";
                    configurationController.removeSettings(key);
                }
                System.Windows.Application.Current.Shutdown();
            }
            else if (dialogResult == System.Windows.Forms.DialogResult.No)
            {
                //Do Nothing
            }
        }
        //Modifica del commento --> si apre il file con il programma di default
        private void EditComment_Click(object sender, RoutedEventArgs e)
        {
            //Si recupera il percorso del file di commento dell'esperimento
            string commentPath = getExpCommentPath();
            //Controllo sia un file txt
            if (Path.GetExtension(commentPath).Equals(".txt"))
            {
                //Si apre il file con il programma di default
                System.Diagnostics.Process.Start(commentPath);
            }            
        }
        //Funzione che apre un filedialog per selezionare la cartella degli esperimenti
        private void PathCmd_Click(object sender, RoutedEventArgs e)
        {
            changeExpPath();
        }
        //Funzione che apre un filedialog per selezionare la cartella degli esperimenti
        private void changeExpPath()
        {
            System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Desideri cambiare percorso?\n\nNota: l'applicativo sarà riavviato chiudendo ogni esperimento e deposizione!", "Nuovo percorso esperimenti", MessageBoxButton.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                System.Windows.Forms.FolderBrowserDialog openFileDlg = new System.Windows.Forms.FolderBrowserDialog();
                var result = openFileDlg.ShowDialog();

                //Controllo se l'utente preme Cancel/Annulla dalla finestra di scelta del percorso
                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    //Do nothing
                }
                else if (result.ToString() != string.Empty)
                {
                    //Si disabilita, togliendo, l'evento legato alla chiusura dell'applicativo --> non deve chiedere tale conferma durante il riavvio
                    this.Closing -= Window_Closing;
                    expPath = openFileDlg.SelectedPath;
                    configurationController.addSettings("path", expPath);
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
            }
            else if (dialogResult == System.Windows.Forms.DialogResult.No)
            {
                //Do Nothing
            }
        }
        //Evento sulla chiusura dell'applicativo tramite la "X"
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Si chiede la conferma per la chiusura dell'applicazione
            System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Si desidera chiudere Ground Control?", "Chiusura Ground Control", MessageBoxButton.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                //Si chiude l'applicazione e si toglie il riferimento al percorso temporaneo
                if (configurationController.containsTemppath())
                {
                    string key = "temppath";
                    configurationController.removeSettings(key);
                }
            }
            else if (dialogResult == System.Windows.Forms.DialogResult.No)
            {
                //Blocca la chiusura dell'applicativo
                e.Cancel = true;
            }
        }
        //Evento che riavvio l'applicativo
        private void RestartCmd_Click(object sender, RoutedEventArgs e)
        {
            //Si chiede la conferma per il refresh dell'applicazione
            System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Si desidera riavviare Ground Control?", "Refresh Ground Control", MessageBoxButton.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                //Si riavvia l'applicazione togliendo l'evento che domanda se si desidera chiudere l'applicativo
                this.Closing -= Window_Closing;
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            else if (dialogResult == System.Windows.Forms.DialogResult.No)
            {
                //Do Nothing
            }
        }
        //Evento che si verifica quando si modifica il testo nella searchbox degli esperimenti --> in cima alla lista
        private void ExpSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Aggiornamento della lista filtrando anche tramite il testo scritto nella casella di ricerca
            Init(1);
        }
        //Creazione/aggiornamento della lista degli esperimenti aperti di recente
        private void updateRecentExpMenu()
        {
            OrderedDictionary recentExperiments = configurationController.getRecenteExperiments();
            //Controllo della lunghezza del dizionario, altrimenti non si svolgono operazioni
            if (recentExperiments.Count > 0)
            {
                //Recupero delle chiavi in un array
                string[] keys = new string[recentExperiments.Count];
                recentExperiments.Keys.CopyTo(keys, 0);
                //Reset degli items del menu
                RecentExp.Items.Clear();
                //Si cicla sul dizionario tramite la lista di chiavi
                foreach (string key in keys)
                {
                    //Recupero del valore dal dizionario
                    string value = (string)recentExperiments[key];
                    //Creazione del menuitem da associare al menuitem
                    MenuItem recentExp = new MenuItem();
                    //Si assegna il valore all'header del menuitem
                    recentExp.Header = value;
                    //Colorazione rossa del carattere nel caso il percorso base non combaciasse (vedi nel caso si cambiasse percorso)
                    if (!value.Contains(expPath))
                    {
                        //Testo rosso
                        recentExp.Foreground = System.Windows.Media.Brushes.Red;
                    }
                    else
                    {
                        //Testo nero
                        recentExp.Foreground = System.Windows.Media.Brushes.Black;
                    }
                    //Aggiunta dell'evento legato al menuitem con il nome dell'esperimento recente
                    recentExp.Click += RecentExp_Click;

                    //Aggiunta del menuitem dell'esperimento recente
                    RecentExp.Items.Add(recentExp);
                }
            }
        }
        private void RecentExp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //L'oggetto mittente è un menuitem
                MenuItem recentExp = sender as MenuItem;
                string content = (string)recentExp.Header;
                if (content.Contains(expPath))
                {
                    //Estrazione del nome dell'esperimento
                    content = getExpName(content);
                    //Creazione di un ListviewItem unicamente per il metodo di createTabItem che lo richiede come parametro
                    ListViewItem listViewItem = new ListViewItem();
                    listViewItem.Content = content;

                    //La funzione della classe ExpTabControlController ritorna un oggetto TabsBody
                    tabBody = myExpTabControlController.createTabItem(tabBody, listViewItem, expPath);

                    //Parte relativa all'aggiunta del esperimento appena aperto all'elenco degli esperimenti aperti di recente --> chiave: nome esperimento, valore: percorso esperimento
                    string expname = content;
                    string singleExpPath = expPath + @"\" + expname;
                    //Aggiunta di chiave-valore al file di configurazione
                    configurationController.addSettings(expname, singleExpPath);
                    //Aggiornamento degli esperimenti recenti
                    updateRecentExpMenu();
                }
                else
                {
                    MessageBox.Show("L'esperimento selezionato non fa parte del percorso in analisi. Pertanto non è possibile aprirlo", "Questo esperimento non può essere aperto.");
                }
            }
            catch(Exception exception)
            {
                MessageBox.Show("Errore nell'apertura dell'esperimento recente", exception.Message);
            }
            
            
        }
        //Ritornare le parti del percoso in modo da ritornare il nome dell'esperimento
        private string getExpName(string actualpath)
        {
            //stringa da rimuovere
            string removeString = expPath + @"\";
            int index = actualpath.IndexOf(removeString);
            string cleanPath = (index < 0) ? actualpath : actualpath.Remove(index, removeString.Length);
            return cleanPath;
        }
        //Compressione dell'esperimento selezionato
        private void CompressCmd_Click(object sender, RoutedEventArgs e)
        {
            if (checkItemType())
            {
                string foldername;
                ListViewItem listViewItem = (ListViewItem)ExperimentViewer.SelectedItem;    //Si recupare l'item selezionato

                //Verifica che sia effettivamente selezionato un item della lista
                //Se così fosse si comprime la cartella dell'esperimento selezionato
                if (listViewItem != null)
                {
                    string itemName = (string)listViewItem.Content;                         //Si recupera il nome dell'item selezionato (nome della cartella dell'esperimento)
                    foldername = expPath + @"\" + itemName;
                    string compressfile = foldername + ".zip";
                    try
                    {
                        //Si crea il file compresso
                        ZipFile.CreateFromDirectory(foldername, compressfile);
                        MessageBox.Show(itemName + " è stato compresso!", "Risultato compressione in .zip");
                    }
                    catch (ArgumentNullException nullargs)
                    {
                        MessageBox.Show("Possibili cause del problema:\n\n" +
                                        "La cartella da comprimere o il file compresso sono Null\n",
                                        nullargs.Message);
                    }
                    catch (ArgumentException args)
                    {
                        MessageBox.Show("Possibili cause del problema:\n\n" +
                                        "La cartella da comprimere o il file compresso è vuoto\n" +
                                        "La cartella da comprimere o il file compresso contiene unicamente spazi bianchi\n" +
                                        "La cartella da comprimere o il file compresso contiene almeno un carattere invalido\n",
                                        args.Message);
                    }
                    catch (DirectoryNotFoundException directorynotfound)
                    {
                        MessageBox.Show("Possibili cause del problema:\n\n" +
                                        "La cartella che si sta comprimendo non è valida o non esiste.\n",
                                        directorynotfound.Message);
                    }
                    catch (PathTooLongException pathTolong)
                    {
                        MessageBox.Show("Possibili cause del problema:\n\n" +
                                        "I nomi indicati eccedono la lunghezza.\n",
                                        pathTolong.Message);
                    }
                    catch (UnauthorizedAccessException notaccess)
                    {
                        MessageBox.Show("Possibili cause del problema:\n\n" +
                                        "Il secondo parametro indica una directory\n" +
                                        "Non si possiedono le credenziali per l'accesso.\n",
                                        notaccess.Message);
                    }
                    catch (NotSupportedException notSupException)
                    {
                        MessageBox.Show("Possibili cause del problema:\n\n" +
                                        "La cartella da comprimere o il file compresso contiene un formato non valido.\n",
                                        notSupException.Message);
                    }
                    catch (IOException ioexception)
                    {
                        MessageBox.Show("Possibili cause del problema:\n\n" +
                                        "Il file compresso esiste già\n" +
                                        "Un file dell'esperimento non può essere aperto\n" +
                                        "Si sta aprendo un file mentre viene compresso\n",
                                        ioexception.Message);
                    }
                }
                else
                {
                    //Do nothing
                }
            }
        }
        //Cancellazione dell'esperimento
        private void DeleteExpMenu_Click(object sender, RoutedEventArgs e)
        {

            //Si chiede la conferma per l'eliminazione della directory dell'esperimento
            System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Si desidera eliminare l'esperimento selezionato?", "Eliminazione esperimento", MessageBoxButton.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    //Si recupera l'item selezionato dalla lista
                    ListViewItem listViewItem = (ListViewItem)ExperimentViewer.SelectedItem;
                    //Se ne recupera il nome
                    string itemname = (string)listViewItem.Content;
                    //Si istanzia la stringa del percorso: percorso base degli esperimenti + il nome dell'item selezionato
                    string folderpath = expPath + @"\" + itemname;
                    //Creazione della variabile relativa alla directory da cancellare
                    var dir = new DirectoryInfo(folderpath);
                    dir.Attributes = dir.Attributes & ~FileAttributes.ReadOnly;
                    //Si cancella la variabile, l'esperimento
                    dir.Delete(true);
                    //Si cancella l'esperimento dal file di configurazione, se presente
                    configurationController.removeSettings(itemname);
                    //Aggiornamento della lista degli esperimenti
                    Init(0);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (dialogResult == System.Windows.Forms.DialogResult.No)
            {
                //Do Nothing
            }
        }
    }
}