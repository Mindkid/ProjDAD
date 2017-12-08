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

        private int numerOfPlayers;
        private String serverName;

        private int roundID;
        private int roundTime;
        public Server(Form1 form, int roundTime, int numerOfPlayers, String serverName)
        {
            roundID = 0;
            this.form = form;
            this.roundTime = roundTime;
            this.numerOfPlayers = numerOfPlayers;
            this.serverName = serverName;
            clients = new Dictionary<IClientApp, string>();
            chatRooms = new List<ChatRoom>();
            gameHistory = new Dictionary<int, Form1>();
            requestClientInput = new System.Timers.Timer(roundTime);
            requestClientInput.Elapsed += RequestClientInput_Elapsed;

            Console.WriteLine(" ------ " + serverName +" STARTED -----");
        }

        private void RequestClientInput_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dictionary<String, KeyConfiguration.KEYS> pacmanMoves = new Dictionary<String, KeyConfiguration.KEYS>();

            KeyConfiguration.KEYS key;
            String playerID;
            foreach (IClientApp client in clients.Keys)
            {
                key = client.sendKey();
                playerID = clients[client];
               // Console.WriteLine("Recieved: " + key + " From: " + playerID);
                pacmanMoves.Add(playerID,key);

            }
            sendMovesToClients(pacmanMoves);

            gameHistory.Add(roundID, form);
        }

        public void addClient(IClientApp clientApp)
        {
            Monitor.Enter(this);
            chatRooms.Add(clientApp.GetChatRoom());

            clients.Add(clientApp, "pacman" + clients.Count);

            if (clients.Count == numerOfPlayers)
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

        private void sendMovesToClients(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves )
        {
            int firstAttempt = 0;
            foreach (IClientApp client in clients.Keys)
            {
                sendMoveToClient(pacmanMoves, client, serverName, firstAttempt);
            }       
        }

        private void sendMoveToClient(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves, IClientApp client, String serverName, int attempt)
        {
            try
            {
                client.receiveKey(pacmanMoves, serverName);
            }
            catch(SocketException exc)
            {
                if(attempt <= KeyConfiguration.MAX_ATTEMPTS)
                {
                    Thread.Sleep(ConnectionLibrary.INTERVAL_RESEND);
                    sendMoveToClient(pacmanMoves, client, serverName, attempt++);
                }
                else
                    Console.WriteLine(exc.Message);
            }
        
        }
        public List<ChatRoom> getChatRooms()
        {
            return chatRooms;
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

                boardThread.Join();
                chatThread.Join();
            }
            requestClientInput.Start();
        }

        private void startBoard(IClientApp client, int attempt)
        {
            try
            {
                client.setPacmanName(clients[client]);
            }
            catch(SocketException exc)
            {
                if (attempt <= KeyConfiguration.MAX_ATTEMPTS)
                {
                    Thread.Sleep(ConnectionLibrary.INTERVAL_RESEND);
                    startBoard(client, attempt++);
                }
                else
                    Console.WriteLine(exc.Message);
            }
        
        }

        private void startChat(ChatRoom chat, int attempt)
        {
            try
            {
                chat.setClientChatRooms(chatRooms);
            }   
            catch (SocketException exc)
            {
                if (attempt <= KeyConfiguration.MAX_ATTEMPTS)
                {
                    Thread.Sleep(ConnectionLibrary.INTERVAL_RESEND);
                    startChat(chat, attempt++);
                }
                else
                    Console.WriteLine(exc.Message);
            }
        }

        public void ping()
        {
            // FAZER NADA 
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
