﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChat.Models;
using System.Text.RegularExpressions;

namespace WebChat.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Configuration/
        AdminRepository config;
        Regex input = new Regex("^[a-zA-Z0-9]{1,25}$"); //Validering av input
        private ChannelRepository channelRep;           //instans av Channel sitt Repository
        private UserRepository userRep;                 //instans av User sitt Repository
        private Permitted_user user;                    //en tilatt bruker, brukes i mange metoder
        private MessageRepository messageRep;
        private MessageFormViewModel view;              //Meldingenes ViewModel


        /// <summary>
        /// Konstruktør som oppretter de forskjellige repositoriene slik at 
        /// kontrolleren kan benytte seg av disse
        /// </summary>
        public AdminController()
        {
            messageRep = new MessageRepository();
            userRep = new UserRepository();
            channelRep = new ChannelRepository();
            try
            {
                config = new AdminRepository(User.Identity.Name);
            }
            catch (NullReferenceException e)
            {
                
            }
        }

        /// <summary>
        /// Returnerer view for admin-menyen
        /// </summary>
        /// <param name="_usr"></param>
        /// <param name="_chtr"></param>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("Index");
        }

        /// <summary>
        /// returnerer view med brukere og handlinger for disse
        /// </summary>
        /// <returns></returns>
        #region Users
        [Authorize]
        public ActionResult ListUsers()
        {
            List<aspnet_User> users = userRep.showAllUsers().ToList();
            return View(users);
        }
        /// <summary>
        /// returnerer detaljene til aktuell bruker
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DetailsUsers(string id)
        {
            aspnet_User user = userRep.showUser(id);
            if (user == null)
                return View("NotFound");
            return View(user);
        }
        /// <summary>
        /// returnerer lovlige instiller for aktuell bruker
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult EditUsers(string id)
        {
            aspnet_User User = userRep.showUser(id);
            if (User == null)
                return View("NotFound");
            return View(User);
        }

        /// <summary>
        /// POST: /User/Edit/id
        /// Sender inn endre skjemaet
        /// </summary>
        /// <param name="id">Brukerens id</param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult EditUsers(FormCollection collection)
        {
            Image image = new Image();
            HttpPostedFileBase file;
            try
            {
                aspnet_User User = new aspnet_User();//userRep.showUser(id);
                UpdateModel(User);
                String i = User.UserName;
                User = userRep.showUser(i);
                int imgId = User.imageId;

                file = Request.Files[0];
                //Logikk for å få bilde inn i databasen
                if (file.ContentLength > 0)
                {
                    image.imageName = file.FileName;
                    image.imageSize = file.ContentLength;
                    image.imageType = file.ContentType;
                    byte[] theImage = new byte[file.ContentLength];
                    file.InputStream.Read(theImage, 0, (int)file.ContentLength);
                    image.image1 = theImage;
                    User.imageId = messageRep.addImage(image);
                    messageRep.deleteFile(messageRep.getFile(imgId));
                }

                userRep.updateUser(User);

                return RedirectToAction("DetailsUsers/" + User.UserName);
            }
            catch
            {
                return View();
            }
        }


        /// <summary>
        /// GET: /User/Delete/id
        /// Lar deg slette en bruker
        /// </summary>
        /// <param name="id">Brukerens id</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DeleteUsers(string id)
        {
            aspnet_User User = userRep.showUser(id);
            List<Chatroom> chat = channelRep.showYourChatrooms(User.UserId);
            if (chat != null)
            {
                foreach (Chatroom u in chat)
                {
                    DeleteChatrooms(u.Id);
                }
            }
            if (User == null)
                return View("NotFound");
            userRep.deleteUser(User);

            return RedirectToAction("ListUsers");
        }
        #endregion
        #region Messages
        /// <summary>
        /// Returnerer alle beskjeder 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult ListMessages()
        {
            List<Message> messages = messageRep.showAllMessages().ToList();
            return View(messages);
        }

        /// <summary>
        /// returnerer detaljer over valgt melding
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DetailsMessages(int id)
        {
            Message Message = messageRep.showMessage(id);
            if (Message == null)
                return View("NotFound");
            return View(Message);
        }
        /// <summary>
        /// valg for oppretelse av ny melding
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CreateMessages()
        {
            return View(new Message());
        }

        /// <summary>
        /// Her lagrer man en beskjed i databasen
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Submit(FormCollection collection)
        {
            try
            {
                Message mess = new Message();
                Chatroom chat = new Chatroom();

                //Logikk for å lagre en beskjed i databasen
                mess.Date = System.DateTime.Now;
                mess.AuthorId = userRep.getUserId(User.Identity.Name);
                UpdateModel(chat, "chat");
                UpdateModel(mess, "mess");
                mess.Chatroom = chat.Id;
                if (mess.Message1 == null || mess.Message1.Length > 200) //Skjekker om beskjedeb er tom
                {
                    ModelState.AddModelError("mess.Message1", "Må være mellom 1 og 200 tegn");
                    return View("Room", new MessageFormViewModel(mess.Chatroom, mess));
                }
                messageRep.addMessage(mess);

                return RedirectToAction("../Channel/Room/" + chat.Name);
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// GET: /Message/Edit/id
        /// forandre en beskjed
        /// </summary>
        /// <param name="id">beskjed id</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult EditMessages(int id)
        {
            Message Message = messageRep.showMessage(id);
            if (Message == null)
                return View("NotFound");
            return View(Message);
        }

        /// <summary>
        /// POST: /Message/Edit/id
        /// Lagre endringer i beskjeden
        /// </summary>
        /// <param name="id">beskjed id</param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult EditMessages(int id, FormCollection collection)
        {
            try
            {
                Message Message = messageRep.showMessage(id);
                UpdateModel(Message);
                messageRep.updateMessage(Message);

                return RedirectToAction("ListMessages");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// GET: /Message/Delete/id
        /// Lar deg slette en beskjed
        /// </summary>
        /// <param name="id">beskjed id</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DeleteMessages(int id)
        {
            Message Message = messageRep.showMessage(id);
            if (Message == null)
                return View("NotFound");
            return View(Message);
        }

        /// <summary>
        /// Post: /Message/Delete/id
        /// Oppdaterer databasen
        /// </summary>
        /// <param name="id">beskjed id</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult DeleteMessages(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                Message Message = messageRep.showMessage(id);
                if (Message == null)
                    return View("NotFound");
                messageRep.deleteMessage(Message);

                return RedirectToAction("ListMessages");
            }
            catch
            {
                return View();
            }
        }
        #endregion
        #region Chatroom
        /// <summary>
        /// Liste over alle chatrooms
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult ListRooms()
        {
            List<Chatroom> chat = channelRep.showAllChatRooms().ToList();
            return View(chat);
        }
        /// <summary>
        /// Detaljer for valgt chatroom
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DetailsRooms(int id)
        {
            Chatroom chat = channelRep.showChatroom(id);
            if (chat == null)
                return View("NotFound");
            return View(chat);
        }
        /// <summary>
        /// Viser et chatroom
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Room(int id)
        {
            Chatroom channel = channelRep.showChatroom(id);
            user = userRep.showPermittedUser(userRep.getUserId(User.Identity.Name).ToString());
            List<Permitted_user> users = userRep.showUserWithChatroom(id);
            view = new MessageFormViewModel(id);


            if (channel == null)                //Skjekker om rommet finnes
                return View("Notfound");
            if (user == null)
                return View("Error");
            foreach (Permitted_user u in users) //Skjekker om brukeren har tilgang til rommet eller om man eier det.
            {
                if (channel.IsPublic == "No" && u.UserId == user.UserId)
                    return View(view);
                else if (channel.OwnerId == (userRep.getOwner(channel).UserId))
                    return View(view);
            }
            if (channel.IsPublic == "Yes")
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
        public ActionResult CreateChatrooms()
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
        public ActionResult CreateChatrooms(FormCollection collection)
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
        public ActionResult EditChatrooms(int id)
        {
            Chatroom channel = channelRep.showChatroom(id);
            List<Permitted_user> users;

            users = userRep.showUserWithChatroom(id); //Henter alle brukere som har tilgang til rommet
 


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
        public ActionResult EditChatrooms(int id, FormCollection collection)
        {
            try
            {
                //Logikk for å endre ett rom
                Chatroom room = channelRep.showChatroom(id);
                UpdateModel(room, "room");
                List<Permitted_user> users = userRep.showUserWithChatroom(id);
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
                channelRep.updateChannel(room);

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
                return RedirectToAction("ListRooms");
            }
            catch
            {
                return View("DatabaseError");
            }
        }

        /// <summary>
        /// GET: /Channel/Delete/id
        /// Lar deg slette rom
        /// </summary>
        /// <param name="id">Navn på rommet</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult DeleteChatrooms(int id)
        {
            Chatroom channel = channelRep.showChatroom(id);
            if (channel == null)
                return View("Notfound");

            channelRep.deleteChatrooms(channel);

            return RedirectToAction("ListRooms");
        }

        #endregion
        #region filer
        public ActionResult Download(int id)
        {
            Image fil = messageRep.getFile(id);

            return File(fil.image1.ToArray(), "application/octet-stream", fil.imageName);
        }
        public ActionResult Image(int id)
        {
            Image fil = messageRep.getFile(id);
            return File(fil.image1.ToArray(), fil.imageType);
        }
        #endregion

    }
        
}
