using Microsoft.NodeXL.ExcelTemplatePlugIns;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace NodeXL4Tableau
{
    public class MiRNADataReader : IGraphDataProvider2 
    {
        protected string output;
        protected List<Edge> edges = new List<Edge>();
        protected List<Node> nodes = new List<Node>();
        public MiRNADataReader(string sourcefile, string outputFile)
        {
            output = outputFile;
            using (StreamReader sr = new StreamReader(sourcefile))
            {
                while (sr.Peek() != -1)
                {
                    string line = sr.ReadLine().Trim();
                    if (line.Equals(""))
                    {
                        continue;
                    }
                    string[] tks = line.Split('\t');
                    string[] miRNAs = tks[2].Split('|');
                    string[] diseases = tks[5].Split('|');
                    for (int i = 0; i < miRNAs.Length; i++)
                    {
                        Node s = new Node(miRNAs[i]);
                        if (!nodes.Contains(s))
                        {
                            nodes.Add(s);
                        }
                        for (int j = 0; j < diseases.Length; j++)
                        {
                            Node t = new Node(diseases[j]);
                            if (!nodes.Contains(t))
                            {
                                nodes.Add(t);
                            }
                            Edge e = new Edge(s, t);
                            if (!edges.Contains(e))
                            {
                                edges.Add(e);
                            }
                        }
                    }
                }
            }
        }
        public string Description
        {
            get { return "miRNA-Cancer Data"; }
        }

        public string Name
        {
            get { return this.GetType().Name; }
        }

        public bool TryGetGraphDataAsTemporaryFile(out string pathToTemporaryFile)
        {
            pathToTemporaryFile = output;
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\n",
                NewLineHandling = NewLineHandling.Replace
            };
            try
            {
                using (XmlWriter writer = XmlWriter.Create(output, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("graphml", "http://graphml.graphdrawing.org/xmlns");
                    writer.WriteStartElement("graph");
                    writer.WriteAttributeString("edgedefault", "undirected");

                    foreach (Node n in nodes)
                    {
                        writer.WriteStartElement("node");
                        writer.WriteAttributeString("id", n.ID);
                        writer.WriteEndElement();
                    }

                    foreach (Edge e in edges)
                    {
                        writer.WriteStartElement("edge");
                        writer.WriteAttributeString("source", e.Source.ID);
                        writer.WriteAttributeString("target", e.Target.ID);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
