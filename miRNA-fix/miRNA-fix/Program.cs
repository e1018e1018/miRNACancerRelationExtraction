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

namespace GeneNameTaggerTest
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
            List<string> sents=new List<string>();
            
            string sent = "OBJECTIVE: To construct and screen an effective anti-miR-221, miRNA-120 vector of siRNA.";
            Console.WriteLine(sent);
            int a=miRNARecognition(sent);
            Console.WriteLine(a);
            sent = "aaaaa ablation impairs liver regeneration in an estrogen-dependent manner.";
            Console.WriteLine(sent);
            a = miRNARecognition(sent);
            Console.WriteLine(a);
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

        private static int miRNARecognition(string sent)
        {
            int flag = 0;
            List<string> words = new List<string>();
            List<string> Extractedwords = new List<string>();
            foreach (string s in Regex.Split(sent, @", "))
            {
                words.Add(s);
            }
            foreach (string word in words)
            {
                Match m = Regex.Match(word, @"\b(?'MIR'[Mm]i[Rr]-*\d+[a-z]*/*\d*\*{0,1})");
                Match n = Regex.Match(word, @"\b(?'MIRNA'[Mm]iRNA-*\d+[a-z]?/*\d*\*?)");
                Match o = Regex.Match(word, @"\b(?'LET'[Ll][Ee][Tt]-*\d+[a-z]?\d*\*?)");
                Match aa = Regex.Match(word, @"\b(?'ANTI'[Aa][Nn][Tt][Ii]-)");

                if (aa.Success)
                {
                    continue;
                }
                else
                {
                    if (m.Success || n.Success || o.Success)
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
                        flag = 1;
                    }
                    else
                    {
                        flag = 0;
                    }
                }     
            }
            foreach (string Extracted in Extractedwords)
            {
                Console.WriteLine(Extracted);
            }
            return flag;
        }
    }
}
