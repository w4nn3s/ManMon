using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Renci.SshNet;
using System.Diagnostics;

namespace ManMon
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }



        private void btnConnect_Click(object sender, EventArgs e)
        {

            //string[] lines = System.IO.File.ReadAllLines(@"in1.txt");
            //string[] lines = System.IO.File.ReadAllLines(@"in5.txt");
            string[] lines = System.IO.File.ReadAllLines(@"in.txt");
            string[][] hostinfo = new string[lines.Length][];
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


            string user = txtuser.Text;
            string pass = txtpass.Text;
            //string host = txthost.Text;
            string cmd = "run";

            btnCollect.Text = "Collecting...";
            btnCollect.Enabled = false;



            string[] switchcmds = new string[] { "run","ver","mac","arp","intstatus", "vlan", "stpdet", "cdpdet", "ipintbrief", "logg", "powerinline", "etherchannel", "swenvall" };
            string[] routercmds = new string[] { "run", "ver", "mac", "arp", "cdpdet","ipintbrief","logg", "rtenvall", "etherchannel", "rtenvall" };


            //CLEAR ALL OLD FILES
            foreach (string swcmd in switchcmds)
            {
                Array.ForEach(System.IO.Directory.GetFiles(@"output\" + swcmd + "\\"), System.IO.File.Delete);
            }

            foreach (string rtcmd in routercmds)
            {
                Array.ForEach(System.IO.Directory.GetFiles(@"output\" + rtcmd + "\\"), System.IO.File.Delete);
            }




            //execute all switch commands
            for (int z=0;z<switchcmds.Count();z++)
            {
                
                for (int y = 0; y < i; y++)
                {
                    //if setting is radius, use textbox credentials
                    if (string.Compare(hostinfo[y][2], "radius") != 0)
                    {
                        user = hostinfo[y][2];
                        pass = hostinfo[y][3];
                    }

                    //Execute only if host is enabled and is a switch
                    if ((string.Compare(hostinfo[y][4], "1") == 0) && string.Compare(hostinfo[y][1],"s") == 0)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Users\wcolman\source\repos\accumulator\accumulator\bin\Debug\accumulator.exe");
                        startInfo.Arguments = user + " " + pass + " " + hostinfo[y][0] + " " + switchcmds[z] + " " + hostinfo[y][1];
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(startInfo);
                    }
                }
            }


            for (int z = 0; z < switchcmds.Count(); z++)
            {

                for (int y = 0; y < i; y++)
                {
                    //if setting is radius, use textbox credentials
                    if (string.Compare(hostinfo[y][2], "radius") != 0)
                    {
                        user = hostinfo[y][2];
                        pass = hostinfo[y][3];
                    }

                    //Execute only if host is enabled and is a switch
                    if ((string.Compare(hostinfo[y][4], "1") == 0) && string.Compare(hostinfo[y][1], "r") == 0)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Users\wcolman\source\repos\accumulator\accumulator\bin\Debug\accumulator.exe");
                        startInfo.Arguments = user + " " + pass + " " + hostinfo[y][0] + " " + switchcmds[z] + " " + hostinfo[y][1];
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(startInfo);
                    }
                }
            }

            //btnCollect.Text=

            /*
            System.Threading.Thread.Sleep(4000);
            progress1.Value = 20;
            System.Threading.Thread.Sleep(4000);
            progress1.Value = 40;
            System.Threading.Thread.Sleep(4000);
            progress1.Value = 60;
            System.Threading.Thread.Sleep(4000);
            progress1.Value = 80;
            System.Threading.Thread.Sleep(4000);
            progress1.Value = 100;
            */
            btnCollect.Enabled = true;
            btnCollect.Text = "Collect";

            //System.Threading.Thread.Sleep(500);
           // progress1.Value = 0;
            //progress1.Visible = false;
            //execute all router commands





        }
    

        private void Readhostinfo()
        {

        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            Parser.Parser parser = new Parser.Parser();

            parser.ReadHosts();
            parser.ReadPorts();
        }
    }
}
