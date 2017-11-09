﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster
{
    public interface IPCS
    {
        void creatClientNode(String processID, String clientURL, String filename);
        void creatServerNode(String processID, String serverURL, String roundTime, String maxPlayers);
        void killProcess(String processID);
        void freezeProcess(String processID);
        void unFreezeProcess(String processID);
        String globalStatus();
        String localState(String processID, String roundID);
        void injectDelay(String srcPID, String destPID);
    }
}