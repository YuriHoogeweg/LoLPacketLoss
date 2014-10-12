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
            LogRetriever logretriever = new LogRetriever(this.txtPath.Text);
            
            Dictionary<Patch, int> averagePerPatch = logretriever.GetAveragePerPatch();
        }
    }
}
