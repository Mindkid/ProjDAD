using ConnectorLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster
{
    class Program
    {
       
        static void Main(string[] args)
        {
            try
            {
                String filename = args[0];
                String[] lines;
                lines = System.IO.File.ReadAllLines(filename);
                treatFile(lines);
            }
            catch(Exception)
            {
                Console.WriteLine("error open the file...");
                //DO_NOTHING OR DEFINE
            }
            Console.WriteLine("ENTER INSTRUCTION:");
            String line = Console.ReadLine();
            while(line.ToLower() != "exit")
            {
                doInstruction(line);
                Console.WriteLine("ENTER INSTRUCTION:");
                line = Console.ReadLine();
            }
        }

        static void treatFile(String[] fileLines)
        {
            foreach (String line in fileLines)
                doInstruction(line);
        }

        static void doInstruction(String line)
        {
            String[] arguments = line.Split(' ');
            try
            {
                switch (arguments[0].ToLower())
                {
                    /*
                     *                  Structure of the command
                     * StartClient PID PCS_URL CLIENT_URL MSEC_PER_ROUND NUM_PLAYERS [filename]
                     *    [0]      [1]   [2]     [3]            [4]         [5]         [6]
                     */
                    case "startclient":
                        if (arguments.Length == KeyConfiguration.SIZE_ARGS_WITH_FILE)
                            getIPCS().creatClientNode(arguments[1], arguments[3], arguments[5], arguments[6]);
                        else
                            getIPCS().creatClientNode(arguments[1], arguments[3], arguments[5]);
                        break;
                    /*
                     * StartServer PID PCS_URL SERVER_URL MSEC_PER_ROUND NUM_PLAYERS 
                     *    [0]      [1]   [2]      [3]            [4]         [5]    
                     */
                    case "startserver":
                        getIPCS().creatServerNode(arguments[1], arguments[3], arguments[4], arguments[5]);
                        break;
                    /*
                     * GlobalStatus PCS_URL
                     *     [0]        [1]  
                     */
                    case "globalstatus":
                        getIPCS().globalStatus();
                        break;
                    /*
                     * Kill PID PCS_URL
                     * [0]  [1]  [2]
                     */
                    case "kill":
                        getIPCS().killProcess(arguments[1]);
                        break;
                    /*
                     * Freeze PID PCS_URL
                     *   [0]  [1]   [2]
                     */
                    case "freeze":
                        getIPCS().freezeProcess(arguments[1]);
                        break;
                    /*
                    * Unfreeze PID PCS_URL
                    *   [0]  [1]   [2]
                    */
                    case "unfreeze":
                        getIPCS().unFreezeProcess(arguments[1]);
                        break;
                    /*
                     * InjectDelay src_PID dest_PID PCS_URL
                     *    [0]        [1]     [2]     [3]
                     */
                    case "injectdelay":
                        getIPCS().injectDelay(arguments[1], arguments[2]);
                        break;
                    /*
                     * LocalState  PID round_ID PCS_URL
                     *    [0]      [1]    [2]     [3]
                     */
                    case "localstate":
                        getIPCS().localState(arguments[1], arguments[2]);
                        break;
                    /*
                     * Wait MILLI_SECONDS
                     *  [0]     [1]
                     */
                    case "wait":
                        System.Threading.Thread.Sleep(int.Parse(arguments[1]));
                        break;
                    default:
                        Console.WriteLine("Command not found...");
                        Console.WriteLine("Please see section 5 of: https://fenix.tecnico.ulisboa.pt/downloadFile/845043405456678/DAD-Project-1718.pdf");
                        break;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static IPCS getIPCS()
        {
            IPCS ipcs = null;
            try
            {
                ipcs = (IPCS)Activator.GetObject(typeof(IPCS), ConnectionLibrary.buildPCSURL());
            }
            catch(Exception)
            {
                Console.WriteLine("Couldn't find IPS.");
            }

            return ipcs;
        }
    }
}
