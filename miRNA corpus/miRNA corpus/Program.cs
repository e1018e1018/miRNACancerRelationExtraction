using IASL.BioTextMining.ThirdParty.TsujiiLab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace miRNA_corpus
{
    class Program
    {
        static void Main(string[] args)
        {
            int counter = 0;
            string line;
            string[] txt = new string[1000000];
            string[,] abstext = new string[100000, 3];

            System.IO.StreamReader file = new System.IO.StreamReader(@"TrainingSet.txt");
            while ((line = file.ReadLine()) != null)
            {
                /*Console.WriteLine(line);*/
                txt[counter] = line;
                GENIATagger genia = GENIATagger.GetInstance(@"D:\文件\學校\研究所\畢業\microRNA\GENIATagger");
                abstext[counter, 1] = genia.Tokenize(txt[counter]);
                counter++;
            }

            

            file.Close();
            Console.ReadLine();


        }
    }
}
