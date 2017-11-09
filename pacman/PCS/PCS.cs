using PuppetMaster;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;


namespace PCS
{
    // CLASS NEED MONITORS? Ask Professor
    public class PCS : MarshalByRefObject , IPCS
    {
        private static long DELAY = 10000;
        
        private Dictionary<String, int> processIDs;
        private Dictionary<String, IProcessToPCS> processes;

        private List<int> randomTokens;
        private List<String> serversURL;

        /*
         * This are the paths to the given EXE files
         */ 
        private static String clientPath = "..\\..\\..\\..\\pacman\\pacman\\bin\\Release\\pacman.exe";
        private static String serverPath = "..\\..\\..\\..\\pacman\\PacmanServer\\bin\\Release\\PacmanServer.exe";


        public PCS() 
        {
            processIDs = new Dictionary<string, int>();
            processes = new Dictionary<string, IProcessToPCS>();
            randomTokens = new List<int>();
            serversURL = new List<String>();
        }

        /*
         * This method launchs a Client Node 
         */ 
        public void creatClientNode(String processID, String clientURL, String filename)
        {
            String arguments = serversURL.Count + " ";
            foreach (String url in serversURL)
                arguments += url + " ";
            arguments += filename;

            launchProcess(processID, clientURL, clientPath, arguments);

            Console.WriteLine("Client Node was launched..");
        }

        /*
        * This method launchs a Server Node 
        */
        public void creatServerNode(String processID, String serverURL, String roundTime, String maxPlayers)
        {
            String arguments = roundTime + " " + maxPlayers;

            launchProcess(processID, serverURL, serverPath, arguments);

            serversURL.Add(serverURL);
            Console.WriteLine("Server Node was launched..");
        }

        /*
         * This process launchs a process or a client or a server
         * and them connects the PCS with the gicen client or server
         */ 
        private void launchProcess(String processID, String url, String path, String arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(path);
            String port = url.Split(':')[1].Split('/')[0];
            startInfo.Arguments = port + " " + arguments;

            processIDs.Add(processID, Process.Start(startInfo).Id);

            //IProcessToPCS process = (IProcessToPCS)Activator.GetObject(typeof(IProcessToPCS), url);

            //processes.Add(processID, process);
        }

        /*
         * This method kills a process
         */
        public void killProcess(String processID)
        { 
            try
            {
                Process.GetProcessById(processIDs[processID]).Kill();
                Console.WriteLine(processID + ": was killed...");
            }
            catch(Exception e)
            {
                Console.WriteLine(processID + ": could not be killed...");
                processIDs.Remove(processID);
            }
            
        }

        public void freezeProcess(String processID)
        {
            try
            {
                processes[processID].freezeProcess();
            }
            catch(SocketException)
            {
                Console.WriteLine("Couldn't freeze process: " + processID); 
            }
        }

        public void unFreezeProcess(String processID)
        {
            try
            {
                processes[processID].unFreezeProcess();
            }
            catch(SocketException)
            {
                Console.WriteLine("Couldn't unfreeze process: " + processID);
            }
        }

        public String globalStatus()
        {
            String status = "";
            int token = generateRandomUniqueToken();
            foreach (IProcessToPCS iptpcs in processes.Values)
                status += iptpcs.takeSnapshot(token);
            return status;
        }

        public String localState(String processID, String roundID)
        {
            String state = "Process: " + processID + "\r\n";
            try
            {
                state += processes[processID].getBoardState(int.Parse(roundID));
                state += "\r\n";
            }
            catch(SocketException)
            {
                state += "----PROCESS HAS CRASHED---- \r\n";
            }

            return state;
        }

        public void injectDelay(String srcPID, String destPID)
        {
            processes[srcPID].injectDelay(destPID, DELAY);
        }

        private int generateRandomUniqueToken()
        {
            Random generator = new Random();
            int randomToken = generator.Next();
            while (randomTokens.Contains(randomToken))
                randomToken = generator.Next();
            randomTokens.Add(randomToken);
  
            return randomToken;
        }

    }
}
