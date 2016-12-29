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

    public class SentenceProcessor
    {
        //영어의 5가지 형식을 위해 필수적인 단어를 제외한 수식어구를 제거하거나, 트리에 붙이고
        //영어의 문장 형식을 결정해주는 기능입니다

        public int sentenceType = 0;
        public string[] SetTags = null;

        public double Score = 0;

        public class Sentence{
            public int type; // 결정 된 영어 문장 형식이 들어 갈 변수
            public int num = 0;

            Sentence[] child;

            public Sentence(int t){
                type = t;
                num = 0;
            }

            public void addChild(Sentence chi){
                Sentence[] temp = new Sentence[num];
                
                num++;

                child = new Sentence[num];

                for(int i = 0; i > num - 1; i++){
                    child[i] = temp[i];
                }

                child[num - 1] = chi;

            }
        }

        public class WordNode
        {
            //Tree를 표현하기 위한 Linked Object 입니다

            public String word; // 본 노드의 단어
            public String tag; // 품사

            public WordNode[] childs; // 노드의 자식

            int numberOfChild;

            public WordNode(String myword, String mytag)
            {
                word = myword;
                tag = mytag;

                numberOfChild = 0;
            }

            public void copy(WordNode node)
            {
                word = node.word;
                tag = node.tag;
                if( node.childs != null ){
                    childs = node.childs;
                }
                else
                {
                    childs = null;
                }
            }

            public void addChild(String cTag, String cWord, WordNode node)
            {

                if (childs != null)
                {
                    WordNode[] temps = new WordNode[childs.Length];

                    for (int i = 0; i < childs.Length; i++)
                    {
                        temps[i] = childs[i];
                    }

                    numberOfChild++;

                    childs = new WordNode[childs.Length + 1];

                    for (int i = 0; i < childs.Length - 1; i++)
                    {
                        childs[i] = temps[i];
                    }

                    childs[childs.Length - 1] = new WordNode(cWord, cTag);

                    if (node.childs != null)
                    {
                        childs[childs.Length - 1].childs = node.childs;
                    }
                }
                else
                {
                    childs = new WordNode[1];
                    childs[0] = new WordNode(cWord, cTag);

                    numberOfChild++;
                }

            }

        }

        String[] words;
        String[] tempWords;
        String[] POS_Tags;

        InputStream modelIn;
        POSModel model;
        POSTaggerME tagger;

        public WordNode[] nodes;

        public SentenceProcessor(String[] words)
        {
            this.words = new String[words.Length];
            tempWords = new String[words.Length];
            POS_Tags = new String[words.Length];

            for (int i = 0; i < words.Length; i++)
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

        public String[] getPOS_Tags()
        {
            String[] result = tagger.tag(words);

            return result;
        }

        public bool checkNoun(String tag)
        {
            if (('N' == tag.ToCharArray()[0] && 'N' == tag.ToCharArray()[1])
                || tag.CompareTo("PRP") == 0 || tag.CompareTo("FW") == 0)
            {
                return true;
            }

            return false;
        }

        public bool checkWord(String pos, String tag)
        {
            if (pos.ToCharArray()[0] == tag.ToCharArray()[0] && pos.ToCharArray()[1] == tag.ToCharArray()[1])
            {
                return true;
            }

            return false;
        }

        public bool haveCheck(String tag)
        {
            if (tag.CompareTo("have") == 0 || tag.CompareTo("has") == 0 || tag.CompareTo("had") == 0 )
            {
                return true;
            }

            return false;
        }

        public bool beVerbCheck(String word)
        {
            bool result = false;

            if (word.CompareTo("be") == 0 || word.CompareTo("are") == 0 || word.CompareTo("is") == 0
                || word.CompareTo("were") == 0 || word.CompareTo("was") == 0 || word.CompareTo("am") == 0)
            {
                result = true;
            }

            return result;
        }

        public void NNcheck(int cnt, int max, String[] mytags, String[] words)
        {
            if (checkNoun(mytags[cnt]) && checkNoun(mytags[cnt + 1]))
            {

                if (cnt + 1 < max - 1)
                {
                    NNcheck(cnt + 1, max, mytags, words);
                }

                words[cnt] += " " + words[cnt + 1];
                mytags[cnt + 1] = "null";

            }
        }

        public void TreeStep0(String[] mytags, String[] words, WordNode[] nodes)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (checkWord("NN", mytags[i]))
                {
                    NNcheck(i, mytags.Length, mytags, words);
                }
            }
        }

        public void TreeStep1(String[] mytags, String[] words, WordNode[] nodes)
        {
            int pos = 0;

            for (int i = 0; i < mytags.Length; i++)
            {
                if (mytags[i].CompareTo("RB") == 0 || mytags[i].CompareTo("DT") == 0 ||
                    mytags[i].CompareTo("CD") == 0 || mytags[i].CompareTo("MD") == 0 ||
                    mytags[i].CompareTo("PRP$") == 0)
                {
                    
                    if (mytags[i].CompareTo("RB") == 0)
                    {
                        if (words[i].CompareTo("really") == 0 || words[i].CompareTo("so") == 0)
                        {
                            for (int k = i + 1; k < mytags.Length; k++)
                            {
                                if (checkWord("JJ", mytags[k]) || checkWord("VB", mytags[k]))
                                {
                                    nodes[k].addChild(mytags[i], words[i], nodes[i]);
                                    k = mytags.Length;
                                }
                            }
                        }
                        else if (words[i].CompareTo("not") == 0)
                        {
                            for (int a = i; a < mytags.Length; a++ )
                            {
                                if (checkWord("JJ", mytags[a]) || checkWord("VB", mytags[a]))
                                {
                                    nodes[a].addChild(mytags[i], words[i], nodes[i]);
                                    a = mytags.Length;
                                }
                            }
                        }
                        else
                        {
                            nodes[pos].addChild(mytags[i], words[i], nodes[i]);
                        }

                    }

                    mytags[i] = "null";
                }

                if (checkWord("VB", mytags[i]) || checkWord("JJ", mytags[i]))
                {
                    pos = i;
                }
            }
        }

        public void TreeStep1_1(String[] mytags, String[] words, WordNode[] nodes)
        {
            int pos = 0;

            for (int i = 0; i < mytags.Length; i++)
            {
                if (words[i].CompareTo("don't") == 0 || words[i].CompareTo("didn't") == 0 || words[i].CompareTo("hasn't") == 0 ||
                    words[i].CompareTo("hadn't") == 0 || words[i].CompareTo("haven't") == 0)
                {
                    for (int a = i+1; a < mytags.Length; a++)
                    {
                        if (checkWord("VB", mytags[a]))
                        {
                            nodes[a].addChild(mytags[i], "not", nodes[i]);
                            a = mytags.Length;
                        }
                    }

                    mytags[i] = "null";
                }


            }
        }

        public void TreeStep1_2(String[] mytags, String[] words, WordNode[] nodes)
        {
            for (int i = 0; i < mytags.Length; i++)
            {
                if (beVerbCheck(words[i]))
                {
                    if (mytags[i + 1].CompareTo("VBG") == 0)
                    {
                        nodes[i].addChild(mytags[i + 1], words[i + 1], nodes[i + 1]);
                        mytags[i + 1] = "null";
                    }
                    else if (mytags[i + 1].CompareTo("VBD") == 0 || mytags[i + 1].CompareTo("VBN") == 0)
                    {
                        nodes[i].addChild(mytags[i + 1], words[i + 1], nodes[i + 1]);
                        mytags[i + 1] = "null";
                    }
                }
            }
        }

        public void TreeStep2(String[] mytags, String[] words, WordNode[] nodes)
        {
            int pos = 0;

            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (mytags[i].CompareTo("IN") == 0 && checkNoun(mytags[i + 1]))
                {
                    nodes[pos].addChild(mytags[i+1], words[i+1], nodes[i + 1]);
                    mytags[i] = "null";
                    mytags[i+1] = "null";
                }

                if (checkWord("VB", mytags[i]) || checkWord("JJ", mytags[i]))
                {
                    pos = i;
                }
            }
        }

        public void TreeStep3(String[] mytags, String[] words, WordNode[] nodes)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if(checkNoun(mytags[i])){
                    if(checkWord("JJ", mytags[i+1])){
                        nodes[i].addChild(mytags[i + 1], words[i + 1], nodes[i + 1]);
                        mytags[i + 1] = "null";
                    }
                }
            }
        }

        public void TreeStep4(String[] mytags, String[] words, WordNode[] nodes)
        {
            int pos = 0;


            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (mytags[i].CompareTo("TO") == 0)
                {
                    if (checkWord("NN", mytags[i + 1]))
                    {
                        nodes[pos].addChild(mytags[i + 1], words[i + 1], nodes[i + 1]);

                        mytags[i] = "null";
                        mytags[i + 1] = "null";
                    }
                }

                if (checkWord("VB", mytags[i]) || checkWord("JJ", mytags[i]))
                {
                    pos = i;
                }
            }
        }

        public void TreeStep5(String[] mytags, String[] words, WordNode[] nodes)
        {
            for (int i = 1; i < mytags.Length - 1; i++)
            {
                if (mytags[i].CompareTo("TO") == 0 && haveCheck(words[i - 1]))
                {
                    if (mytags[i + 1].CompareTo("VB") == 0)
                    {
                        words[i - 1] = words[i + 1];
                        mytags[i] = "null";
                        mytags[i + 1] = "null";
                    }
                }
                else if (mytags[i - 1].CompareTo("TO") == 0 && checkNoun(words[i + 1]) && mytags[i].CompareTo("VB") == 0)
                {
                    words[i - 1] = words[i];
                    mytags[i - 1] = "NN";

                    nodes[i - 1].addChild(mytags[i], words[i], nodes[i]);
                    nodes[i - 1].addChild(mytags[i + 1], words[i + 1], nodes[i + 1]);

                    mytags[i] = "null;"; mytags[i+1] = "null;";
                }
            }
        }

        public void TreeStep6(String[] mytags, String[] words, WordNode[] nodes)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (haveCheck(words[i]) && checkWord("VB", mytags[i + 1]))
                {
                    words[i] = words[i + 1];
                    mytags[i + 1] = "null";
                }
            }
        }

        public void TreeStep7(String[] mytags, String[] words, WordNode[] nodes)
        {
            for (int i = 1; i < mytags.Length - 1; i++)
            {
                if (mytags[i].CompareTo("TO") == 0 && mytags[i + 1].CompareTo("VB") == 0)
                {
                    nodes[i - 1].addChild(mytags[i + 1], words[i + 1], nodes[i + 1]);

                    mytags[i] = "null";
                    mytags[i + 1] = "null";
                }
            }
        }

        public void TreeStep8(String[] mytags, String[] words, WordNode[] nodes)
        {
            for (int i = 0; i < mytags.Length - 2; i++)
            {
                if (checkNoun(mytags[i]) && mytags[i + 1].CompareTo("VBG") == 0)
                {
                    nodes[i].addChild(mytags[i + 1], words[i + 1], nodes[i + 1]);
                    mytags[i + 1] = "null";
                    if (checkNoun(mytags[i + 2]))
                    {
                        nodes[i].addChild(mytags[i + 2], words[i + 2], nodes[i + 2]);
                        mytags[i + 2] = "null";
                    }
                }
            }
        }

        public void TreeStep9(String[] mytags, String[] words, WordNode[] nodes)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if (checkNoun(mytags[i]) &&
                    (mytags[i + 1].CompareTo("VBG") == 0))
                {
                    nodes[i].addChild(mytags[i + 1], words[i + 1], nodes[i + 1]);
                    mytags[i + 1] = "null";

                }
            }
        }

        public void TreeStep10(String[] mytags, String[] words, WordNode[] nodes)
        {
            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if ((checkWord("JJ", mytags[i]) || mytags[i].CompareTo("VBG") == 0 ) && checkNoun(mytags[i + 1]))
                {
                    nodes[i + 1].addChild(mytags[i], words[i], nodes[i]);
                    mytags[i] = "null";
                }
            }
        }

        public void TreeStep11(String[] mytags, String[] mywords, WordNode[] mynodes)
        {
            bool metVerb = false;
            int verbCnt = 0;

            for (int i = 0; i < mytags.Length - 1; i++)
            {
                if(!metVerb){
                    if (checkWord("VB", mytags[i]))
                    {
                        metVerb = true;
                        verbCnt  = i;
                    }
                }

                if (mytags[i].CompareTo("WDT") == 0 || mytags[i].CompareTo("WP") == 0 )
                {
                    if( checkWord("VB", mytags[i+1]) ){
                        if(beVerbCheck(mywords[i+1])){

                            mynodes[i - 1].addChild(mytags[i], mywords[i], nodes[i]);
                            mynodes[i - 1].addChild(mytags[i + 1], mywords[i + 1], nodes[i + 1]);
                            mynodes[i - 1].addChild(mytags[i + 2], mywords[i + 2], nodes[i + 2]);

                            mytags[i] = "null";
                            i++; mytags[i] = "null"; 
                            i++; mytags[i] = "null";
                        }
                    }
                    else
                    {
                        mytags[i] = "null";
                    }
                }
            }
        }

        public void TreeStep12(String[] mytags, String[] words, WordNode[] nodes)
        {
            bool metVerb = false;

            for (int i = 0; i < mytags.Length - 1; i++)
            {
                
                if (checkNoun(mytags[i]) && metVerb &&
                    (mytags[i + 1].CompareTo("VBD") == 0 || mytags[i + 1].CompareTo("VBN") == 0))
                {
                    nodes[i].addChild(mytags[i + 1], words[i + 1], nodes[i + 1]);
                    mytags[i + 1] = "null";

                }

                if (!metVerb)
                {
                    metVerb = true;
                }
            }
        }

        public void renewTags(String[] mytags, WordNode[] nodes)
        {
            for (int i = 0; i < mytags.Length; i++)
            {
                if (mytags[i].CompareTo("null") == 0)
                {
                    pushTag(mytags, i, nodes);
                }
            }
        }

        public void pushTag(String[] mytags, int cnt, WordNode[] nodes)
        {
            //12가지 과정의 함수에서 제거되거나 트리의 자식으로 포함되면서 비게 된 NULL 값이 없어지도록 당겨주는 함수

            for (int i = cnt; i < mytags.Length - 1; i++)
            {
                mytags[i] = mytags[i + 1];
                words[i] = words[i + 1];
                nodes[i].copy(nodes[i + 1]);
            }
            mytags[mytags.Length - 1] = "null";
        }

        public int[] relativeClause(String[] mytags, int entry, Sentence sentence)
        {
            bool metNoun = false;
            bool metVerb = false;
            bool metAdj = false;

            int[] result = new int[2];
            result[0] = 0; result[1] = 1;

            for (int i = entry; i < mytags.Length; i++)
            {
                if (checkWord("VB", mytags[i]))
                {
                    if (!metVerb)
                    {
                        metVerb = true;
                        result[0] = 1;
                        result[1] = i;
                    }
                    else
                    {
                        
                    }
                }

                if (checkNoun(mytags[i]))
                {
                    if (!metNoun)
                    {
                        result[0] = 3;
                        result[1] = i;
                        metNoun = true;
                    }
                    else
                    {
                        result[0] = 4;
                        result[1] = i;
                    }

                }
                else if (checkWord("JJ", mytags[i]))
                {
                    if (!metNoun)
                    {
                        result[0] = 3;
                        result[1] = i;
                    }
                    else
                    {
                        result[0] = 5;
                        result[1] = i;
                    }

                }


            }

            return result;
        }

        public Sentence SentenceTypeSetter(String[] mytags)
        {
            //12가지 과정을 거친 후 실행하게 되면 문장의 형식을 변수에 저장하게 됩니다

            bool metNoun = false, Sub = false; 
            bool metVerb = false;
            bool metAdj = false;

            Sentence sentence = new Sentence(0);

            for (int i = 0; i < mytags.Length; i++)
            {
                if (checkWord("VB", mytags[i]))
                {
                    if (!metVerb)
                    {
                        metVerb = true;
                        sentence.type = 1;
                    }
                    else
                    {
                        int[] result = relativeClause(mytags, i, sentence);
                        Sentence st = new Sentence(result[0]);
                        sentence.addChild(st);
                        i = result[1];
                    }
                }

                if (checkNoun(mytags[i]))
                {

                    if (!Sub)
                    {
                        Sub = true;
                    }
                    else
                    {
                        if (!metNoun)
                        {
                            sentence.type = 3;
                            metNoun = true;
                        }
                        else
                        {
                            if(checkWord("VB", mytags[i+1])){
                                int[] result = relativeClause(mytags, i+1, sentence);
                                Sentence st = new Sentence(result[0]);
                                sentence.addChild(st);
                                i = result[1];
                            }
                            else
                            {
                                sentence.type = 4;
                            }
                        }
                    }
                    
                }

                if(checkWord("JJ", mytags[i])){
                    if (!metNoun)
                    {
                        sentence.type = 2;
                    }
                    else
                    {
                        sentence.type = 5;
                    }
                }



                
            }

            return sentence;
        }

        public string linkChilds(SentenceProcessor.WordNode wn)
        {
            String result = "";

            if (wn.childs != null)
            {
                for (int i = 0; i < wn.childs.Length; i++)
                {
                    result += "<=" + wn.childs[i].word;
                    if(wn.childs[i].childs != null){
                        result += "<=" + linkChilds(wn.childs[i]);
                    }

                    if (wn.childs.Length < 1 && i != wn.childs.Length - 1)
                    {
                        result += ",";
                    }

                    result += " ";
                }
            }

            return result;
        }

        bool flipPoint = false;

        public String BaseSentence()
        {
            String[] mytags = new String[words.Length];
            String[] result = new String[words.Length];

            nodes = new WordNode[words.Length];

            for (int i = 0; i < mytags.Length; i++)
            {
                nodes[i] = new WordNode(words[i], POS_Tags[i]);
                mytags[i] = POS_Tags[i];
            }

            //TreeStep0(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep1(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep1_1(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep1_2(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep2(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep3(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep4(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep5(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep6(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep7(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep8(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep9(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep10(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep11(mytags, words, nodes); renewTags(mytags, nodes);
            TreeStep12(mytags, words, nodes); renewTags(mytags, nodes);
            // 문장에 꼭 필요한 Base Word들을 결정하고 수식 어구를 제거하거나 트리를 제거하기 위해 12가지 과정을 순차적으로 거치게 됩니다

            String treeStr = "";

            int tagCnt = 0;

            for (int i = 0; i < words.Length; i++)
            {
                if (mytags[i].CompareTo("null") != 0)
                {
                    result[i] = words[i];
                    treeStr += words[i] + linkChilds(nodes[i]);
                    tagCnt++;
                }
                else
                {
                    result[i] = "";
                }
            }

            Sentence sentence = new Sentence(0);
            sentence = SentenceTypeSetter(mytags);

            sentenceType = sentence.type;

            SetTags = new string[tagCnt];

            for (int i = 0; i < tagCnt; i++)
            {
                SetTags[i] = mytags[i];
            }


            return treeStr;
        }

    }
}
