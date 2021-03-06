﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

using System.Web.Mvc;

namespace WebChat.Models
{

    /// <summary>
    /// metoder brukt i admin seksjonen av nettstedet.
    /// </summary>
    public class AdminRepository
    {
        private ChannelDataClassesDataContext db;
        public string username = "";
        public string userID = "ikke satt";
        public Guid UserGuid;
        List<Permitted_user> permittedUsers;

        /// <summary>
        /// Konstruktør
        /// </summary>
        /// <param name="_userName">tar inn brukernavn som parameter</param>
        public AdminRepository(string _userName)
        {
            db = new ChannelDataClassesDataContext();
            permittedUsers = (from users in db.Permitted_users
                              where users.Chatroom == ShowChatrooms().FirstOrDefault().Id
                              select users).ToList();

            username = _userName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Alle chatrooms</returns>
        public List<Chatroom> ShowChatrooms()
        {
            List<Chatroom> chatrooms = (from c in db.Chatrooms
                                        select c).ToList();
            return chatrooms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatroomName">navn på chatroom</param>
        /// <returns>returnerer om rommet er offentlig</returns>
        public string IsPublic(int chatroomId)
        {
            return (from p in db.Chatrooms
                    where p.Id == chatroomId
                    select p).FirstOrDefault().IsPublic;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usr">bruker id</param>
        /// <returns>chatroom basert på eier.</returns>
        public List<Chatroom> ShowChatrooms(Guid usr)
        {
            List<Chatroom> chatrooms = (from c in db.Chatrooms 
                                        where c.OwnerId == usr
                                        select c).ToList();
            return chatrooms;  
        }

        /// <summary>
        /// returnerer liste over tilatte brukere
        /// </summary>
        public List<Permitted_user> PermittedUsers
        {
            get
            {
                return permittedUsers;
            }
        }

        /// <summary>
        /// tilater en bruker å gå inn i ett chatroom
        /// </summary>
        /// <param name="chatroomName">navn på chatroom</param>
        public void SetPermittedUsers(int chatroomId)
        {
            permittedUsers = (from users in db.Permitted_users
                              where users.Chatroom == chatroomId
                            select users).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatroomName">navn på chatroom</param>
        /// <returns>Returnerer max antall brukere</returns>
        /*public int GetMaxUsers(int chatroomId)
        {
            return (from ch in db.Chatrooms
                    where ch.Id == chatroomId
                    select ch.MaximumUsers).FirstOrDefault();
        }*/

        /// <summary>
        /// returnerer liste med brukere
        /// </summary>
        public List<aspnet_User> ShowUsers
        {
            get
            {
                List<aspnet_User> usrs = (from u in db.aspnet_Users select u).ToList();
                return usrs;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_userName">navn på bruker</param>
        /// <returns>Brukerid basert på navn</returns>
        public Guid GetUserID(string _userName)
        {
            aspnet_User usr = (from u in db.aspnet_Users
                                where u.UserName == _userName
                                select u).FirstOrDefault();
            return usr.UserId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_userID">bruker id</param>
        /// <returns>Brukernavn basert på id</returns>
        public string GetUserName(Guid _userID)
        {
            aspnet_User usr = (from u in db.aspnet_Users
                               where u.UserId == _userID
                               select u).FirstOrDefault();
            return usr.UserName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returnerer liste tilatte brukernavn</returns>
        public List<string> getUserNamesPermitted()
        {
            List<string> output = new List<string>();
            for (int i = 0; i < permittedUsers.Count; i++ )
            {
                output.Add(GetUserName(permittedUsers.ElementAt(i).UserId));
            }
            return output;
        }
    }
}