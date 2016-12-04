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

namespace LLCS.NLP
{

    class SentenceProcessor
    {

        class WordNode{
            public String word;
            public String tag;

            public WordNode[] childs;

            int numberOfChild;

            public WordNode(String myword, String mytag)
            {
                word = myword;
                tag = mytag;

                numberOfChild = 0;
            }

            public void addChild(String cTag, String cWord) {

                if (numberOfChild > 1)
                {
                    childs = new WordNode[1];
                    childs[0] = new WordNode(cWord, cTag);
                }
                else
                {
                    WordNode[] temps = new WordNode[numberOfChild];

                    for (int i = 0; i < numberOfChild; i++)
                    {
                        temps[i] = childs[i];
                    }

                    numberOfChild++;

                    childs = new WordNode[numberOfChild];

                    for (int i = 0; i < numberOfChild - 1; i++)
                    {
                        childs[i] = temps[i];
                    }

                    childs[numberOfChild - 1] = new WordNode(cWord, cTag);

                }

            }
        }

        String[] words;
        String[] tempWords;
        String[] POS_Tags;
        
        InputStream modelIn;
        POSModel model;
        POSTaggerME tagger;

        public SentenceProcessor(String[] words)
        {
            this.words = new String[words.Length];
            tempWords = new String[words.Length];
            POS_Tags = new String[words.Length];

            for (int i = 0; i < words.Length; i++ )
            {
                this.words[i] = words[i];
                tempWords[i] = words[i];
            }

            modelIn = new FileInputStream("en-pos-maxent.bin");
            model = new POSModel(modelIn);
            tagger = new POSTaggerME(model);

            POS_Tags = getPOS_Tags();
        }

        public void setWords(String[] w)
        {
            words = new String[w.Length];

            for (int i = 0; i < w.Length; i++)
            {
                words[i] = w[i];
            }
        }

        String[] getPOS_Tags()
        {
            String[] result = tagger.tag(words);

            return result;
        }

        bool checkNoun(String tag)
        {
            if (('N' == tag.ToCharArray()[0] && 'N' == tag.ToCharArray()[1])
                || tag.CompareTo("PRP") == 0 )
            {
                return true;
            }

            return false;
        }

        bool checkWord(String pos, String tag)
        {
            if (pos.ToCharArray()[0] == tag.ToCharArray()[0] && pos.ToCharArray()[1] == tag.ToCharArray()[1])
            {
                return true;
            }

            return false;
        }

        bool haveCheck(String tag)
        {
            if (tag.CompareTo("have") == 0 || tag.CompareTo("has") == 0 || tag.CompareTo("had") == 0 || tag.CompareTo("haven't") == 0 ||
                tag.CompareTo("hadn't") == 0 || tag.CompareTo("hasn't") == 0)
            {
                return true;
            }

            return false;
        }

        bool beVerbCheck(String word)
        {
            bool result = false;

            if(word.CompareTo("be") == 0 || word.CompareTo("are") == 0 || word.CompareTo("is") == 0
                || word.CompareTo("were") == 0 || word.CompareTo("was") == 0 || word.CompareTo("am") == 0)
            {
                result = true;
            }

            return result;
        }

        public void NNcheck(int cnt, int max, String[] mytags, String[] words)
        {
            if (checkNoun(mytags[cnt]) && checkNoun(mytags[cnt+1]))
            {

                if(cnt + 1 <  max - 1){
                    NNcheck(cnt + 1, max, mytags, words);
                }

                words[cnt] += " " + words[cnt + 1];
                mytags[cnt + 1] = "null";

            }
        }

