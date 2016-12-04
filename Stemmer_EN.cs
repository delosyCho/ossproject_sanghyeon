using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.IO;

using LLCS.NLP;

namespace LLCS.Forms
{
    public partial class Stemmer_EN : Form
    {
        public Stemmer_EN()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            PorterStemmer_EN s = new PorterStemmer_EN();
            try
            {
                label1.Text = s.StemWord(textBox1.Text);
            }
            catch
            {

            }
            
        }
    }
}
