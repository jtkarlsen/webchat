using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebChat.Models
{
    public class ChannelIndexViewModel
    {
        private ChannelRepository channelRep;
        private UserRepository userRep;
        public Chatroom room { get; set; }
        public List<Chatroom> rooms { get; set; }
        public List<Chatroom> yourRooms { get; set; }
        public List<Permitted_user> users { get; set; }
        public Permitted_user user { get; set; }

        /// <summary>
        /// Metoden henter ut lister med rom og brukere som sendes til view
        /// </summary>
        /// <param name="userId">Brukers id</param>
        public ChannelIndexViewModel(Guid userId) 
        {
            userRep = new UserRepository();
            channelRep = new ChannelRepository();
            rooms = channelRep.showNotYourChatrooms(userId).ToList();
            yourRooms = channelRep.showYourChatrooms(userId).ToList();
            users = userRep.showYourPermittedRooms(userId).ToList();
            
        }
    }
}