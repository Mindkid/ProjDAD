using ConnectorLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using  System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace pacman
{
    public class ClientApp: MarshalByRefObject, IClientApp, IProcessToPCS
    {
        private ChatRoom chat;
        private List<IPacmanServer> servers;
        private Form1 form;

        //This are to be used in sendig the messages
        int serverKeyRequests;
        private KeyConfiguration.KEYS keyToSend;

        //This struct it's used to retrieve the values
        // sended by the servers to do the quorum
   
        private Dictionary<PacmanMove, int> movesQuorum;

        private Dictionary<String, int> setNameQuorum;
        int serverNameRequests;

        System.Timers.Timer aTimer = new System.Timers.Timer();
 
        private String pacmanName;
        private int round;
        private int previousRound;
        /*
         * This attribute saves the keys LIFO
         */
        private Stack<KeyConfiguration.KEYS> keyHistory;

        private Dictionary<int, Form1> gameHistory;

        public ClientApp(List<IPacmanServer> servers, Form1 form, String nickName, int roundTime ,List<string> plays)
        {
            RemotingServices.Marshal(this, nickName, typeof(ClientApp));
            round = 0;
            previousRound = 1;
            chat = new ChatRoom(form, nickName);
            this.servers = servers;
            serverKeyRequests = servers.Count;
            serverNameRequests = 0;
            this.form = form;
            keyHistory = new Stack<KeyConfiguration.KEYS>();
            gameHistory = new Dictionary<int, Form1>();

            movesQuorum = new Dictionary<PacmanMove, int>(new PacmanMove.EqualityComparer());
            setNameQuorum = new Dictionary<String, int>();

            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = roundTime;
            aTimer.Enabled = true;

            if (plays.Count != 0)
            {
                foreach (String play in plays)
                    addKey(KeyConfiguration.transformKey(play));
            }    

            int firstAttemped = 0;
            foreach (IPacmanServer server in servers)
            {
                addClientToServer(server, firstAttemped);
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
       
                sendKey();
                previousRound = round;
        }

        private void addClientToServer(IPacmanServer server, int attempted)
        {
          
            try
            {
                server.addClient(this);
            }
            catch(Exception)
            {
                if(attempted <= KeyConfiguration.MAX_ATTEMPTS)
                {
                    Thread.Sleep(ConnectionLibrary.INTERVAL_RESEND);
                    addClientToServer(server, attempted++);
                }
            }
            
        }

        public void addKey(KeyConfiguration.KEYS key)
        {
            keyHistory.Push(key);
        }

        private void sendKey()
        {
            int attempt = 0;
           
            foreach (IPacmanServer server in servers)
            {
                Thread thread = new Thread(() =>sendKeyToServer(server, keyToSend, pacmanName, attempt));
                thread.Start();
            }
            updateKeyToSend();
            
        }

        private void sendKeyToServer(IPacmanServer server, KeyConfiguration.KEYS key, String clientName, int attempt)
        {
            try
            {
                server.sendKey(keyToSend, pacmanName);
            }
            catch(Exception)
            {
                if(attempt <= KeyConfiguration.MAX_ATTEMPTS)
                {
                    Thread.Sleep(ConnectionLibrary.INTERVAL_RESEND);
                    sendKeyToServer(server, key, clientName, attempt++);
                }
            }
        }
        private void updateKeyToSend()
        {
            try
            {
                keyToSend = keyHistory.Pop();
            }
            catch(Exception)
            {
                keyToSend = KeyConfiguration.KEYS.NON_PRESSED;
            }
        }

        public void receiveKey(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves, String serverName)
        {
            Monitor.Enter(this);
            if (form.Enabled)
                receivedKey(pacmanMoves, serverName);
            Monitor.Exit(this);
        }

        private void receivedKey(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves, String serverName)
        {
            int counter = 1;
            Monitor.Enter(this);
            PacmanMove pacMoves = new PacmanMove(pacmanMoves, serverName);
            try
            { 
                movesQuorum.Add(pacMoves, counter);
            }
            catch(Exception e)
            {
                if(!e.Message.Equals("SERVER"))
                {
                    counter = movesQuorum[pacMoves];
                    counter++;
                    movesQuorum[pacMoves] = counter;
                }
            }
            
            if (counter >= (servers.Count / 2) + 1)
            {
                actualizeBoard(pacmanMoves, round);
                round++;
            }
            
            if (movesQuorum.Count == servers.Count)
                movesQuorum.Clear();
            Monitor.Exit(this);
        }

        private void actualizeBoard(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves, int round)
        {
            if (form.Created)
            {
                foreach (String s in pacmanMoves.Keys)
                    form.Invoke(form.movePacmanDel, new object[] { s, pacmanMoves[s] });
                gameHistory.Add(round, form);
            }
        }
        public ChatRoom GetChatRoom()
        {
            return chat;
        }

        public void setPacmanName(String pacname)
        {
            int counter = 1;             
            Monitor.Enter(this);
            try
            {
                setNameQuorum.Add(pacname, counter);
            }
            catch(Exception)
            {
                counter = setNameQuorum[pacname];
                setNameQuorum[pacname] = counter++;
            }
            if(counter >= (servers.Count / 2) + 1)
                pacmanName = pacname;

            serverNameRequests++;

            if (serverNameRequests == servers.Count)
            {
                setNameQuorum.Clear();
                if (form.Enabled)
                {
                    aTimer.Start();
                    round++;
                }
                    
            }
                
               
           
            Monitor.Exit(this);
            
        }
        public String getPacmanName()
        {
            return pacmanName;
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
            return pacmanName + " ok!";
        }

        public string getBoardState(int roundID)
        {
            String boardStatus = "";
            try
            {
                boardStatus = (String) gameHistory[roundID].Invoke(form.boardStatus);
            }
            catch(Exception)
            {
                boardStatus = "----- BOARD ERROR ----";
            }
            return boardStatus;
        }

        public void injectDelay(string destinationProcess, long delayTime)
        {
            throw new NotImplementedException();
        }
    }
}
