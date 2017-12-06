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
        public static int SIZE_ARGS_WITH_FILE = 7;
        public enum KEYS { UP_KEY, DOWN_KEY, LEFT_KEY, RIGHT_KEY, NON_PRESSED };

        public static KEYS transformKey(string key)
        {
            KEYS actualKey;
            switch (key)
            {
                case "RIGHT":
                    actualKey = KEYS.RIGHT_KEY;
                    break;
                case "LEFT":
                    actualKey = KEYS.LEFT_KEY;
                    break;
                case "UP":
                    actualKey = KEYS.UP_KEY;
                    break;
                case "DOWN":
                    actualKey = KEYS.DOWN_KEY;
                    break;
                default:
                    actualKey = KEYS.NON_PRESSED;
                    break;
            }
            return actualKey;
        }
    }
}
