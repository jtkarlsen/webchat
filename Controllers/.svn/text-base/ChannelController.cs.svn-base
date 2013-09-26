using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChat.Models;
using System.Web.Security;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace WebChat.Controllers
{
    /// <summary>
    /// Channel Controller, tar seg av alt som har med chatrom og gjøre
    /// </summary>
    public class ChannelController : Controller
    {
        Regex input = new Regex("^[a-zA-Z0-9]{1,25}$"); //Validering av input
        private ChannelRepository channelRep;           //instans av Channel sitt Repository
        private UserRepository userRep;                 //instans av User sitt Repository
        private Permitted_user user;                    //en tilatt bruker, brukes i mange metoder
        private MessageFormViewModel view;              //Meldingenes ViewModel
        private MessageRepository messRep;              //instans av Message sitt Repository
        private ChannelIndexViewModel channelIndex;     //Forsiden til chatrommene sin ViewModel

        /// <summary>
        /// Konstruktør
        /// </summary>
        public ChannelController()
        {
            channelRep = new ChannelRepository();
            messRep = new MessageRepository();
            userRep = new UserRepository();
        }

        /// <summary>
        /// GET: /Channel/
        /// </summary>
        /// <returns>Index viewet</returns>
        [Authorize]
        public ActionResult Index()
        {
            channelIndex = new ChannelIndexViewModel(userRep.getUserId(User.Identity.Name));
            return View(channelIndex);
        }

        /// <summary>
        /// GET: /Channel/Room/id
        /// Returnerer det rommet du vil ha
        /// </summary>
        /// <param name="id">Navn på rommet</param>
        [Authorize]
        public ActionResult Room(int id)
        {
            Chatroom channel = channelRep.showChatroom(id);
            user = userRep.showPermittedUser(userRep.getUserId(User.Identity.Name).ToString());
            List<Permitted_user> users = userRep.showUserWithChatroom(id);
            view = new MessageFormViewModel(id);


            if (channel == null)                //Skjekker om rommet finnes
                return View("Notfound");/*
            if (user == null)
                return View("Error");*/
            foreach (Permitted_user u in users) //Skjekker om brukeren har tilgang til rommet eller om man eier det.
            {
                if (user != null)
                {
                    if (u.UserId == user.UserId)
                    {
                        return View(view);
                    }
                }
                if (channel.IsPublic == "No")
                    return View(view);
                else if (channel.OwnerId == userRep.getUserId(User.Identity.Name))
                    return View(view);
            }
            if (channel.IsPublic == "Yes")
                return View(view);
            else if (channel.OwnerId == userRep.getUserId(User.Identity.Name))
                return View(view);

            //Kommer man hit har det skjedd noe galt
            return View("Error");
        }

        /// <summary>
        /// GET: /Channel/Create
        /// returnerer ett skjema for å lage ett chatrom
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create()
        {           
            return View("Create", new ChannelFormViewModel(new Chatroom(), User.Identity.Name));
        } 

        /// <summary>
        /// POST: /Channel/Create
        /// Sender inn skjema og legger til i databasen
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Create(FormCollection collection)
        {
                try
                {

                    Chatroom room = new Chatroom();
                    List<Permitted_user> users = new List<Permitted_user>();
                    user = new Permitted_user();

                    //Logikk for å legge til ett nytt rom
                    room.Date = System.DateTime.Now;
                    room.OwnerId = userRep.getUserId(User.Identity.Name);
                    UpdateModel(room, "room");
                    if (room.Name == null) //Skjekker om man skriver inn romnavn
                    {
                        ModelState.AddModelError("room.Name", "Dette feltet må fylles ut");
                        return View(new ChannelFormViewModel(room, User.Identity.Name));
                    }
                    if (!input.IsMatch(room.Name)) //Skjekker om rommet er i rett format
                    {
                        ModelState.AddModelError("room.Name", "Navnet kan bare være av bokstaver og tall, samt 25 eller færre tegn.");
                        return View(new ChannelFormViewModel(room, User.Identity.Name));
                    }
                    channelRep.addChannel(room);

                    string[] selectedValues;
                    if (!(collection["Users"] == null)) //Henter en liste over brukerid, og skjekker om en er null
                    {
                        selectedValues = collection["Users"].Split(',');

                        if (room.IsPublic == "No") //Hvis rommet er public er det ikke vits å ha tilatte brukere
                        {
                            foreach (string s in selectedValues) //For hver bruker du velger legger han dem til i databasen
                            {
                                user = new Permitted_user();
                                user.Chatroom = room.Id;
                                user.UserId = Guid.Parse(s);
                                userRep.addPermittedUsers(user);
                            }
                        }
                    }
                    //Om alt gikk bra sendes du til Channel/Index
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View("DatabaseError"); //Sendes til ett error view
                }
        }
        
        
        /// <summary>
        /// GET: /Channel/Edit/id
        /// Sender deg til ett endre skjema
        /// </summary>
        /// <param name="id">Navn på rommet</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(int id) 
        {


            Chatroom channel;
            List<Permitted_user> users;

            try
            {
                channel = channelRep.showChatroom(id);
                if (userRep.getUserId(User.Identity.Name) != channel.OwnerId) //Skjekker om pålogget bruker eier rommet
                {
                    users = new List<Permitted_user>();
                    return View("Error");
                }
                else
                {
                    users = userRep.showUserWithChatroom(id); //Henter alle brukere som har tilgang til rommet
                }
                if (users.Count == 0)
                    users = new List<Permitted_user>();


                if (channel == null)
                    return View("Notfound");
            }
            catch
            {
                    return View("Error"); 
            }


            return View(new ChannelFormViewModel(channel, users, User.Identity.Name));
        }

        /// <summary>
        /// POST: /Channel/Edit/id
        /// Sender inn endre skjemaet
        /// </summary>
        /// <param name="id">Navn på rommet</param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                //Logikk for å endre ett rom
                Chatroom room = channelRep.showChatroom(id);
                List<Permitted_user> users = userRep.showUserWithChatroom(id);
                UpdateModel(room, "room");
                channelRep.updateChannel(room);
                if (room.Name == null) //Skjekker om man har skrevet inn romnavn
                {
                    ModelState.AddModelError("room.Name", "Dette feltet må fylles ut");
                    return View(new ChannelFormViewModel(room, users, User.Identity.Name));
                }
                if (!input.IsMatch(room.Name)) //Skjekker om rommet er i rett format
                {
                    ModelState.AddModelError("room.Name", "Navnet kan bare være av bokstaver og tall, samt 25 eller færre tegn.");
                    return View(new ChannelFormViewModel(room, users, User.Identity.Name));
                }

                
                //Samme logikk som i Create
                if (!(collection["Users"] == null))
                {
                    string[] selectedValues = collection["Users"].Split(',');
                    
                        foreach (string s in selectedValues)
                        {
                            user = new Permitted_user();
                            user.Chatroom = room.Id;
                            user.UserId = Guid.Parse(s);
                            userRep.addPermittedUsers(user);
                        }                  
                }
                //Her legger han brukere du velger å fjerne fra rommet ditt i en liste
                //og så sletter dem fra databasen
                if (!(collection["CurrentUsers"] == null))
                {
                    string[] selectedCurrentUsers = collection["CurrentUsers"].Split(',');
                        foreach (string s in selectedCurrentUsers)
                        {
                            Permitted_user u = userRep.showPermittedUser(s);
                            userRep.deletePermittedUsers(u);
                            System.Threading.Thread.Sleep(10); // Fant ut at databasen ikke håndterte spørringene raskt nok, og ikke alle gikk gjennom hvær gang, så la inn en liten pause.
                        }

                }
                
                return RedirectToAction("Index");
            }


            catch (NullReferenceException e)
            {
                return View("Error");
            }
        }

        /// <summary>
        /// GET: /Channel/Delete/id
        /// Lar deg slette rom
        /// </summary>
        /// <param name="id">Navn på rommet</param>
        /// <returns></returns>
 
        [Authorize]
        public ActionResult Delete(int id)
        {
            Chatroom channel = channelRep.showChatroom(id);
            if (channel == null)
                return View("Notfound");

            if (channel.OwnerId == userRep.getUserId(User.Identity.Name))
                channelRep.deleteChatrooms(channel);
            else
                return View("Index");

            return RedirectToAction("Index");
        }

    }
}
