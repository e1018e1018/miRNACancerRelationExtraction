using Smrf.NodeXL.Core;
using Smrf.NodeXL.Layouts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace NodeXL4Tableau
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-UK96O0N;Initial Catalog=miRNA_final;Integrated Security=True"))
            IGraph g = reader.GetGraph();
            CircleLayout cl = new CircleLayout();
            //FruchtermanReingoldLayout pl = new FruchtermanReingoldLayout();
            //HarelKorenFastMultiscaleLayout hl = new HarelKorenFastMultiscaleLayout();
            LayoutContext lc = new LayoutContext(new Rectangle(0, 0, 1600, 900));
            cl.LayOutGraph(g, lc);
            using (StreamWriter sw = new StreamWriter("tableau.txt"))
            {
                sw.Write("ID\tRelation\tRelType\tNE\tX\tY\tY'\tNEType\tMeSH\tSent\n");
                    
                foreach (var edge in g.Edges)
                {
                    sw.Write("{0}\t{4}\t{5}\t{1}\t{2}\t{3}\t{3}\tmiRNA\t{6}\t{7}\n", edge.ID, edge.Vertex1.Name, 
                        edge.Vertex1.Location.X, edge.Vertex1.Location.Y, edge.Name, edge.Tag, "", edge.GetValue(MiRNADataReader.SENTENCE));
                    sw.Write("{0}\t{4}\t{5}\t{1}\t{2}\t{3}\t{3}\tCancer\t{6}\t{7}\n", edge.ID, edge.Vertex2.Name,
                        edge.Vertex2.Location.X, edge.Vertex2.Location.Y, edge.Name, edge.Tag, edge.Vertex2.GetValue(MiRNADataReader.MeSH), edge.GetValue(MiRNADataReader.SENTENCE));
                }
            }
        }
    }
}
