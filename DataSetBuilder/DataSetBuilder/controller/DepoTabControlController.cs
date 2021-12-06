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
        /*Object from graphical interface*/
        //General
        private Button playButton;
        private Button pauseButton;
        private Button prevButton;
        private Button nextButton;
        private ComboBox imageSpeed;
        private Image depoImage;
        private StackPanel datasStackPanel;
        private CheckBox showExt;
        //Significant version of ms
        private TextBox actualMs;
        private Label maxMs;
        private Slider SliderMs;
        private Thumb sliderThumb;
        private TextBox searchMs;
        //Extended version of ms
        private StackPanel ExtStackPanel;
        private TextBox extActualMs;
        private Label extMaxMs;
        private Slider extSliderMs;
        private Thumb extSliderThumb;
        //Other variables
        private String depoPath;
        private String dataPath;
        private String basePath;
        private IDictionary<String, MyDepoData> depoDatas = new Dictionary<String, MyDepoData>();
        private IDictionary<String, DepoItemBody> depoStructures = new Dictionary<String, DepoItemBody>();
        private Boolean isAutomatic = false;
        private MyExpTabItemModel myExpTabItemModel;
        private TabControl actualTabControl;
        private String minValue;
        //Controllers
        ImageSearcher imageSearcher = new ImageSearcher();
        PyrometerSearcher pyrometerSearcher = new PyrometerSearcher();
        CNCSearcher cncSearcher = new CNCSearcher();

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
            this.showExt.Click += ShowExt_Click;
        }

        private void ShowExt_Click(object sender, RoutedEventArgs e)
        {
            if (showExt.IsChecked==true)
            {
                ExtStackPanel.Visibility = Visibility.Visible;
            }
            else if (showExt.IsChecked == false)
            {
                ExtStackPanel.Visibility = Visibility.Collapsed;
            }
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

                    if ((long.Parse(this.searchMs.Text) >= this.SliderMs.Minimum) && (long.Parse(this.searchMs.Text) <= long.Parse(maxMs.Content.ToString())))
                    {
                        //Lunghezza maggiore di 0 e valore inferiore al massimo
                        shortMsResearch(searchedValue - (long)this.SliderMs.Minimum, myDepoData);
                    }
                    else if (long.Parse(this.searchMs.Text) >= long.Parse(extractMs(myDepoData.getImages()[0])) && long.Parse(this.searchMs.Text) <= long.Parse(extractMs(myDepoData.getImages()[myDepoData.getImages().Count - 1])))
                    {
                        //Valore maggiore di max ms e inferiore al massimo scrivibile
                        longMsResearch(searchedValue, myDepoData);
                    }
                    else
                    {
                        String firstFormat = this.SliderMs.Minimum.ToString() + "-" + this.SliderMs.Maximum.ToString();
                        String secondFormat = extractMs(myDepoData.getImages()[0]) + "-" + extractMs(myDepoData.getImages()[myDepoData.getImages().Count - 1]);
                        MessageBox.Show("I formati numerici associati sono i seguenti:\n\n" + firstFormat + "\n" + secondFormat, "Formato errato");
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
            string result = imageSearcher.shortSearcher(searchedValue, myDepoData);         
            setImage(result);
            string temperature, laserOn, powerFeedback;
            if (myDepoData.getPyrometerList().Any())
            {
                string pyroResult = pyrometerSearcher.shortSearch(searchedValue, myDepoData);
                temperature = extractTemp(pyroResult);
            }
            else
            {
                temperature = "No value";
            }
            if (myDepoData.getCNList().Any())
            {
                string cncResult = cncSearcher.shortSearch(searchedValue, myDepoData);
                laserOn = extractLaserOn(cncResult);
                powerFeedback = extractPowerFeedback(cncResult);
            }
            else
            {
                laserOn = "No value";
                powerFeedback = "No value";
            }
            updateDatas(temperature, laserOn, powerFeedback);
        }
        private void longMsResearch(long searchedValue, MyDepoData myDepoData)
        {
            string result = imageSearcher.longSearch(searchedValue, myDepoData);
            
            setImage(result);
            string temperature, laserOn, powerFeedback;

                if (myDepoData.getPyrometerList().Any())
                {
                    string pyroResult = pyrometerSearcher.longSearch(searchedValue, myDepoData);
                    temperature = extractTemp(pyroResult);
                }
                else
                {
                    temperature = "No value";
                }
                if (myDepoData.getCNList().Any())
                {
                    string cncResult = cncSearcher.longSearch(searchedValue, myDepoData);
                    laserOn = extractLaserOn(cncResult);
                    powerFeedback = extractPowerFeedback(cncResult);
                }
                else
                {
                    laserOn = "No value";
                    powerFeedback = "No value";
                }           
      
            updateDatas(temperature, laserOn, powerFeedback);
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
            if (!powerFeedback.Any())
            {
                powerFeedback = "No value";
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
            if (!laserOn.Any())
            {
                laserOn = "No value";
            }
            else
            {
                index = laserOn.IndexOf("\t");
                laserOn = laserOn.Substring(0, index);
            }            
            
            return laserOn;
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
            longMsResearch(long.Parse(this.extActualMs.Text), myDepoData);
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
            long actualMs = long.Parse(this.actualMs.Text);
            shortMsResearch(actualMs, myDepoData);
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
            longMsResearch(actualMs, myDepoData);
            //MessageBox.Show(myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), "post_shortSearch");
        }

        private void setImage(string filename)
        {
            MyDepoData myDepoData = depoDatas[getDepoName()];
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
            //General
            this.playButton = depoItemBody.PlayImage;
            this.pauseButton = depoItemBody.PauseImage;
            this.prevButton = depoItemBody.PrevImage;
            this.nextButton = depoItemBody.NextImage;
            this.imageSpeed = depoItemBody.ImageSpeed;
            this.depoImage = depoItemBody.DepoImage;
            this.datasStackPanel = depoItemBody.DataList;
            this.showExt = depoItemBody.Show;
            //Significant version of ms
            this.actualMs = depoItemBody.ActualMs;
            this.maxMs = depoItemBody.MaxMs;
            this.SliderMs = depoItemBody.MsSlider;
            this.searchMs = depoItemBody.SearchMs;
            //Extendend version of ms
            this.extActualMs = depoItemBody.ExtendActualMs;
            this.extMaxMs = depoItemBody.ExtendMaxMs;
            this.extSliderMs = depoItemBody.ExtendMsSlider;
            this.ExtStackPanel = depoItemBody.ExtractPanel;
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

            string maxString = extractMs(maxMs);
            string actualString = extractMs(actualMs);
            string minString = extractMs(minMs);

            long max = long.Parse(maxString);
            long actual = long.Parse(extractMs(actualMs));
            long min = long.Parse(minString);

            minString = extractDifferentDigit(minString, maxString);

            //this.maxMs.Content = ((max-min)+ (int)Int64.Parse(minString)).ToString();
            this.actualMs.Text = ((actual - min) + (int)Int64.Parse(minString)).ToString();
            //this.SliderMs.Maximum = max - min + (int)Int64.Parse(minString);

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

            this.maxMs.Content = ((max - min) + long.Parse(minString)).ToString();
            this.actualMs.Text = ((actual - min) + long.Parse(minString)).ToString();
            this.SliderMs.Maximum = max - min + long.Parse(minString);
            this.SliderMs.Minimum = actual - min + long.Parse(minString);

            this.extMaxMs.Content = maxString;
            this.extActualMs.Text = actual.ToString();
            this.extSliderMs.Maximum = long.Parse(maxString);
            this.extSliderMs.Minimum = min;
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
