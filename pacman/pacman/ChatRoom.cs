using ConnectorLibrary;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace pacman
{
     public class ChatRoom : MarshalByRefObject
    {

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
       

        public ChatRoom(Form1 form, String nickname)
        {
            conversation = new List<Message>();
            clientsChatRooms = new List<ChatRoom>();
            conversationForm = form;
            this.nickname = nickname;
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
            broadCastMessage(message);
        }

        public void receiveMessage(Message message)
        {
            Monitor.Enter(this);
            if(!conversation.Contains(message))
            {
                conversation.Add(message);
                conversation.Sort();
                updateClientConversation();
            
                Thread thread = new Thread(() => broadCastMessage(message));
                thread.Start();
            }
            Monitor.Exit(this);
        }

        private void broadCastMessage(Message message)
        {
            int firstAttempt = 0;
            foreach (ChatRoom chat in clientsChatRooms)
            {
               Thread thread =  new Thread(() => sendMessage(chat, message, firstAttempt));
                thread.Start();
            }
            
        }

        private void sendMessage(ChatRoom chat, Message message, int attempt)
        {
            try
            {
                chat.receiveMessage(message);
            }
            catch (SocketException)
            {
                Thread.Sleep(ConnectionLibrary.INTERVAL_RESEND);

                if (attempt <= KeyConfiguration.MAX_ATTEMPTS)
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
