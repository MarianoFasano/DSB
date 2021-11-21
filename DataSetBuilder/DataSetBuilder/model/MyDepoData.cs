using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataSetBuilder.model
{
    class MyDepoData
    {
        
        private List<String> images;
        private List<String> pyrometers;
        private List<String> cns;
        //Those aren't necessary at the moment
        private List<String> visionBoxes;
        private List<String> dsbs;

        private String DepoPath;

        private String DeviceCN = "DeviceCN";
        private String DeviceDSB = "DeviceDSB";
        private String DevicePyrometer = "DevicePyrometer";
        private String imagePrefix = "DeviceImaging";
        private String visionBoxPrefix = "DeviceVisionBox";
        private String ending;

        private uint nrImages = 0;
        //private ushort nrOtherFiles = 0;
        private uint actualImage = 0;

        public MyDepoData(String directoryPath)
        {
            this.images = new List<string>();
            this.cns = new List<string>();
            this.pyrometers = new List<string>();
            //this.dsbs = new List<string>();
            //this.visionBoxes = new List<string>();
            this.DepoPath = directoryPath;
            this.ending = extractEnding(directoryPath);
            initLists(directoryPath);
        }

        private string extractEnding(string directoryPath)
        {
            String temp = directoryPath;
            int start = temp.IndexOf("Deposition") + "Deposition".Length;
            return temp.Substring(start, temp.Length - start);
        }

        private void initLists(String directoryPath)
        {
            //List of images
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath + @"\" + imagePrefix + ending);
            foreach (var file in directoryInfo.GetFiles())
            {
                if (Path.GetExtension(file.Name).Equals(".jpeg"))
                {
                    nrImages++;
                    this.images.Add(file.Name);
                }
            }

            //List of CN files
            directoryInfo = new DirectoryInfo(directoryPath + @"\" + DeviceCN);
            foreach (var file in directoryInfo.GetFiles())
            {
                if (Path.GetExtension(file.Name).Equals(".txt"))
                {
                    this.cns.Add(file.Name);
                }
            }
            //List of pyrometer files
            directoryInfo = new DirectoryInfo(directoryPath + @"\" + DevicePyrometer);
            foreach (var file in directoryInfo.GetFiles())
            {
                if (Path.GetExtension(file.Name).Equals(".txt"))
                {
                    this.pyrometers.Add(file.Name);
                }
            }

        }
        //Return the list containing the image names
        public List<String> getImages()
        {
            return this.images;
        }
        //Return the list containing the CN file name
        public List<String> getCNList()
        {
            return this.cns;
        }
        //Return the list containing the pyrometer file name
        public List<String> getPyrometerList()
        {
            return this.pyrometers;
        }

        //Increasing actual image number, the selected one --> help to avoid increasing over the max number
        public void upActualImage()
        {
            if (this.actualImage < nrImages-1)
            {
                this.actualImage++;
            }            
        }
        //Decreasing actual image number, the selected one --> help to avoid decreasing under the min number
        public void downActualImage()
        {
            if (this.actualImage > 0)
            {
                this.actualImage--;
            }
        }
        //Get the actual image number, the selected one
        public uint getActualImage()
        {
            return this.actualImage;
        }
        //Get the directory string
        public String getDirectory()
        {
            return this.DepoPath;
        }
        //Get the image directory name
        public string getImageDirectory()
        {
            return (imagePrefix + ending + @"\");
        }
    }
}
