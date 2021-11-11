using DataSetBuilder.model;
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
    class DepoTabItemController
    {
        //Object from graphical interface
        private Button playButton;
        private Button pauseButton;
        private Button prevButton;
        private Button nextButton;
        private ComboBox imageSpeed;
        private Image depoImage;
        
        //Other variables
        private String depoPath;
        private String dataPath;
        private IDictionary<String, MyDepoData> depoDatas = new Dictionary<String, MyDepoData>();
        private Boolean isAutomatic = false;

        public DepoTabItemController(Button play, Button pause, Button prev, Button next, ComboBox speed, Image image)
        {
            this.playButton = play;
            this.pauseButton = pause;
            this.prevButton = prev;
            this.nextButton = next;
            this.imageSpeed = speed;
            this.depoImage = image;
            initButtonsAction();
        }

        private void initButtonsAction()
        {
            this.playButton.Click += PlayButton_Click;
            this.pauseButton.Click += PauseButton_Click;
            this.prevButton.Click += PrevButton_Click;
            this.nextButton.Click += NextButton_Click;
        }

        public void setPath(String path)
        {
            this.depoPath = path;
        }

        internal void openDepsData(object sender, MouseButtonEventArgs e)
        {

            ListViewItem listViewItem = sender as ListViewItem;
            foo((string)listViewItem.Content);
            initFirstImage((string)listViewItem.Content);
        }

        private void foo(String depoName)
        {
            if (!depoDatas.ContainsKey(depoName))
            {
                this.dataPath = depoPath + @"\" + depoName;
                depoDatas.Add(depoName, new MyDepoData(dataPath));
            }
        }

        private void initFirstImage(String depoName)
        {
            MyDepoData myDepoData = depoDatas[depoName];
            BitmapImage bitmapImage = new BitmapImage(new Uri(dataPath + @"\" + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
            this.depoImage.Source = bitmapImage;
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            nextImage();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            MyDepoData myDepoData = depoDatas[getDepoName()];
            myDepoData.downActualImage();
            BitmapImage bitmapImage = new BitmapImage(new Uri(dataPath + @"\" + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
            this.depoImage.Source = bitmapImage;
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
            BitmapImage bitmapImage = new BitmapImage(new Uri(dataPath + @"\" + myDepoData.getImages().ElementAt((int)myDepoData.getActualImage()), UriKind.RelativeOrAbsolute));
            this.depoImage.Source = bitmapImage;
        }
        private String getDepoName()
        {
            String removeString = depoPath + @"\";
            int index = dataPath.IndexOf(removeString);
            string cleanPath = (index < 0) ? dataPath : dataPath.Remove(index, removeString.Length);
            return cleanPath;
        }
    }
}
