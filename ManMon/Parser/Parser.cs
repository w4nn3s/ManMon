using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManMon.Parser
{
    class Parser
    {
        string[][] hostinfo = null;
        Port[] ports = null;


        public void ReadHosts()
        {
            string[] lines = System.IO.File.ReadAllLines(@"in1.txt");
            //string[] lines = System.IO.File.ReadAllLines(@"in.txt");
            hostinfo = new string[lines.Length][];
            int i = 0;
            string[] lijn;

            foreach (string line in lines)
            {
                lijn = line.Split('\t');
                hostinfo[i] = new string[5];
                hostinfo[i][0] = lijn[0]; //host
                hostinfo[i][1] = lijn[1]; //type
                hostinfo[i][2] = lijn[2]; //user
                hostinfo[i][3] = lijn[3]; //pass
                hostinfo[i][4] = lijn[4]; //1=enabled, 0=disabled
                i++;
            }
        }

        private void ReadPorts()
        {
            for (int i = 0; i < hostinfo.Count(); i++)
            {
                if (string.Compare(hostinfo[i][1], "s") == 0)
                {
                    string filename = hostinfo[i][0] + "intstatus.txt";
                    string[] lines = System.IO.File.ReadAllLines(@"output\intstatus\" + filename);
                    foreach (string line in lines)
                    {
                        Console.WriteLine(line + "\n");
                    }
                }
            }

        }
}
}
