﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pacman;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Net.Sockets;

namespace PacmanServer
{
    class Server : MarshalByRefObject, IPacmanServer
    {
        private static int NUMBER_OF_PLAYERS = 2;
        private List<ChatRoom> chatRooms;

        public Server()
        {
            chatRooms = new List<ChatRoom>();
        }

        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public void addChatRoom(ChatRoom chat)
        {
            chatRooms.Add(chat);
            if(chatRooms.Count >= NUMBER_OF_PLAYERS)
            {
                Thread thread = new Thread(() => startChating());
                thread.Start();
            }
        }

        public void startChating()
        {
            foreach (ChatRoom c in chatRooms)
            {
                try
                {
                    c.setClientChatRooms(chatRooms);
                }
                catch(SocketException exc)
                {
                    //Do nothing ou fazer retry a Definir 
                }
            }
            chatRooms.Clear();
        }
    }
}