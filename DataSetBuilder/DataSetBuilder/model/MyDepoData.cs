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
        private DirectoryInfo directoryInfo;
        private List<String> images;
        private List<String> datas;
        private String DepoPath;

        private uint nrImages = 0;
        private ushort nrOtherFiles = 0;
        private uint actualImage = 0;

        public MyDepoData(String directoryPath)
        {
            this.directoryInfo = new DirectoryInfo(directoryPath);
            this.images = new List<string>();
            this.datas = new List<string>();
            this.DepoPath = directoryPath;
            initLists();
        }

        private void initLists()
        {
            foreach(var file in this.directoryInfo.GetFiles())
            {
                if (Path.GetExtension(file.Name).Equals(".txt"))
                {
                    nrOtherFiles++;
                    this.datas.Add(file.Name);
                }
                if (Path.GetExtension(file.Name).Equals(".jpeg"))
                {
                    nrImages++;
                    this.images.Add(file.Name);
                }
            }
        }
        public List<String> getImages()
        {
            return this.images;
        }
        public List<String> getDataFiles()
        {
            return this.datas;
        }

        //Increasing actual image number, the selected one
        public void upActualImage()
        {
            if (this.actualImage < nrImages-1)
            {
                this.actualImage++;
            }            
        }
        //Decreasing actual image number, the selected one
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
    }
}
