using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace LLCS.NLP
{
    //Datafile created by SentiWordNet : http://sentiwordnet.isti.cnr.it/  License : https://creativecommons.org/licenses/by-sa/3.0/

    class SentimentalManager
    {
        public IDictionary<String, double> dictionary;

        public SentimentalManager()
        {
            String path = "C:\\Users\\Administrator\\Desktop\\SentiWordNet_Data_LLCS.txt";
            Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
            
            System.IO.StreamReader fs = new StreamReader(path, encode);
            
            dictionary = new Dictionary<String, double>();

            String line = "";

            while ((line = fs.ReadLine()) != null)
            {
                String[] token = line.Split('@');

                if (token.Length == 2)
                {
                    double score = double.Parse(token[1]);
                    dictionary.Add(token[0], score);
                }
                else
                {

                }
                
            }
            
        }


        public double extract(String word, String pos)
        {
            String key = word + "#" + pos;

            return dictionary[key];
        }

        public double extract(String word)
        {
            return dictionary[word];
        }


    }
}
