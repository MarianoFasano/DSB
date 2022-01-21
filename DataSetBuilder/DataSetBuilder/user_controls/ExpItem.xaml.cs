using DataSetBuilder.controller;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
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

namespace DataSetBuilder.user_controls
{
    /// <summary>
    /// Logica di interazione per ExpItem.xaml
    /// </summary>
    /// 

    /*
    User control - file xaml con relativa classe cs integrabili in altre interfacce
    */
    public partial class ExpItem : UserControl
    {
        //Variabile legata al TabControl delle deposizioni
        private DepoTabItem depoTabItem;
        private DepoTabControlController depoTabControlController;

        //Riferimenti ai percorsi delle deposizioni
        string basePath;
        string depoName;

        //Domanda cambiamento immagine
        string questionDialog = "Desideri cambiare immagine di \"Provino\"?";
        //Percorso immagine predefinita
        string defaultImage = @"/Immagini/GC_addImage.png";

        public ExpItem(DepoTabControlController depoTabControlController)
        {
            InitializeComponent();
            this.depoTabControlController = depoTabControlController;
        }
        //Settare la variabile DepoTabItem
        public void setDepoTabControl(DepoTabItem depoTabItem)
        {
            this.depoTabItem = depoTabItem;
        }
        //Torna il valore della depoTabItem
        public DepoTabItem getDepoTabItem()
        {
            return this.depoTabItem;
        }
        //Chiamata all'inizializzazione della lista delle deposizioni, memorizzando percorso fino alla deposizione e nome della deposizione
        internal void initDepoList(string depoName, string path)
        {
            this.basePath = path;
            this.depoName = depoName;
            Init(0);
        }
        //Inizializzazione della lista delle deposizioni
        private void Init(int number)
        {
            //Si ripulisce la listbox --> vale per l'aggiornamento
            DepositionViewer.Items.Clear();
            //Si ottiene la stringa della deposizione --> percorso base + nome della deposizione
            string depoPath = basePath + @"\" + depoName;
            try
            {
                //Si ottengono le cartelle nella directory in formato stringa indicata al metodo statico
                string[] depoDirectories = Directory.GetDirectories(depoPath);
                //Controllo se la lista di cartelle ne contiene, contenere cartelle significa che ci sono delle deposizioni
                if (isEmpty(depoDirectories.Length))
                {
                    //Se l'array risulta vuoto si aggiunge il messaggio che indica all'utente che non sono presenti deposizioni per l'esperimento aperto
                    DepositionViewer.Items.Add(emptyMessage());
                }
                else
                {
                    //In caso di presenza di deposizioni, si cicla su di esse andando a creare un item per la lista e gli si aggiunge l'evento di apertura della deposizione gestita nella classe DepoTabControlController
                    for (int i = 0; i < depoDirectories.Length; i++)
                    {
                        var listItem = new ListViewItem();
                        string depoName = depoDirectories[i].Remove(0, depoPath.Length + 1);

                        //Istruzione switch che discerne fra la ricarica completa della lista e la ricarica dovuta alla ricerca dinamica di una deposizione
                        switch (number)
                        {
                            //Caso associato all'inizializzazione
                            case 0:
                                if (depoName.Contains("Deposition"))
                                {
                                    listItem.Content = depoName;
                                    listItem.MouseDoubleClick += depoTabControlController.openDepsData;
                                    DepositionViewer.Items.Add(listItem);
                                }
                                break;
                            //Caso associato all'aggiornamento della lista dovuto alla ricerca nel campo di testo
                            case 1:
                                if (depoName.Contains("Deposition") && depoName.Contains(DepoSearchBox.Text))
                                {
                                    listItem.Content = depoName;
                                    listItem.MouseDoubleClick += depoTabControlController.openDepsData;
                                    DepositionViewer.Items.Add(listItem);
                                }
                                break;
                        }
                    }
                }
            }
            catch (DirectoryNotFoundException directoryNotFound)
            {
                //Nel caso l'eccezione fosse causata da una directory non trovata si ricorda all'utente che il nome dell'esperimento non può contenere la parentesi aperta '('
                MessageBox.Show("Attenzione, nome non valido: il nome dell'esperimento non può contentere il simbolo '(' !", directoryNotFound.Message);
            }
            catch(Exception e)
            {
                //Nel caso fosse qualsiasi altra eccezione, la si mostra come messaggio
                MessageBox.Show(e.Message);
            }
            
            
              
        }
        //Controllo booleano se l'array contiene o meno elementi --> vuoto se lungo zero
        private bool isEmpty(int length)
        {
            if (length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Costruzione della label contenente il messaggio vuoto
        private Label emptyMessage()
        {
            var label = new Label();
            label.Content = "Nessuna deposizione trovata";
            label.HorizontalAlignment = HorizontalAlignment.Center;
            return label;
        }
        //Evento che si verifica quando si modifica il testo nella searchbox degli esperimenti --> in cima alla lista
        private void DepoSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Aggiornamento della lista
            Init(1);
        }
        //Aggiornamento della lista delle deposizioni, evento legato al clicksulla voce "Aggiorna" del menu "opzioni"
        private void UpdateDepoView_Click(object sender, RoutedEventArgs e)
        {
            Init(0);
        }
        //Evento legato al click del mouse sul menuitem del commento dell'esperimento
        private void ViewDetailsMenu_Click(object sender, RoutedEventArgs e)
        {
            string fileName = getDepoCommentPath();
            DepoDetails.Visibility = viewComment();
            ViewDetailsMenu.Header = commentText();
            updateDetails(fileName);
            updateSize();
        }
        //Cambia il testo del menuitem legato al commento in base al fatto che sia attualmente visibile o meno
        private object commentText()
        {
            if (DepoDetails.Visibility.Equals(System.Windows.Visibility.Collapsed))
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
            if (DepoDetails.Visibility.Equals(System.Windows.Visibility.Collapsed))
            {
                return System.Windows.Visibility.Visible;
            }
            else
            {
                return System.Windows.Visibility.Collapsed;
            }
        }

        //Funzione che carica i dettagli negli appositi controlli a ogni selezione dell'item
        private void DepositionViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem listViewItem = sender as ListViewItem;

            //Recupero dei dati necessari, file di testo, immagine e aggiornamento dei dettagli
            string fileName = getDepoCommentPath();
            Provino.Source = getProvinoImage();
            updateDetails(fileName);
            updateSize();
        }

