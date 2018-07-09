using System;
using System.Collections;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using NTTU.BigODM.Bio.Thirdparty.NCBI;
using System.Data;
using IASL.BioTextMining.NamedEntityIdentification.Dictionary;
using IASL.BioTextMining.NamedEntityIdentification.Dictionary.DataStructure;

namespace miRNA
{
    class Program
    {
        static void Main(string[] args)
        {
            /*NamedEntityRecognizer<Descriptor> tagger = new NamedEntityRecognizer<Descriptor>();
            Descriptor id = new Descriptor("6314");
            tagger.Dictionary.Add("ATXN7", id);
            tagger.Dictionary.Add("Ataxin 7", id);
            tagger.Dictionary.Add("single nucleotide polymorphisms", new Descriptor("NULL"));
            tagger.Dictionary.Add("SNPs", new Descriptor("NULL"));
            var x = tagger.Recognize("Ataxin 7 (ATXN7) single nucleotide polymorphisms (SNPs) were genotyped by Sanger DNA sequencing after PCR amplification.");*/
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-CT9PVC8\\SQLEXPRESS;Initial Catalog=Liver_Cancer_Biomarker;Integrated Security=True"))
                using(StreamWriter sw=new StreamWriter("log.txt"))
            {
                //
                // Open the SqlConnection.
                //
                con.Open();
                using (StreamReader sr = new StreamReader(@"C:\Users\EricLee\Desktop\HCC_201611_12052.txt"))
                    //Open file
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                        string[] TabArray = line.Split('\t');      
                        string[] Gene = TabArray[7].Split('|');
                        string[] GeneID = TabArray[8].Split('|');
                        string Date=TabArray[6];
                        string a = Date;
                        // 一些例外處理
                        if (a.Length < 6 )
                        {
                            a = String.Format("{0}-{1}-{2}", TabArray[6], 01, 01);
                        }
                        else if (a.Length > 6 && a.Length < 8)
                        {
                            a = String.Format("{0}-{1}", TabArray[6], 01);
                        }else if (a.Length > 11)
                        {
                            string[] twomonths = TabArray[6].Split('-');
                            a = String.Format("{0}-{1}", twomonths[0], 15);
                        }
                        DateTime currect = Convert.ToDateTime(a);

                        SqlCommand insert_ART = new SqlCommand("INSERT INTO ARTICLE (ART_PMID, ART_JOURNAL, ART_ISSN,ART_PUB) VALUES ('" + TabArray[0] + "','" + TabArray[3].Replace("'", "''") + "','" + TabArray[4] + "','"+currect.ToString("yyyy-MM-dd")+"')", con);
                        insert_ART.ExecuteNonQuery();
                        insert_ART.CommandText = "select scope_identity()";

                        string fk = null;
                            using (SqlDataReader reader = insert_ART.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    fk = reader[0].ToString();
                                }
                            }
                        SqlCommand insert_SENT = new SqlCommand("INSERT INTO SENTENCE(ART_ID,TEXT)VALUES('" + fk + "','" + Escape(TabArray[2])+"')", con);
                        insert_SENT.ExecuteNonQuery();
                        insert_SENT.CommandText = "select scope_identity()";
                        string s_fk = null;
                        using (SqlDataReader reader = insert_SENT.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                 s_fk = reader[0].ToString();
                            }
                        }
                        SqlCommand insert_ANN = new SqlCommand("INSERT INTO ANNOTATION (SENT_ID,ART_ID, TYPE) VALUES ('" + s_fk + "','" + fk + "','" + "Gene" + "')", con);
                                    insert_ANN.ExecuteNonQuery();
                                    insert_ANN.CommandText = "select scope_identity()";
                                    string fk_aid = null;
                                    using (SqlDataReader reader = insert_ANN.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            fk_aid = reader[0].ToString();
                                        }
                                    }

