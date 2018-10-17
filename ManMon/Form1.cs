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
                    System.Threading.Thread.Sleep(1000);
                }
                System.Threading.Thread.Sleep(6500);
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

                    //Execute only if host is enabled and is a router
                    if ((string.Compare(hostinfo[y][4], "1") == 0) && string.Compare(hostinfo[y][1], "r") == 0)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Users\wcolman\source\repos\accumulator\accumulator\bin\Debug\accumulator.exe");
                        startInfo.Arguments = user + " " + pass + " " + hostinfo[y][0] + " " + switchcmds[z] + " " + hostinfo[y][1];
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(startInfo);
                        //System.Threading.Thread.Sleep(5000);
                    }
                }
            }

            btnCollect.Enabled = true;
            btnCollect.Text = "Collect";






        }
    

        private void Readhostinfo()
        {

        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            Parser.Parser parser = new Parser.Parser();

            parser.ReadOUI();
            parser.ReadHosts();
            parser.ReadPorts();
            lblnswitches.Text = parser.nswitches.ToString() + " switches loaded";
            parser.ReadCDPPorts();
            parser.ReadClients();

           

            Array.Resize(ref parser.switches, parser.nswitches);
            Array.Resize(ref parser.ports, parser.nports);
            Array.Resize(ref parser.clients, parser.nclients);
            Array.Resize(ref parser.usage, parser.nusage);

            parser.AddIPs();


            //GENERATE OUTPUT

            System.IO.StreamWriter file = new System.IO.StreamWriter(@"out.txt");
            for (int i = 0; i < parser.nclients; i++)
            {
                String switchname = null;
                String portname = null;
                int PortUseID = Array.FindIndex(parser.usage, obj => obj.ClientID == parser.clients[i].ID);
                int PortID = Array.FindIndex(parser.ports, obj => obj.ID == parser.usage[PortUseID].PortID);

                int CLIENTID = parser.clients[i].ID;
                string MACADD = parser.clients[i].MACadd;
                string IP = parser.clients[i].IPadd;
                IP = string.IsNullOrEmpty(IP) ? "\t" : IP;
                string OUI = parser.clients[i].guesstimac;
                string SWNAAM = parser.switches[parser.ports[PortID].SwitchID].hostname;
                string PORTNAME = parser.ports[PortID].Portnr;


                //Console.WriteLine(parser.clients[i].ID + " - " + parser.clients[i].MACadd + " --- " + parser.clients[i].IPadd + " --- " + parser.clients[i].guesstimac + " --- " + parser.switches[parser.ports[PortID].SwitchID].hostname + " --- " + parser.ports[PortID].Portnr );
                //file.WriteLine(parser.clients[i].ID + " - " + parser.clients[i].MACadd + " \t " + parser.clients[i].IPadd + " \t " + parser.clients[i].guesstimac + " \t " + parser.switches[parser.ports[PortID].SwitchID].hostname + " \t " + parser.ports[PortID].Portnr);
                file.WriteLine(CLIENTID + " - " + MACADD + " \t " + IP + " \t " + SWNAAM + " \t " + PORTNAME  + "\t\t" + OUI);
                //Console.WriteLine(CLIENTID + " - " + MACADD + " \t " + IP + " \t " + SWNAAM + " \t " + PORTNAME + "\t\t" + OUI);
            }
            file.Close();






        }
    }
}
