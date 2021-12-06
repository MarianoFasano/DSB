using DataSetBuilder.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataSetBuilder.controller
{
    class PyrometerSearcher
    {
        private short count = 0;
        private ulong LRCounter = 0;
        private short bigO = 23;
        private short offset = -1;

        private string pyroShortBS(List<string> pyroLines, long searchedMs, List<string> imagesList)
        {
            resetCounter();
            resetLRCount();
            offset = -1;
            string min = imagesList[0];
            min = extractMs(min);

            return pyroShortBS(pyroLines, searchedMs, 0, pyroLines.Count() - 1, min);
        }

        private string pyroShortBS(List<string> pyroLines, long searchedMs, int left, int right, string min)
        {
            count++;
            if (left > right)
            {
                //Do nothing!
                LRCounter++;
                if (LRCounter > 100)
                {
                    resetLRCount();
                    return pyroShortBS(pyroLines, searchedMs + (offset * 7), 0, pyroLines.Count() - 1, min);
                }
                resetCounter();
                return pyroShortBS(pyroLines, searchedMs + offset, 0, pyroLines.Count() - 1, min);
            }

            int middle = (left + right) / 2;
            string originalElement = pyroLines.ElementAt(middle);
            string element = extractFromPyroLine(originalElement);

            if (searchedMs < 0)
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

        private string pyroLongBS(List<string> pyroLines, long searchedMs, string minMsValue)
        {
            resetCounter();
            offset = -1;
            return pyroLongBS(pyroLines, searchedMs, 0, pyroLines.Count() - 1, minMsValue);
        }

        private string pyroLongBS(List<string> pyroLines, long searchedMs, int left, int right, string minMsValue)
        {
            count++;
            if (left > right)
            {
                //Do nothing!
                resetCounter();
                if (LRCounter > 100)
                {
                    resetLRCount();
                    return pyroLongBS(pyroLines, searchedMs + (offset * 7), 0, pyroLines.Count() - 1, minMsValue);
                }
                return pyroLongBS(pyroLines, searchedMs + (offset * 7), 0, pyroLines.Count() - 1, minMsValue);
            }

            int middle = (left + right) / 2;
            string originalElement = pyroLines.ElementAt(middle);
            string element = extractFromPyroLine(originalElement);

            //MessageBox.Show((long.Parse(element) - long.Parse(minMsValue)).ToString(), "Ciao");
            //MessageBox.Show((searchedMs - long.Parse(minMsValue)).ToString(), "Ciao");

            if ((searchedMs - long.Parse(minMsValue)) < 0)
            {
                offset = 1;
                resetCounter();
                return pyroLongBS(pyroLines, searchedMs + offset, 0, pyroLines.Count() - 1, minMsValue);
            }

            if (count > bigO)
            {
                resetCounter();
                return pyroLongBS(pyroLines, searchedMs + offset, 0, pyroLines.Count() - 1, minMsValue);
            }

            if ((long.Parse(element) - long.Parse(minMsValue)) == (searchedMs - long.Parse(minMsValue)))
            {
                return originalElement;
            }
            else if ((long.Parse(element) - long.Parse(minMsValue)) > (searchedMs - long.Parse(minMsValue)))
            {
                return pyroLongBS(pyroLines, searchedMs, left, middle - 1, minMsValue);
            }
            else
            {
                return pyroLongBS(pyroLines, searchedMs, middle + 1, right, minMsValue);
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

        private string extractMs(string msString)
        {
            string ms = msString;
            int start = "ms".Length;
            ms = ms.Substring(start, ms.IndexOf("_") - 2);
            return ms;
        }

        private void resetCounter()
        {
            this.count = 0;
        }
        private void resetLRCount()
        {
            this.LRCounter = 0;
        }
        private List<string> extractFromPyroList(List<string> pyroLines)
        {
            //Hard coded, need refactoring
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(0);
            pyroLines.RemoveAt(pyroLines.Count - 1);
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

        internal string shortSearch(long searchedValue, MyDepoData myDepoData)
        {
            List<string> pyroLines;
            if (myDepoData.checkOldVersion())
            {
                pyroLines = File.ReadAllLines(myDepoData.getDirectory() + @"\" + myDepoData.getPyrometerList()[0]).Cast<string>().ToList();
            }
            else
            {
                pyroLines = File.ReadAllLines(myDepoData.getPyroFileDirectory() + @"\" + myDepoData.getPyrometerList()[0]).Cast<string>().ToList();
            }
            
            pyroLines = extractFromPyroList(pyroLines);
            return pyroShortBS(pyroLines, searchedValue, myDepoData.getImages());
        }

        internal string longSearch(long searchedValue, MyDepoData myDepoData)
        {
            List<string> pyroLines;
            if (myDepoData.checkOldVersion())
            {
                pyroLines = File.ReadAllLines(myDepoData.getDirectory() + @"\" + myDepoData.getPyrometerList()[0]).Cast<string>().ToList();
            }
            else
            {
                pyroLines = File.ReadAllLines(myDepoData.getPyroFileDirectory() + @"\" + myDepoData.getPyrometerList()[0]).Cast<string>().ToList();
            }
            pyroLines = extractFromPyroList(pyroLines);
            string minMsValue = extractMs(myDepoData.getImages()[0]);
            return pyroLongBS(pyroLines, searchedValue, minMsValue);
        }
    }
}