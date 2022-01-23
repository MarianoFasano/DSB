using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataSetBuilder.model
{
    public class MyDepoData
    {
        
        private List<String> images;
        private List<String> pyrometers;
        private List<List<String>> cns;
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
        private bool isOldVersion = false;

        public MyDepoData(String directoryPath)
        {
            this.images = new List<string>();
            this.cns = new List<List<string>>();
            this.pyrometers = new List<string>();
            //this.dsbs = new List<string>();
            //this.visionBoxes = new List<string>();
            this.DepoPath = directoryPath;
            //this.ending = extractEnding(directoryPath);       deprecated, because the folders name won't contain the creation date anymore
            initLists(directoryPath);
        }

  /* 
   * Funzione che estrae il finale del nome della directory --> la data
   * 
   * private string extractEnding(string directoryPath)
        {
            String temp = directoryPath;
            int start = temp.IndexOf("Deposition") + "Deposition".Length;
            return temp.Substring(start, temp.Length - start);
        }*/

        //Funzione che ritorna la booleana della versione del file system della deposizione --> vecchia=true, nuova=false
        //TODO: probabilmente l'accessibilità potrebbe diventare private, poiché l'accesso al file system avviene all'inizializzazione della classe
        public bool checkOldVersion()
        {
            return this.isOldVersion;
        }

        private void initLists(String directoryPath)
        {
            //Istanza della classe directoryInfo che permette in seguito di recuperare le cartelle e i file all'interno della cartella specificata dall'argomento
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            //Se la cartella contiene più di un file (il commento della deposizione), si tratta della vecchia versione del file system
            if (directoryInfo.GetFiles().Length > 1)
            {
                //Valore booleano che conferma che si tratta della vecchia versione messo a true
                this.isOldVersion = true;

                //Si cicla su ogni file ritornato dal metodo GetFiles() --> array di FileInfo!
                foreach (var file in directoryInfo.GetFiles())
                {
                    //Se l'estensione è jpeg il nome del file è aggiunto alla lista di stringhe dei nomi
                    if (Path.GetExtension(file.Name).Equals(".jpeg"))
                    {
                        //Si aumenta il conteggio delle immagini e si aggiunge il nome del file alla lista dedicata (lista di stringhe)
                        nrImages++;
                        this.images.Add(file.Name);
                    }
                    //Se l'estensione è txt, file di testo, e il nome contiene CNC --> si aggiunge la lista di righe alla lista di files CNC
                    else if (Path.GetExtension(file.Name).Equals(".txt") && file.Name.Contains("CNC"))
                    {
                        //CNS è una lista di lista di stringhe (i files CN possono essere più di uno)
                        List<String> cncFileLines = File.ReadLines(directoryPath + @"\" + file.Name).Cast<string>().ToList();
                        this.cns.Add(cncFileLines);
                    }
                    //Se l'estensione è txt, file di testo, e il nome contiene pyrometer --> si aggiungono le righe alla lista del pirometro
                    else if (Path.GetExtension(file.Name).Equals(".txt") && file.Name.Contains("Pyrometer"))
                    {
                        //Si recupera la lista di stringhe e si assegna alla variabile
                        List<String> pyrometersFileLines = File.ReadLines(directoryPath + @"\" + file.Name).Cast<string>().ToList();
                        this.pyrometers = pyrometersFileLines;
                    }
                    //DSB config file
                    /*else if (Path.GetExtension(file.Name).Equals(".config"))
                    {
                        List<String> dsbFileLines = (List<string>)File.ReadLines(directoryPath + "\t" + file.Name);
                        this.dsbs = dsbFileLines;
                    }*/
                }
            }
            else
            {
               
                    //Se la directory non contiene files, ma cartelle,
                    //si assegna all'istanza directoryInfo una nuova istanza basata sul percorso delle immagini
                    String imagePath = directoryPath + @"\" + imagePrefix;
                    directoryInfo = new DirectoryInfo(imagePath);

                    foreach (var file in directoryInfo.GetFiles())
                    {
                        //Si verifica che si tratta di immagini prima di aggiungere il nome alla lista
                        if (Path.GetExtension(file.Name).Equals(".jpeg"))
                        {
                            nrImages++;
                            this.images.Add(file.Name);
                        }
                    }
                    //si assegna all'istanza directoryInfo una nuova istanza basata sul percorso dei files cn
                    String cnPath = directoryPath + @"\" + DeviceCN;
                    directoryInfo = new DirectoryInfo(cnPath);

                    foreach (var file in directoryInfo.GetFiles())
                    {
                        //Si verifica che si tratta di un file di testo
                        if (Path.GetExtension(file.Name).Equals(".txt"))
                        {
                            //CNS è una lista di lista di stringhe (i files CN possono essere più di uno)
                            List<String> cncFileLines = File.ReadLines(cnPath + @"\" + (file.Name)).Cast<string>().ToList();
                            this.cns.Add(cncFileLines);
                        }
                    }
                    //si assegna all'istanza directoryInfo una nuova istanza basata sul percorso dei files del pirometro
                    String pyrometerPath = directoryPath + @"\" + DevicePyrometer;
                    directoryInfo = new DirectoryInfo(pyrometerPath);

                    foreach (var file in directoryInfo.GetFiles())
                    {
                        //Si verifica che si tratta di un file di testo
                        if (Path.GetExtension(file.Name).Equals(".txt"))
                        {
                            //Si recupera la lista di stringhe e si assegna alla variabile
                            List<String> pyrometersFileLines = File.ReadLines(pyrometerPath + @"\" + (file.Name)).Cast<string>().ToList();
                            this.pyrometers = pyrometersFileLines;
                        }
                    }
                               
                
            }
            

        }
        //Return the list containing the image names
        public List<String> getImages()
        {
            return this.images;
        }
        //Return the list containing the CN file name
        public List<List<String>> getCNList()
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
        //Si imposta il riferimento all'immagine attuale all'argomento passato
        public void setActualImage(int number)
        {
            this.actualImage = (uint)number;
        }
        //Get the directory string
        public String getDirectory()
        {
            return this.DepoPath;
        }
        //Get the image directory name
        public string getImageDirectory()
        {
            return (imagePrefix + @"\");
        }

        public string getPyroFileDirectory()
        {
            return (this.DepoPath + @"\" + this.DevicePyrometer);
        }

        public string getCNCFileDirectory()
        {
            return (this.DepoPath + @"\" + this.DeviceCN);
        }
        public uint getMaxNrImage()
        {
            return this.nrImages;
        }
    }
}
