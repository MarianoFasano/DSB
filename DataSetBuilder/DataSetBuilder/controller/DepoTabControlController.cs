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
    class DepoTabControlController
    {
        //Object from graphical interface
        private Button playButton;
        private Button pauseButton;
        private Button prevButton;
        private Button nextButton;
        private ComboBox imageSpeed;
        private Image depoImage;
        private TextBox actualMs;
        private Label maxMs;
        private Slider SliderMs;
        private Thumb sliderThumb;
        private TextBox searchMs;
        private StackPanel datasStackPanel;
        
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
        private ulong counterCount = 0;
        private short bigO = 25;
        private short offset = -1;

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
            this.SliderMs.FocusableChanged += SliderMs_FocusableChanged1; ;
        }

        private void SliderMs_FocusableChanged1(object sender, DependencyPropertyChangedEventArgs e)
        {
            //TODO
        }

        //On Enter down
        private void SearchMs_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {

                if (this.searchMs.Text.All(char.IsDigit))
                {
                    long searchedValue = long.Parse(this.searchMs.Text);
                    MyDepoData myDepoData = depoDatas[getDepoName()];

                    if ((long.Parse(this.searchMs.Text) >= 0) && (long.Parse(this.searchMs.Text) <= long.Parse(maxMs.Content.ToString())))
                    {
                        //Lunghezza maggiore di 0 e valore inferiore al massimo
                        shortMsResearch(searchedValue, myDepoData);
                    }
                    else if (long.Parse(this.searchMs.Text) >= long.Parse(extractMs(myDepoData.getImages()[0])) && long.Parse(this.searchMs.Text) <= long.Parse(extractMs(myDepoData.getImages()[myDepoData.getImages().Count - 1])))
                    {
                        //Valore maggiore di max ms e inferiore al massimo scrivibile
                        longMsResearch(searchedValue, myDepoData);
                    }
                    else
                    {
                        MessageBox.Show(long.Parse(extractMs(myDepoData.getImages()[myDepoData.getImages().Count - 1])).ToString(), "Formato errato");
                    }
                }
                else
                {
                    MessageBox.Show("Nel campo di ricerca devono essere scritti unicamente dei numeri", "Formato errato");
                }
                this.searchMs.Clear();
            }
        }

        private void shortMsResearch(long searchedValue, MyDepoData myDepoData)
        {
            string result = binarySearch(searchedValue, myDepoData.getImages());
            setImage(result);

            List<string> pyroLines = File.ReadAllLines(myDepoData.getPyroFileDirectory() + @"\" + myDepoData.getPyrometerList()[0]).Cast<string>().ToList();
            List<string> CNCLines = File.ReadAllLines(myDepoData.getCNCFileDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList();

            pyroLines = extractFromPyroList(pyroLines);
            CNCLines = extractFromCNCList(CNCLines);

            string pyroResult = pyroShortBS(pyroLines, searchedValue, myDepoData.getImages());
            string cncResult = cncShortBS(CNCLines, searchedValue, myDepoData.getImages());

            string temperature = extractTemp(pyroResult);
            string laserOn = extractLaserOn(cncResult);
            string powerFeedback = extractPowerFeedback(cncResult);

            updateDatas(temperature, laserOn, powerFeedback);
        }
        private void longMsResearch(long searchedValue, MyDepoData myDepoData)
        {
            string result = longBinarySearch(searchedValue, myDepoData.getImages());
            setImage(result);

            List<string> pyroLines = File.ReadAllLines(myDepoData.getPyroFileDirectory() + @"\" + myDepoData.getPyrometerList()[0]).Cast<string>().ToList();
            List<string> CNCLines = File.ReadAllLines(myDepoData.getCNCFileDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList();

            pyroLines = extractFromPyroList(pyroLines);
            CNCLines = extractFromCNCList(CNCLines);

            string pyroResult = pyroLongBS(pyroLines, searchedValue);
            string cncResult = cncLongBS(CNCLines, searchedValue);

            string temperature = extractTemp(pyroResult);
            string laserOn = extractLaserOn(cncResult);
            string powerFeedback = extractPowerFeedback(cncResult);

            updateDatas(temperature, laserOn, powerFeedback);
        }

        private string cncShortBS(List<string> cNCLines, long searchedValue, List<string> imagesList)
        {
            resetCounter();
            resetCounterCount();
            offset = -1;
            string min = imagesList[0];
            min = extractMs(min);

            return cncShortBS(cNCLines, searchedValue, 0, cNCLines.Count() - 1, min);
        }

        private void resetCounterCount()
        {
            this.counterCount = 0;
        }

        private string cncShortBS(List<string> cNCLines, long searchedMs, int left, int right, string min)
        {
            count++;
            if (left > right)
            {
                //Do nothing!
            }

            int middle = (left + right) / 2;
            string originalElement = cNCLines.ElementAt(middle);
            string element = extractFromCNCLine(originalElement);

            if (searchedMs < 0)
            {
                offset = 1;
                resetCounter();
                return cncShortBS(cNCLines, searchedMs + offset, 0, cNCLines.Count() - 1, min);
            }

            if (count > bigO)
            {
                resetCounter();
                return cncShortBS(cNCLines, searchedMs + offset, 0, cNCLines.Count() - 1, min);
            }

            if ((long.Parse(element) - long.Parse(min) == searchedMs))
            {
                return originalElement;
            }
            else if ((long.Parse(element) - long.Parse(min) > searchedMs))
            {
                return cncShortBS(cNCLines, searchedMs, left, middle - 1, min);
            }
            else
            {
                return cncShortBS(cNCLines, searchedMs, middle + 1, right, min);
            }
        }

        private string extractPowerFeedback(string cncResult)
        {
            string powerFeedback = cncResult;
            int index;

            //The hardcoded way...
            for (int i = 0; i < 15; i++)
            {
                index = powerFeedback.IndexOf("\t") + "\t".Length;
                powerFeedback = powerFeedback.Substring(index);
            }
            return powerFeedback;
        }

        private string extractLaserOn(string cncResult)
        {
            string laserOn = cncResult;
            int index;

            //The hardcoded way...
            for(int i=0; i<14; i++)
            {
                index = laserOn.IndexOf("\t") + "\t".Length;
                laserOn = laserOn.Substring(index);
            }

            index = laserOn.IndexOf("\t");
            laserOn = laserOn.Substring(0, index);
            return laserOn;
        }

        private string cncLongBS(List<string> cNCLines, long searchedValue)
        {
            resetCounter();
            offset = -1;
            return cncLongBS(cNCLines, searchedValue, 0, cNCLines.Count() - 1);
        }

        private string cncLongBS(List<string> cNCLines, long searchedMs, int left, int right)
        {
            count++;
            if (left > right)
            {
                //Do nothing!
            }

            int middle = (left + right) / 2;
            string originalElement = cNCLines.ElementAt(middle);
            string element = extractFromCNCLine(originalElement);

            if (searchedMs < 0)
            {
                offset = 1;
                resetCounter();
                return cncLongBS(cNCLines, searchedMs + offset, 0, cNCLines.Count() - 1);
            }
            if (count > bigO)
            {
                resetCounter();
                return cncLongBS(cNCLines, searchedMs + offset, 0, cNCLines.Count() - 1);
            }

            if ((long.Parse(element) == searchedMs))
            {
                return originalElement;
            }
            else if ((long.Parse(element) > searchedMs))
            {
                return cncLongBS(cNCLines, searchedMs, left, middle - 1);
            }
            else
            {
                return cncLongBS(cNCLines, searchedMs, middle + 1, right);
            }
        }

        private string extractFromCNCLine(string originalElement)
        {
            string local = originalElement;
            int start = local.IndexOf("\t") + "\t".Length + 1;      //+1, jump the first line char
            local = local.Substring(start);
            local = local.Substring(0, local.IndexOf("\t"));
            return local;
        }

        private List<string> extractFromCNCList(List<string> cNCLines)
        {
            cNCLines.RemoveAt(0);
            return cNCLines;
        }

        //Clear and populate the stackpanel with temperature, laseron TODO, powerfeedback TODO datas
        private void updateDatas(string temperature, string laserOn, string powerFeedback)
        {
            //Clear the stackPanel from the actual values
            datasStackPanel.Children.Clear();

            //Labels to append to the clean stackPanel
            Label Temperature = new Label();
            Label LaserOn = new Label();
            Label PowerFeedback = new Label();

            Temperature.Content = "Temperature:\t" + temperature;
            LaserOn.Content = "LaserOn:\t\t" + laserOn;
            PowerFeedback.Content = "PowerFeedback:\t" + powerFeedback;

            datasStackPanel.Children.Add(Temperature);
            datasStackPanel.Children.Add(LaserOn);
            datasStackPanel.Children.Add(PowerFeedback);
        }

        //Extract the temperature value from his line (string)
        private string extractTemp(string pyroResult)
        {
            string temp = pyroResult;
            int start = temp.IndexOf("Read:.\t") + "Read:.\t".Length;
            temp = temp.Substring(start);
            string local = temp.Substring(0, temp.IndexOf("\t"));
            start = temp.IndexOf(local) + local.Length;
            temp = temp.Substring(start + "\t".Length);
            return temp;
        }

        private List<string> extractFromPyroList(List<string> pyroLines)
        {
            //Hard coded, need refactoring
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(pyroLines.Count-1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            pyroLines.RemoveAt(pyroLines.Count - 1);
            return pyroLines;
        }

        private string pyroShortBS(List<string> pyroLines, long searchedMs, List<string> imagesList)
        {
            resetCounter();
            resetCounterCount();
            offset = -1;
            string min = imagesList[0];
            min = extractMs(min);

            return pyroShortBS(pyroLines, searchedMs, 0, pyroLines.Count()-1, min);
        }

        private string pyroShortBS(List<string> pyroLines, long searchedMs, int left, int right, string min)
        {
            count++;
            if (left > right)
            {
                //Do nothing!
            }

            int middle = (left + right) / 2;
            string originalElement = pyroLines.ElementAt(middle);
            string element = extractFromPyroLine(originalElement);

            if (searchedMs<0)
            {
                offset = 1;
                resetCounter();
                return pyroShortBS(pyroLines, searchedMs + offset, 0, pyroLines.Count() - 1, min);
            }

            if (count > bigO)
            {
                resetCounter();
                return pyroShortBS(pyroLines, searchedMs + offset, 0, pyroLines.Count() - 1, min);
            }

            if ((long.Parse(element) - long.Parse(min) == searchedMs))
            {
                return originalElement;
            }
            else if ((long.Parse(element) - long.Parse(min) > searchedMs))
            {
                return pyroShortBS(pyroLines, searchedMs, left, middle - 1, min);
            }
            else
            {
                return pyroShortBS(pyroLines, searchedMs, middle + 1, right, min);
            }
        }

        private string pyroLongBS(List<string> pyroLines, long searchedMs)
        {
            resetCounter();
            offset = -1;
            return pyroLongBS(pyroLines, searchedMs, 0, pyroLines.Count() -1);
        }

        private string pyroLongBS(List<string> pyroLines, long searchedMs, int left, int right)
        {
            count++;
            if (left > right)
            {
                //Do nothing!
            }

            int middle = (left + right) / 2;
            string originalElement = pyroLines.ElementAt(middle);
            string element = extractFromPyroLine(originalElement);

            if (searchedMs < 0)
            {
                offset = 1;
                resetCounter();
                return pyroLongBS(pyroLines, searchedMs + offset, 0, pyroLines.Count() - 1);
            }

            if (count > bigO)
            {
                resetCounter();
                return pyroLongBS(pyroLines, searchedMs + offset, 0, pyroLines.Count() - 1);
            }

            if ((long.Parse(element) == searchedMs))
            {
                return originalElement;
            }
            else if ((long.Parse(element) > searchedMs))
            {
                return pyroLongBS(pyroLines, searchedMs, left, middle - 1);
            }
            else
            {
                return pyroLongBS(pyroLines, searchedMs, middle + 1, right);
            }
        }

        private string extractFromPyroLine(string element)
        {
            string local = element;
            int start = local.IndexOf("Read:") + "Read:".Length + 2;
            //int start = "Read:.\t".Length;
            local = local.Substring(start);
            local = local.Substring(0, local.IndexOf("\t"));
            return local;
        }

        private String binarySearch(long searchedMs, List<string> imagesList)
        {
            //ALGO IMPLEMENTETION, TWO WAYS
            resetCounter();
            resetCounterCount();
            offset = -1;
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

            if (searchedMs < 0)
            {
                offset = 1;
                resetCounter();
                return binarySearch(imagesList, searchedMs + offset, 0, imagesList.Count() - 1);
            }

            if (count > bigO)
            {
                resetCounter();
                return binarySearch(imagesList, searchedMs + offset, 0, imagesList.Count - 1);
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
            resetCounterCount();
            offset = -1;
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

            if (searchedMs < 0)
            {
                offset = 1;
                resetCounter();
                return longBinarySearch(imagesList, searchedMs + offset, 0, imagesList.Count() - 1);
            }

            if (count > bigO)
            {
                resetCounter();
                return longBinarySearch(imagesList, searchedMs + offset, 0, imagesList.Count - 1);
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
            shortMsResearch(0, myDepoData);

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
            littleMsDataSearch(this.actualMs.Text, myDepoData);
        }

        private void littleMsDataSearch(string stringValue, MyDepoData myDepoData)
        {
            long searchedValue = long.Parse(stringValue);

            List<string> pyroLines = File.ReadAllLines(myDepoData.getPyroFileDirectory() + @"\" + myDepoData.getPyrometerList()[0]).Cast<string>().ToList();
            List<string> CNCLines = File.ReadAllLines(myDepoData.getCNCFileDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList();

            pyroLines = extractFromPyroList(pyroLines);
            CNCLines = extractFromCNCList(CNCLines);

            string pyroResult = pyroShortBS(pyroLines, searchedValue, myDepoData.getImages());
            string cncResult = cncShortBS(CNCLines, searchedValue, myDepoData.getImages());

            string temperature = extractTemp(pyroResult);
            string laserOn = extractLaserOn(cncResult);
            string powerFeedback = extractPowerFeedback(cncResult);

            updateDatas(temperature, laserOn, powerFeedback);
        }

        private void setImage(string filename)
        {
            MyDepoData myDepoData = depoDatas[getDepoName()];
            BitmapImage bitmapImage = new BitmapImage(new Uri(dataPath + @"\" + myDepoData.getImageDirectory() + filename, UriKind.RelativeOrAbsolute));
            this.depoImage.Source = bitmapImage;
            myDepoData.setActualImage(myDepoData.getImages().IndexOf(filename));
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
            this.datasStackPanel = depoItemBody.DataList;
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
            this.actualMs.Text = (actual - min).ToString();
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
