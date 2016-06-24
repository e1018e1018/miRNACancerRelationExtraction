using IASL.BioTextMining.ThirdParty.LingPipe;
using IASL.BioTextMining.ThirdParty.TsujiiLab;
using NTTU.BigODM.Bio.Thirdparty.NCBI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinalLab
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(@"Final_PubMedID.txt");
            while ((line = file.ReadLine()) != null)
            {
                PubmedSearchMethod(line);
                
            }

        }
        public static void PubmedSearchMethod(string input)
        {
            List<string> pmids = PubMedEUtilities.Search(input);
            string filename = input + ".txt";
            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (string pmid in pmids)
                {
                    sw.WriteLine("PMID: " + pmid);

                    bool isSucess = false;
                    Abstract abs = PubMedEUtilities.FetchByID(pmid, ref isSucess);
                    GENIATagger genia = GENIATagger.GetInstance(@"D:\GENIATagger");
                    string tokenizedTitle = genia.Tokenize(abs.TitleRawTxt);
                    sw.WriteLine("Title: ");
                    sw.WriteLine(tokenizedTitle);

                    //string[] chunk = genia.GetChunk(tokenizedTitle);
                    //string nes = genia.GetNamedEntities(tokenizedTitle);

                    sw.WriteLine("Abstract: ");
                    string absText = abs.AbstractRawTxt;
                    if (absText != null)
                    {
                        List<string> sents = SentenceSpliter.SplitSentence(absText);
                        foreach (string sent in sents)
                        {
                            string tokenizedAbstract = genia.Tokenize(sent);
                            sw.WriteLine(tokenizedAbstract);
                        }

                    }

                    sw.WriteLine();
                    sw.WriteLine();

                }
            }

        }
    }
}
