using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChat.Models;
using System.IO;

namespace WebChat.Controllers
{
    /// <summary>
    /// Message sin Controller, tar seg av alle beskjeder.
    /// For det meste er dette admin metoder, men Submit lar deg poste en beskjed
    /// </summary>
    public class MessageController : Controller
    {
        private MessageRepository messageRep;
        private UserRepository userRep;


        /// <summary>
        /// Kontruktør
        /// </summary>
        public MessageController()
        {
            messageRep = new MessageRepository();
            userRep = new UserRepository();
        }

        /// <summary>
        /// GET: /Message/
        /// Sender deg til en oversikt av alle beskjeder
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            List<Message> message = messageRep.showAllMessages().ToList();
            return View(message);
        }


        /// <summary>
        /// GET: /Message/Details/id
        /// Her kan du se detaljer over beskjeder
        /// </summary>
        /// <param name="id">beskjed id</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Details(int id)
        {
            Message Message = messageRep.showMessage(id);
            if (Message == null)
                return View("NotFound");
            return View(Message);
        }
 
        /// <summary>
        /// GET: /Message/Create
        /// Opprette en beskjed
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create()
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
                Image image = new Image();
                HttpPostedFileBase file;
                
                //Logikk for å lagre en beskjed i databasen
                mess.Date = System.DateTime.Now;
                mess.AuthorId = userRep.getUserId(User.Identity.Name);
                UpdateModel(chat, "chat");
                UpdateModel(mess, "mess");
                mess.Chatroom = chat.Id;
                if (mess.Message1 != null)
                {
                    if (mess.Message1.Length > 200) //Skjekker om beskjedeb er tom
                    {
                        ModelState.AddModelError("mess.Message1", "Må være under 200 tegn");
                        return View("Room", new MessageFormViewModel(mess.Chatroom, mess));
                    }
                }

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
                    mess.imageId = messageRep.addImage(image);

                }

                messageRep.addMessage(mess);

                return RedirectToAction("../Channel/Room/"+chat.Id);
            }
            catch
            {
                return View("../Channel/");
            }
        }
        
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

        /// <summary>
        /// GET: /Message/Edit/id
        /// forandre en beskjed
        /// </summary>
        /// <param name="id">beskjed id</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(int id)
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
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                Message Message = messageRep.showMessage(id);
                UpdateModel(Message);
                messageRep.updateMessage(Message);
 
                return RedirectToAction("Index");
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
        public ActionResult Delete(int id)
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
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                Message Message = messageRep.showMessage(id);
                if (Message == null)
                    return View("NotFound");
                messageRep.deleteMessage(Message);
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
