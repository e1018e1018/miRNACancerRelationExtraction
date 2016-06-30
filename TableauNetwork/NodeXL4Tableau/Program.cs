using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeXL4Tableau
{
    class Program
    {
        static void Main(string[] args)
        {
            MiRNADataReader reader = new MiRNADataReader(@"..\..\..\Data\input.txt", "output.xml");
            string path;
            if (reader.TryGetGraphDataAsTemporaryFile(out path))
            {

            }
        }
    }
}
