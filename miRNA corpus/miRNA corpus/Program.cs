using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IASL.BioTextMining.Cores.Text.Annotation;
using IASL.BioTextMining.Cores.Text.Articles;
using IASL.BioTextMining.Cores.Text.Articles.Abstracts;
using IASL.BioTextMining.NamedEntityIdentification.BioCreAtIvE;
using IASL.BioTextMining.NamedEntityIdentification.BioCreAtIvE.Normalization.BCII;
using IASL.BioTextMining.ThirdParty.TsujiiLab;
using System.Text.RegularExpressions;

namespace miRNA_corpus
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
            List<string> sents=new List<string>();
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(@"26807169.txt");
            while ((line = file.ReadLine()) != null)
            {

                string sent = line;
                
                string a = miRNARecognition(sent);
                if (a != null)
                {
                    string b = MetastasisRecognition(sent);
                    if (b != null)
                    {
                        Console.WriteLine(sent);
                        Console.WriteLine(a + " " + b);
                    }
                }
                
                /*sent = "aaaaa ablation impairs liver regeneration in an estrogen-dependent manner.";
                Console.WriteLine(sent);
                a = miRNARecognition(sent);
                Console.WriteLine(a);*/
                /*BCIIGeneNameTagger _tagger;
                _tagger = BCIIGeneNameTagger.GetInstance(GENIATagger.GetInstance(@"D:\Resources\IASL\Models\ThirdParty\TsujiiLab\GENIATagger\"),@"D:\Resources\IASL\Models\BioCreAtIvE\BCII\GM\BCII.model");
                Abstract article = null;
                Abstract expected = null;
                article = new Abstract("21329655");
                Sentence sent = new Sentence("PPARγ ligands induce growth inhibition and apoptosis through p63 and p73 in human ovarian cancer cells.", article);
                article.SentencesOfTitle.Add(sent);
                sent = new Sentence("Peroxisome proliferator-activated receptor gamma (PPARγ) agonists, including thiazolidinediones (TZDs), can induce anti-proliferation, differentiation, and apoptosis in various cancer cell types. This study investigated the mechanism of the anticancer effect of TZDs on human ovarian cancer.", article);
                article.SentencesOfAbstract.Add(sent);
                sent = new Sentence("Six human ovarian cancer cell lines (NIH:OVCAR3, SKOV3, SNU-251, SNU-8, SNU-840, and 2774) were treated with the TZD, which induced dose-dependent inhibition of cell growth.", article);
                article.SentencesOfAbstract.Add(sent);
                _tagger.Tag(article);
                expected = new Abstract("21329655");
                sent = new Sentence("PPARγ ligands induce growth inhibition and apoptosis through p63 and p73 in human ovarian cancer cells.", expected);
                sent.Entities.Add(new BCIIAnnotationInfo(0, 5, BCIIGeneNameTagger.ANNOTATION, "PPARγ"));
                sent.Entities.Add(new BCIIAnnotationInfo(53, 56, BCIIGeneNameTagger.ANNOTATION, "p63"));
                sent.Entities.Add(new BCIIAnnotationInfo(59, 62, BCIIGeneNameTagger.ANNOTATION, "p73"));
                expected.SentencesOfTitle.Add(sent);
                sent = new Sentence("Peroxisome proliferator-activated receptor gamma (PPARγ) agonists, including thiazolidinediones (TZDs), can induce anti-proliferation, differentiation, and apoptosis in various cancer cell types. This study investigated the mechanism of the anticancer effect of TZDs on human ovarian cancer.", expected);
                sent.Entities.Add(new BCIIAnnotationInfo(0, 45, BCIIGeneNameTagger.ANNOTATION, "Peroxisome proliferator-activated receptor gamma"));
                sent.Entities.Add(new BCIIAnnotationInfo(46, 51, BCIIGeneNameTagger.ANNOTATION, "PPARγ"));
                expected.SentencesOfAbstract.Add(sent);
                sent = new Sentence("Six human ovarian cancer cell lines (NIH:OVCAR3, SKOV3, SNU-251, SNU-8, SNU-840, and 2774) were treated with the TZD, which induced dose-dependent inhibition of cell growth.", expected);
                sent.Entities.Add(new BCIIAnnotationInfo(42, 47, BCIIGeneNameTagger.ANNOTATION, "SKOV3"));
                //sent.Entities.Add(new IASL.BioTextMining.Cores.Text.Annotation.AnnotationInfo(48, 55, BCIIGeneNameTagger.ANNOTATION, "SNU-251"));
                sent.Entities.Add(new BCIIAnnotationInfo(56, 61, BCIIGeneNameTagger.ANNOTATION, "SNU-8"));
                expected.SentencesOfAbstract.Add(sent);

                for (int j = 0; j < expected.SentencesOfTitle.Count; j++)
                {
                    Sentence exp_sent = expected.SentencesOfTitle[j];
                    for (int i = 0; i < exp_sent.Entities.Count; i++)
                    {
                        Assert.AreEqual(exp_sent.Entities[i], article.SentencesOfTitle[j].Entities[i], "IASL.BioTextMining.NamedEntityIdentification.BioCreAtIvE.BCIIGeneNameTagger.Tag 未傳回預期的值。");
                    }
                }

                for (int j = 0; j < expected.SentencesOfAbstract.Count; j++)
                {
                    Sentence exp_sent = expected.SentencesOfAbstract[j];
                    for (int i = 0; i < exp_sent.Entities.Count; i++)
                    {
                        Assert.AreEqual(exp_sent.Entities[i], article.SentencesOfAbstract[j].Entities[i], "IASL.BioTextMining.NamedEntityIdentification.BioCreAtIvE.BCIIGeneNameTagger.Tag 未傳回預期的值。");
                    }
                }*/
            }
        }

        private static string miRNARecognition(string sent)
        {
            int flag = 0;
            List<string> words = new List<string>();
            List<string> Extractedwords = new List<string>();
            string replaceSent = sent.Replace(@"and", @",");
            string pattern = @",| ";

            //open the file, when the program find the relation of miRNA and metastasis, write resluts into the file.
            //the file have to be opened in main function, write the all resluts in the same file
            foreach (string s in Regex.Split(replaceSent, pattern))
            {
                words.Add(s);
            }
            flag = 0;
            foreach (string word in words)
            {
                Match m = Regex.Match(word, @"\b(?'MIR'[Mm]i[Rr]-*\d+[A-z]*/*\d*\*{0,1}[A-z]*)");
                Match n = Regex.Match(word, @"\b(?'MIRNA'[Mm]iRNA-*\d+[A-z]?/*\d*\*?[A-z]*)");
                Match o = Regex.Match(word, @"\b(?'LET'[Ll][Ee][Tt]-*\d+[A-z]?\d*\*?[A-z]*)");
                Match p = Regex.Match(word, @"\b(?'MICRORNA'[Mm]icroRNA-*\d+[A-z]*/*\d*\*{0,1}[A-z]*)");
                Match aa = Regex.Match(word, @"\b(?'ANTI'[Aa][Nn][Tt][Ii]-)");                

                if (aa.Success)
                {
                    continue;
                }
                else
                {
                    if (m.Success || n.Success || o.Success || p.Success)
                    {

                        if (m.Success)
                        {
                            Extractedwords.Add(m.Groups["MIR"].Value);
                        }
                        if (n.Success)
                        {
                            Extractedwords.Add(n.Groups["MIRNA"].Value);
                        }
                        if (o.Success)
                        {
                            Extractedwords.Add(o.Groups["LET"].Value);
                        }
                        if (p.Success)
                        {
                            Extractedwords.Add(p.Groups["MICRORNA"].Value);
                        }
                        flag = 1;
                    }
                }

            }
            string ExtractedEntities = "";
            if (flag == 1)
            {

                foreach (string Extracted in Extractedwords)
                {
                    ExtractedEntities += Extracted + "|";
                }

            }
            if (ExtractedEntities.EndsWith("|"))
                ExtractedEntities = ExtractedEntities.Substring(0, ExtractedEntities.Length - 1);
            else
                ExtractedEntities = null;

            return ExtractedEntities;
        }
        private static string MetastasisRecognition(string sent)
        {
            int flag = 0;
            List<string> words = new List<string>();
            List<string> Extractedwords = new List<string>();
            string replaceSent = sent.Replace(@"and", @",");
            string pattern = @",| ";

            foreach (string s in Regex.Split(replaceSent, pattern))
            {
                words.Add(s);
            }
            flag = 0;

            foreach (string word in words)
            {
                if (word.Equals("metastasis") || word.Equals("Metastasis"))
                {
                    Extractedwords.Add(word);
                    flag = 1;
                }
            }

            string ExtractedEntities = "";
            if (flag == 1)
            {

                foreach (string Extracted in Extractedwords)
                {
                    ExtractedEntities += Extracted + "|";
                }

            }
            if (ExtractedEntities.EndsWith("|"))
                ExtractedEntities = ExtractedEntities.Substring(0, ExtractedEntities.Length - 1);
            else
                ExtractedEntities = null;

            return ExtractedEntities;
        }
    }
}
