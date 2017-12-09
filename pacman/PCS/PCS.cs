using ConnectorLibrary;
using PuppetMaster;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public void creatClientNode(String processID, String clientURL, String numberOfPlayers, String filename, String roundTime)
        {
            String arguments = serversURL.Count + " ";
            foreach (String url in serversURL)
                arguments += url + " ";

            arguments += roundTime + " ";
            arguments += numberOfPlayers + " "; 
            arguments += filename;

            launchProcess(processID, clientURL, clientPath, arguments);

            Console.WriteLine("Client Node was launched..");
        }

        public void creatClientNode(String processID, String clientURL, String numberOfPlayers, String roundTime)
        {
            String arguments = serversURL.Count + " ";
            foreach (String url in serversURL)
                arguments += url + " ";

            arguments += roundTime + " ";
            arguments += numberOfPlayers;
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
            Console.WriteLine("Server Node was launched with this args");
        }

        /*
         * This process launchs a process or a client or a server
         * and them connects the PCS with the gicen client or server
         */ 
        private void launchProcess(String processID, String url, String path, String arguments)
        {
            int firstAttemp = 0;

            ProcessStartInfo startInfo = new ProcessStartInfo(path);
            
            startInfo.Arguments = url + " " + arguments;
            processIDs.Add(processID, Process.Start(startInfo).Id);

            Console.WriteLine("Process launched with this args: " + startInfo.Arguments);
            Thread thread = new Thread(() => fetchIProcessToPCS(processID, url, firstAttemp));
            thread.Start();
        }

        private void fetchIProcessToPCS(String processID, String url, int attempt)
        {
            try
            {
                IProcessToPCS process = (IProcessToPCS) Activator.GetObject(typeof(IProcessToPCS), url);
                processes.Add(processID, process);
            }
            catch(Exception)
            {
                if (attempt <= KeyConfiguration.MAX_ATTEMPTS)
                {
                    Thread.Sleep(ConnectionLibrary.INTERVAL_RESEND);
                   fetchIProcessToPCS(processID, url, attempt++);
                }
                else
                    Console.WriteLine("Could connect to: " + processID);
            }
        }


        /*
         * This method kills a process
         */
        public void killProcess(String processID)
        { 
            try
            {
                Process.GetProcessById(processIDs[processID]).Kill();
                processes.Remove(processID);
                processIDs.Remove(processID);
                
                Console.WriteLine(processID + ": was killed...");
            }
            catch(Exception )
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

        //TO BE MODIFIED
        public String globalStatus()
        {
            String status = "";
            int token = generateRandomUniqueToken();
            foreach (IProcessToPCS iptpcs in processes.Values)
            {
                try
                {
                    status += iptpcs.takeSnapshot(token);
                }
                catch(Exception)
                {
                    status += processes.FirstOrDefault(x => x.Value == iptpcs).Key + "it\'s down!";
                }
            }
            return status;
        }

        public String listProcess()
        {
            String procNames = "";
            foreach(String process in processes.Keys)
            {
                procNames += process + "\r\n";
            }
            return procNames;
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
