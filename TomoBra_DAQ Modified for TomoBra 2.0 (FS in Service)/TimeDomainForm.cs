using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TomoBra_DAQ
{
    class TimeDomainForm
    {
        public static double[] LoadArray(String filename)
        {
            double[] data_d = new double[File.ReadAllLines(filename).Length];
            bool result = true;
            double value;
            int i = 0;
            string line;
            //List<string> lines = new List<string>();
            // Use using StreamReader for disposing.
            using (StreamReader r = new StreamReader(filename))
            {
                // Use while != null pattern for loop
                while ((line = r.ReadLine()) != null)
                {
                    result = result & double.TryParse(line, out value);
                    data_d[i++] = value;
                }
            }
            return data_d;
        }

        public static void StoreArrays(double[] Array1, double[] Array2, String filename)
        {
            int nRows = Array1.Length;
            string[] lines = new string[nRows];
            for (uint i = 0; i < nRows; i++)
                lines[i] = Array1[i].ToString() + "\t" + Array2[i].ToString();
            System.IO.File.WriteAllLines(filename, lines);
        }

    }
   
}

