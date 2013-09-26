using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebChat.Models
{
    /// <summary>
    /// Klasse som henter ut lister med brukere som skal vises i viewet
    /// </summary>
    public class ChannelFormViewModel
    {
        //
        // GET: /ChannelFormViewModel/

        private ChannelRepository channelRep;
        private UserRepository userRep;
        public Chatroom room { get; set; }
        public List<Permitted_user> permittedUsers { get; set; }
        public MultiSelectList newUsers { get; private set; }
        public MultiSelectList currentUsers { get; private set; }
        public List<aspnet_User> allUsers { get; set; } // Liste med brukere
        public List<aspnet_User> finalUsers { get; set; } // Liste med brukere
        private string[] selectedValuesUser = null, selectedValuesCurrentUser = null; // Valgte verdier i viewet

        /// <summary>
        /// Metoden henter en liste med brukere som brukeren skal kunne markere og gi tilgang til chatroomet.
        /// Listen vil ekskludere brukeren som oppretter chatroomet.
        /// </summary>
        /// <param name="c">Et chatroom objekt</param>
        /// <param name="userName">Brukernavn til brukeren</param>
        public ChannelFormViewModel(Chatroom c, string userName)
        {
            userRep = new UserRepository();
            channelRep = new ChannelRepository();
            room = c;
            allUsers = userRep.showAllUsers().ToList();
            finalUsers = userRep.showAllUsers().ToList();
            foreach (aspnet_User u in allUsers)  // sjekker etter brukeren som laget chatroomet. For så å fjerne brukerobjektet fra en liste som blir sendt til viewet.
            {
                if (u.LoweredUserName == userName)
                    finalUsers.Remove(u); //sletter brukeren fra viewet som brukes
            }
            newUsers = new MultiSelectList(finalUsers,
                "UserId",
                "UserName",
                selectedValuesUser);
            /*
             * Skal brukes i Oblig 2
            bannedList = new MultiSelectList(channelRep.showAllUsers(),
                "UserId",
                "UserName",
                selectedValuesbannedUser);
              */

        }
        /// <summary>
        /// Metoden henter ut to lister som sendes til viewet. Den ene er av brukere med tilgang, og den andre er med brukere som ikke har tilgang.
        /// Brukeren skal kunne velge i disse listene for å legge dem til i den andre listen.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="u"></param>
        /// <param name="userName"></param>
        public ChannelFormViewModel(Chatroom c, List<Permitted_user> u, string userName)
        {
            userRep = new UserRepository();
            channelRep = new ChannelRepository();
            room = c;
            permittedUsers = u;

            allUsers = userRep.showUsersNotInChatroom(u);
            finalUsers = userRep.showUsersNotInChatroom(u);
            foreach (aspnet_User us in allUsers) // sjekker etter brukeren som laget chatroomet. For så å fjerne brukerobjektet fra en liste som blir sendt til viewet.
            {
                if (us.LoweredUserName == userName)
                    finalUsers.Remove(us);
            }
            // Liste med brukere som ikke er tilatt i rommet
            newUsers = new MultiSelectList(finalUsers,
                "UserId",
                "UserName",
                selectedValuesUser);
            // Liste med brukere som er i rommet
            currentUsers = new MultiSelectList(userRep.showCurrentUsers(u),
                "UserId",
                "UserName",
                selectedValuesCurrentUser);
            /**
             * Skal brukes i Oblig 2
             * 
             * bannedUsers = b;
            bannedList = new MultiSelectList(channelRep.showBannedUsers(b),
                "UserId",
                "UserName",
                selectedValuesbannedUser);
            notBannedList = new MultiSelectList(channelRep.showUnBannedUsers(b),
                "UserId",
                "UserName",
                selectedValuesUnBannedUsers);
            */

        }
    }
}
