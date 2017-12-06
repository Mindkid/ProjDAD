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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();

            String port = args[0].Split(':')[1].Split('/')[0];
            props["port"] = int.Parse(port);
            String file = args[0].Split(' ').Last();
            List<string> plays= new List<string>();
            if (file.Contains(':'))
            {
                file = null;
            }
            if(file != null)
            {
                using (var reader = new StreamReader(@file))
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
            getServers(servers, args);

            Application.Run(new Form1(server, KeyConfiguration.NUMBER_OF_PLAYERS));
        }

        static void getServer(String url)
        {  
            return (IPacmanServer)Activator.GetObject(typeof(IPacmanServer), url);
        }
    }
}
