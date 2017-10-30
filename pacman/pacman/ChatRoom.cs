using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pacman
{
    class ChatRoom : MarshalByRefObject
    {
        private List<Message> conversation;
        private List<ChatRoom> clientsChatRoom;
        private Form1 conversationForm;

        public ChatRoom(PacmanServer server, Form1 form)
        {
            conversation = new List<Message>();
            this.clientsChatRoom = server.getClientsChatRoom();
        }

        public void sendMessage(String message)
        {
            Thread thread = new Thread(() => broadCastMessage(message));
            thread.Start();
        }

        public void receiveMessage(Message message)
        {
            conversation.Add(message);
            conversation.Sort();
            updateClientConversation(conversation);
        }

        private void broadCastMessage(String stringMessage)
        {
            Message message = new Message(stringMessage, conversation.Count + 1);

            foreach (ChatRoom chat in clientsChatRoom)
                chat.receiveMessage(message);
        }
        
        private void updateClientConversation(List<Message> conversation)
        {
            conversationForm.Invoke(conversationForm.refreshConversation, conversation)
        }

    }
}
