using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebChat.Models
{
    /// <summary>
    /// View Model for admin siden av nettstedet
    /// </summary>
    public class AdminViewModel
    {
        private AdminRepository rep;
        public Chatroom room { get; set; }
        public SelectList chatrooms { get; private set; }
        public SelectList users { get; private set; }
        public SelectList permittedUsers { get; private set; }

        private string username;
        public int chatroomId { get; set; }
        public bool isPublic { get; set; }
        public int maxUsers { get; set; }

        private ChannelDataClassesDataContext db;

        /// <summary>
        /// Konstruktør som gir oss tilgang på flere lister
        /// i våre admin views.
        /// </summary>
        /// <param name="c">Ett chatroom</param>
        /// <param name="_username">brukernavn til den som er pålogget</param>
        public AdminViewModel(Chatroom c, string _username)
        {
            username = _username;
            db = new ChannelDataClassesDataContext();
            rep = new AdminRepository(username);
            room = c;
            users = new SelectList(rep.ShowUsers, "UserId", "UserName");
            chatrooms = new SelectList(rep.ShowChatrooms(), "OwnerId", "Name");
            UpdatePermittedUsersList(c.Id);
            //permittedUsers = new SelectList(rep.PermittedUsers, "Chatroom", "UserId");
            permittedUsers = new SelectList(rep.getUserNamesPermitted());
            
            if (rep.IsPublic(chatroomId).ToLower() == "yes")
                isPublic = true;
            else isPublic = false;

           // maxUsers = rep.GetMaxUsers(chatroomId);
        }

        /// <summary>
        /// Oppdaterer liste med tilatte brukere
        /// </summary>
        /// <param name="chatRoomName">Navn på chatroom som trenger brukere definert</param>
        private void UpdatePermittedUsersList(int chatRoomId)
        {
            rep.SetPermittedUsers(chatRoomId);
            permittedUsers = new SelectList(rep.PermittedUsers, "Chatroom", "UserId");
        }       
 
    }
}
