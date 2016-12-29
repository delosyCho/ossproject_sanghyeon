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
    class HangeulStemmerModeller
    {
        //StemmingDictionary,Keys의 상위 객체
        
        class StemmingDictionary
        {
            //모음의 종류에 따라 600개의 객체가 생성됨
            //각 모음의 어간만 따로 저장

            public int Length = 0;
            public String Words;
            public String[] Keys; // 추출 된 형태인 어간이 저장되는 배열
            public String[] PosTags;
            //추출 한 데이터에서 얻은 단어의 품사를 저장하는 데이터

            public String[] dicWord;
            //모든 어간들을 저장하는 배열

            int Count = 0; int keyNum = 0; int[] keyCount;

            public StemmingDictionary()
            {
                Length = 0; Count = 0; keyNum = 0;
            }

            public String POS_Tagging(string stemmed)
            {
                String result = "None";

                for (int i = 0; i < Keys.Length; i++ )
                {
                    if(Keys[i].CompareTo(stemmed) == 0){
                        result = PosTags[i];
                    }
                }

                return result;
            }

            public StemmingDictionary(String wd, String k)
            {
                String[] token = Words.Split(' ');
                String[] wordT = new String[token.Length];

                int count = 0;

                for (int i = 0; i < token.Length; i++)
                {
                    bool isExist = false;

                    for (int a = 0; a < count; a++ )
                    {
                        if(wordT[a].CompareTo(token[i]) == 0){
                            isExist = true;
                        }
                    }

                    if(!isExist){
                        wordT[count] = token[i];
                        count++;
                    }
                }

                dicWord = new string[count];

                for (int i = 0; i < count; i++)
                {
                    dicWord[i] = wordT[i];
                }

                Count = count;
                keyNum++;

                Keys = new string[keyNum];
                keyCount = new int[keyNum];
                Keys[0] = k;
                keyCount[0] = count;
            }

            public void Add(String wd, String k, String pos)
            {
                //상위 객체에서 국어원의 데이터를 파싱하여 단어를 전달하면 
                //본 객체의 Dictionary의 중복여부를 확인하고 Dictionary에 단어를 추가하는 함수

                Words = wd;
                
                String[] token = Words.Split(' ');
                String[] wordT = new String[token.Length];

                int count = 0;

                for (int i = 0; i < token.Length; i++)
                {
                    bool isExist = false;

                    for (int a = 0; a < count; a++)
                    {
                        if (wordT[a].CompareTo(token[i]) == 0)
                        {
                            isExist = true;
                        }
                    }

                    if (!isExist)
                    {
                        wordT[count] = token[i];
                        count++;
                    }
                }

                string[] tDic = new string[Count];

                for (int i = 0; i < Count; i++ )
                {
                    tDic[i] = dicWord[i];
                }

                Count += count;
                dicWord = new string[Count];

                for (int i = 0; i < tDic.Length; i++ )
                {
                    dicWord[i] = tDic[i];
                }
                for (int i = Count - count; i < Count; i++)
                {
                    dicWord[i] = wordT[i - (Count - count)];
                }

                keyNum++;

                string[] kTemp = new string[keyNum - 1];
                string[] pTemp = new string[keyNum - 1];
                int[] cTemp = new int[keyNum - 1];


                for (int i = 0; i < keyNum - 1; i++)
                {
                    pTemp[i] = PosTags[i];
                    kTemp[i] = Keys[i];
                    cTemp[i] = keyCount[i];
                }

                PosTags = new string[keyNum];
                Keys = new string[keyNum];
                keyCount = new int[keyNum];

                for (int i = 0; i < keyNum - 1; i++)
                {
                    Keys[i] = kTemp[i];
                    keyCount[i] = cTemp[i];
                    PosTags[i] = pTemp[i];
                }

                PosTags[keyNum - 1] = pos;
                Keys[keyNum - 1] = k;
                keyCount[keyNum - 1] = count;
            }

            public string stemmedWord(string word)
            {
                //어간 추출을 위해 호출하는 함수
                //추출 된 어간을 Return

                string result = word;

                int index = -1; int current = 0;

                for (int i = 0; i < Count; i++ )
                {
                    if(word.CompareTo(dicWord[i]) == 0){
                        index = i;
                        
                        for (int a = 0; a < keyCount.Length; a++)
                        {
                            if (index < current + keyCount[a])
                            {
                                result = Keys[a];
                                a = keyCount.Length;
                            }
                            else
                            {
                                current += keyCount[a];
                            }

                        }

                        i = Count;
                    }
                }


                return result;
            }

            public void saveFile(String filePath)
            {
                //Dictionary를 저장하여 Load하여 다시 사용 할 수 있도록 하기 위한 함수
                //본 객체가 아닌 상위 객체의 Save에서 실행되기 때문에 따로 호출할 필요가 없음

                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
                FileStream fs2 = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
                System.IO.StreamWriter fs = new StreamWriter(fs2, encode);

                if (Keys == null)
                {
                    fs.Close();
                    return;
                }
                
                string str = "", str2 = "", str3 = "";
                fs.WriteLine(keyNum + "");

                for (int i = 0; i < keyNum - 1; i++ )
                {
                    str += Keys[i] + "@";
                    str2 += keyCount[i] + "@";
                    str3 += PosTags[i] + "@";
                }

                str += Keys[keyNum - 1];
                str2 += keyCount[keyNum - 1];
                str3 += PosTags[keyNum - 1];

                fs.WriteLine(str);
                fs.WriteLine(str2);
                fs.WriteLine(str3);

                fs.WriteLine(dicWord.Length + "");

                str = "";

                for (int i = 0; i < dicWord.Length - 1; i++)
                {
                    str += dicWord[i] + "@";
                }
                str += dicWord[dicWord.Length - 1];

                fs.WriteLine(str);
                fs.Close();
            }

            public void read(string filePath)
            {
                //저장 된 파일을 읽어들여 객체를 생성해주는 함수
                //본 객체가 아닌 상위 함수에서 호출 되기 때문에 따로 호출 할 필요 없음

                Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
                FileStream fs2 = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                System.IO.StreamReader fs = new StreamReader(fs2, encode);

                try
                {
                    keyNum = int.Parse(fs.ReadLine());
                }
                catch
                {
                    return;
                }
                

                Keys = new string[keyNum];
                keyCount = new int[keyNum];
                PosTags = new string[keyNum];

                string str = fs.ReadLine(), str2 = fs.ReadLine(), str3 = fs.ReadLine();
                string[] token = str.Split('@'), token2 = str2.Split('@'), toekn3 = str3.Split('@');

                for (int i = 0; i < keyNum; i++)
                {
                    Keys[i] = token[i];
                    keyCount[i] = int.Parse(token2[i]);
                    PosTags[i] = toekn3[i];
                }

                int dicLength = int.Parse(fs.ReadLine());

                token = fs.ReadLine().Split('@');

                dicWord = new string[token.Length];

                for (int i = 0; i < dicWord.Length; i++)
                {
                    dicWord[i] = token[i];
                }
            }

        }

        class StemmingKeys
        {
            public string word = "";
            public string key = "";
            public string POS = "";

            public StemmingKeys()
            {
                word = "";
                key = "";
                POS = "";
            }

            public void addWord(String wd)
            {
                String[] tokens = word.Split(' ');
                bool isExsit = false;

                for (int i = 0; i < tokens.Length; i++)
                {
                    if( wd.CompareTo(tokens) == 0 ){
                        isExsit = true;
                    }
                }

                if(!isExsit){
                    word += wd + " ";
                }
            }
        }

        int voewl = 12593;
        int constant = 12623;

        //Author: Cho Sanghyeon Inje Univ.
        //
        //한 스트링 단위로 처리를 하며
        //단순히 StemWord 함수에 원하는 String을 넣으면 결과물이 Return 됩니다!
        //출처와 원저자만 밝혀주시면 수정 배포 사용은 자유입니다


        String word;
        StemmingKeys[,] sk;
        StemmingDictionary[] sd;
        int[] keyCount;
        StemmingDictionary[] dictionary;

        public HangeulStemmerModeller()
        {
            sk = new StemmingKeys[600, 6000];
            sd = new StemmingDictionary[600];

            for (int i = 0; i < 600; i++)
            {
                sd[i] = new StemmingDictionary();

                for (int j = 0; j < 6000; j++)
                {
                    sk[i, j] = new StemmingKeys();
                }
            }

                keyCount = new int[600];
        }

        public string Seperate(string data)
        {
            int a, b, c;//자소버퍼 초성중성종성순
            string result = " ";//분리결과가 저장되는 문자열
            int cnt;

            //한글의 유니코드

            // ㄱ ㄲ ㄴ ㄷ ㄸ ㄹ ㅁ ㅂ ㅃ ㅅ ㅆ ㅇ ㅈ ㅉ ㅊ ㅋ ㅌ ㅍ ㅎ
            int[] ChoSung ={ 0x3131, 0x3132, 0x3134, 0x3137, 0x3138, 0x3139, 0x3141
            , 0x3142, 0x3143, 0x3145, 0x3146, 0x3147, 0x3148, 0x3149, 0x314a
            , 0x314b, 0x314c, 0x314d, 0x314e };

            // ㅏ ㅐ ㅑ ㅒ ㅓ ㅔ ㅕ ㅖ ㅗ ㅘ ㅙ ㅚ ㅛ ㅜ ㅝ ㅞ ㅟ ㅠ ㅡ ㅢ ㅣ
            int[] JwungSung = {   0x314f, 0x3150, 0x3151, 0x3152, 0x3153, 0x3154, 0x3155
            , 0x3156, 0x3157, 0x3158, 0x3159, 0x315a, 0x315b, 0x315c, 0x315d, 0x315e
            , 0x315f, 0x3160, 0x3161, 0x3162, 0x3163 };

            // ㄱ ㄲ ㄳ ㄴ ㄵ ㄶ ㄷ ㄹ ㄺ ㄻ ㄼ ㄽ ㄾ ㄿ ㅀ ㅁ ㅂ ㅄ ㅅ ㅆ ㅇ ㅈ ㅊ ㅋ ㅌ ㅍ ㅎ
            int[] JongSung = { 0, 0x3131, 0x3132, 0x3133, 0x3134, 0x3135, 0x3136
            , 0x3137, 0x3139, 0x313a, 0x313b, 0x313c, 0x313d, 0x313e, 0x313f
            , 0x3140, 0x3141, 0x3142, 0x3144, 0x3145, 0x3146, 0x3147, 0x3148
            , 0x314a, 0x314b, 0x314c, 0x314d, 0x314e };


            int x;
            for (cnt = 0; cnt < data.Length; cnt++)
            {
                x = (int)data[cnt];
                //한글일 경우만 분리 시행
                if (x >= 0xAC00 && x <= 0xD7A3)
                {
                    c = x - 0xAC00;
                    a = c / (21 * 28);
                    c = c % (21 * 28);
                    b = c / 28;
                    c = c % 28;
                    /*
                    a = (int)a;
                    b = (int)b;
                    c = (int)c;
                    */
                    result += string.Format("{0}{1}", (char)ChoSung[a], (char)JwungSung[b]);
                    // $c가 0이면, 즉 받침이 있을경우
                    if (c != 0)
                        result += string.Format("{0}", (char)JongSung[c]);
                }
                else
                {
                    result += string.Format("{0}", (char)x);
                }
            }
            return result;
        }
        //source: http://nonstop.pe.kr/dotnet/409
        //개발자세상
        //.net에서 string형은 유니코드형식
        //모든데이터가 unicode로 되어있다고 가정하고 시작한다.
        //입력데이터가 유니코드가아닐경우 string.format로 유니코드로 변환해주어야한다.

        public bool CheckSeperate(string data)
        {
            int a, b, c;//자소버퍼 초성중성종성순
            string result = " ";//분리결과가 저장되는 문자열
            int cnt;

            //한글의 유니코드

            // ㄱ ㄲ ㄴ ㄷ ㄸ ㄹ ㅁ ㅂ ㅃ ㅅ ㅆ ㅇ ㅈ ㅉ ㅊ ㅋ ㅌ ㅍ ㅎ
            int[] ChoSung ={ 0x3131, 0x3132, 0x3134, 0x3137, 0x3138, 0x3139, 0x3141
            , 0x3142, 0x3143, 0x3145, 0x3146, 0x3147, 0x3148, 0x3149, 0x314a
            , 0x314b, 0x314c, 0x314d, 0x314e };

            // ㅏ ㅐ ㅑ ㅒ ㅓ ㅔ ㅕ ㅖ ㅗ ㅘ ㅙ ㅚ ㅛ ㅜ ㅝ ㅞ ㅟ ㅠ ㅡ ㅢ ㅣ
            int[] JwungSung = {   0x314f, 0x3150, 0x3151, 0x3152, 0x3153, 0x3154, 0x3155
            , 0x3156, 0x3157, 0x3158, 0x3159, 0x315a, 0x315b, 0x315c, 0x315d, 0x315e
            , 0x315f, 0x3160, 0x3161, 0x3162, 0x3163 };

            // ㄱ ㄲ ㄳ ㄴ ㄵ ㄶ ㄷ ㄹ ㄺ ㄻ ㄼ ㄽ ㄾ ㄿ ㅀ ㅁ ㅂ ㅄ ㅅ ㅆ ㅇ ㅈ ㅊ ㅋ ㅌ ㅍ ㅎ
            int[] JongSung = { 0, 0x3131, 0x3132, 0x3133, 0x3134, 0x3135, 0x3136
            , 0x3137, 0x3139, 0x313a, 0x313b, 0x313c, 0x313d, 0x313e, 0x313f
            , 0x3140, 0x3141, 0x3142, 0x3144, 0x3145, 0x3146, 0x3147, 0x3148
            , 0x314a, 0x314b, 0x314c, 0x314d, 0x314e };


            int x;
            for (cnt = 0; cnt < data.Length; cnt++)
            {
                x = (int)data[cnt];
                //한글일 경우만 분리 시행
                if (x >= 0xAC00 && x <= 0xD7A3)
                {
                    c = x - 0xAC00;
                    a = c / (21 * 28);
                    c = c % (21 * 28);
                    b = c / 28;
                    c = c % 28;
                    /*
                    a = (int)a;
                    b = (int)b;
                    c = (int)c;
                    */
                    result += string.Format("{0}{1}", (char)ChoSung[a], (char)JwungSung[b]);
                    // $c가 0이면, 즉 받침이 있을경우
                    if (c != 0)
                        return true;
                }
                else
                {
                    result += string.Format("{0}", (char)x);
                }
            }
            return false;
        }

        public int isExist(int index, string str){
            int result = -1;

            for (int i = 0; i < keyCount[index]; i++ )
            {
                if(sk[index, i].key.CompareTo(str) == 0){
                    result = i; i = keyCount[index] + 1;
                }
            }

            return result;
        }

        public bool checkKey(int index, int count, string word)
        {
            bool result = false;

            string[] token = sk[index, count].word.Split(' ');

            for (int i = 0; i < token.Length; i++)
            {
                if (token[i].CompareTo(word) == 0)
                {
                    result = true; i = token.Length;
                }
            }

            return result;
        }

        public bool LearningTorch(String filePath)
        {
            //본 함수를 통해 데이터 파싱 및 모델링과 Dictionary 생성이 이루어짐
            //국립국어원 데이터의 Filepath가 매개변수로 사용됨

            Encoding encode = System.Text.Encoding.GetEncoding("ks_c_5601-1987");

            FileStream fs2 = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            System.IO.StreamReader fs = new StreamReader(filePath, encode);

            string str = fs.ReadLine();

            while(str != null){
                try
                {
                    string[] token = str.Split('\t');

                    if (token.Length == 3)
                    {
                        char[] posCh = new char[2];

                        string word = token[1];
                        string key = token[2].Split('/')[0];
                        string temp = key.ToCharArray()[0] + "";
                        posCh[0] = token[2].Split('/')[1].ToCharArray()[0];
                        posCh[1] = token[2].Split('/')[1].ToCharArray()[1];

                        string firstChar = Seperate(key.ToCharArray()[0] + "");

                        char vow = firstChar.ToCharArray()[1];
                        char con = firstChar.ToCharArray()[2];

                        char[] chs = word.ToCharArray();

                        if(chs[chs.Length - 1] == ',' || chs[chs.Length - 1] == '.'){
                            char[] nw = new char[chs.Length - 1];

                            for (int a = 0; a < chs.Length - 1; a++)
                            {
                                nw[a] = chs[a];
                            }

                            word = new string(nw);
                        }

                        int i = vow - voewl;
                        int j = con - constant;

                        int index = i * 20 + j;

                        int count = isExist(index, key);

                        if (count != -1)
                        {
                            if (!checkKey(index, count, word))
                            {
                                sk[index, count].word += word + " ";
                            }
                        }
                        else
                        {
                            sk[index, keyCount[index]].key = key;
                            sk[index, keyCount[index]].word += word + " ";

                            sk[index, keyCount[index]].POS = new string(posCh);

                            keyCount[index]++;
                        }
                    }
                }
                catch
                {

                }

                str = fs.ReadLine();
            }

            int total = 0;

            for (int i = 0; i < 600; i++)
            {
                total += keyCount[i];
            }

            Console.WriteLine("t: " + total);

            return true;
        }

        public string getKeys(string wd)
        {

            string word = wd;

            string firstChar = Seperate(wd.ToCharArray()[0] + "");

            char vow = firstChar.ToCharArray()[1];
            char con = firstChar.ToCharArray()[2];

            int i = vow - voewl;
            int j = con - constant;

            int index = i * 20 + j;
            //단어의 모음과 자음(받침 모음은 제외)을 이용하여 단어의 Index를 계산함 (0 ~ 600)

            for (int a = 0; a < keyCount[index]; a++)
            {
                if( sk[index, a].key.CompareTo(wd) == 0 ){
                    return sk[index, a].word;
                }
            }

            return "Not Exist";
        }

        public void registerDictionary()
        {
            for (int a = 0; a < 600; a++ )
            {
                for (int k = 0; k < keyCount[a]; k++ )
                {
                    sd[a].Add(sk[a, k].word, sk[a, k].key, sk[a, k].POS);
                }
            }
        }

        public string StemmingWord(string word) {
            //단어의 어간 추출을 위한 함수
            
            string firstChar = Seperate(word.ToCharArray()[0] + "");

            char vow = firstChar.ToCharArray()[1];
            char con = firstChar.ToCharArray()[2];

            int i = vow - voewl;
            int j = con - constant;

            int index = i * 20 + j;

            return sd[index].stemmedWord(word); 
        }

        public string POStags(string word)
        {
            //단어의 품사 태깅을 위한 함수

            string firstChar = Seperate(word.ToCharArray()[0] + "");

            char vow = firstChar.ToCharArray()[1];
            char con = firstChar.ToCharArray()[2];

            int i = vow - voewl;
            int j = con - constant;

            int index = i * 20 + j;

            return sd[index].POS_Tagging(sd[index].stemmedWord(word));
        }

        public void save(string filePath)
        {
            for (int i = 0; i < 600; i++ )
            {
                sd[i].saveFile(filePath + "\\" +i);
            }
        }

        public void open(string filePath, int index){
            sd[index].read(filePath);
        }

        public void check()
        {
            int j = 0;
        }

    }
}
