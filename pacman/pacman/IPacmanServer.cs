using ConnectorLibrary;
using System;

namespace pacman
{
    public interface IPacmanServer
    {
        void addClient(IClientApp clientApp);
        void ping();
        void sendKey(KeyConfiguration.KEYS key, String player);
    }
}
