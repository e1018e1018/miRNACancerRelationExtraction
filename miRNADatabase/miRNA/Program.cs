using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
                    //
                    // The following code uses an SqlCommand based on the SqlConnection.
                    //
                    SqlCommand command = new SqlCommand("INSERT INTO ARTICLE (ART_PMID) VALUES ('" + pmid + "')", con);
                    command.ExecuteNonQuery();
                    command.CommandText = "select scope_identity()";
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("{0}",
                            reader[0]);
                        }
                    }
                    

                    foreach (XmlNode sent in miRNA.SelectNodes("sentence"))
                    {
                        string text = sent.Attributes["text"].Value;
                        string id = sent.Attributes["origId"].Value;
                        SqlCommand command_2 = new SqlCommand("INSERT INTO ARTICLE (ART_PMID) VALUES ('" + text + "')", con);
                        foreach (XmlNode entity in sent.SelectNodes("entity"))
                        {
                            string entext = entity.Attributes["text"].Value;
                        }
                        foreach (XmlNode pair in sent.SelectNodes("pair"))
                        {
                            string e1 = pair.Attributes["e1"].Value;
                            string e2 = pair.Attributes["e2"].Value;
                            string inter = pair.Attributes["interatcion"].Value;
                            SqlCommand command_3= new SqlCommand("INSERT INTO PAIR(E1) VALUES ('" + e1+ "')", con);
                            SqlCommand command_4 = new SqlCommand("INSERT INTO PAIR(E2) VALUES ('" + e2 + "')", con);
                            SqlCommand command_5 = new SqlCommand("INSERT INTO PAIR(INTERACTION) VALUES ('" + inter + "')", con);

                        }
                    }

                }
            }


        }
    }
}
