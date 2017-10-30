using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman
{
    [Serializable]
    class Message : IComparable
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

        public int CompareTo(object obj1, object obj2)
        {
            int result = 0;
            Message message1 = (Message)obj1;
            Message message2 = (Message)obj2;

            if (message1.getMessageID() > message2.getMessageID())
                result = 1;
            else
                result = -1;

            if (message1.getMessageID() == message2.getMessageID())
                result = message1.GetDateTime().CompareTo(message2.GetDateTime());
           
            return result;
        }
    }
}
