using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLCS.NLP
{
    class WordGrammarDictionary
    {

        String[,] numberType = { { "하나", "둘", "셋", "넷", "다섯", "여섯", "일곱", "여덟", "아홉", "열" }, { "일", "이", "삼", "사", "오", "육", "칠", "팔", "구", "십" } };
        String[] highNumberType = { "", "십", "백", "천", "만", "억" };

        public WordGrammarDictionary()
        {

        }

        public String[] getGrammarDictionary(String[] words, String[] unit)
        {
            String[] result = new String[words.Length * unit.Length];

            for (int i = 0; i < unit.Length; i++)
            {
                for (int j = 0; j < words.Length; j++)
                {
                    result[i * words.Length + i] = words[j] + unit[i];
                }
            }

            return result;
        }

        public String[] getNumberUnit(String[] number, String[] unit)
        {
            String[] result = new String[number.Length * unit.Length];

            for (int i = 0; i < unit.Length; i++)
            {
                for (int j = 0; j < number.Length; j++)
                {
                    result[i * number.Length + j] = number[j] + unit[i];
                }
            }

            return result;
        }

        int getHighIndex(int number)
        {
            int result = 0;

            if (number > 0 && number < 10)
            {
                result = 0;
            }
            else if (number > 10 && number < 100)
            {
                result = 1;
            }
            else if (number > 100 && number < 1000)
            {
                result = 2;
            }
            else if (number > 1000 && number < 10000)
            {
                result = 3;
            }
            else if (number > 10000 && number < 100000000)
            {
                result = 4;
            }

            return result;
        }

        int getSquared(int index, int number)
        {
            int result = 1;

            for (int i = 0; i < index; i++)
            {
                result = result * 10;
            }

            result = result * number;

            return result;
        }

        public String getNumberAsString(int number)
        {
            String result = "";

            String numberStr = number + "";

            if (number < 0)
            {
                result = "마이너스";
            }
            else if (number == 0)
            {
                result = "영";
                return result;
            }

            int highIndex = getHighIndex(number);

            if (highIndex == 5)
            {
                String tempNumber = "";

                for (int i = 9; i < numberStr.Length; i++)
                {
                    tempNumber += numberStr[i];
                }

                int mynumber = int.Parse(tempNumber);

                result += getNumberAsString(mynumber) + "억";

                for (int i = 9; i < numberStr.Length; i++)
                {
                    String temp = numberStr[i] + "";
                    int tNum = int.Parse(temp);

                    number -= getSquared(i, tNum);
                }

                numberStr = number + "";

            }

            if (highIndex == 4)
            {
                String tempNumber = "";

                for (int i = 4; i < numberStr.Length; i++)
                {
                    tempNumber += numberStr[i];
                }

                int mynumber = int.Parse(tempNumber);

                result += getNumberAsString(mynumber) + "만";

                for (int i = 4; i < numberStr.Length; i++)
                {
                    String temp = numberStr[i] + "";
                    int tNum = int.Parse(temp);

                    number -= getSquared(i, tNum);
                }

                numberStr = number + "";

            }

            if (highIndex == 3)
            {
                result += numberType[1, number] + highNumberType[3];
            }

            if (highIndex == 2)
            {
                result += numberType[1, number] + highNumberType[2];
            }

            if (highIndex == 1)
            {
                result += numberType[1, number] + highNumberType[1];
            }

            if (highIndex == 0)
            {
                result += numberType[1, number];
            }

            return result;
        }

    }
}
