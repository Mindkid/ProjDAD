using System;
using System.Collections.Generic;

namespace ConnectorLibrary
{
    public interface IServer
    {
        void addClient(ChatRoom chatRoom);
        Dictionary<String, int> getClients();
    }
}
