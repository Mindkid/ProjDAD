using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman
{
    [Serializable]
    public class Message : IEquatable<Message>, IComparable
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

        public Message()
        {
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
            if (this.getMessageID() == other.getMessageID())
                result = true;

            if (this.getMessageID() == other.getMessageID())
                if (this.GetDateTime().CompareTo(other.GetDateTime()) == 0)
                    result = true;
            return result;
        }

        public int CompareTo(object obj)
        {
            int result = 0;
            Message other = (Message)obj;

            if (this.getMessageID() <= other.getMessageID())
                result = -1;

            if (this.getMessageID() == other.getMessageID())
                result = this.GetDateTime().CompareTo(other.GetDateTime());
                 
            return result;
        }
    }
}
