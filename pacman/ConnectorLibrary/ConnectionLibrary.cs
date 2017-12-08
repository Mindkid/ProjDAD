using System;

namespace ConnectorLibrary
{
    public abstract class ConnectionLibrary
    {
        /*
         * This is an attribute that sets the
         * milliseconds for resending a packet
         */
        public static int INTERVAL_RESEND = 2000;

        public static int SERVER_PORT = 8086;
        public static String SERVER_NAME = "Server";

        public static int PCS_PORT = 11000;
        public static String PCS_NAME = "PCS";

        public static String buildServerURL()
        {
            return "tcp://localhost:" + SERVER_PORT + "/" + SERVER_NAME;
        }

        public static String buildPCSURL()
        {
            return "tcp://localhost:" + PCS_PORT + "/" + PCS_NAME;
        }

    }
}
