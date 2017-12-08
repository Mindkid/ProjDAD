using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman
{
    [Serializable]
    public class Message : IEquatable<Message>
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

        public String outputMessage()
        {
            return nickname + ": " + message + "\r\n";
        }

        public bool Equals(Message other)
        {
            bool result = false;
            if (this.getMessageID() >= other.getMessageID())
                result = true;

            if (this.getMessageID() == other.getMessageID())
                if (this.GetDateTime().CompareTo(other.GetDateTime()) == 1)
                    result = true;

            return result;
        }
    }
}
