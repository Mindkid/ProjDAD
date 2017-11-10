using ConnectorLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pacman
{
    public class ClientApp: MarshalByRefObject, IClientApp
    {
        private ChatRoom chat;
        private IPacmanServer server;
        private Form1 form;

        private String pacmanName;
        /*
         * This attribute saves the keys LIFO
         */
        private Stack<KeyConfiguration.KEYS> keyHistory;

        private Dictionary<int, Form1> gameHistory;

        public ClientApp(IPacmanServer server, Form1 form, String nickName)
        {
            chat = new ChatRoom(server, form, nickName);
            this.server = server;
            this.form = form;
            keyHistory = new Stack<KeyConfiguration.KEYS>();
            gameHistory = new Dictionary<int, Form1>();
            server.addClient(this);
        }

        public void addKey(KeyConfiguration.KEYS key)
        {
            keyHistory.Push(key);  
        }

        public KeyConfiguration.KEYS sendKey()
        {
            KeyConfiguration.KEYS key;
            try
            {
                key = keyHistory.Pop();
            }
            catch(Exception)
            {
                key = KeyConfiguration.KEYS.NON_PRESSED;
            }
            return key;
        }

        public void receiveKey(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves, int round)
        {
            Thread thread = new Thread(() => actualizeBoard(pacmanMoves, round));
            thread.Start();
        }

        private void actualizeBoard(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves, int round)
        {
            foreach (String s in pacmanMoves.Keys)
                form.Invoke(form.movePacmanDel, new object[] { s, pacmanMoves[s] });
            gameHistory.Add(round, form);
        }
        public ChatRoom GetChatRoom()
        {
            return chat;
        }

        public void setPacmanName(String pacname)
        {
            pacmanName = pacname;
        }
        public String getPacmanName()
        {
            return pacmanName;
        }
    }
}
