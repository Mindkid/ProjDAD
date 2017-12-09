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
            int roundTime = 0;
            String filename = null;
            String port = args[0].Split(':')[2].Split('/')[0];
            String nickname = args[0].Split(':')[2].Split('/')[1];
            List<string> plays = new List<string>();

            int numberURLs = int.Parse(args[1]);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            props["port"] = int.Parse(port);

            if (args.Length == numberURLs + 5)
            {
                filename = args[args.Length - 1];
                numberOfPlayers = int.Parse(args[args.Length - 2]);
                roundTime = int.Parse(args[args.Length - 3]);
            }
            else
            {
                numberOfPlayers = int.Parse(args[args.Length - 1]);
                roundTime = int.Parse(args[args.Length - 2]);
            }
            

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


            String[] serverUrls  = args.SubArray(2, numberURLs);

            foreach (String url in serverUrls)
               servers.Add((IPacmanServer)Activator.GetObject(typeof(IPacmanServer), url));    
            

            Application.Run(new Form1(servers, numberOfPlayers, plays, nickname, roundTime));
        }
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
