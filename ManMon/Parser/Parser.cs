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
        Switch[] switches = null;
        public int nswitches = 0;
        int nports = 0;

        public void init()
        {
             Parser p = new Parser();
        }


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

        public void ReadPorts()
        {
            int startports = 0;
            for (int i = 0; i < hostinfo.Count(); i++)
            {
                if (string.Compare(hostinfo[i][1], "s") == 0)
                {
                    switches[nswitches] = new Switch();
                    //switches[nswitches].hostname = hostinfo[i][0];
                    nswitches++;
                    switches[nswitches].ID = nswitches;
                    

                    string filename = hostinfo[i][0] + "_intstatus.txt";
                    string[] lines = System.IO.File.ReadAllLines(@"output\intstatus\" + filename);
                    foreach (string line in lines)
                    {

                        if(line.Contains('#'))
                        {
                            startports = 0;
                        }

                        if(startports==1)
                        {
                            ports[nports] = new Port();
                            ports[nports].Portnr =line.Split(' ')[0];
                            Console.WriteLine(ports[nports].Portnr);
                            nports++;
                        }

                        if (line.Contains("Duplex")&&(line.Contains("Speed")))
                        {
                            startports = 1;
                        }

                            Console.WriteLine(line);

                    }
                }
            }

        }
}
}
