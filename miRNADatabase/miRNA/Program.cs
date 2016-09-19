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
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-UK96O0N;Initial Catalog=miRNA_final;Integrated Security=True"))
            {
                //
                // Open the SqlConnection.
                //
                con.Open();
                XmlDocument miRNA = new XmlDocument();
                miRNA.Load(@"C:\Users\e1018\Desktop\專題\miRNA-Train-Corpus.xml");
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
                    SqlCommand d_command_0 = new SqlCommand("INSERT INTO ARTICLE (ART_PMID, ART_JOURNAL,ART_ISSN) VALUES ('" + pmid + "','" + a.JournalName.Replace("'", "''") + "','" + a.ISSN + "')", con);
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
                        int title = Convert.ToInt32(id.Substring(id.Length - 1));
                        int istitle = 0;
                        istitle = (title == 0) ? 1 : 0;
                        SqlCommand s_command = new SqlCommand("INSERT INTO SENTENCE (ART_ID,IS_TITLE,TEXT) VALUES ('" + fk + "','" + istitle + "','" + Escape(text) + "')", con);
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
                        Dictionary<string, string> entityMap = new Dictionary<string, string>();
                        foreach (XmlNode entity in sent.SelectNodes("entity"))
                        {
                            string entext = entity.Attributes["text"].Value;
                            string e_type = entity.Attributes["type"].Value;
                            string charOffset = entity.Attributes["charOffset"].Value;

                            SqlCommand e_command = new SqlCommand("INSERT INTO ANNOTATION (SENT_ID,ART_ID, TYPE) VALUES ('" + s_fk + "','" + fk + "','" + e_type + "')", con);
                            e_command.ExecuteNonQuery();
                            e_command.CommandText = "select scope_identity()";
                            string fk_aid = null;
                            using (SqlDataReader reader = d_command_0.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    fk_aid = reader[0].ToString();
                                }
                            }
                            string[] sArray = charOffset.Split('-');
                            e_command = new SqlCommand("INSERT INTO ENTITY (ANN_ID, TEXT,FIRST_LETTER_NUM,LAST_LETTER_NUM) VALUES ('" + fk_aid + "','" + Escape(entext) + "','" + sArray[0] + "','" + sArray[1] + "')", con);
                            e_command.ExecuteNonQuery();
                            string eid = entity.Attributes["id"].Value;
                            entityMap.Add(eid, fk_aid);
                        }


                        foreach (XmlNode pair in sent.SelectNodes("pair"))
                        {
                            string e1 = pair.Attributes["e1"].Value;
                            string e2 = pair.Attributes["e2"].Value;
                            string fk_e1 = entityMap[e1];
                            string fk_e2 = entityMap[e2];
                            string inter = pair.Attributes["interaction"].Value;
                            string p_type = pair.Attributes["type"].Value;
                            if (inter == "True")
                            {
                                inter = "1";
                            }
                            else
                            {
                                inter = "0";
                            }
                            SqlCommand pa_command = new SqlCommand("INSERT INTO ANNOTATION (SENT_ID,ART_ID, TYPE) VALUES ('" + s_fk + "','" + fk + "','" + p_type + "')", con);
                            pa_command.ExecuteNonQuery();
                            pa_command.CommandText = "select scope_identity()";
                            string fk_paid = null;
                            using (SqlDataReader reader = pa_command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    fk_paid = reader[0].ToString();
                                }
                            }
                            SqlCommand p_command = new SqlCommand("INSERT INTO PAIR(ANN_ID,E1,E2,INTERACTION) VALUES ('" + fk_paid + "','" + fk_e1 + "','" + fk_e2 + "','" + inter + "')", con);
                            p_command.ExecuteNonQuery();
                            p_command.CommandText = "select scope_identity()";
                            string pid = pair.Attributes["id"].Value;
                            entityMap.Add(pid, fk_paid);
                            //string p_type = pair.Attributes["type"].Value;
                            //SqlCommand p_command = new SqlCommand("INSERT INTO PAIR(E1,E2,INTERACTION) VALUES ('" + e1 + "','" + e2 + "','" + inter + "')", con);
                            //p_command.ExecuteNonQuery();
                            //p_command.CommandText = "select scope_identity()";
                            //annTable.Rows.Add(s_fk, fk, p_type);
                            //using (var adapter = new SqlDataAdapter("SELECT * from ANNOTATION", con))
                            //using (new SqlCommandBuilder(adapter))
                            //{
                            //    adapter.Fill(annTable);
                            //    adapter.Update(annTable);
                            //}
                        }
                    }
                }
            }
        }

        private static string Escape(string text)
        {
            return text.Replace("'", "''");
        }
    }
}