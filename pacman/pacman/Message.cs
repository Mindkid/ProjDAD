using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman
{
    [Serializable]
    class Message 
    {
        private String message;
        private int message_ID;
        private DateTime dateTime;

        public Message(String message, int message_ID)
        {
            this.message = message;
            this.message_ID = message_ID;
            this.dateTime = new DateTime();
        }

        public String getMessage()
        {
            return message;
        }

        public int getMessageID()
        {
            return message_ID;
        }

        public DateTime GetDateTime()
        {
            return dateTime;
        }
    }
}
