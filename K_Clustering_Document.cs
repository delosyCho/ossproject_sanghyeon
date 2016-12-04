using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LLCS.DataType;
using LLCS.MachineLearning;


namespace LLCS.Forms
{
    public partial class K_Clustering_Document : Form
    {
        Dataframe dataframe;

        public K_Clustering_Document()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string dataFileName = openFileDialog1.FileName;
                System.IO.StreamReader fs = System.IO.File.OpenText(openFileDialog1.FileName);

                string[] token = fs.ReadLine().Split(' ');

                int length = int.Parse(token[0]);
                int numLabel = int.Parse(token[1]);

                dataframe = new Dataframe(numLabel, length);

                for (int i = 0; i < length; i++ )
                {
                    token = fs.ReadLine().Split('@');

                    for(int j = 0; j < numLabel; j++){
                        dataframe.Features[j, i] = double.Parse(token[j]);
                    }
                }
            }
            else
            {
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int[] result = BasicMethod.K_Means_Clustering(dataframe, label1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataframe = new Dataframe();
            dataframe.makeRandomData(9, 500);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //int[] result = BasicMethod.K_Means_Clustering(dataframe, label1);
        }
    }
}