                        NamedEntityRecognizer<Descriptor> tagger = new NamedEntityRecognizer<Descriptor>();
                        for(int x = 0; x < GeneID.Length; x++)
                        {
                            Descriptor id = new Descriptor(GeneID[x]);
                            for (int y = 0; y < Gene.Length; y++)
                            {                                
                                tagger.Dictionary.Add(Gene[y], id);
                                var recognize = tagger.Recognize(Escape(TabArray[2]));
                                SqlCommand insert_ENT = new SqlCommand("INSERT INTO ENTITY(ANN_ID,TEXT,GENE_ID,FIRST_LETTER_NUM,LAST_LETTER_NUM)VALUES('" + fk_aid + "','" + Gene[x] + "','" + GeneID[x] + "','" + recognize.ToString() + "','" + recognize.Last() +"')", con);
                                insert_ENT.ExecuteNonQuery();
                                insert_ENT.CommandText = "select scope_identity()";
                            }
                        }
                        
                       
                    }
                     
                }

                //XmlDocument miRNA = new XmlDocument();
                //miRNA.Load(@"C:\Users\e1018\Desktop\專題\miRNA-Train-Corpus.xml");
                ////for (int i = 0; i < doc.SelectNodes("corpus//document").Count; i++)
                ////{
                ////    XmlNode mode = doc.SelectNodes("corpus//document")[i];
                ////}
                //foreach (XmlNode doc in miRNA.SelectNodes("corpus//document"))
                //{
                //    string pmid = doc.Attributes["origId"].Value;
                //    bool isS = false;
                //    Abstract a = PubMedEUtilities.FetchByID(pmid, ref isS);
                //    //
                //    // The following code uses an SqlCommand based on the SqlConnection.
                //    //
                //    DateTime dt;
                //    try
                //    {
                //        dt = Convert.ToDateTime(a.PublicationDate);
                //    }
                //    catch
                //    {
                //        //
                //        sw.WriteLine(pmid);
                //        continue;
                //    }
                //    SqlCommand d_command_0 = new SqlCommand("INSERT INTO ARTICLE (ART_PMID, ART_JOURNAL, ART_ISSN,PUB_DATE) VALUES ('" + pmid + "','" + a.JournalName.Replace("'", "''") + "','" + a.ISSN + "','" + dt.ToString("yyyyMMdd") + "')", con);
                //    //SqlCommand d_command_1 = new SqlCommand("INSERT INTO ARTICLE (ART_JOURNAL) VALUES ('" + + "')", con);
                //    //SqlCommand d_command_2 = new SqlCommand("INSERT INTO ARTICLE (ART_ISSN) VALUES ('" + pmid + "')", con);
                //    d_command_0.ExecuteNonQuery();
                //    d_command_0.CommandText = "select scope_identity()";
                //    Console.WriteLine("【ARTICLE】" + pmid + "   " + a.JournalName.Replace("'", "''") + "   " + a.ISSN + "   " + dt.ToString("yyyyMMdd"));
                //    string fk = null;
                //    using (SqlDataReader reader = d_command_0.ExecuteReader())
                //    {
                //        while (reader.Read())
                //        {
                //            fk = reader[0].ToString();
                //        }
                //    }


                //    int MeshCounts = a.MeSHTerms.Count;
                //    string fk_mesh = null;
                //    for (int C = 0; C < MeshCounts; C++)
                //    {
                //        string[] mesh_h = a.MeSHTerms[C].Split('|');
                //        SqlCommand m_command = new SqlCommand("SELECT MH_ID FROM MESH_HEADING WHERE MESH_ID ='" + mesh_h[1] + "'", con);
                //        //m_command.ExecuteNonQuery();

                //        SqlDataReader reader = m_command.ExecuteReader();
                //        try
                //        {
                //            while (reader.Read())
                //            {
                //                fk_mesh = reader[0].ToString();
                //                //string temp = reader[0].ToString() + "," + reader[1].ToString();
                //                //listBox1.Items.Add(temp);
                //            }
                //        }
                //        finally
                //        {
                //            reader.Close();
                //        }
                //        SqlCommand meta_command = new SqlCommand("INSERT INTO METAINFO (MH_ID,ART_ID) VALUES ('" + fk_mesh + "','" + fk + "')", con);
                //        meta_command.ExecuteNonQuery();
                //        meta_command.CommandText = "select scope_identity()";
                //        Console.WriteLine("【METAINFO】"+fk_mesh+"     "+fk);
                //    }

                //    foreach (XmlNode sent in doc.SelectNodes("sentence"))
                //    {
                //        string text = sent.Attributes["text"].Value;
                //        string id = sent.Attributes["origId"].Value;
                //        int title = Convert.ToInt32(id.Substring(id.Length - 1));
                //        int istitle = 0;
                //        istitle = (title == 0) ? 1 : 0;
                //        SqlCommand s_command = new SqlCommand("INSERT INTO SENTENCE (ART_ID,IS_TITLE,TEXT) VALUES ('" + fk + "','" + istitle + "','" + Escape(text) + "')", con);
                //        s_command.ExecuteNonQuery();
                //        s_command.CommandText = "select scope_identity()";
                //        Console.WriteLine("【SENTENCE】" + fk + "   " + istitle + "   " + Escape(text));
                //        string s_fk = null;
                //        using (SqlDataReader reader = s_command.ExecuteReader())
                //        {
                //            while (reader.Read())
                //            {
                //                s_fk = reader[0].ToString();
                //            }
                //        }
                //        Dictionary<string, string> entityMap = new Dictionary<string, string>();
                //        foreach (XmlNode entity in sent.SelectNodes("entity"))
                //        {
                //            string entext = entity.Attributes["text"].Value;
                //            string e_type = entity.Attributes["type"].Value;
                //            string charOffset = entity.Attributes["charOffset"].Value;

                //            SqlCommand e_command = new SqlCommand("INSERT INTO ANNOTATION (SENT_ID,ART_ID, TYPE) VALUES ('" + s_fk + "','" + fk + "','" + e_type + "')", con);
                //            e_command.ExecuteNonQuery();
                //            e_command.CommandText = "select scope_identity()";
                //            Console.WriteLine("【ANNOTATION(ENTITY)】" + s_fk + "   " + fk + "   " + e_type);
                //            string fk_aid = null;
                //            using (SqlDataReader reader = d_command_0.ExecuteReader())
                //            {
                //                while (reader.Read())
                //                {
                //                    fk_aid = reader[0].ToString();
                //                }
                //            }
                //            string[] sArray = charOffset.Split('-');
                //            e_command = new SqlCommand("INSERT INTO ENTITY (ANN_ID, TEXT,FIRST_LETTER_NUM,LAST_LETTER_NUM) VALUES ('" + fk_aid + "','" + Escape(entext) + "','" + sArray[0] + "','" + sArray[1] + "')", con);
                //            e_command.ExecuteNonQuery();
                //            Console.WriteLine("【ENTITY】" + fk_aid + "   " + Escape(entext) + "   " + sArray[0] + "   " + sArray[1]);
                //            string eid = entity.Attributes["id"].Value;
                //            entityMap.Add(eid, fk_aid);
                //        }


                //        foreach (XmlNode pair in sent.SelectNodes("pair"))
                //        {
                //            string e1 = pair.Attributes["e1"].Value;
                //            string e2 = pair.Attributes["e2"].Value;
                //            string fk_e1 = entityMap[e1];
                //            string fk_e2 = entityMap[e2];
                //            string inter = pair.Attributes["interaction"].Value;
                //            string p_type = pair.Attributes["type"].Value;
                //            if (inter == "True")
                //            {
                //                inter = "1";
                //            }
                //            else
                //            {
                //                inter = "0";
                //            }
                //            SqlCommand pa_command = new SqlCommand("INSERT INTO ANNOTATION (SENT_ID,ART_ID, TYPE) VALUES ('" + s_fk + "','" + fk + "','" + p_type + "')", con);
                //            pa_command.ExecuteNonQuery();
                //            pa_command.CommandText = "select scope_identity()";
                //            Console.WriteLine("【ANNOTATION(PAIR)】" + s_fk + "   " + fk + "   " + p_type);
                //            string fk_paid = null;
                //            using (SqlDataReader reader = pa_command.ExecuteReader())
                //            {
                //                while (reader.Read())
                //                {
                //                    fk_paid = reader[0].ToString();
                //                }
                //            }
                //            SqlCommand p_command = new SqlCommand("INSERT INTO PAIR(ANN_ID,E1,E2,INTERACTION) VALUES ('" + fk_paid + "','" + fk_e1 + "','" + fk_e2 + "','" + inter + "')", con);
                //            p_command.ExecuteNonQuery();
                //            p_command.CommandText = "select scope_identity()";
                //            Console.WriteLine("【PAIR】" + fk_paid + "   " + fk_e1 + "   " + fk_e2 + "   " + inter);
                //            string pid = pair.Attributes["id"].Value;
                //            entityMap.Add(pid, fk_paid);
                //            //string p_type = pair.Attributes["type"].Value;
                //            //SqlCommand p_command = new SqlCommand("INSERT INTO PAIR(E1,E2,INTERACTION) VALUES ('" + e1 + "','" + e2 + "','" + inter + "')", con);
                //            //p_command.ExecuteNonQuery();
                //            //p_command.CommandText = "select scope_identity()";
                //            //annTable.Rows.Add(s_fk, fk, p_type);
                //            //using (var adapter = new SqlDataAdapter("SELECT * from ANNOTATION", con))
                //            //using (new SqlCommandBuilder(adapter))
                //            //{
                //            //    adapter.Fill(annTable);
                //            //    adapter.Update(annTable);
                //            //}

                //        }
                 //   }
                //}
            }
        }

        private static string Escape(string text)
        {
            return text.Replace("'", "''");
        }
        public DataTable RemoveDuplicateRows(DataTable dTable, string colName)
{
   Hashtable hTable = new Hashtable();
   ArrayList duplicateList = new ArrayList();

   //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
   //And add duplicate item value in arraylist.
   foreach (DataRow drow in dTable.Rows)
   {
      if (hTable.Contains(drow[colName]))
         duplicateList.Add(drow);
      else
         hTable.Add(drow[colName], string.Empty); 
   }

   //Removing a list of duplicate items from datatable.
   foreach (DataRow dRow in duplicateList)
      dTable.Rows.Remove(dRow);

   //Datatable which contains unique records will be return as output.
      return dTable;
}
    }
}