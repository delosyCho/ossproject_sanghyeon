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

namespace LLCS.Forms
{
    public partial class HMM_forn : Form
    {
        WordBag wordbag;

        WordBag[] wb; int number = 0;

        public HMM_forn()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int result = wordbag.BinarySearchWord(textBox1.Text);

            label1.Text = "Result: " + result;
        }

        private void HMM_forn_Load(object sender, EventArgs e)
        {
            wordbag = new WordBag();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");

                FileStream fs2 = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);


                string dataFileName = openFileDialog1.FileName;
                System.IO.StreamReader fs = new StreamReader(openFileDialog1.FileName, encode);

                string str = fs.ReadLine();
                string wholeWords = "";

                while (str != null)
                {
                    wholeWords += str;

                    str = fs.ReadLine();
                }

                string[] bags = wholeWords.Split(' ');

                wordbag.getBag(bags, 15);
                wordbag.SortingBag();

                int result = wordbag.BinarySearchWord("approaches");

                label1.Text = "Result: " + wordbag.Cnts.Length;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "Length : " + wordbag.bags.Length + " ";

            for (int i = 0; i < wordbag.bags.Length; i++)
            {
                label1.Text += " , " + wordbag.bags[i];
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            number = int.Parse(textBox1.Text);

            wb = new WordBag[number];


            for (int a = 0; a < number; a++ )
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    wb[a] = new WordBag();

                    Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");

                    FileStream fs2 = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);


                    string dataFileName = openFileDialog1.FileName;
                    System.IO.StreamReader fs = new StreamReader(openFileDialog1.FileName, encode);

                    string str = fs.ReadLine();
                    string wholeWords = "";

                    while (str != null)
                    {
                        wholeWords += str;

                        str = fs.ReadLine();
                    }

                    string[] bags = wholeWords.Split(' ');

                    wb[a].getBag(bags, 15);
                    wb[a].SortingBag();
                }
            }

            for (int a = 0; a < number; a++)
            {
                WordBag[] temp = new WordBag[number - 1];

                int cnt = 0;

                for (int b = 0; b < number; b++)
                {
                    if(a != b){
                        temp[cnt] = wb[b];
                        cnt++;
                    }
                }

                wb[a].get_TD_IDF(temp);
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }


        private void button7_Click(object sender, EventArgs e)
        {
            double[] score = new double[number];

            if (checkBox1.Checked)
            {
                for (int i = 0; i < wordbag.bags.Length; i++)
                {

                    for (int a = 0; a < number; a++ )
                    {
                        if (wb[a].LinearSearch(wordbag.bags[i]) != -1)
                        {
                            int index = wb[a].LinearSearch(wordbag.bags[i]);
                            score[a] += wb[a].Cnts[index] * wb[a].TD_IDF[index];
                        }
                    }
                    
                }
            }
            else
            {
                for (int i = 0; i < wordbag.bags.Length; i++)
                {

                    for (int a = 0; a < number; a++)
                    {
                        if (wb[a].LinearSearch(wordbag.bags[i]) != -1)
                        {
                            int index = wb[a].LinearSearch(wordbag.bags[i]);
                            score[a] += wb[a].Cnts[index];
                        }
                    }

                }
            }

            label1.Text = "";

            for (int a = 0; a < number; a++ )
            {
                label1.Text += "  Score" + a + " :  "  + score[a];
            }

            

            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            
        }
    }
}
