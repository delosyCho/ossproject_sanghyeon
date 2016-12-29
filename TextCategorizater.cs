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

namespace LLCS.NLP
{
    class TextCategorizater
    {
        WordBag wordbag;
        WordBag[] wb; int number = 0;

        public TextCategorizater(int n)
        {
            number = n;
            //분류 할 라벨의 갯수
        }

        public void getWordbags(OpenFileDialog openFileDialog1)
        {
            wb = new WordBag[number];
            // 다이얼로그를 통해 파일을 선택하고 그 텍스트 파일을 통해 WordBag을 생성합니다

            for (int a = 0; a < number; a++)
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
                    if (a != b)
                    {
                        temp[cnt] = wb[b];
                        cnt++;
                    }
                }

                wb[a].get_TD_IDF(temp);
            }

        }

        public double[] getScore(bool td_idf)
        {
            //각 라벨에 대한 점수(확률)을 계산합니다. 높을 수록 그 라벨의 주제에 대한 텍스트일 확률이 높음

            double[] score = new double[number];

            if (td_idf)
            {
                //이 경우, td_idf 값으로 계산 된 확률을 출력합니다

                for (int i = 0; i < wordbag.bags.Length; i++)
                {

                    for (int a = 0; a < number; a++)
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
                //단순 빈도수에 의해 계산된 확률을 출력합니다

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


            return score;
        }

        public string[] TextSummarization(int NumberOfSentence, string[] Sentence, int bagIndex)
        {
            string[] result = new string[NumberOfSentence];

            double[] maxScore = new double[NumberOfSentence];

            for (int i = 0; i < maxScore.Length; i++)
            {
                maxScore[i] = -999;
            }

            for (int i = 0; i < Sentence.Length; i++)
            {
                double score = 0;

                string[] token = Sentence[i].Split();

                for (int j = 0; j < token.Length; j++)
                {
                    int index = wb[bagIndex].LinearSearch(token[j]);

                    if (index != -1)
                    {
                        score += (wb[bagIndex].Cnts[index] * wb[bagIndex].TD_IDF[index]);
                    }
                }

                for (int j = 0; j < maxScore.Length; j++)
                {
                    if (maxScore[j] < score)
                    {
                        maxScore[j] = score;
                        j = maxScore.Length;
                    }
                }

                for (int x = 0; x < maxScore.Length; x++)
                {
                    for (int y = 0; y < maxScore.Length - 1; y++)
                    {
                        if(maxScore[y] > maxScore[y+1]){
                            double temp = maxScore[y];
                            maxScore[y] = maxScore[y + 1];
                            maxScore[y + 1] = temp;
                        }
                    }
                }
            }

            return result;
        }

    }
}
