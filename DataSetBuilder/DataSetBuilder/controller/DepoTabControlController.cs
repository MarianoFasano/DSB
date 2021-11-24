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
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DataSetBuilder.controller
{
    class DepoTabControlController
    {
        //Object from graphical interface
        private Button playButton;
        private Button pauseButton;
        private Button prevButton;
        private Button nextButton;
        private ComboBox imageSpeed;
        private Image depoImage;
        private Label actualMs;
        private Label maxMs;
        private Slider SliderMs;
        private TextBox searchMs;
        
        //Other variables
        private String depoPath;
        private String dataPath;
        private String basePath;
        private IDictionary<String, MyDepoData> depoDatas = new Dictionary<String, MyDepoData>();
        private IDictionary<String, DepoItemBody> depoStructures = new Dictionary<String, DepoItemBody>();

        private Boolean isAutomatic = false;
        private MyExpTabItemModel myExpTabItemModel;
        private TabControl actualTabControl;
        private short count = 0;

        public DepoTabControlController(MyExpTabItemModel myExpTabItemModel, String basePath)
        {
            this.myExpTabItemModel = myExpTabItemModel;
            this.basePath = basePath;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabItem tabItem = (TabItem)actualTabControl.SelectedItem;
                //Mandatory check to avoid tabItem=null happened on drag&drop the tabItem
                if (tabItem != null && (string)tabItem.Header != null)
                {
                    DepoItemBody depoItemBody = this.depoStructures[(string)tabItem.Header];
                    
                    if (depoItemBody != null)
                    {
                        assignIControl(depoItemBody);
                        setDataPath((string)tabItem.Header);
                    }
                }
            }
        }

        private void initControlsAction()
        {
            this.playButton.Click += PlayButton_Click;
            this.pauseButton.Click += PauseButton_Click;
            this.prevButton.Click += PrevButton_Click;
            this.nextButton.Click += NextButton_Click;
            this.searchMs.KeyDown += SearchMs_KeyDown;
        }

        private void SearchMs_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                //MessageBox.Show(this.searchMs.Text, "Sono stato modificato");
                //max length 13
                // 0 to label content lentgh
                //Valore attuale dei ms
                long searchedValue = long.Parse(this.searchMs.Text);
                MyDepoData myDepoData = depoDatas[getDepoName()];

                if (long.Parse(this.searchMs.Text) >= 0 && long.Parse(this.searchMs.Text) <= long.Parse(maxMs.Content.ToString()))
                {
                    //MessageBox.Show(this.searchMs.Text, "Lunghezza maggiore di 0 e valore inferiore al massimo");
                    string result = binarySearch(searchedValue, myDepoData.getImages());
                    //MessageBox.Show(result, "Risultato binary search");
                }
                else if (long.Parse(this.searchMs.Text) >= long.Parse(extractMs(myDepoData.getImages()[0])) && long.Parse(this.searchMs.Text)<=long.Parse(extractMs(myDepoData.getImages()[myDepoData.getImages().Count - 1])))
                {
                    //MessageBox.Show(this.searchMs.Text, "Valore maggiore di max ms e inferiore al massimo scrivibile");
                    string result = longBinarySearch(searchedValue, myDepoData.getImages());
                    //MessageBox.Show(result, "Risultato binary search");
                }
                else
                {
                    MessageBox.Show(long.Parse(extractMs(myDepoData.getImages()[myDepoData.getImages().Count - 1])).ToString(), "Formato errato");
                }
            }
        }

        private String binarySearch(long searchedMs, List<string> imagesList)
        {
            //ALGO IMPLEMENTETION, TWO WAYS
            return binarySearch(imagesList, searchedMs, 0, imagesList.Count -1);

        }

        private string binarySearch(List<string> imagesList, long searchedMs, int left, int right)
        {
            count++;
            if (left > right)
            {
                //Do nothing!
            }

            int middle = (left + right) / 2;
            string originalElement = imagesList[middle];
            string min = imagesList[0];
            string extractElement = extractMs(originalElement);
            min = extractMs(min);

            if(count > 25)
            {
                resetCounter();
                return binarySearch(imagesList, searchedMs-1, 0, imagesList.Count - 1);
            }

            if ((long.Parse(extractElement)-long.Parse(min) == searchedMs))
            {
                return originalElement;
            }
            else if ((long.Parse(extractElement) - long.Parse(min) > searchedMs))
            {
                return binarySearch(imagesList, searchedMs, left, middle-1);
            }
            else
            {
                return binarySearch(imagesList, searchedMs, middle+1, right);
            }
        }

        private String longBinarySearch(long searchedMs, List<string> imagesList)
        {
            //ALGO IMPLEMENTETION, TWO WAYS
            resetCounter();
            return longBinarySearch(imagesList, searchedMs, 0, imagesList.Count - 1);

        }

        private string longBinarySearch(List<string> imagesList, long searchedMs, int left, int right)
        {
            count++;
            if (left > right)
            {
                //Do nothing!
            }

            int middle = (left + right) / 2;
            string originalElement = imagesList[middle];
            string extractElement = extractMs(originalElement);

            if (count > 25)
            {
                resetCounter();
                return longBinarySearch(imagesList, searchedMs - 1, 0, imagesList.Count - 1);
            }

            if ((long.Parse(extractElement) == searchedMs))
            {
                return originalElement;
            }
            else if ((long.Parse(extractElement) > searchedMs))
            {
                return longBinarySearch(imagesList, searchedMs, left, middle - 1);
            }
            else
            {
                return longBinarySearch(imagesList, searchedMs, middle + 1, right);
            }
        }

        private void resetCounter()
        {
            this.count = 0;
        }

        public void setDepoPath(String path)
        {
            this.depoPath = path;
        }
        public void setDataPath(String path)
        {
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

            TabItem tabItem = new TabItem();
            tabItem.Header = (string)listViewItem.Content;
            DepoItemBody depoItemBody = new DepoItemBody();
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
            BitmapImage bitmapImage = new BitmapImage(new Uri(dataPath + @"\" + myDepoData.getImageDirectory() + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
            this.depoImage.Source = bitmapImage;

            //Extract actual and max ms
            setMsLabels(myDepoData);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            nextImage();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            MyDepoData myDepoData = depoDatas[getDepoName()];
            myDepoData.downActualImage();
            BitmapImage bitmapImage = new BitmapImage(new Uri(dataPath + @"\" + myDepoData.getImageDirectory() + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
            this.depoImage.Source = bitmapImage;
            setMsLabels(myDepoData);
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
            BitmapImage bitmapImage = new BitmapImage(new Uri(dataPath + @"\"+ myDepoData.getImageDirectory() + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
            this.depoImage.Source = bitmapImage;
            setMsLabels(myDepoData);
        }

        private String getDepoName()
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

        //Assign the correct buttons to the controller and re-init the button actions
        private void assignIControl(DepoItemBody depoItemBody)
        {
            this.playButton = depoItemBody.PlayImage;
            this.pauseButton = depoItemBody.PauseImage;
            this.prevButton = depoItemBody.PrevImage;
            this.nextButton = depoItemBody.NextImage;
            this.imageSpeed = depoItemBody.ImageSpeed;
            this.depoImage = depoItemBody.DepoImage;
            this.actualMs = depoItemBody.ActualMs;
            this.maxMs = depoItemBody.MaxMs;
            this.SliderMs = depoItemBody.MsSlider;
            this.searchMs = depoItemBody.SearchMs;
        }

        //Set this.tabControl based on a key value
        public void setActualTabControl(String key)
        {
            this.actualTabControl = this.myExpTabItemModel.getTabControl(key);
            this.actualTabControl.SelectionChanged += TabControl_SelectionChanged;
        }



        //responsabilità del controller o del mydepodata? --> spostare in mydepodata
        private void setMsLabels(MyDepoData myDepoData)
        {
            string maxMs = myDepoData.getImages()[myDepoData.getImages().Count - 1];
            string actualMs = myDepoData.getImages()[(int)myDepoData.getActualImage()];
            string minMs = myDepoData.getImages()[0];


            int max = (int)Int64.Parse(extractMs(maxMs));
            int actual = (int)Int64.Parse(extractMs(actualMs));
            int min = (int)Int64.Parse(extractMs(minMs));

            this.maxMs.Content = (max-min).ToString();
            this.actualMs.Content = (actual - min).ToString();
            this.SliderMs.Maximum = max - min;
        }
        private string extractMs(string msString)
        {
            string ms = msString;
            int start = ms.IndexOf("ms") + "ms".Length;
            ms = ms.Substring(start, ms.IndexOf("_")-2);
            return ms;
        }
    }
}
