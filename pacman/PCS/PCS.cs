using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCS
{
    public class PCS : MarshalByRefObject
    {
        private Dictionary<String, int> processIDs;
        private static String clientPath = "..\\..\\..\\..\\pacman\\pacman\\bin\\Release\\pacman.exe";
        private static String serverPath = "..\\..\\..\\..\\pacman\\PacmanServer\\bin\\Release\\PacmanServer.exe";


        public PCS()
        {
            processIDs = new Dictionary<string, int>();
        }

        /*
         * This method launchs a Client Node 
         */ 
        public void creatClientNode(String processID, String clientURL, String filename)
        {
          
            String arguments = " " + filename;
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
            Console.WriteLine("Server Node was launched..");
        }


        private void launchProcess(String processID, String url, String path, String arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(path);
            String port = url.Split(':')[1].Split('/')[0];
            startInfo.Arguments = port + " " + arguments;

            processIDs.Add(processID, Process.Start(startInfo).Id);
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
            }
        }

    }
}
