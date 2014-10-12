using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AveragePacketLoss
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            string[] filePaths = GetAllFilesInFolder(this.txtPath.Text);
            List<int> allDropValues = new List<int>();
            
            // Create one list of int values of every dropped packet
            foreach(string filepath in filePaths){
                allDropValues.AddRange(GetAllDroppedPackages(filepath));
            }

            // At this point we have all the individual dropped values and can calculate the average and peak etc.
            int[] TotalAveragePeak = GetTotalAveragePeak(allDropValues);

            lblTotalValue.Text = TotalAveragePeak[0].ToString();
            lblAverageValue.Text = TotalAveragePeak[1].ToString();
            lblPeakValue.Text = TotalAveragePeak[2].ToString();
        }

        private int[] GetTotalAveragePeak(List<int> allValues)
        {           
            int total = 0;
            int average = 0;
            int peak = 0;

            // Set Total and Peak
            foreach(int value in allValues){
                total += value;

                if(value > peak){
                    peak = value;
                }
            }

            // Set average
            average = total / allValues.Count;

            return new int[3]{total, average, peak};
        }

        private string[] GetAllFilesInFolder(string path)
        {
            return Directory.GetFiles(path);
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

        private List<int> GetAllDroppedPackages(string path)
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

                    if(int.TryParse(sixthString, out n) == true){
                        AllDroppedPackages.Add(Convert.ToInt32(sixthString));
                    }                    
                }
            }

            return AllDroppedPackages;
        }
    }
}
