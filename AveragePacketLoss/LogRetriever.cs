using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace AveragePacketLoss
{
    class LogRetriever
    {
        private string LeaguePath;
        private Patch[] Patches;

        /// <summary>
        /// Constructor for LogRetriever
        /// </summary>
        /// <param name="LeaguePath">Path to League of Legends folder</param>
        /// <param name="PatchNumber"></param>
        public LogRetriever(string LeaguePath)
        {
            this.LeaguePath = LeaguePath;
            SetPatchObjects();
        }

        /// <summary>
        /// Reads Patches.XML to gather information about the start and end date of various patch versions
        /// </summary>
        /// <returns>Array of Patch Objects</returns>
        public void SetPatchObjects()
        {
            List<Patch> patchList = new List<Patch>();

            XmlDocument document = new XmlDocument();
            document.Load(Application.StartupPath + @"/Patches.xml");

            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                double patchNumber = Double.Parse(node.Attributes["number"].Value);
                string startdatestr = node.SelectSingleNode("startdate").InnerText;
                string enddatestr = node.SelectSingleNode("enddate").InnerText;

                DateTime startdate = DateTimeFromString(startdatestr);
                DateTime enddate = DateTimeFromString(enddatestr);

                patchList.Add(new Patch(patchNumber, startdate, enddate));
            }

            this.Patches = patchList.ToArray();
        }

        /// <summary>
        /// Returns a DateTime object parsed from a string in yyyy-MM-dd format
        /// </summary>
        /// <param name="datestring">string in yyyy-MM-dd</param>
        /// <returns>DateTime object</returns>
        private static DateTime DateTimeFromString(string datestring)
        {
            return DateTime.ParseExact(datestring, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets files belonging to each patch
        /// </summary>
        /// <param name="patches">Patches to filter files by</param>
        /// <returns>Dictionary which has a list of filenames for every patch object</returns>
        private Dictionary<Patch, List<string>> GetFilesPerPatch(Patch[] patches)
        {
            string[] filenames = Directory.GetFiles(LeaguePath);
            Dictionary<Patch, List<string>> filenamesfrompatches = new Dictionary<Patch, List<string>>();

            foreach (string filename in filenames)
            {
                string datestring = Path.GetFileName(filename).Substring(0, 10);
                DateTime fileDate = DateTime.ParseExact(datestring, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

                // Loop through all patches and put in the right dictionary entry
                foreach (Patch patch in patches)
                {
                    if (fileDate > patch.EndDate || fileDate < patch.StartDate)
                    {
                        continue;
                    }
                    else
                    {
                        if (!filenamesfrompatches.ContainsKey(patch))
                        {
                            filenamesfrompatches.Add(patch, new List<string>());
                        }

                        filenamesfrompatches[patch].Add(filename);
                    }
                }
            }

            return filenamesfrompatches;
        }

        public Dictionary<Patch, int> GetAveragePerPatch()
        {
            Dictionary<Patch, int> averagePerPatch = new Dictionary<Patch, int>();
            Dictionary<Patch, List<string>> filesPerPatch = GetFilesPerPatch(this.Patches);

            // Get average drops for each patch
            foreach (KeyValuePair<Patch, List<string>> patchEntry in filesPerPatch)
            {               
                int average = GetAverage(patchEntry.Value.ToArray());
                averagePerPatch.Add(patchEntry.Key, average);
            }

            return averagePerPatch;
        }

        public List<int> GetTotalAveragePeak(string[] filepaths)
        {
            List<int> allDropValues = new List<int>();

            // Create one list of int values of every dropped packet
            foreach (string filepath in filepaths)
            {
                allDropValues.AddRange(GetAllLostPackets(filepath));
            }

            // At this point we have all the individual dropped values and can calculate the average and peak etc.           

            int total = 0;
            int average = 0;
            int peak = 0;

            // Set Total and Peak
            foreach (int value in allDropValues)
            {
                total += value;

                if (value > peak)
                {
                    peak = value;
                }
            }

            // Set average
            average = total / allDropValues.Count;

            return new List<int> { total, average, peak };
        }

        private int GetAverage(string[] filepaths)
        {
            return GetTotalAveragePeak(filepaths)[1];
        }

        private string[] GetAllFilesInFolder()
        {
            return Directory.GetFiles(LeaguePath);
        }
        
        private int GetAmountOfCommas(string line)
        {
            int count = 0;

            foreach (char c in line)
            {
                if (c == ',')
                {
                    count++;
                }
            }

            return count;
        }

        private List<int> GetAllLostPackets(string path)
        {
            List<int> AllDroppedPackages = new List<int>();

            // Get packet loss number from every line that has enough comma's to contain packet loss information            
            StreamReader file = new StreamReader(path);
            string line;

            while ((line = file.ReadLine()) != null)
            {
                if (GetAmountOfCommas(line) > 10)
                {
                    string sixthString = line.Split(',')[6];
                    int n;

                    if (int.TryParse(sixthString, out n) == true)
                    {
                        AllDroppedPackages.Add(Convert.ToInt32(sixthString));
                    }
                }
            }

            return AllDroppedPackages;
        }
    }
}
