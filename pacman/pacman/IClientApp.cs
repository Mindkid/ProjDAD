using ConnectorLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace pacman
{
    public interface IClientApp
    { 
        void receiveKey(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves, String serverName);
        ChatRoom GetChatRoom();
        void setPacmanName(String pacname);

    }
}
