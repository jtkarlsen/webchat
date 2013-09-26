using System;
using System.Linq;
using System.Collections.Generic;
using WebChat.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebChat.Models 
{
    /// <summary>
    /// Chatrommene sine metoder
    /// </summary>
    public class ChannelRepository
    {
        private ChannelDataClassesDataContext db;

        /// <summary>
        /// Konstruktør
        /// </summary>
        public ChannelRepository()
        {
            db = new ChannelDataClassesDataContext();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Alle chatrooms</returns>
        public IQueryable<Chatroom> showAllChatRooms()
        {
            return db.Chatrooms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">navn på chatroom</param>
        /// <returns>Returnerer ett enkelt room</returns>
        public Chatroom showChatroom(int id)
        {
            Chatroom room = (from o in db.Chatrooms
                             where o.Id == id
                             select o).FirstOrDefault();
            return room;
            
        }

        /// <summary>
        /// Legger ett rom til databasen
        /// </summary>
        /// <param name="newChatRoom">ett rom som skal legges i databasen</param>
        public void addChannel(Chatroom newChatRoom) 
        {
            db.Chatrooms.InsertOnSubmit(newChatRoom);
            db.SubmitChanges();
        }

        /// <summary>
        /// Oppdaterer endringer i ett chatroom
        /// </summary>
        /// <param name="room">ett rom som skal endres</param>
        public void updateChannel(Chatroom newroom)
        {
            db.SubmitChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId">bruker id</param>
        /// <returns>Rom som eies av en bruker</returns>
        public List<Chatroom> showYourChatrooms(Guid userId)
        {
            List<Chatroom> room = (from u in db.Chatrooms
                                   where u.OwnerId == userId
                                   select u).ToList();
            return room;
        }

        /// <summary>
        /// Her leter han etter alle rom der du ikke er eier i
        /// </summary>
        /// <param name="userId">bruker id</param>
        /// <returns>Returnerer alle rom du ikke eier</returns>
        public List<Chatroom> showNotYourChatrooms(Guid userId)
        {
            List<Chatroom> room = (from u in db.Chatrooms
                                   where u.OwnerId != userId
                                   select u).ToList();
            return room;
        }

        /// <summary>
        /// Sletter ett chatroom
        /// </summary>
        /// <param name="room">ett chatroom klar til å slettes</param>
        public void deleteChatrooms(Chatroom room)
        {
            db.Chatrooms.DeleteOnSubmit(room);
            db.SubmitChanges();
        }
    }
}

