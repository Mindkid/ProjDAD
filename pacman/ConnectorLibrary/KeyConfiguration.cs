using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectorLibrary
{
    public abstract class KeyConfiguration
    {
        public static int NUMBER_OF_PLAYERS = 2;
        public enum KEYS { UP_KEY, DOWN_KEY, LEFT_KEY, RIGHT_KEY, NON_PRESSED };
    }
}
