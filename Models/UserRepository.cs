using System;
using System.Linq;
using WebChat.Models;
using System.Collections.Generic;

namespace WebChat.Models
{
    /// <summary>
    /// Brukerens metoder
    /// </summary>
    public class UserRepository
    {
        private ChannelDataClassesDataContext db;

        /// <summary>
        /// Kontruktør
        /// </summary>
        public UserRepository() 
        {
            db = new ChannelDataClassesDataContext();
        }

        /// <summary>
        /// Søker i databasen å returnerer alle brukere
        /// </summary>
        /// <returns>alle brukere</returns>
        public IQueryable<aspnet_User> showAllUsers() 
        {
            return db.aspnet_Users;
        }
        public aspnet_User showUser(string id)
        {
            aspnet_User user = (from o in db.aspnet_Users
                                where o.UserName.Equals(id)
                                select o).FirstOrDefault();
            return user;
        }

        /// <summary>
        /// Oppdaterer en bruker
        /// </summary>
        /// <param name="user">en bruker</param>
        public void updateUser(aspnet_User user)
        {
            db.SubmitChanges();
        }

        /// <summary>
        /// Sletter en bruker
        /// </summary>
        /// <param name="user">en bruker</param>
        public void deleteUser(aspnet_User user)
        {
            db.aspnet_Users.DeleteOnSubmit(user);
            db.SubmitChanges();
        }

        /// <summary>
        /// Henter brukerid basert på navn
        /// </summary>
        /// <param name="name">navn på en bruker</param>
        /// <returns>en bruker id</returns>
        public Guid getUserId(string name)
        {
            aspnet_User user = (from o in db.aspnet_Users
                                where o.LoweredUserName == name
                                select o).FirstOrDefault();
            return user.UserId;
        }

        /// <summary>
        /// Henter ut en spesifikk bruker i Permitt_user tabellen
        /// </summary>
        /// <param name="userId">bruker id</param>
        /// <returns>en bruker fra Permitted_user</returns>
        public Permitted_user showPermittedUser(string userId)
        {
            Permitted_user user = (from u in db.Permitted_users
                                   where u.UserId == Guid.Parse(userId)
                                   select u).FirstOrDefault();

            return user;
        }

        /// <summary>
        /// Henter brukere fra brukertabellen
        /// </summary>
        /// <param name="userId">liste over Permitted_user</param>
        /// <returns>Returnerer brukere fra aspnet_User tabellen</returns>
        public List<aspnet_User> showCurrentUsers(List<Permitted_user> userId)
        {
            List<aspnet_User> users = new List<aspnet_User>();
            foreach (Permitted_user id in userId)
            {
                aspnet_User user = (from u in db.aspnet_Users
                                    where id.UserId == u.UserId
                                    select u).FirstOrDefault();
                users.Add(user);
            }
            return users;
        }

        /// <summary>
        /// Henter ut alle brukere som har tilgang til ett rom
        /// </summary>
        /// <param name="chatroom">rom navn</param>
        /// <returns>liste over tilatte brukere</returns>
        public List<Permitted_user> showUserWithChatroom(int chatroom)
        {
            List<Permitted_user> users = (from u in db.Permitted_users
                                         where u.Chatroom == chatroom
                                         select u).ToList();
            return users;
        }

        /// <summary>
        /// Henter alle rom du har tilgang på
        /// </summary>
        /// <param name="userId">bruker id</param>
        /// <returns></returns>
        public List<Permitted_user> showYourPermittedRooms(Guid userId)
        {
            List<Permitted_user> user = (from u in db.Permitted_users
                                         where u.UserId == userId
                                         select u).ToList();
            return user;
        }

        /// <summary>
        /// Henter alle brukere untatt de du har tilatt
        /// </summary>
        /// <param name="userId">liste med Permitted_user</param>
        /// <returns>liste fra aspnet_User tabellen</returns>
        public List<aspnet_User> showUsersNotInChatroom(List<Permitted_user> userId)
        {

            List<aspnet_User> users = showAllUsers().ToList();
            List<aspnet_User> tempUsers = showAllUsers().ToList();
            aspnet_User user = new aspnet_User();
            foreach (Permitted_user us in userId)
            {
                foreach (aspnet_User u in tempUsers)
                {
                    if (u.UserId == us.UserId)
                        users.Remove(u);
                }
            }
            return users;
        }

        /// <summary>
        /// Legger til tilatte brukere
        /// </summary>
        /// <param name="user">en tilatt bruker</param>
        public void addPermittedUsers(Permitted_user user)
        {
            db.Permitted_users.InsertOnSubmit(user);
            db.SubmitChanges();
        }

        /// <summary>
        /// Fjerner bruker fra ditt rom
        /// </summary>
        /// <param name="user">en tilatt bruker</param>
        public void deletePermittedUsers(Permitted_user user)
        {
            db.Permitted_users.DeleteOnSubmit(user);
            db.SubmitChanges();
        }

        /// <summary>
        /// Henter ut brukernavn basert på id
        /// </summary>
        /// <param name="userId">bruker id</param>
        /// <returns>brukernavn</returns>
        public string getUserName(Guid userId)
        {
            string username = (from u in db.aspnet_Users
                               where u.UserId == userId
                               select u.LoweredUserName).FirstOrDefault();

            return username;
        }

        /// <summary>
        /// Henter eier av ett chatroom
        /// </summary>
        /// <param name="room">ett chatroom</param>
        /// <returns>aspnet_User</returns>
        public aspnet_User getOwner(Chatroom room)
        {
            aspnet_User user = (from u in db.aspnet_Users
                                where u.UserId == room.OwnerId
                                select u).FirstOrDefault();
            return user;
        }
        #region metoder til bruk i Oblig 2
        /// <summary>
        /// Henter liste over alle brukere som ikke er bannet fra rommet
        /// </summary>
        /// <param name="userId">liste med bannede brukere</param>
        /// <returns>liste med brukere som ikke er bannet</returns>
        public List<aspnet_User> showUnBannedUsers(List<Banned_user> userId)
        {

            List<aspnet_User> users = showAllUsers().ToList();
            List<aspnet_User> tempUsers = showAllUsers().ToList();
            aspnet_User user = new aspnet_User();
            foreach (Banned_user us in userId)
            {
                foreach (aspnet_User u in tempUsers)
                {
                    if (u.UserId == us.UserId)
                        users.Remove(u);
                }
            }
            return users;
        }

        /// <summary>
        /// Henter liste over brukere som er bannet i ett chatroom
        /// </summary>
        /// <param name="userId">liste med bannede brukere</param>
        /// <returns>liste med brukere</returns>
        public List<aspnet_User> showBannedUsers(List<Banned_user> userId)
        {
            List<aspnet_User> users = new List<aspnet_User>();
            foreach (Banned_user id in userId)
            {
                aspnet_User user = (from u in db.aspnet_Users
                                    where id.UserId == u.UserId
                                    select u).FirstOrDefault();
                users.Add(user);
            }
            return users;
        }

        /// <summary>
        /// Henter liste over Banned_user objekter
        /// </summary>
        /// <param name="chatroom">navn på ett chatroom</param>
        /// <returns>Banned_user objekter</returns>
        public List<Banned_user> getBannedUsers(int chatroom)
        {
            List<Banned_user> user = (from u in db.Banned_users
                                      where u.Chatroom == chatroom
                                      select u).ToList();


            return user;
        }
        #endregion
    }
}