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

        public PacmanMove(Dictionary<String, KeyConfiguration.KEYS> move)
        {
            this.move = move;
        }
        public Dictionary<String, KeyConfiguration.KEYS> getMove()
        {
            return move;
        }

        public class EqualityComparer : IEqualityComparer<PacmanMove>
        {

            public bool Equals(PacmanMove x, PacmanMove y)
            {
                bool result = true;

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
                return result;
            }

            public int GetHashCode(PacmanMove obj)
            {
                return 0;
            }

        }

    }
}
