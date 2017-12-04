using System;
using System.Collections.Generic;
using pacman;
using System.Timers;
using System.Threading;
using System.Net.Sockets;
using ConnectorLibrary;

namespace PacmanServer
{
    class Server : MarshalByRefObject, IPacmanServer, IProcessToPCS
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
          
        }

        private void RequestClientInput_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dictionary<String, KeyConfiguration.KEYS> pacmanMoves = new Dictionary<String, KeyConfiguration.KEYS>();

            foreach (IClientApp client in clients.Keys)
                pacmanMoves.Add(clients[client], client.sendKey());

            sendMovesToClients(pacmanMoves, roundID);

            gameHistory.Add(roundID, form);
            roundID++;
        }

        public void addClient(IClientApp clientApp, IProcessToPCS clientProcess)
        {
            Monitor.Enter(this);
            
            addChatRoom(clientApp.GetChatRoom());

            String playerID = "pacman" + clients.Count;

            clients.Add(clientApp, playerID);

            if (clients.Count == KeyConfiguration.NUMBER_OF_PLAYERS)
            {
                Thread thread = new Thread(() => startGame());
                thread.Start();
                requestClientInput.Start();
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
            int firstAttempt = 0;
            foreach (IClientApp client in clients.Keys)
            {
                Thread sendThread = new Thread(() => sendMoveToClient(pacmanMoves, round, client, firstAttempt));
                sendThread.Start();
            }
                
        }

        private void sendMoveToClient(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves, int round, IClientApp client, int attempt)
        {
            try
            {
                client.receiveKey(pacmanMoves, roundID);
            }
            catch(SocketException)
            {
                if(attempt <= KeyConfiguration.MAX_ATTEMPTS)
                {
                    Thread.Sleep(ConnectionLibrary.INTERVAL_RESEND);
                    sendMoveToClient(pacmanMoves, round, client, attempt++);
                }
            }
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
            int initialAttempt = 0;

            foreach (IClientApp c in clients.Keys)
            {
                Thread boardThread = new Thread(() => startBoard(c, initialAttempt));
                Thread chatThread = new Thread(() => startChat(c.GetChatRoom(), initialAttempt));

                boardThread.Start();
                chatThread.Start();
            }
        }

        private void startBoard(IClientApp client, int attempt)
        {
            try
            {
                client.setPacmanName(clients[client]);
            }
            catch(SocketException)
            {     
                if (attempt <= KeyConfiguration.MAX_ATTEMPTS)
                {
                    Thread.Sleep(ConnectionLibrary.INTERVAL_RESEND);
                    startBoard(client, attempt++);
                }
            }
        
        }

        private void startChat(ChatRoom chat, int attempt)
        {
            try
            {
                chat.setClientChatRooms(chatRooms);
            }   
            catch (SocketException)
            {
                if (attempt <= KeyConfiguration.MAX_ATTEMPTS)
                {
                    Thread.Sleep(ConnectionLibrary.INTERVAL_RESEND);
                    startChat(chat, attempt++);
                }
            }
        }

        public void freezeProcess()
        {
            Monitor.Enter(this);
        }

        public void unFreezeProcess()
        {
            Monitor.Exit(this);
        }

        public String takeSnapshot(int randomToken)
        {
            return "SERVER ok!\r\n";
        }

        public string getBoardState(int roundID)
        {
            String boardStatus = "";
            try
            {
                boardStatus = (String)gameHistory[roundID].Invoke(form.boardStatus);
            }
            catch (Exception)
            {
                boardStatus = "----- BOARD ERROR ----\r\n";
            }
            return boardStatus;
        }

        public void injectDelay(string destinationProcess, long delayTime)
        {
            throw new NotImplementedException();
        }
    }
}