        public void TreeStep0(String[] mytags)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (checkNoun(mytags[i]))
                {
                    NNcheck(i, mytags.Length, mytags, words);
                }
            }
        }

        public void TreeStep1(String[] mytags)
        {
            for (int i = 0; i < mytags.Length; i++)
            {
                if (mytags[i].CompareTo("RB") == 0 || mytags[i].CompareTo("DT") == 0 ||
                    mytags[i].CompareTo("CD") == 0 || mytags[i].CompareTo("MD") == 0 || 
                    mytags[i].CompareTo("PRP$") == 0)
                {
                    mytags[i] = "null";
                }
            }
        }

        public void TreeStep1_1(String[] mytags, String[] words)
        {
            for (int i = 0; i < mytags.Length; i++)
            {
                if (beVerbCheck(words[i]))
                {
                    if(mytags[i+1].CompareTo("VBG") == 0){
                        mytags[i + 1] = "null";
                    }
                    else if (mytags[i + 1].CompareTo("VBD") == 0 || mytags[i + 1].CompareTo("VBN") == 0)
                    {
                        mytags[i + 1] = "null";
                    }
                }
            }
        }

        public void TreeStep2(String[] mytags)
        {
            for (int i = 0; i < mytags.Length; i++)
            {
                if (mytags[i].CompareTo("IN") == 0)
                {
                    while(!checkWord("NN", mytags[i])){
                        mytags[i] = "null";
                        i++;

                        if( i >= mytags.Length ){
                            return;
                        }
                    }

                    mytags[i] = "null";
                }
            }
        }

        public void TreeStep3(String[] mytags)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (checkWord("JJ", mytags[i]))
                {
                    if (checkWord("NN", mytags[i]))
                    {
                        mytags[i] = "null";
                    }
                }
            }
        }

        public void TreeStep4(String[] mytags)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (mytags[i].CompareTo("TO") == 0)
                {
                    if (checkWord("NN", mytags[i+1]))
                    {
                        mytags[i] = "null";
                        mytags[i+1] = "null";
                    }
                }
            }
        }

        public void TreeStep5(String[] mytags)
        {
            for (int i = 1; i < mytags.Length - 1; i++)
            {
                if (mytags[i].CompareTo("TO") == 0 && haveCheck(words[i-1]))
                {
                    if (mytags[i+1].CompareTo("VB") == 0)
                    {
                        words[i - 1] = words[i + 1];
                        mytags[i] = "null";
                        mytags[i + 1] = "null";
                    }
                }
            }
        }

        public void TreeStep6(String[] mytags)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (haveCheck(words[i]) && checkWord("VB", mytags[i+1]))
                {
                    words[i] = words[i + 1];
                    mytags[i + 1] = "null";
                }
            }
        }

        public void TreeStep7(String[] mytags)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (mytags[i].CompareTo("TO") == 0 && mytags[i+1].CompareTo("VB") == 0)
                {
                    mytags[i] = "null";
                    mytags[i + 1] = "null";
                }
            }
        }

        public void TreeStep8(String[] mytags)
        {
            for (int i = 0; i < mytags.Length - 2; i++)
            {
                if (checkNoun(mytags[i]) && mytags[i + 1].CompareTo("VBG") == 0)
                {
                    mytags[i+1] = "null";
                    if( checkNoun(mytags[i+2]) ){
                        mytags[i + 2] = "null";
                    }
                }
            }
        }

        public void TreeStep9(String[] mytags)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (checkNoun(mytags[i]) &&
                    (mytags[i + 1].CompareTo("VBG") == 0 || checkNoun(mytags[i + 1])))
                {
                    mytags[i + 1] = "null";
                    
                }
            }
        }

        public void TreeStep10(String[] mytags)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (checkWord("JJ", mytags[i]) && checkNoun(mytags[i + 1]) )
                {
                    mytags[i] = "null";
                }
            }
        }

        public void renewTags(String[] mytags)
        {
            for (int i = 0; i < mytags.Length; i++ )
            {
                if(mytags[i].CompareTo("null") == 0){
                    pushTag(mytags, i);
                }
            }
        }

        public void pushTag(String[] mytags, int cnt)
        {
            for (int i = cnt; i < mytags.Length - 1; i++)
            {
                mytags[i] = mytags[i + 1];
                words[i] = words[i + 1];
                
            }
            mytags[mytags.Length - 1] = "null";
        }

        public String[] BaseSentence()
        {
            String[] mytags = new String[words.Length];
            String[] result = new String[words.Length];

            WordNode[] nodes = new WordNode[words.Length];

            for (int i = 0; i < mytags.Length; i++)
            {
                mytags[i] = POS_Tags[i];
            }

            TreeStep0(mytags); renewTags(mytags);
            TreeStep1(mytags); renewTags(mytags);
            TreeStep1_1(mytags, words); renewTags(mytags);
            TreeStep2(mytags); renewTags(mytags);
            TreeStep3(mytags); renewTags(mytags);
            TreeStep4(mytags); renewTags(mytags);
            TreeStep5(mytags); renewTags(mytags);
            TreeStep6(mytags); renewTags(mytags);
            TreeStep7(mytags); renewTags(mytags);
            TreeStep8(mytags); renewTags(mytags);
            TreeStep9(mytags); renewTags(mytags);
            TreeStep10(mytags); renewTags(mytags);
            
            for (int i = 0; i < words.Length; i++)
            {
                if (mytags[i].CompareTo("null") != 0)
                {
                    result[i] = words[i];
                }
                else
                {
                    result[i] = "";
                }
            }

            return result;
        }

    }
}
