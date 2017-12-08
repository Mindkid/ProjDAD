using ConnectorLibrary;

namespace pacman
{
    public interface IPacmanServer
    {
        void addClient(IClientApp clientApp);
        void ping();
        //void sendKey(KeyConfiguration.KEYS key);
    }
}
