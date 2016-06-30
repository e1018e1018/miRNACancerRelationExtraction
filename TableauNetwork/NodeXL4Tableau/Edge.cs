using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeXL4Tableau
{
    public class Edge
    {
        public Node Source { get; set; }
        public Node Target { get; set; }

        public Edge(Node source, Node target)
        {
            Source = source;
            Target = target;
        }

        public override string ToString()
        {
            return string.Format("{0}->{1}", Source.ToString(), Target.ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Edge p = obj as Edge;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return ToString().Equals(p.ToString());
        }
    }
}
