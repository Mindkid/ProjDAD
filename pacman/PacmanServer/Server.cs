using System;
using System.Collections.Generic;
using pacman;
using System.Timers;
using System.Threading;
using System.Net.Sockets;
using ConnectorLibrary;

namespace PacmanServer
{
    class Server : MarshalByRefObject, IPacmanServer
    {
        private Dictionary<IClientApp, String> clients;
        private List<ChatRoom> chatRooms;
        private Dictionary<int, Form1> gameHistory;

        private Form1 form;
        private System.Timers.Timer requestClientInput;

        int roundID;
        public Server(Form1 form, int roundTime)
        {
            roundID = 0;
            this.form = form;
            clients = new Dictionary<IClientApp, string>();
            chatRooms = new List<ChatRoom>();
            gameHistory = new Dictionary<int, Form1>();

            requestClientInput = new System.Timers.Timer(roundTime);
            requestClientInput.Elapsed += RequestClientInput_Elapsed;
            requestClientInput.Start();
        }

        private void RequestClientInput_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dictionary<String, KeyConfiguration.KEYS> pacmanMoves = new Dictionary<String, KeyConfiguration.KEYS>();

            foreach (IClientApp client in clients.Keys)
                pacmanMoves.Add(clients[client], client.sendKey());

            //Console.WriteLine("This are the moves:  " + pacmanMoves.ToString());

            sendMovesToClients(pacmanMoves, roundID);

            gameHistory.Add(roundID, form);
            roundID++;
        }

        public void addClient(IClientApp clientApp)
        {
            Monitor.Enter(this);
            
            addChatRoom(clientApp.GetChatRoom());

            String playerID = "pacman" + clients.Count;

            clients.Add(clientApp, playerID);

            if (clients.Count >= KeyConfiguration.NUMBER_OF_PLAYERS)
            {
                Thread thread = new Thread(() => startGame());
                thread.Start();

            }
            Monitor.Exit(this);
        }

        private void actualizeBoard(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves)
        {
            foreach (String s in pacmanMoves.Keys)
                form.Invoke(form.movePacmanDel, new object[] { s, pacmanMoves[s] });
        }

        private void sendMovesToClients(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves, int round)
        {
            foreach (IClientApp client in clients.Keys)
                client.receiveKey(pacmanMoves, roundID);
        }

        public List<ChatRoom> getChatRooms()
        {
            return chatRooms;
        }

        private void addChatRoom(ChatRoom chat)
        {
            chatRooms.Add(chat);
        }

        private void startGame()
        {
            foreach (IClientApp c in clients.Keys)
                c.setPacmanName(clients[c]); 
            startChat();
        }

        private void startChat()
        {
            foreach (ChatRoom c in chatRooms)
            {
                try
                {
                    c.setClientChatRooms(chatRooms);
                }
                catch (SocketException)
                {
                    //DO NOTHING OR REDIFINE
                }
            }
            chatRooms.Clear();
        }
    }
}
