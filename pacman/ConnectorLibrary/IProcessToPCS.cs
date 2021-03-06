﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorLibrary
{
    public interface IProcessToPCS
    { 
        void freezeProcess();
        
        void unFreezeProcess();

        /*
         * Request a snapshot, the token it's represented
         * with a random int for more detail see:
         * https://en.wikipedia.org/wiki/Chandy-Lamport_algorithm
         */
        String takeSnapshot(int randomToken);

        String getBoardState(int roundID);

        /*
         * This process injects a delay in the channel
         * between the target process and the destination 
         */ 
        void injectDelay(String destinationProcess, long delayTime);

    }
}
