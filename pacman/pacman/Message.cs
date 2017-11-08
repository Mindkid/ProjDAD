using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman
{
    [Serializable]
    public class Message : IComparable
    {
        private String message;
        private String nickname;
        private int message_ID;
        private DateTime dateTime;

        public Message(String message, String nickname,  int message_ID) 
        {
            this.message = message;
            this.nickname = nickname;
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

        public int CompareTo(object obj)
        {
            int result;
            Message message = (Message)obj;

            if (this.getMessageID() > message.getMessageID())
                result = 1;
            else
                result = -1;

            if (this.getMessageID() == message.getMessageID())
                result = this.GetDateTime().CompareTo(message.GetDateTime());

            return result;
        }

        public String outputMessage()
        {
            return nickname + ": " + message + "\r\n";
        }
    }
}
