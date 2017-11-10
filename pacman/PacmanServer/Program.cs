using ConnectorLibrary;
using pacman;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace PacmanServer
{
    class Program
    {
        private static int ROUND_TIME = 1000;

        static void Main(String[] args)
        {
            
            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            props["port"] = ConnectionLibrary.SERVER_PORT;
            TcpChannel channel = new TcpChannel(props, null, provider);
            ChannelServices.RegisterChannel(channel, false);

            Form1 form = new Form1(KeyConfiguration.NUMBER_OF_PLAYERS, ROUND_TIME);

            Server server = new Server(form, ROUND_TIME);
            RemotingServices.Marshal(server, ConnectionLibrary.SERVER_NAME, typeof(Server));
            System.Console.WriteLine("Enter instruction:");
            String instruction = System.Console.ReadLine().ToLower();

            while (!instruction.Equals("exit"))
            {
                switch (instruction)
                {
                    case "list":
                        List<ChatRoom> usernames = server.getChatRooms();
                        System.Console.WriteLine("This are the numeber of clients " + usernames.Count);
                        foreach (ChatRoom c in usernames)
                            System.Console.WriteLine("This are the name of client: " + c.getNickName());

                        break;
                }
                System.Console.WriteLine("Enter instruction:");
                instruction = System.Console.ReadLine().ToLower();
            }
        }
    }
}
