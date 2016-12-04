using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using LLCS.NLP;
using opennlp.tools.tokenize;
using opennlp.tools.postag;
using opennlp.tools.parser;
using java.io;
using java.util;
using java.text;

namespace LLCS.Forms
{
    public partial class POS_Tagger : Form
    {
        String[] words;
        SentimentalManager sm;

        int[] a = new int[10];

        public POS_Tagger()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            InputStream modelIn = new FileInputStream("en-pos-maxent.bin");
            POSModel model = new POSModel(modelIn);
            // initialize POSTaggerME
            POSTaggerME tagger = new POSTaggerME(model);

            words = textBox1.Text.Split();
            String[] result = tagger.tag(words);

            label1.Text = "";

            for (int i = 0; i < result.Length; i++)
            {
                label1.Text += result[i] + ", ";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            sm = new SentimentalManager();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double Score = sm.extract(textBox1.Text, "a");

            label1.Text = textBox1.Text + " : " + Score;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            double Score = sm.extract(textBox1.Text);

            label1.Text = textBox1.Text + " : " + Score;
        }

        private void POS_Tagger_Load(object sender, EventArgs e)
        {

        }
    }
}
