using ConnectorLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace pacman
{
    public interface IClientApp
    {
        KeyConfiguration.KEYS sendKey();
        void receiveKey(Dictionary<String, KeyConfiguration.KEYS> pacmanMoves);
        ChatRoom GetChatRoom();
        void setPacmanName(String pacname);

    }
}
