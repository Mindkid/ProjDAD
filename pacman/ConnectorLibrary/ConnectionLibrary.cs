using System;

namespace ConnectorLibrary
{
    public abstract class ConnectionLibrary
    {
        public static int SERVER_PORT = 8086;
        public static string SERVER_NAME = "Server";

        public static String buildServerURL()
        {
            return "tcp://localhost:" + SERVER_PORT + "/" + SERVER_NAME;
        }
    }
}