        //Adattamento delle dimensioni dell'area dei dettagli della deposizione
        private void updateSize()
        {
            DepoDetails.Height = DepoDockPanel.ActualHeight * 0.75;
            Provino.Height = DepoDetails.ActualHeight / 2;
            DepoComment.Height = DepoDetails.ActualHeight / 2;
        }

        //Aggiornamento dei dettagli della deposizione
        private void updateDetails(string filepath)
        {
            //Aggiornamento del commento in formato txt
            if (System.IO.Path.GetExtension(filepath).Equals(".txt"))
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(System.IO.File.ReadAllText(filepath));
                FlowDocument document = new FlowDocument(paragraph);
                DepoComment.Document = document;
            }
        }

        private ImageSource getProvinoImage()
        {
            //Controllo se l'item selezionato dalla lista sia effettivamente un listviewitem
            if (checkItemType())
            {
                var icon = Resources.Source;
                string imagepath;
                BitmapImage tmpImage;
                ListViewItem listViewItem = (ListViewItem)DepositionViewer.SelectedItem;    //Si recupare l'item selezionato

                //Verifica che sia effettivamente selezionato un item della lista
                //Se così non fosse si apre la directory degli esperimenti
                if (listViewItem != null)
                {
                    string itemName = (string)listViewItem.Content;                         //Si recupera il nome dell'item selezionato (nome della cartella della deposizione)
                    string depoPath = basePath + @"\" + depoName;
                    string path = depoPath + @"\" + itemName;
                    string[] files = Directory.GetFiles(path);                              //Si ottengono i files presenti nella cartella sottoforma di array di stringhe
                    List<string> fileNames = new List<string>();
                    
                    //Si verifica che la lista contenga degli elementi
                    if (files.Length > 0)
                    {
                        //Si estraggono in una lista le stringhe che contengono la parola Deposition, Provino e che sono dei file jpeg
                        for (int i = 0; i < files.Length; i++)
                        {
                            //Si controlla che il nome del file sia quello auspicato e in caso affermativo si aggiunge il percorso alla lista dei risultati corretti, che ci aspettiamo sia uno o il primo
                            string filename = getDepoName(files[i]);
                            if (filename.Contains(".jpeg") && filename.Contains("Deposition") && filename.Contains("Provino"))
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

        //Verifica se l'item selezionato della lista è un listviewitem --> se sì torna true
        private bool checkItemType()
        {
            if (DepositionViewer.SelectedItem is ListViewItem)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Funzione per recuperare il percorso del commento della deposizione
        private string getDepoCommentPath()
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
            catch (Exception exception)
            {
                //In caso di eccezioni si visualizza un messaggio e 
                MessageBox.Show(exception.Message);
            }


            //Controllo se l'item selezionato dalla lista sia effettivamente un listviewitem
            if (checkItemType())
            {

                string commentPath;
                ListViewItem listViewItem = (ListViewItem)DepositionViewer.SelectedItem;    //Si recupare l'item selezionato

                //Verifica che sia effettivamente selezionato un item della lista
                //Se così non fosse si apre la directory degli esperimenti
                if (listViewItem != null)
                {

                    string itemName = (string)listViewItem.Content;                         //Si recupera il nome dell'item selezionato (nome della cartella della deposizione)
                    string depoPath = basePath + @"\" + depoName;
                    string path = depoPath + @"\" + itemName;
                    string[] files = Directory.GetFiles(path);                              //Si ottengono i files presenti nella cartella sottoforma di array di stringhe
                    List<string> fileNames = new List<string>();
                    //Si verifica che la lista contenga degli elementi
                    if (files.Length > 0)
                    {
                        //Si estraggono in una lista le stringhe che contengono la parola Deposition e che sono dei file txt
                        for(int i = 0; i < files.Length; i++)
                        {
                            string filename = getDepoName(files[i]);
                            if(filename.Contains(".txt") && filename.Contains("Deposition"))
                            {
                                fileNames.Add(filename);
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
                return defaultComment;
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
                if (files.Length > 0 && isImage(files[0]))
                {
                    System.Windows.Forms.DialogResult dialogResult = (System.Windows.Forms.DialogResult)MessageBox.Show(questionDialog, "Immagine \"Provino\"", MessageBoxButton.YesNo);
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
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
            Bitmap bitmap = convertToBitmap(tmpImage);
            ImageCodecInfo imageCodecInfo;
            System.Drawing.Imaging.Encoder encoder;
            EncoderParameter encoderParameter;
            EncoderParameters encoderParameters;

            //Parametrizzazione per il salvataggio
            imageCodecInfo = GetEncoderInfo("image/jpeg");
            encoder = System.Drawing.Imaging.Encoder.Quality;
            encoderParameters = new EncoderParameters(1);

            //Salvataggio dell'immagine
            ListViewItem listViewItem = (ListViewItem)DepositionViewer.SelectedItem;
            string imagename = "_Provino.jpeg";
            string deponame = (string)listViewItem.Content;
            string filename = (string)listViewItem.Content + imagename;
            string depoPath = basePath + @"\" + depoName + @"\" + deponame;
            string imagepath = depoPath + @"\" + filename;

            encoderParameter = new EncoderParameter(encoder, 100L);
            encoderParameters.Param[0] = encoderParameter;
            Bitmap image = convertToBitmap(tmpImage);

            image.Save(imagepath, imageCodecInfo, encoderParameters);

        }
        //Convertitore di BitmapImage in Bitmap
        private Bitmap convertToBitmap(BitmapImage tmpImage)
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
        private bool isImage(string filename)
        {
            if (System.IO.Path.GetExtension(filename).Equals(".jpg") || System.IO.Path.GetExtension(filename).Equals(".bmp") || System.IO.Path.GetExtension(filename).Equals(".png"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Modifica del commento --> si apre il file con il programma di default
        private void EditComment_Click(object sender, RoutedEventArgs e)
        {
            //Si recupera il percorso del file di commento dell'esperimento
            string commentPath = getDepoCommentPath();
            //Controllo sia un file txt
            if (System.IO.Path.GetExtension(commentPath).Equals(".txt"))
            {
                //Si apre il file con il programma di default
                System.Diagnostics.Process.Start(commentPath);
            }
        }
        //Compressione della deposizione selezionata in file zip
        private void CompressDepo_Click(object sender, RoutedEventArgs e)
        {
            if (checkItemType())
            {
                string foldername;
                ListViewItem listViewItem = (ListViewItem)DepositionViewer.SelectedItem;    //Si recupare l'item selezionato

                //Verifica che sia effettivamente selezionato un item della lista
                //Se così fosse si comprime la cartella della deposizione selezionata
                if (listViewItem != null)
                {
                    string itemName = (string)listViewItem.Content;                         //Si recupera il nome dell'item selezionato (nome della cartella della deposizione)
                    string depoPath = basePath + @"\" + depoName;
                    foldername = depoPath + @"\" + itemName;
                    string compressfile = foldername + ".zip";
                    try
                    {
                        //Si crea il file compresso
                        ZipFile.CreateFromDirectory(foldername, compressfile);
                        //Messaggio di avvenuta compressione
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
        //Ritornare le parti del percoso in modo da ritornare il nome dell'esperimento
        private string getDepoName(string actualpath)
        {
            ListViewItem listViewItem = (ListViewItem)DepositionViewer.SelectedItem;
            //stringa da rimuovere
            string itemname = (string)listViewItem.Content;
            string removeString = basePath + @"\" + depoName + @"\" + itemname;
            int index = actualpath.IndexOf(removeString);
            string cleanPath = (index < 0) ? actualpath : actualpath.Remove(index, removeString.Length);
            return cleanPath;
        }
    }
}
