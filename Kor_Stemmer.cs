using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LLCS.NLP;

namespace LLCS.Forms
{
    
    public partial class Kor_Stemmer : Form
    {
        HangeulStemmer ks;

        public Kor_Stemmer()
        {
            InitializeComponent();
            ks = new HangeulStemmer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String test = textBox1.Text;
            HangeulStemmer hs = new HangeulStemmer();

            String str = hs.Seperate(test);
            char[] v = str.ToCharArray();
            label1.Text += (int)v[1] + " " + (int)v[2];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ks.LearningTorch(openFileDialog1.FileName);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ks.check();
        }
    }
}
