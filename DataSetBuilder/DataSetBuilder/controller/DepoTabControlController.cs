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
                string searchedValString = this.searchMs.Text;

                if (searchedValString.All(char.IsDigit))
                {
                    long searchedValue = long.Parse(searchedValString);
                    MyDepoData myDepoData = depoDatas[getDepoName()];
                
                    if (long.Parse(searchedValString) >= long.Parse(extractMs(myDepoData.getImages()[0])) && long.Parse(searchedValString) <= long.Parse(extractMs(myDepoData.getImages()[myDepoData.getImages().Count - 1])))
                    {
                        //Valore maggiore di max ms e inferiore al massimo scrivibile
                        msResearch(searchedValue, myDepoData);
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
       
        private void msResearch(long searchedValue, MyDepoData myDepoData)
        {
            searchImage(searchedValue, myDepoData);
            string temperature = searchTemperature(searchedValue, myDepoData);
            CncResult cncResult = searchCncDatas(searchedValue, myDepoData);        
      
            updateDatas(temperature, cncResult);
        }

        private void searchImage(long searchedValue, MyDepoData myDepoData)
        {
            string result = imageSearcher.longSearch(searchedValue, myDepoData);
            setImage(result);
        }
        private string searchTemperature(long searchedValue, MyDepoData myDepoData)
        {
            string temperature;
            //Se la lista di file relativa al pirometro contiene elementi, si ricerca e si estrae la temperatura
            if (myDepoData.getPyrometerList().Any())
            {
                //Si ricava la riga del file in corrispondenza dei ms passati
                string pyroResult = pyrometerSearcher.longSearch(searchedValue, myDepoData);
                //Si estrae la temperatura dalla riga
                temperature = extractTemp(pyroResult);
            }
            else
            {
                temperature = "No value";
            }
            return temperature;
        }

        private CncResult searchCncDatas(long searchedValue, MyDepoData myDepoData)
        {
            CncResult cncResult = new CncResult();
            if (myDepoData.getCNList().Any())
            {
                //TODO: sostituire non appena si esegue il refactoring del problema I/O
                String measureString;
                if (myDepoData.checkOldVersion())
                {
                    measureString = File.ReadAllLines(myDepoData.getDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList().ElementAt(0);
                }
                else
                {
                    measureString = File.ReadAllLines(myDepoData.getCNCFileDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList().ElementAt(0);
                }
                string stringCncResult = cncSearcher.longSearch(searchedValue, myDepoData);
                cncResult.settingMeasure(measureString);
                cncResult.settingValues(stringCncResult);
                
            }
            else
            {
                cncResult.getMeasures().Add("No Cnc File");
                cncResult.getValues().Add("No values");
            }
            return cncResult;
        }



        //Clear and populate the stackpanel with temperature, laseron TODO, powerfeedback TODO datas
        private void updateDatas(string temperature, CncResult cncResult)
        {
            //Clear the stackPanel from the actual values
            datasStackPanel.Children.Clear();

            //Labels to append to the clean stackPanel
            Label Temperature = new Label();

            Temperature.Content = "Temperature:\t" + temperature;

            datasStackPanel.Children.Add(Temperature);

            //Si cicla le liste di cncResult
            List<String> measures = cncResult.getMeasures();
            List<String> values = cncResult.getValues();

            for(int i = 0; i < measures.Count; i++)
            {
                Label label = new Label();
                label.Content = measures[i] + ":\t" + values[i];
                datasStackPanel.Children.Add(label);
            }
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

            string temperature = searchTemperature(long.Parse(this.extActualMs.Text), myDepoData);
            CncResult cncResult = searchCncDatas(long.Parse(this.extActualMs.Text), myDepoData);
            updateDatas(temperature, cncResult);
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
            //shortMsResearch(actualMs, myDepoData);
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
            msResearch(actualMs, myDepoData);

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
