﻿using Smrf.NodeXL.Core;
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
            MiRNADataReader reader = new MiRNADataReader(@"..\..\..\Data\input.txt");
            IGraph g = reader.GetGraph();
            CircleLayout cl = new CircleLayout();
            LayoutContext lc = new LayoutContext(new Rectangle(0, 0, 1600, 900));
            cl.LayOutGraph(g, lc);
            using (StreamWriter sw = new StreamWriter("tableau.txt"))
            {
                sw.Write("ID\tRelation\tRelType\tNE\tX\tY\tY'\tNEType\n");
                    
                foreach (var edge in g.Edges)
                {
                    sw.Write("{0}\t{4}\t{5}\t{1}\t{2}\t{3}\t{3}\tmiRNA\n", edge.ID, edge.Vertex1.Name, 
                        edge.Vertex1.Location.X, edge.Vertex1.Location.Y, edge.Name, edge.Tag);
                    sw.Write("{0}\t{4}\t{5}\t{1}\t{2}\t{3}\t{3}\tCancer\n", edge.ID, edge.Vertex2.Name,
                        edge.Vertex2.Location.X, edge.Vertex2.Location.Y, edge.Name, edge.Tag);                    
                }
            }
        }
    }
}
