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
    public partial class SentenceTree : Form
    {
        
        public SentenceTree()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SentenceProcessor sp = new SentenceProcessor(textBox1.Text.Split());
            label1.Text = "";

            String[] baseStr = sp.BaseSentence();

            for (int i = 0; i < baseStr.Length; i++)
            {
                label1.Text += " " + baseStr[i];
            }
        }
    }
}
