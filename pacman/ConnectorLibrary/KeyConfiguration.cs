using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectorLibrary
{
    public abstract class KeyConfiguration
    {
        /*
      * This attribute sets the number of attempts
      * that the message it's resended
      */
        public static int MAX_ATTEMPTS = 3;

        public static int NUMBER_OF_PLAYERS = 2;
        public enum KEYS { UP_KEY, DOWN_KEY, LEFT_KEY, RIGHT_KEY, NON_PRESSED };
    }
}
