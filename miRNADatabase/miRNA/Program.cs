using System;
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
            using (SqlConnection con = new SqlConnection("Data Source=localhost;Initial Catalog=miRNA_final;Integrated Security=True"))
            {
                //
                // Open the SqlConnection.
                //
                con.Open();

                ///////////////////////////////////////////////////////////////////////////////////////////////////
                // Clear all content first
                // REMOVE me after complete!
                SqlCommand clean_command = new SqlCommand("DELETE FROM ENTITY;DELETE FROM ANNOTATION; DELETE FROM SENTENCE; DELETE FROM ARTICLE;", con);
                //SqlCommand clean_command = new SqlCommand("DELETE FROM ARTICLE", con);
                clean_command.ExecuteNonQuery();
                // reset identity
                clean_command = new SqlCommand("DBCC CHECKIDENT ('[ARTICLE]', RESEED, 0)", con);
                clean_command.ExecuteNonQuery();
                clean_command = new SqlCommand("DBCC CHECKIDENT ('[SENTENCE]', RESEED, 0)", con);
                clean_command.ExecuteNonQuery();
                clean_command = new SqlCommand("DBCC CHECKIDENT ('[ANNOTATION]', RESEED, 0)", con);
                clean_command.ExecuteNonQuery();
                ///////////////////////////////////////////////////////////////////////////////////////////////////


                XmlDocument miRNA = new XmlDocument();
                miRNA.Load(@"..\..\..\Dataset\miRNA-Test-Corpus.xml");
                //for (int i = 0; i < doc.SelectNodes("corpus//document").Count; i++)
                //{
                //    XmlNode mode = doc.SelectNodes("corpus//document")[i];
                //}


                DataTable annotationTable = new DataTable();
                DataColumn annID = new DataColumn();
                annID.DataType = System.Type.GetType("System.Int32");
                annID.AutoIncrement = true;
                annID.AutoIncrementSeed = 1;
                annID.AutoIncrementStep = 1;
                annotationTable.Columns.Add(annID);
                annotationTable.Columns.Add("SENT_ID", typeof(int));
                annotationTable.Columns.Add("ART_ID", typeof(int));
                annotationTable.Columns.Add("TYPE", typeof(string));

                DataTable entityTable = new DataTable();
                entityTable.Columns.Add("ANN_ID", typeof(int));
                entityTable.Columns.Add("TEXT", typeof(string));
                entityTable.Columns.Add("FIRST_LETTER_NUM", typeof(int));
                entityTable.Columns.Add("LAST_LETTER_NUM", typeof(int));

                foreach (XmlNode doc in miRNA.SelectNodes("corpus//document"))
                {
                    string pmid = doc.Attributes["origId"].Value;
                    bool isS = false;
                    Abstract a = PubMedEUtilities.FetchByID(pmid, ref isS);
                    //
                    // The following code uses an SqlCommand based on the SqlConnection.
                    //
                    // TODO 請再把 jorunal 等資訊加回
                    SqlCommand d_command_0 = new SqlCommand("INSERT INTO ARTICLE (ART_PMID) VALUES ('" + pmid + "')", con);
                    //SqlCommand d_command_0 = new SqlCommand("INSERT INTO ARTICLE (ART_PMID, ART_JOURNAL,ART_ISSN) VALUES ('" + pmid + "','" + a.JournalName.Replace("'", "''") + "','" + a.ISSN + "')", con);
                    //SqlCommand d_command_1 = new SqlCommand("INSERT INTO ARTICLE (ART_JOURNAL) VALUES ('" + + "')", con);
                    //SqlCommand d_command_2 = new SqlCommand("INSERT INTO ARTICLE (ART_ISSN) VALUES ('" + pmid + "')", con);
                    d_command_0.ExecuteNonQuery();
                    d_command_0.CommandText = "select scope_identity()";
                    string art_id = d_command_0.ExecuteScalar().ToString();

                    foreach (XmlNode sent in doc.SelectNodes("sentence"))
                    {
                        Dictionary<string, int> entIDMap = new Dictionary<string, int>();

                        string text = sent.Attributes["text"].Value;
                        string id = sent.Attributes["origId"].Value;
                        int title = Convert.ToInt32(id.Substring(id.Length - 1));
                        int istitle = 0;
                        istitle = (title == 0) ? 1 : 0;
                        SqlCommand s_command = new SqlCommand("INSERT INTO SENTENCE (ART_ID,IS_TITLE,TEXT) VALUES ('" + art_id + "','" + istitle + "','" + text.Replace("'", "''") + "')", con);
                        s_command.ExecuteNonQuery();
                        s_command.CommandText = "select scope_identity()";
                        string sent_id = s_command.ExecuteScalar().ToString();
                        foreach (XmlNode entity in sent.SelectNodes("entity"))
                        {
                            string entext = entity.Attributes["text"].Value;
                            string e_type = entity.Attributes["type"].Value;
                            string charOffset = entity.Attributes["charOffset"].Value;
                            string[] sArray = charOffset.Split('-');
                            //SqlCommand e_command = new SqlCommand("INSERT INTO ENTITY (ANN_ID,TEXT,FIRST_LETTER_NUM,LAST_LETTER_NUM) VALUES ('" + ann_fk + "','" + entext + "','" + sArray[0] + "','" + sArray[1] + "')", con);

                            DataRow newAnnRow = annotationTable.NewRow();
                            newAnnRow.SetField("SENT_ID", sent_id);
                            newAnnRow.SetField("ART_ID", art_id);
                            newAnnRow.SetField("TYPE", e_type);
                            int ann_id = (int)newAnnRow.ItemArray[0];
                            annotationTable.Rows.Add(newAnnRow);
                            entityTable.Rows.Add(ann_id, entext, sArray[0], sArray[1]);
                            entIDMap.Add(entity.Attributes["id"].Value, ann_id);
                        }                        

                        foreach (XmlNode pair in sent.SelectNodes("pair"))
                        {
                            string e1 = pair.Attributes["e1"].Value;
                            string e2 = pair.Attributes["e2"].Value;
                            int e1fk = entIDMap[e1];
                            int e2fk = entIDMap[e2];
                            //string inter = pair.Attributes["interatcion"].Value;
                            //string p_type = pair.Attributes["type"].Value;
                            //SqlCommand p_command = new SqlCommand("INSERT INTO PAIR(E1,E2,INTERACTION) VALUES ('" + e1 + "','" + e2 + "','" + inter + "')", con);
                            //p_command.ExecuteNonQuery();
                            //p_command.CommandText = "select scope_identity()";
                            //annotationTable.Rows.Add(sent_id, art_id, p_type);
                            //using (var adapter = new SqlDataAdapter("SELECT * from ANNOTATION", con))
                            //using (new SqlCommandBuilder(adapter))
                            //{
                            //    adapter.Fill(annotationTable);
                            //    adapter.Update(annotationTable);
                            //}
                        }
                    }
                    //最後再一次更新
                    using (var adapter = new SqlDataAdapter("SELECT * from ANNOTATION", con))
                    using (new SqlCommandBuilder(adapter))
                    {
                        adapter.Fill(annotationTable);
                        adapter.Update(annotationTable);
                    }
                    using (var adapter = new SqlDataAdapter("SELECT * from ENTITY", con))
                    using (new SqlCommandBuilder(adapter))
                    {
                        adapter.Fill(entityTable);
                        adapter.Update(entityTable);
                    }

                    annotationTable.Clear();
                    entityTable.Clear();

                }
            }
        }
    }
}