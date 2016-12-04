using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLCS.NLP
{
    class WordBag
    {
        int Sum = 0;

        public int[] Cnts; //한번에 리턴받지 못하므로 전역변수로 저장합니다.
        public double[] TD_IDF;
        public int total = 0;

        string[] resultBags;
        public string[] bags;

        public int[,] transition_probability;

        public void getTransitionProbability( string[] documents )
        {
            transition_probability = new int[bags.Length, bags.Length];

            for (int k = 0; k < documents.Length; k++ )
            {
                string[] token = documents[k].Split(' ');

                for (int i = 0; i < token.Length - 1; i++ )
                {
                    int index1, index2;

                    index1 = LinearSearch(token[i]);
                    index2 = LinearSearch(token[i + 1]);

                    if(index1 != -1 && index2 != -1){
                        transition_probability[index1, index2]++;
                    }

                }
            }

        }

        public void get_TD_IDF( WordBag[] otherbags )
        {
            TD_IDF = new double[bags.Length];

            for (int k = 0; k < bags.Length; k++ )
            {
                double tf, idf;

                int countOfWordDocuments = 1;
                int totalFrequency = Cnts[k];

                for (int i = 0; i < otherbags.Length; i++ )
                {
                    int index = otherbags[i].BinarySearchWord(bags[k]);

                    if( index != -1 ){
                        countOfWordDocuments++;
                        totalFrequency += otherbags[i].Cnts[index];
                    }
                }

                try
                {
                    tf = Cnts[k] / totalFrequency;
                    idf = (otherbags.Length + 1) / countOfWordDocuments; idf = Math.Log(idf);

                    TD_IDF[k] = tf * idf;
                }
                catch (DivideByZeroException e)
                {
                    
                }
               

            }
        }

        public int LinearSearch(string word)
        {
            int index = -1;

            for (int i = 0; i < bags.Length; i++ )
            {
                if( string.Compare(word, bags[i]) == 0 ){
                    index = i; i = bags.Length;
                }
            }

            return index; 
        }

        public int BinarySearchWord( string word )
        {
            int index = -1;
            int current = bags.Length / 2;
            int distance = bags.Length / 2;

            while( distance >= 1 ){
                int i = string.Compare(word, bags[current]);

                distance = distance / 2;
                Console.WriteLine("current: "+current);
                if (string.Compare(word, bags[current]) == 0 || string.Compare(word, bags[current - 1]) == 0)
                {
                    index = current; Console.WriteLine("Result: " + current);
                    break;
                }
                else if (string.Compare(word, bags[current]) < 0)
                {
                    current -= distance;
                }else{
                    current += distance;
                }
            }

            return index;
        }

        public void SortingBag()
        {

            for (int a = 0; a < bags.Length; a++ )
            {
                bool isChange = false;

                for (int i = 0; i < bags.Length - 1; i++ )
                {

                    if (string.Compare(bags[i], bags[i+1]) > 0)
                    {
                        string temp = bags[i]; int temp2 = Cnts[i];
                        bags[i] = bags[i + 1]; Cnts[i] = Cnts[i + 1];
                        bags[i + 1] = temp; Cnts[i + 1] = temp2;

                        isChange = true;
                    }

                }

                if(!isChange){
                    a = bags.Length;
                }
            }

        }

        public int[] getBag(string[] Str, int numberOfWords)
        {
            bags = Str[0].Split(); Cnts = new int[bags.Length];
            resultBags = new string[numberOfWords];
            //NaiveBayes에 사용할 가장 빈도가 높은 단어들입니다. 데이터의 규모가 크다면 더 큰 배열을 사용 하시는게 좋습니다.

            int numberOfBags = bags.Length;

            for (int i = 0; i < numberOfBags; i++)
            {
                Cnts[i] = 0;
            }

            for (int i = 0; i < Str.Length; i++)
            {
                string[] mybag = Str[i].Split();

                for (int j = 0; j < mybag.Length; j++)
                {
                    bool isHaveSameBag = false; int SameBagIndex = -1;

                    for (int k = 0; k < bags.Length; k++)
                    {
                        if (mybag[j] == bags[k])
                        {
                            isHaveSameBag = true; SameBagIndex = k;
                        }
                    }

                    if (isHaveSameBag)
                    {
                        Cnts[SameBagIndex]++;
                    }
                    else
                    {
                        string[] tempBag = new string[numberOfBags + 1];
                        int[] tempCnt = new int[numberOfBags + 1];

                        for (int k = 0; k < numberOfBags; k++)
                        {
                            tempBag[k] = bags[k];
                            tempCnt[k] = Cnts[k];
                        }

                        tempBag[numberOfBags] = mybag[j]; tempCnt[numberOfBags] = 1;
                        numberOfBags++;

                        bags = new string[numberOfBags];
                        Cnts = new int[numberOfBags];

                        for (int k = 0; k < numberOfBags; k++)
                        {
                            bags[k] = tempBag[k];
                            Cnts[k] = tempCnt[k];
                        }

                    }
                }
            }

            for (int j = 0; j < numberOfBags; j++)
            {
                for (int i = 0; i < numberOfBags - 1; i++)
                {
                    if (Cnts[i] < Cnts[i + 1])
                    {
                        int temp = Cnts[i]; string temps = bags[i];
                        Cnts[i] = Cnts[i + 1]; bags[i] = bags[i + 1];
                        Cnts[i + 1] = temp; bags[i + 1] = temps;
                    }
                }
            }

            if (numberOfBags > numberOfWords)
            {
                for (int i = 0; i < numberOfWords; i++)
                {
                    resultBags[i] = bags[i];
                    total += Cnts[i];
                }
            }
            else
            {
                resultBags = new string[numberOfBags];

                for (int i = 0; i < numberOfBags; i++)
                {
                    resultBags[i] = bags[i];
                    total += Cnts[i];
                }
            }

            Sum = 0;

            for (int i = 0; i < Cnts.Length; i++ )
            {
                Sum += Cnts[i];
            }

            return Cnts;
        }

        public string[] getBag(string[] Str)
        {
            string[] bags = Str[0].Split(); Cnts = new int[bags.Length];
            string[] resultBags = new string[50];
            //NaiveBayes에 사용할 가장 빈도가 높은 단어들입니다. 데이터의 규모가 크다면 더 큰 배열을 사용 하시는게 좋습니다.

            int numberOfBags = bags.Length;

            for (int i = 0; i < numberOfBags; i++)
            {
                Cnts[i] = 1;
            }

            for (int i = 0; i < Str.Length; i++)
            {
                string[] mybag = Str[i].Split();

                for (int j = 0; j < mybag.Length; j++)
                {
                    bool isHaveSameBag = false; int SameBagIndex = -1;

                    for (int k = 0; k < bags.Length; k++)
                    {
                        if (mybag[j] == bags[k])
                        {
                            isHaveSameBag = true; SameBagIndex = k;
                        }
                    }

                    if (isHaveSameBag)
                    {
                        Cnts[SameBagIndex]++;
                    }
                    else
                    {
                        string[] tempBag = new string[numberOfBags + 1];
                        int[] tempCnt = new int[numberOfBags + 1];

                        for (int k = 0; k < numberOfBags; k++)
                        {
                            tempBag[k] = bags[k];
                            tempCnt[k] = Cnts[k];
                        }

                        tempBag[numberOfBags] = mybag[j]; tempCnt[numberOfBags] = 1;
                        numberOfBags++;

                        bags = new string[numberOfBags];
                        Cnts = new int[numberOfBags];

                        for (int k = 0; k < numberOfBags; k++)
                        {
                            bags[k] = tempBag[k];
                            Cnts[k] = tempCnt[k];
                        }

                    }
                }
            }

            for (int j = 0; j < numberOfBags; j++)
            {
                for (int i = 0; i < numberOfBags - 1; i++)
                {
                    if (Cnts[i] < Cnts[i + 1])
                    {
                        int temp = Cnts[i]; string temps = bags[i];
                        Cnts[i] = Cnts[i + 1]; bags[i] = bags[i + 1];
                        Cnts[i + 1] = temp; bags[i + 1] = temps;
                    }
                }
            }

            if (numberOfBags > 100)
            {
                for (int i = 0; i < 100; i++)
                {
                    resultBags[i] = bags[i];
                    total += Cnts[i];
                }
            }
            else
            {
                resultBags = new string[numberOfBags];

                for (int i = 0; i < numberOfBags; i++)
                {
                    resultBags[i] = bags[i];
                    total += Cnts[i];
                }
            }

            return resultBags;
        }
    }
}
