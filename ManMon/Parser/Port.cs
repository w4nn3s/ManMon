﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManMon.Parser
{
    class Port
    {
        public int ID = 0;
        public int SwitchID = 0;
        public string Portnr = null;
        public string PortDesc = null;
        public string Status = null;
        public string Vlan = null;
        public int uplink = 0; //uplinks are tagged 1;
    }
}
