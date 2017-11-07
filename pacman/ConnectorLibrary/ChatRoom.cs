using ConnectorLibrary;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ConnectorLibrary
{
    public class ChatRoom : MarshalByRefObject
    {
        private List<Message> conversation;
        private List<ChatRoom> clientsChatRoom;
        private Form1 conversationForm;
        private IServer server;


        public ChatRoom(IServer server, Form1 form)
        {
            this.server = server;
            conversation = new List<Message>();
        }

        public void registerClient(String nickname, int port)
        {
            server.addClient(nickname, this);}

        public void sendMessage(String message)
        {
            getChatRooms(server.getClients());
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
            String messages = "";
            foreach (Message x in conversation)
                messages += x.getMessage() + "/r/n";

            conversationForm.Invoke(conversationForm.refreshConversation, messages);
        }

        private void getChatRooms(Dictionary<String, int> usernames)
        {
            foreach(int port in usernames.Values)
            {
                ChatRoom chatRoom = (ChatRoom)Activator.GetObject(typeof(ChatRoom), ConnectionLibrary.buildChatRoomURL(port));
                clientsChatRoom.Add(chatRoom);
            }
        }

    }
}
