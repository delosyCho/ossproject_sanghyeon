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
    class HangeulStemmer
    {
        class StemmingKeys
        {
            public string word = "";
            public string key = "";

            public StemmingKeys()
            {
                word = "";
                key = "";
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
        int[] keyCount;

        public HangeulStemmer()
        {
            sk = new StemmingKeys[600, 30000];

            for (int i = 0; i < 600; i++)
            {
                for (int j = 0; j < 30000; j++)
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
                        string word = token[1];
                        string key = token[2].Split('/')[0];
                        string temp = key.ToCharArray()[0] + "";

                        string firstChar = Seperate(key.ToCharArray()[0] + "");

                        char vow = firstChar.ToCharArray()[1];
                        char con = firstChar.ToCharArray()[2];


                        int i = vow - voewl;
                        int j = con - constant;

                        int index = i * 20 + j;

                        if (index > 2000)
                        {
                            int asd = 56;
                        }

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

        public void check()
        {
            int j = 0;
        }

    }
}
