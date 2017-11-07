using ConnectorLibrary;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace PacmanServer
{
    class Program
    {
        static void Main()
        {
            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            props["port"] = ConnectionLibrary.SERVER_PORT;

            TcpChannel channel = new TcpChannel(props, null, provider);

            ChannelServices.RegisterChannel(channel, false);
            Server server = new Server();
            RemotingServices.Marshal(server, ConnectionLibrary.SERVER_NAME, typeof(Server));
            System.Console.WriteLine("Press any key to exit...");
            System.Console.ReadLine();

        }
    }
}
