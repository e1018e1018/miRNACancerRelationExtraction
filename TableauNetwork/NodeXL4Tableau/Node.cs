using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeXL4Tableau
{
    public class Node
    {
        public Node(string id)
        {
            ID = id;
        }
        public string ID { get; set; }
        public override string ToString()
        {
            return ID;
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Node p = obj as Node;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return ID.Equals(p.ID);
        }
    }
}
