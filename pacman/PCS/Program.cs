using ConnectorLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace PCS
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            props["port"] = ConnectionLibrary.SERVER_PORT;
            TcpChannel channel = new TcpChannel(props, null, provider);
            ChannelServices.RegisterChannel(channel, false);

            PCS pcs = new PCS();
            RemotingServices.Marshal(pcs, ConnectionLibrary.SERVER_NAME, typeof(PCS));

        }
    }
}
