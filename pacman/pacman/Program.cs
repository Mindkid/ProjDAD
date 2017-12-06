using ConnectorLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace pacman {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] args) {
            int numberOfPlayers = 0;
            String filename = null;
            String port = args[0].Split(':')[2].Split('/')[0];
            String nickname = args[0].Split(':')[2].Split('/')[1];
            List<string> plays = new List<string>();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            props["port"] = int.Parse(port);

            if (args.Length == KeyConfiguration.SIZE_ARGS_WITH_FILE)
            {
                filename = args[args.Length - 1];
                numberOfPlayers = int.Parse(args[args.Length - 2]);
            }
            else
                numberOfPlayers = int.Parse(args[args.Length - 1]);
            if (filename != null)
            {
                using (var reader = new StreamReader(@filename))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        plays.Add(values[1]);
                    }
                }
            }

            TcpChannel channel = new TcpChannel(props, null, provider);
            ChannelServices.RegisterChannel(channel, false);

            List<IPacmanServer> servers = new List<IPacmanServer>();

            int numberURLs = int.Parse(args[1]);
            getServers(servers, args.SubArray(2, numberURLs));

            Application.Run(new Form1(servers, numberOfPlayers, plays, nickname));
        }

        static void getServers(List<IPacmanServer> servers, String[] urlArray)
        {  
            foreach(String url in urlArray)
            {
                try
                {
                    servers.Add((IPacmanServer)Activator.GetObject(typeof(IPacmanServer), url));
                }
                catch(Exception)
                {
                    //DO NOTHING OR REDEFINE
                }
            }
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
