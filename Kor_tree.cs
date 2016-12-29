using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLCS.NLP
{
    class Kor_tree
    {
        public struct strArr
        {
            public string[] words;
            public string[] Link;
        }

        public class product_ordering_Corpos{
            //주문에 관한 단어들의 말뭉치에 관한 객체입니다

            public int Level = 0; int[] L_Cnt;
            public string[] Corposes;
            public strArr[] level_words; // 특정 메뉴를 뜻하는 말뭉치에서 각 단어의 index에 따라 저장한 배열
            //e.g 상하이[0] 스파이시[1] 버거[2]
            public string[] options;

            public product_ordering_Corpos(string[] corposes, int level)
            {
                Corposes = new string[corposes.Length];

                for (int i = 0; i < corposes.Length; i++ )
                {
                    Corposes[i] = corposes[i];
                }

                Level = level;
                level_words = new strArr[Level];
            }

            public product_ordering_Corpos(int level)
            {
                Level = level;
                level_words = new strArr[Level];
            }

            public void add_corpos(string cor){
                string[] temp = new string[Corposes.Length];

                for (int i = 0; i < Corposes.Length; i++ )
                {
                    temp[i] = Corposes[i];
                }

                Corposes = new string[temp.Length + 1];
                
                for (int i = 0; i < temp.Length; i++)
                {
                    Corposes[i] = temp[i];
                }
                Corposes[temp.Length] = cor;
                
            }

            public void add_corpos(string[] cor)
            {
                string[] temp = new string[Corposes.Length];

                for (int i = 0; i < Corposes.Length; i++)
                {
                    temp[i] = Corposes[i];
                }

                Corposes = new string[temp.Length + cor.Length];

                for (int i = 0; i < temp.Length; i++)
                {
                    Corposes[i] = temp[i];
                }
                for (int i = temp.Length; i < Corposes.Length; i++ )
                {
                    Corposes[i] = cor[i];
                }

            }

            public void add_corpos(string[] cor, int level)
            {
                
                level_words[level].words = new string[cor.Length];
                for (int i = 0; i < cor.Length; i++)
                {
                    level_words[level].words[i] = cor[i];
                }
            }

            public void registerOptions(string[] op)
            {
                options = new string[op.Length];
                for (int i = 0; i < op.Length; i++)
                {
                    options[i] = op[i];
                }
            }

            public bool searchOptions(string str)
            {
                bool result = false;

                for (int i = 0; i < options.Length; i++)
                {
                    if(str.CompareTo(options[i]) == 0){
                        result = true;
                    }
                }

                return result;
            }

            public string searchOption(string str)
            {
                string result = "";

                for (int i = 0; i < options.Length; i++)
                {
                    if (str.CompareTo(options[i]) == 0)
                    {
                        return options[i];
                    }
                }

                return result;
            }

            public void makeLink(string[] product_Names)
            {
                // 줄임말이 들어왔을 때 그 메뉴를 정확하게 인지하기 위해 각 케이스들을 분석하는 함수입니다

                for (int i = 0; i < Level; i++)
                {
                    level_words[i].Link = new string[level_words[i].words.Length];
                }

                for (int i = 0; i < product_Names.Length; i++)
                {
                    for (int j = 0; j < product_Names[i].Split(' ').Length; j++)
                    {
                        for (int k = 0; k < level_words[j].words.Length; k++ )
                        {
                            if (level_words[j].words[k].CompareTo(product_Names[i].Split(' ')[j]) == 0)
                            {
                                level_words[j].Link[k] += product_Names[i] + "@";
                            }
                        }
                        
                    }
                }

            }

            public string search(string product)
            {
                string[] product_token = product.Split(' ');
                strArr[] candidate = new strArr[product_token.Length];
                int[] Cnts = new int[product_token.Length];

                int[] myCnts = new int[candidate.Length];
                strArr[] proDic = new strArr[candidate.Length];
                for (int i = 0; i < candidate.Length; i++)
                {
                    proDic[i].Link = new string[100];
                }

                for (int i = 0; i < product_token.Length; i++ )
                {
                    for (int a = 0; a < Level; a++ )
                    {
                        for (int k = 0; k < level_words[a].words.Length; k++)
                        {
                            if(level_words[a].words[k].CompareTo(product_token[i]) == 0){
                                string[] tokens = level_words[a].Link[k].Split('@');

                                for (int j = 0; j < tokens.Length - 1; j++)
                                {
                                    bool isExist = false;

                                    for (int s = 0; s < myCnts[i]; s++ )
                                    {
                                        if(proDic[i].Link[s].CompareTo(tokens[j]) == 0){
                                            isExist = true;
                                        }
                                    }

                                    if(!isExist){
                                        proDic[i].Link[myCnts[i]] = tokens[j];
                                        myCnts[i]++;
                                    }
                                }

                            }
                        }
                    }
                }


                int allSmaeCnt = 0;
                string selected = "";


                for (int i = 0; i < myCnts[0]; i++)
                {
                    string checkStr = proDic[0].Link[i];
                    bool allSame = true;

                    for (int j = 1; j < proDic.Length; j++)
                    {
                        bool check1 = false;

                        for (int a = 0; a < myCnts[j]; a++)
                        {
                            if (checkStr.CompareTo(proDic[j].Link[a]) == 0)
                            {
                                check1 = true;
                            }
                        }

                        if (!check1)
                        {
                            allSame = false;
                        }
                    }

                    if(allSame){
                        selected += checkStr + "@";
                        allSmaeCnt++;
                    }
                }

                return selected;

            }

            public void makeCorpos()
            {
                int total = 0;

                for (int i = 0; i < Level; i++)
                {
                    total += level_words[i].words.Length;
                }

                Corposes = new string[total];

                int cnt = 0;

                for (int i = 0; i < Level; i++)
                {
                    for (int j = 0; j < level_words[i].words.Length; j++)
                    {
                        Corposes[cnt] = level_words[i].words[j];
                        cnt++;
                    }
                }
            }

            int getLevel(string str)
            {
                int level = 0;

                for (int i = 0; i < Level; i++)
                {
                    for (int a = 0; a < level_words[i].words.Length; a++ )
                    {
                        if(str.CompareTo(level_words[i].words[a]) == 0){
                            level = i;
                        }
                    }
                }

                return level;
            }

            public bool searchCorpos(string str)
            {
                bool result = false;

                for (int i = 0; i < Corposes.Length; i++)
                {
                    if(str.CompareTo(Corposes[i]) == 0){
                        result = true;
                        i = Corposes.Length;
                    }
                }

                return result;
            }

            public string processOrder(string orders)
            {
                string result = "";
                orders += "  ";
                string[] token = orders.Split(' ');

                string target = "", option = "", number = "";

                int current = 0;

                try
                {
                    for (int i = 0; i < token.Length; i++)
                    {
                        string tempTarget = "";

                        if (searchCorpos(token[i]))
                        {
                            tempTarget += token[i];
                            current = getLevel(token[i]);

                            while (current < getLevel(token[i + 1]) && searchCorpos(token[i]))
                            {
                                tempTarget += " " + token[i+1];
                                current = getLevel(token[i + 1]);

                                i++;
                            }

                            tempTarget = search(tempTarget);

                            if (target.CompareTo("") == 0)
                            {
                                target = tempTarget;
                            }
                            else
                            {
                                result += target + "#" + option + "#" + number + "^";
                                target = tempTarget;
                                option = ""; 
                            }
                        }

                        if (searchOptions(token[i]))
                        {
                            option += searchOption(token[i]) + "@";
                        }

                        if (WordGrammarDictionary.checkNumberWord(token[i]) != -1)
                        {
                            number = WordGrammarDictionary.checkNumberWord(token[i]) + "";
                        }

                        if (WordGrammarDictionary.checkNumberUnitWord(token[i], "개") != -1)
                        {
                            number = WordGrammarDictionary.checkNumberUnitWord(token[i], "개") + "";
                        }
                    }
                }
                catch(Exception e)
                {
                    string s = e.Message;
                }

                if (target.CompareTo("") != 0)
                {
                    result += target + "#" + option + "#" + number + "^";
                }

                return result;
            }

        }

        public string[] getTree()
        {
            return null;
        }

    }
}
