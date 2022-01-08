using DataSetBuilder.controller;
using DataSetBuilder.user_controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        //Dichiarazione della classe DSB_Controller (il suo utilizzo è specificato nella classe stessa)
        DSB_Controller dsb_controller;
        ConfigurationController configurationController = new ConfigurationController();

        //TODO PATH:Percorso della cartella contenente gli esperimenti, anch'essi sono delle cartelle
        //string expPath = @"J:\DTI\_DSB";    //fisso Mariano
        //string expPath = @"D:\_DSB";      //portatile Mariano
        //string expPath = @"J:\DTI\Experiments_Lite";
        string expPath;

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
            Init();
            initTabControl();
            this.dsb_controller = new DSB_Controller(this.tabBody.TabsControl, expPath);
            //Massimizza la finestra
            this.WindowState = System.Windows.WindowState.Normal;
        }

        private void ConfigurationInit()
        {
            PathFromConfig();            
        }

        private void PathFromConfig()
        {
            string key = "path";
            string value = ConfigurationManager.AppSettings[key];
            if (value != null)
            {
                this.expPath = value;
            }
            else
            {
                changeExpPath();
            }
        }

        private void Init()
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
                }
                checkEmptyExpList(ExperimentViewer);
            }
            catch(DirectoryNotFoundException dirEx)
            {
                changeExpPath();
            }            
        }
        //Controllo se la lista degli esperimenti contiene elementi --> se no, inserisce la label di comunicazione
        private void checkEmptyExpList(ListBox experimentViewer)
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
        //Funzione che carica il commento dell'esperimento nel DocumentViewer
        private void ExperimentViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem listViewItem = sender as ListViewItem;

            //TODO: gestire eccezione di file non trovato --> Si scrive nel textbox il contenuto di testo del file txt, letto dalla funzione File.ReadAllText
            string fileName = getExpCommentPath();
            Provino.Source = getProvinoImage();
            updateDetails(fileName);
            updateSize();
        }

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
                                                                                            //Si verifica che la lista contenga degli elementi
                    if (files.Length > 0)
                    {
                        //Si estraggono in una lista le stringhe che contengono la parola Experiment e che sono dei file txt
                        List<string> fileNames = files.Where(e => e.Contains("Experiment")).Where(e => e.Contains(".jpeg")).ToList();
                        //Si verifica che la lista contenga degli elementi
                        if (fileNames.Count > 0)
                        {
                            imagepath = fileNames[0];
                            tmpImage = new BitmapImage((new Uri(imagepath)));
                        }                            
                        else
                        {
                            tmpImage = new BitmapImage(new Uri(@"/Immagini/GC_addImage.png", UriKind.RelativeOrAbsolute));
                        }                            
                    }
                    else
                    {
                        tmpImage = new BitmapImage(new Uri(@"/Immagini/GC_addImage.png", UriKind.RelativeOrAbsolute));
                    }
                }
                else
                {
                   tmpImage = new BitmapImage(new Uri(@"/Immagini/GC_addImage.png", UriKind.RelativeOrAbsolute));
                }
                return tmpImage;
            }
            else
            {
                BitmapImage tmpImage = new BitmapImage(new Uri(@"/Immagini/GC_addImage.png", UriKind.RelativeOrAbsolute));
                return tmpImage;
            }
        }

        //Adattamento delle dimensioni dell'area dei dettagli dell'esperimento
        private void updateSize()
        {
            ExpDetails.Height = this.ActualHeight / 2;
            Provino.Height = ExpDetails.ActualHeight / 2;
            ExpComment.Height = ExpDetails.ActualHeight / 2;
        }
        //Aggiornamento dei dettagli dell'esperimento
        private void updateDetails(string filepath)
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
            //La funzione della classe DSB_Controller ritorna un oggetto TabsBody
            tabBody = dsb_controller.NewExpTabItem(tabBody, listViewItem, expPath);
        }
        //Visibilità della colonna della lista degli esperimenti
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Ridimensionamento della colonna contenente la lista degli esperimenti (se visibile la nasconde, e viceversa; anche se non avviene tramite la proprietà Visibility, ma con numeri)
            Column.Width = dsb_controller.columnWidth(Column);        
        }
        //Evento legato al click del mouse sul menuitem del commento dell'esperimento
        private void ViewExpCommentMenu_Click(object sender, RoutedEventArgs e)
        {
            String fileName =getExpCommentPath();
            //TODO: dopo aver commentato il dsb_controller
            ExpDetails.Visibility = dsb_controller.viewComment(ExpDetails);
            ViewCommentMenu.Header = dsb_controller.commentText(ExpDetails);
            updateDetails(fileName);
            updateSize();
        }
        //Funzione per recuperare il percorso del commento dell'esperimento
        private string getExpCommentPath()
        {
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
                                                                                            //Si verifica che la lista contenga degli elementi
                    if (files.Length > 0)
                    {
                        //Si estraggono in una lista le stringhe che contengono la parola Experiment e che sono dei file txt
                        List<string> fileNames = files.Where(e => e.Contains("Experiment")).Where(e => e.Contains(".txt")).ToList();
                        //Si verifica che la lista contenga degli elementi
                        if (fileNames.Count > 0)
                            commentPath = fileNames[0];
                        else
                            commentPath = "";
                    }
                    else
                    {
                        commentPath = "";
                    }
                }
                else
                {
                    commentPath = "";
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

        private void SaveImage(BitmapImage tmpImage)
        {
            //Variabili necessarie per il salvataggio dell'immagine
            Bitmap bitmap = convertToBitmap(tmpImage);
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
            Bitmap image = convertToBitmap(tmpImage);

            image.Save(imagepath, imageCodecInfo, encoderParameters);

        }

        private Bitmap convertToBitmap(BitmapImage tmpImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(tmpImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

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

        //TODO: capire esattamente perché sia qui questa funzione
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
        private void ExpsDir_Click(object sender, RoutedEventArgs e)
        {
            string path = expPath;
            openFolder(path);
        }
        //Evento legato al click dell'apertura della directory dell'esperimento selezionato
        private void ExpDir_Click(object sender, RoutedEventArgs e)
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

            //Si chiede la conferma per il refresh dell'applicazione
            System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Si desidera riavviare Ground Control?", "Refresh Ground Control", MessageBoxButton.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                //Si riavvia l'applicazione
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            else if (dialogResult == System.Windows.Forms.DialogResult.No)
            {
                //Do Nothing
            }
        }
        //Comando di chiusura dell'applicazione
        private void QuitCmd_Click(object sender, RoutedEventArgs e)
        {
            //Si chiede la conferma per la chiusura dell'applicazione
            System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show("Si desidera chiudere Ground Control?", "Chiusura Ground Control", MessageBoxButton.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                //Si chiude l'applicazione
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
                if (result.ToString() != string.Empty)
                {
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
    }
}
