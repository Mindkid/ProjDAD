using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace pacman
{
     public class ChatRoom : MarshalByRefObject
    {
        /*
         * This attribute sets the number of attempts
         * that the message it's resended
         */
        private static int MAX_ATTEMPTS = 3;

        /*
         * This attribute saves all the 
         * conversation of a cient 
         */
        private List<Message> conversation;


        /*
         * This attribute saves a reference
         * of the others clients
         */
        private List<ChatRoom> clientsChatRooms;
        private Form1 conversationForm;
        private String nickname;
       

        public ChatRoom(IPacmanServer server, Form1 form, String nickname)
        {
            conversation = new List<Message>();
            clientsChatRooms = new List<ChatRoom>();
            conversationForm = form;
            this.nickname = nickname;
            server.addChatRoom(this);
        }

        public String getNickName()
        {
            return nickname;
        }
        /*
         * This method it's called by the server
         * so that updates the clientsChatRooms
         */
        public void setClientChatRooms(List<ChatRoom> chatRooms)
        {
            this.clientsChatRooms = chatRooms;
        }

        public void sendMessage(String stringMessage)
        {
            Message message = new Message(stringMessage, nickname, conversation.Count + 1);
            Thread thread = new Thread(() => broadCastMessage(message));
            thread.Start();
        }

        public void receiveMessage(Message message)
        {
            conversation.Add(message);
            conversation.Sort();
            updateClientConversation();
        }

        private void broadCastMessage(Message message)
        {
            int firstAttempt = 0;
            foreach (ChatRoom chat in clientsChatRooms)
                sendMessage(chat, message, firstAttempt);
        }

        private void sendMessage(ChatRoom chat, Message message, int attempt)
        {
            try
            {
                chat.receiveMessage(message);
            }
            catch (SocketException exc)
            {
                if (attempt < MAX_ATTEMPTS)
                    sendMessage(chat, message, attempt++);
            }
        }

        private void updateClientConversation()
        {
            String messages = "";

            foreach (Message m in conversation)
                messages += m.outputMessage();

            conversationForm.Invoke(conversationForm.refreshConversation, messages);
        }

    }
}
