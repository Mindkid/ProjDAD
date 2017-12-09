using System;
using System.Collections.Generic;
using ConnectorLibrary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman
{
    public class PacmanMove
    {
        private Dictionary<String, KeyConfiguration.KEYS> move;
        private String serverName;

        public PacmanMove(Dictionary<String, KeyConfiguration.KEYS> move, String serverName)
        {
            this.serverName = serverName;
            this.move = move;
        }
        public Dictionary<String, KeyConfiguration.KEYS> getMove()
        {
            return move;
        }

        public String getServerName()
        {
            return serverName;
        }

        public class EqualityComparer : IEqualityComparer<PacmanMove>
        {

            public bool Equals(PacmanMove x, PacmanMove y)
            {
                bool result = true;
                if (x.getServerName() != y.getServerName())
                {
                    for (int i = 0; i < x.getMove().Count; i++)
                    {
                        if (x.getMove().ElementAt(i).Key == y.getMove().ElementAt(i).Key)
                        {
                            if (x.getMove().ElementAt(i).Value != y.getMove().ElementAt(i).Value)
                            {
                                result = false;
                                break;

                            }
                        }
                    }
                }
                else
                    throw new Exception("SERVER");
                

                return result;
            }

            public int GetHashCode(PacmanMove obj)
            {
                return 0;
            }

        }

    }
}
