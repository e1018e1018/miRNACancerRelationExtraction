using IASL.BioTextMining.ThirdParty.TsujiiLab;
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
            string[,] abstext = new string[1000000, 6];
            /*int a = 0;
            int b = 0;
            int TP = 0;
            int FP = 0;
            int TN = 0;
            int FN = 0;
            int ACC = 0;
            int PRE = 0;
            int REC = 0;*/

            List<string> word = new List<string>();
            string[,] result = new string[1000000, 2];

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
            for (int i = 0; i < counter; i++)
            {
                if (abstext[i, 0] != temp)
                {
                    foreach (string s in Regex.Split(abstext[i, 1], @", "))
                    {
                        word.Add(s);
                    }
                    for (int x = 0; x < word.Count; x++)
                    {
                        Match m = Regex.Match("anti-miR-22", @"\b(?'MIR'[Mm]i[Rr]-*\d+[a-z]*/*\d*\*{0,1})");                                      
                        if (m.Success)
                        {
                            //result[num, 0] = abstext[i, 0];
                            result[num, 1] = m.Groups["MIR"].Value;
                            Console.WriteLine(result[num, 1]);
                            //result[num, 2] = abstext[i, 4];
                            num++;
                            /*for(a = 0; a < i; a++)
                            {
                                if (result[num, 1] == abstext[a, 4])
                                {
                                    TP++;
                                }
                                else
                                {
                                    FN++; 
                                }
                            }  */                        
                        }
                        Match n = Regex.Match(word[x], @"\b(?'MIRNA'[Mm]iRNA-*\d+[a-z]?\d*\*?)");
                        if (n.Success)
                        {
                            result[num, 0] = abstext[i, 0];
                            result[num, 1] = n.Groups["MIRNA"].Value;
                            //result[num, 2] = abstext[i, 4];
                            num++;
                            /*for (a = 0; a < i; a++)
                            {
                                if (result[num, 1] == abstext[a, 4])
                                {
                                    TP++;
                                }
                                else
                                {
                                    FN++;
                                }
                            }*/
                        }
                        /*else if (word[x] == "miRNA-" + "%d")
                        {
                            result[num, 0] = abstext[i, 0];
                            result[num, 1] = word[x];
                            result[num, 2] = abstext[i, 4];
                            num++;
                        }*/
                        Match o = Regex.Match(word[x], @"\b(?'LET'[Ll][Ee][Tt]-*\d+[a-z]?\d*\*?)");
                        if (o.Success)
                         {
                             result[num, 0] = abstext[i, 0];
                             result[num, 1] =o.Groups["LET"].Value;
                             //result[num, 2] = abstext[i, 4];
                             num++;
                            
                        }
                    }
                }
                temp = abstext[i, 0];
            }
            for (int c = 0; c < num; c++)
            {
                Console.WriteLine(result[c, 1]);
            }
            Console.WriteLine("num:" + num);
            /*for (a = 0; a < num; a++)
            {
                for (b = 0; b < counter; b++)
                {
                    if (result[a, 1] == abstext[b, 4])
                    {
                        TP++;
                    }
                    else
                    {
                        FN++;
                    }
                }
            }
            ACC = (TP + TN) / num;
            PRE = TP / (TP + FP);
            REC = TP / (TP + FN);
            Console.WriteLine("Accuracy:" + ACC);
            Console.WriteLine("Precision:" + PRE);
            Console.WriteLine("Recall:" + REC);*/

            file.Close();
            Console.ReadLine();


        }
    }
}
