﻿using IASL.BioTextMining.ThirdParty.TsujiiLab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace miRNA_corpus
{
    class Program
    {
        static void Main(string[] args)
        {
            int counter = 0;
            int count = 0;
            int num = 0;
            string line;
            string temp = "a";
            string[] txt = new string[1000000];
            string[,] abstext = new string[100000, 6];

            string[] word = new string[30];
            string[,] result = new string[10000, 3];

            System.IO.StreamReader file = new System.IO.StreamReader(@"TrainingSet.txt");
            while ((line = file.ReadLine()) != null)
            {
                /*Console.WriteLine(line);*/
                count = 0;
                txt[counter] = line;
                foreach (string s in Regex.Split(line, @"\t"))
                {
                    //Console.Write(s);
                    abstext[counter, count] = s;
                    count++;
                }
                /*GENIATagger genia = GENIATagger.GetInstance(@"D:\文件\學校\研究所\畢業\microRNA\GENIATagger");
                abstext[counter, 1] = genia.Tokenize(txt[counter]);*/
                counter++;
            }
            for (int i = 0; i < 100000; i++)
            {
                if (abstext[i, 0] != temp)
                {
                    int j = 0;
                    foreach (string s in Regex.Split(abstext[i, 1], @" "))
                    {
                        word[j] = s;
                        j++;
                    }
                    for (int x = 0; x < j; j++)
                    {
                        if (word[j] == "miR-" + @"%d")
                        {
                            result[num, 0] = abstext[i, 0];
                            result[num, 1] = word[x];
                            result[num, 2] = abstext[i, 4];
                            num++;
                        }
                        else if (word[j] == "microRNA-" + "%d")
                        {
                            result[num, 0] = abstext[i, 0];
                            result[num, 1] = word[x];
                            result[num, 2] = abstext[i, 4];
                            num++;
                        }
                        else if (word[j] == "miRNA-" + "%d")
                        {
                            result[num, 0] = abstext[i, 0];
                            result[num, 1] = word[x];
                            result[num, 2] = abstext[i, 4];
                            num++;
                        }
                        else if (word[j] == "let-" + "%d")
                        {
                            result[num, 0] = abstext[i, 0];
                            result[num, 1] = word[x];
                            result[num, 2] = abstext[i, 4];
                            num++;
                        }
                    }
                }
                temp = abstext[i, 0];
            }
            

            file.Close();
            Console.ReadLine();


        }
    }
}
