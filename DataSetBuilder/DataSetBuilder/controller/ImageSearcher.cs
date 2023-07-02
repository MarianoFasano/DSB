using DataSetBuilder.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataSetBuilder.controller
{
    class ImageSearcher
    {
        private short count = 0;
        private ulong LRCounter = 0;
        private short bigO = 23;
        private short offset = -1;


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

        internal string longSearch(long searchedValue, MyDepoData myDepoData)
        {
            return longBinarySearch(searchedValue, myDepoData.getImages());
        }
        private String longBinarySearch(long searchedMs, List<string> imagesList)
        {
            //ALGO IMPLEMENTETION, TWO WAYS
            resetCounter();
            resetLRCount();
            offset = -1;
            return longBinarySearch(imagesList, searchedMs, 0, imagesList.Count - 1);

        }

        private string longBinarySearch(List<string> imagesList, long searchedMs, int left, int right)
        {
            count++;
            if (left > right)
            {
                //Do nothing!
                resetCounter();
                if (LRCounter > 100)
                {
                    resetLRCount();
                    return longBinarySearch(imagesList, searchedMs + (offset * 7), 0, imagesList.Count() - 1);
                }
                return longBinarySearch(imagesList, searchedMs + offset, 0, imagesList.Count() - 1);
            }

            int middle = (left + right) / 2;
            string originalElement = imagesList[middle];
            string extractElement = extractMs(originalElement);
            string minimum = imagesList[0];
            minimum = extractMs(minimum);

            if ((searchedMs - long.Parse(minimum)) < 0)
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

            if ((long.Parse(extractElement) - long.Parse(minimum)) == (searchedMs - long.Parse(minimum)))
            {
                return originalElement;
            }
            else if ((long.Parse(extractElement) - long.Parse(minimum)) > (searchedMs - long.Parse(minimum)))
            {
                return longBinarySearch(imagesList, searchedMs, left, middle - 1);
            }
            else
            {
                return longBinarySearch(imagesList, searchedMs, middle + 1, right);
            }
        }

/*
 *      Obsoleta, la funzionalità estraeva i ms significativi
 * 
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
*/
    }
}
