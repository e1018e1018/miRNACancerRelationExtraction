﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.RegularExpressions;
using NTTU.BigODM.Bio.Thirdparty.NCBI;
using System.Data;

namespace miRNA
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-UK96O0N;Initial Catalog=miRNA_final;Integrated Security=True"))
            {
                //
                // Open the SqlConnection.
                //
                con.Open();
                XmlDocument miRNA = new XmlDocument();
                miRNA.Load(@"C:\Users\e1018\Desktop\專題\miRNA-Test-Corpus.xml");
                //for (int i = 0; i < doc.SelectNodes("corpus//document").Count; i++)
                //{
                //    XmlNode mode = doc.SelectNodes("corpus//document")[i];
                //}
                foreach (XmlNode doc in miRNA.SelectNodes("corpus//document"))
                {
                    string pmid = doc.Attributes["origId"].Value;
                    bool isS = false;
                    Abstract a = PubMedEUtilities.FetchByID(pmid, ref isS);
                    //
                    // The following code uses an SqlCommand based on the SqlConnection.
                    //
                    SqlCommand d_command_0 = new SqlCommand("INSERT INTO ARTICLE (ART_PMID, ART_JOURNAL,ART_ISSN) VALUES ('" + pmid +"','"+a.JournalName.Replace("'","''")+"','"+a.ISSN+"')", con);
                    //SqlCommand d_command_1 = new SqlCommand("INSERT INTO ARTICLE (ART_JOURNAL) VALUES ('" + + "')", con);
                    //SqlCommand d_command_2 = new SqlCommand("INSERT INTO ARTICLE (ART_ISSN) VALUES ('" + pmid + "')", con);
                    d_command_0.ExecuteNonQuery();
                    d_command_0.CommandText = "select scope_identity()";
                    string fk = null;
                    using (SqlDataReader reader = d_command_0.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            fk = reader[0].ToString();
                        }
                    }

                    foreach (XmlNode sent in doc.SelectNodes("sentence"))
                    {
                        string text = sent.Attributes["text"].Value;
                        string id = sent.Attributes["origId"].Value;
                        int title= Convert.ToInt32(id.Substring(id.Length-1));
                        int istitle = 0;    
                        istitle = (title == 0) ? 1 : 0;
                        SqlCommand s_command = new SqlCommand("INSERT INTO SENTENCE (ART_ID,IS_TITLE,TEXT) VALUES ('" + fk + "','" + istitle + "','" + text + "')", con);
                        s_command.ExecuteNonQuery();
                        s_command.CommandText = "select scope_identity()";
                        string s_fk = null;
                        using (SqlDataReader reader = s_command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                               s_fk = reader[0].ToString();
                            }
                        }
                        foreach (XmlNode entity in sent.SelectNodes("entity"))
                        {
                            string entext = entity.Attributes["text"].Value;
                            string e_type = entity.Attributes["type"].Value;
                            string charOffset = entity.Attributes["charOffset"].Value;
                            string[] sArray = charOffset.Split('-');
                            DataTable annTable = new DataTable();
                            annTable.Columns.Add("SENT_ID", typeof(int));
                            annTable.Columns.Add("ART_ID", typeof(int));
                            annTable.Columns.Add("TYPE", typeof(string));
                            annTable.Rows.Add(s_fk,fk, e_type);
                            //DataTable entityTable = new DataTable();
                            //entityTable.Columns.Add("TEXT", typeof(string));
                            //entityTable.Columns.Add("FIRST_LETTER_NUM", typeof(int));
                            //entityTable.Columns.Add("LAST_LETTER_NUM", typeof(int));

                            //entityTable.Rows.Add(entext, sArray[0], sArray[1]);
                            using(var adapter=new SqlDataAdapter("SELECT * from ANNOTATION",con))
                                using (new SqlCommandBuilder(adapter))
                            {
                                adapter.Fill(annTable);
                                adapter.Update(annTable);
                            }
                        }
                        foreach (XmlNode pair in sent.SelectNodes("pair"))
                        {
                            string e1 = pair.Attributes["e1"].Value;
                            string e2 = pair.Attributes["e2"].Value;
                            string inter = pair.Attributes["interatcion"].Value;
                            string p_type = pair.Attributes["type"].Value;
                            SqlCommand p_command_0 = new SqlCommand("INSERT INTO PAIR(E1) VALUES ('" + e1 + "')", con);
                            SqlCommand p_command_1 = new SqlCommand("INSERT INTO PAIR(E2) VALUES ('" + e2 + "')", con);
                            SqlCommand p_command_2 = new SqlCommand("INSERT INTO PAIR(INTERACTION) VALUES ('" + inter + "')", con);
                        }
                    }
                }
            }
        }
    }
}