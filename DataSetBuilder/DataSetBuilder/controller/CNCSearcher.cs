using DataSetBuilder.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetBuilder.controller
{
    class CNCSearcher
    {
        private short count = 0;
        private ulong LRCounter = 0;
        private short bigO = 23;
        private short offset = -1;

        private string cncShortBS(List<string> cNCLines, long searchedValue, List<string> imagesList)
        {
            resetCounter();
            resetLRCount();
            offset = -1;
            string min = imagesList[0];
            min = extractMs(min);

            return cncShortBS(cNCLines, searchedValue, 0, cNCLines.Count() - 1, min);
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

        private string cncLongBS(List<string> cNCLines, long searchedValue, string minMsValue)
        {
            resetCounter();
            resetLRCount();
            offset = -1;
            return cncLongBS(cNCLines, searchedValue, 0, cNCLines.Count() - 1, minMsValue);
        }

        private string cncLongBS(List<string> cNCLines, long searchedMs, int left, int right, string minMsValue)
        {
            count++;
            if (left > right)
            {
                //Do nothing!
                resetCounter();
                if (LRCounter > 100)
                {
                    resetLRCount();
                    return cncLongBS(cNCLines, searchedMs + (offset * 7), 0, cNCLines.Count() - 1, minMsValue);
                }
                return cncLongBS(cNCLines, searchedMs + (offset * 7), 0, cNCLines.Count() - 1, minMsValue);
            }

            int middle = (left + right) / 2;
            string originalElement = cNCLines.ElementAt(middle);
            string element = extractFromCNCLine(originalElement);

            if ((searchedMs - long.Parse(minMsValue)) < 0)
            {
                offset = 1;
                resetCounter();
                return cncLongBS(cNCLines, searchedMs + offset, 0, cNCLines.Count() - 1, minMsValue);
            }
            if (count > bigO)
            {
                resetCounter();
                return cncLongBS(cNCLines, searchedMs + offset, 0, cNCLines.Count() - 1, minMsValue);
            }

            if ((long.Parse(element) - long.Parse(minMsValue)) == (searchedMs - long.Parse(minMsValue)))
            {
                return originalElement;
            }
            else if ((long.Parse(element) - long.Parse(minMsValue)) > (searchedMs - long.Parse(minMsValue)))
            {
                return cncLongBS(cNCLines, searchedMs, left, middle - 1, minMsValue);
            }
            else
            {
                return cncLongBS(cNCLines, searchedMs, middle + 1, right, minMsValue);
            }
        }
        private string extractMs(string msString)
        {
            string ms = msString;
            int start = "ms".Length;
            ms = ms.Substring(start, ms.IndexOf("_") - 2);
            return ms;
        }

        private string extractFromCNCLine(string originalElement)
        {
            string local = originalElement;
            int start = local.IndexOf("\t") + "\t".Length + 1;      //+1, jump the first line char
            local = local.Substring(start);
            local = local.Substring(0, local.IndexOf("\t"));
            return local;
        }

        private void resetCounter()
        {
            this.count = 0;
        }
        private void resetLRCount()
        {
            this.LRCounter = 0;
        }
        private List<string> extractFromCNCList(List<string> cNCLines)
        {
            cNCLines.RemoveAt(0);
            return cNCLines;
        }

        internal string shortSearch(long searchedValue, MyDepoData myDepoData)
        {
            List<string> CNCLines;
            if (myDepoData.checkOldVersion())
            {
                CNCLines = File.ReadAllLines(myDepoData.getDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList();
            }
            else
            {
                CNCLines = File.ReadAllLines(myDepoData.getCNCFileDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList();
            }

            CNCLines = extractFromCNCList(CNCLines);
            return cncShortBS(CNCLines, searchedValue, myDepoData.getImages()); ;
        }

        internal string longSearch(long searchedValue, MyDepoData myDepoData)
        {
            List<string> CNCLines;
            if (myDepoData.checkOldVersion())
            {
                CNCLines = File.ReadAllLines(myDepoData.getDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList();
            }
            else
            {
                CNCLines = File.ReadAllLines(myDepoData.getCNCFileDirectory() + @"\" + myDepoData.getCNList()[0]).Cast<string>().ToList();
            }
            CNCLines = extractFromCNCList(CNCLines);
            string minMsValue = extractMs(myDepoData.getImages()[0]);
            return cncLongBS(CNCLines, searchedValue, minMsValue);
        }
    }
}
