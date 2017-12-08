using ConnectorLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using  System.Linq;
using System.Threading.Tasks;

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

        private String pacmanName;
        private int round;
        /*
         * This attribute saves the keys LIFO
         */
        private Stack<KeyConfiguration.KEYS> keyHistory;

        private Dictionary<int, Form1> gameHistory;

        public ClientApp(List<IPacmanServer> servers, Form1 form, String nickName, List<string> plays)
        {
            RemotingServices.Marshal(this, nickName, typeof(ClientApp));
            round = 0;
            chat = new ChatRoom(form, nickName);
            this.servers = servers;
            serverKeyRequests = servers.Count;

            this.form = form;
            keyHistory = new Stack<KeyConfiguration.KEYS>();
            gameHistory = new Dictionary<int, Form1>();

            movesQuorum = new Dictionary<PacmanMove, int>(new PacmanMove.EqualityComparer());
            setNameQuorum = new Dictionary<String, int>();

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

        public KeyConfiguration.KEYS sendKey()
        {
            Monitor.Enter(this);
            if (serverKeyRequests == servers.Count)
            {
                updateKeyToSend();
                serverKeyRequests = 1;
            }
            serverKeyRequests++;
            
            Monitor.Exit(this);

            return keyToSend;
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

            int counter = 1;
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
                round++;
                actualizeBoard(pacmanMoves, round);
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
            if(counter >= (servers.Count / 2) +1)
                pacmanName = pacname;

            if (counter == servers.Count)
                setNameQuorum.Clear();
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
