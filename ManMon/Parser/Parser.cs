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
        public Port[] ports = new Port[30000];
        public Switch[] switches = new Switch[5000];
        public Client[] clients = new Client[5000];
        public Portuse[] usage = new Portuse[5000];
        public string[][] OUI = new string[30000][];


        
        public int nswitches = 0;
        public int nclients = 0;
        public int nports = 0;
        public int nusage = 0;

        public void init()
        {
            Parser p = new Parser();
        }

        public void ReadOUI()
        {
            string[] lines = System.IO.File.ReadAllLines(@"oui.txt");
            int nOUI = 0;

            foreach (string line in lines)
            {
                if(line.Contains("base 16"))
                {
                    OUI[nOUI] = new string[2];
                    OUI[nOUI][0] = line.Substring(0, 4).ToLower() + "." + line.Substring(4, 2).ToLower();
                    OUI[nOUI][1] = line.Substring(22, line.Length - 22);
                    nOUI++;
                   // Console.WriteLine(OUI[nOUI][0] + " --- " + OUI[nOUI][1]);
                }
            }

            Array.Resize(ref OUI, nOUI);
        }


        public void ReadHosts()
        {
            //string[] lines = System.IO.File.ReadAllLines(@"in1.txt");
            string[] lines = System.IO.File.ReadAllLines(@"in.txt");
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
                if (string.Compare(hostinfo[i][1], "s") == 0 && string.Compare(hostinfo[i][4], "1") == 0)
                {
                    switches[nswitches] = new Switch();
                    switches[nswitches].hostname = hostinfo[i][0];
                    
                    switches[nswitches].ID = nswitches;


                    string filename = hostinfo[i][0] + "_intstatus.txt";
                    string[] lines = System.IO.File.ReadAllLines(@"output\intstatus\" + filename);
                    
                    foreach (string line in lines)
                    {

                        if (line.Contains('#'))
                        {
                            startports = 0;
                        }

                        if (startports == 1)
                        {
                          //  Console.WriteLine("\n\n\n\nREADING FROM " + hostinfo[i][0]);
                            ports[nports] = new Port();

                            ports[nports].Portnr = line.Substring(0, 10).Trim();
                            ports[nports].PortDesc = line.Substring(10, 19).Trim();
                            ports[nports].Status = line.Substring(29, 13).Trim();
                            ports[nports].Vlan = line.Substring(42, 8).Trim();
                            ports[nports].SwitchID = nswitches;
                            ports[nports].ID = nports;

                            //Console.WriteLine(ports[nports].Portnr + " " + ports[nports].PortDesc + " " + ports[nports].Status + " " + ports[nports].Vlan);
                            nports++;
                        }

                        if (line.Contains("Duplex") && (line.Contains("Speed")))
                        {
                            startports = 1;
                        }

                        //Console.WriteLine(line);

                    }
                    nswitches++;
                }
            }

        }

        public void ReadClients()
        {
            string macadd = null;
            string port = null;
            string oui = " ";

            int startports = 0;
            for (int i = 0; i < hostinfo.Count(); i++)
            {
                if (string.Compare(hostinfo[i][1], "s") == 0 && string.Compare(hostinfo[i][4], "1") == 0)
                {
                    string filename = hostinfo[i][0] + "_mac.txt";
                    string[] lines = System.IO.File.ReadAllLines(@"output\mac\" + filename);


                    foreach (string line in lines)
                    {

                        if (line.Contains("Total Mac Addresses for this"))
                        {
                            startports = 0;
                        }

                        if (startports == 1)
                        {
                            if (line.Contains("CPU") == false && line.Contains("Po") == false && line.Contains("Vl") == false)
                            {
                                macadd = line.Substring(8, 14).Trim();


                                if (nclients > 0)
                                {

                                    if (searchArr(macadd, clients) == 0)
                                    {
                                        clients[nclients] = new Client();
                                        clients[nclients].ID = nclients;
                                        clients[nclients].MACadd = macadd;
                                        oui = GetOUIbymac(macadd);
                                        if (oui!=" ")
                                        {
                                            clients[nclients].guesstimac = oui;
                                        }


                                        nclients++;

                                        usage[nusage] = new Portuse();
                                        usage[nusage].ID = nusage;
                                        usage[nusage].ClientID = nclients - 1;
                                        usage[nusage].PortID = GetPortIDbySwitchnameandPortname(hostinfo[i][0], line.Substring(38, (line.Length - 38)));
                                        nusage++;


                                    }

                                }
                                else
                                {
                                    clients[nclients] = new Client();
                                    clients[nclients].ID = nclients;
                                    clients[nclients].MACadd = macadd;
                                    nclients++;
                                }
                            }

                        }

                        if (line.Contains(" -------- ") && (line.Contains(" ----------- ")))
                        {
                            startports = 1;
                        }
                    }
                }
            }
        }

        public void AddIPs()
        {
            int startports = 0;
            string ip = null;
            string mac = null;
            for (int i = 0; i < hostinfo.Count(); i++)
            {
                if (string.Compare(hostinfo[i][4], "1") == 0)
                {
                    string filename = hostinfo[i][0] + "_arp.txt";
                    string[] lines = System.IO.File.ReadAllLines(@"output\arp\" + filename);
                    

                    foreach (string line in lines)
                    {

                        if (line.Contains('#'))
                        {
                            startports = 0;
                        }
                        if (startports == 1)
                        {
                            ip = line.Substring(10, 16).Trim();
                            mac = line.Substring(38, 14).Trim();

                            if(mac.ToLower()!="incomplete")
                            {
                                //Console.WriteLine(ip + " ---- " + mac);
                                for(int x=0;x<nclients;x++)
                                {
                                    if(clients[x].MACadd==mac)
                                    {
                                        clients[x].IPadd = ip;
                                    }
                                }

                            }
                            


                        }
                        if (line.Contains("Protocol") && (line.Contains("Interface")))
                        {
                            startports = 1;
                        }

                    }
                }
            }
        }



        //Extra Functions

        public int searchArr(string needle, Client[] clients)
        {
            for (var i = 0; i < nclients; i++)
            {
                if (clients[i].MACadd == needle)
                {
                    return 1;
                }
            }
            return 0;
        }

        public int GetPortIDbySwitchnameandPortname(string switchname,string portname)
        {
            int switchID=GetSwitchIDbySwitchname(switchname);
            foreach(Port port in ports)
            {
                if(port.SwitchID==switchID && port.Portnr == portname)
                {
                    return port.ID;
                }
            }

           /* for(int i=0; i< nports;i++ )
            {
                if (ports[i].SwitchID == switchID && ports[i].Portnr == portname)
                {
                    return ports[i].ID;
                }
            }*/

            Console.WriteLine("bleh");
            return -1;
        }

        public int GetSwitchIDbySwitchname(string switchname)
        {
            foreach(Switch sw1tch in switches)
            {
                if(sw1tch.hostname==switchname)
                {
                    return sw1tch.ID;
                }
            }
            return -1;
        }

        public string GetOUIbymac(string macadd)
        {
            
            string ouipart = macadd.Substring(0, 7).ToLower();

            for(int i=0;i<OUI.Count();i++)
            {
                if(OUI[i][0]== ouipart)
                {
                    return OUI[i][1];
                }
            }

            return " ";
        }
    }
}
