using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLCS.NLP
{
    class Word2Vec
    {
        double[,] WeightInput, WeightOutput;
        
        int NumberOfNodes, WordBagLength;

        double[] inputHidden; double[,] inputHiddens;
        double[] outputHidden; double[,] outputHiddens;

        double[] result; double[,] results;
        double Summation;

        double LearningRate = 0.005;

        public Word2Vec(int numberOfNodes, int wordBagLength)
        {
            NumberOfNodes = numberOfNodes;
            WordBagLength = wordBagLength;

            Random rand = new Random();

            WeightInput = new double[WordBagLength, numberOfNodes];
            WeightOutput = new double[numberOfNodes, WordBagLength];

            for (int i = 0; i < numberOfNodes; i++) 
            {
                for (int j = 0; j < WordBagLength; j++)
                {
                    WeightInput[j, i] = (rand.NextDouble() - 1) / 10;
                    WeightOutput[i, i] = (rand.NextDouble() - 1) / 10;
                }
            }


        }

        public double getProbability(int inputVector, int targetVector){
            double result = 0;

            inputHidden = new double[NumberOfNodes];
            outputHidden = new double[WordBagLength];

            for (int i = 0; i < NumberOfNodes; i++)
            {
                inputHidden[i] = WeightInput[inputVector, i];
            }

            for (int i = 0; i < WordBagLength; i++)
            {
                for (int j = 0; j < NumberOfNodes; j++)
                {
                    outputHidden[i] += WeightOutput[j, i] * inputHidden[j];
                }
            }

            result = outputHidden[targetVector];

            return result;
        }

        public double getProbability(int[] inputVector, int targetVector)
        {
            double result = 0;
            results = new double[inputVector.Length, WordBagLength];

            inputHiddens = new double[inputVector.Length, NumberOfNodes];
            outputHiddens = new double[inputVector.Length, WordBagLength];

            for (int k = 0; k < inputVector.Length; k++)
            {
                for (int i = 0; i < NumberOfNodes; i++)
                {
                    inputHiddens[k, i] = WeightInput[inputVector[k], i];
                }

                for (int i = 0; i < WordBagLength; i++)
                {
                    for (int j = 0; j < NumberOfNodes; j++)
                    {
                        outputHiddens[k, i] += WeightOutput[j, i] * inputHiddens[k, j];
                    }
                }

                double[] temp = Softmax(outputHidden);

                for (int i = 0; i < WordBagLength; i++)
                {
                    results[k, i] = temp[i];
                }

            }
                
            return result;
        }

        public void LearningContext(int inputVector, int targetVector)
        {
            double[,] Der_WeightInput = new double[WordBagLength, NumberOfNodes];
            double[,] Der_WeightOutput = new double[NumberOfNodes, WordBagLength];

            for (int i = 0; i < WordBagLength; i++)
            {
                int t = 0;

                if( i == targetVector ){
                    t = 1;
                }

                for (int j = 0; j < NumberOfNodes; j++)
                {
                    Der_WeightOutput[j, i] = (result[i] - t) * inputHidden[j];
                }
            }

            for (int j = 0; j < NumberOfNodes; j++)
            {
                double EHi = 0;

                for(int i = 0; i < WordBagLength; i++){
                    EHi += result[i] * WeightOutput[j, i];
                }

                Der_WeightInput[inputVector, j] += EHi;
            }

            for (int i = 0; i < WordBagLength; i++)
            {
                for (int j = 0; j < NumberOfNodes; j++)
                {
                    WeightInput[i, j] = Der_WeightInput[i, j] * LearningRate;
                    WeightOutput[j, i] = Der_WeightOutput[j, i] * LearningRate;
                }
            }

        }

        public void LearningContext(int[] inputVector, int targetVector)
        {
            double[,] Der_WeightInput = new double[WordBagLength, NumberOfNodes];
            double[,] Der_WeightOutput = new double[NumberOfNodes, WordBagLength];

            double[,] EHi = new double[inputVector.Length, NumberOfNodes];

            for (int k = 0; k < inputVector.Length; k++)
            {
                for (int i = 0; i < WordBagLength; i++)
                {
                    int t = 0;

                    if (i == targetVector)
                    {
                        t = 1;
                    }

                    for (int j = 0; j < NumberOfNodes; j++)
                    {
                        Der_WeightOutput[j, i] = (result[i] - t) * inputHiddens[k, j];
                    }
                }

                for (int j = 0; j < NumberOfNodes; j++)
                {

                    for (int i = 0; i < WordBagLength; i++)
                    {
                        EHi[k, j] += results[k, i] * WeightOutput[j, i];
                    }

                    //Der_WeightInput[inputVector[k], j] += EHi[k];
                }
            }

            double[] EH = new double[NumberOfNodes];
            for (int j = 0; j < NumberOfNodes; j++)
            {
                for (int k = 0; k < inputVector.Length; k++)
                {
                    EH[j] += EHi[k, j];
                }

                EH[j] = EH[j] / inputVector.Length;
            }
            
            for (int k = 0; k < inputVector.Length; k++)
            {
                for (int j = 0; j < NumberOfNodes; j++)
                {
                    Der_WeightInput[inputVector[k], j] += EH[j];
                }
            }

            for (int i = 0; i < WordBagLength; i++)
            {
                for (int j = 0; j < NumberOfNodes; j++)
                {
                    WeightInput[i, j] = Der_WeightInput[i, j] * LearningRate;
                    WeightOutput[j, i] = Der_WeightOutput[j, i] * LearningRate;
                }
            }

        }

        public double[] Softmax(double[] input)
        {
            result = new double[input.Length];
            Summation = 0;

            for (int i = 0; i < input.Length; i++)
            {
                Summation += Math.Exp(input[i]);
            }

            for (int i = 0; i < input.Length; i++)
            {
                result[i] = Math.Exp(input[i]) / Summation;
            }

            return result;
        }


    }
}
