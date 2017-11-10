using ConnectorLibrary;

namespace pacman
{
    public interface IPacmanServer
    {
        void addClient(IClientApp clientApp);
        //void sendKey(KeyConfiguration.KEYS key);
    }
}
