using IASL.BioTextMining.NamedEntityIdentification.Dictionary.MeSH;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeShID
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;      
            List<string> word = new List<string>();
            //Dictionary<string, string> Mesh = new Dictionary<string, string>();
            string[,] abstext = new string[3007055, 4];
            string[] txt = new string[38487];
            int count = 0;
            int counter1 = 0;
            int counter2 = 0;
            int i = 0;
            int j = 0;

            System.IO.StreamReader file1 = new System.IO.StreamReader(@"neoplasm literatures.txt");
            while ((line = file1.ReadLine()) != null)
            {
                count = 0;
                foreach (string a in Regex.Split(line, @"\t"))
                {
                    abstext[counter1, count] = a;
                    count++;
                }
                counter1++;
            }

            System.IO.StreamReader file2 = new System.IO.StreamReader(@"pubmed_result.txt");
            while ((line = file2.ReadLine()) != null)
            {
                /*Console.WriteLine(line);*/
                foreach (string s in Regex.Split(line, @"\t"))
                {
                    txt[counter2] = s;
                }                
                counter2++;
            }
            /*GENIATagger genia = GENIATagger.GetInstance(@"D:\文件\學校\研究所\畢業\microRNA\GENIATagger");
            abstext[counter, 1] = genia.Tokenize(txt[counter]);*/
            for (i = 0; i < counter1; i++)
            {
                for (j = 0; j < counter2; j++)
                {
                    if (abstext[i, 0].Equals(txt[j]))
                    {
                        word.Add(txt[j]);
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(".txt"))
            {
                for (j = 0; j < word.Count; j++)
                {
                    sw.WriteLine(word[j]);
                }
            }
            /*MeSHDictionaryTagger _tagger = MeSHDictionaryTagger.GetInstance(@"D:\文件\學校\研究所\畢業\microRNA\desc2016.xml", @"D:\GENIATagger");
            string parent = "D018572"; // Cancer ID
            string child = "D003560"; // Pubatotr ID
            _tagger.CheckInclude(parent, child);*/
        }
    }
}
