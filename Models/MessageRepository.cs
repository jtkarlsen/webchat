using System;
using System.Linq;
using WebChat.Models;
using System.Collections.Generic;

namespace WebChat.Models
{
    /// <summary>
    /// Repository for meldinger
    /// </summary>
    public class MessageRepository
    {
        private ChannelDataClassesDataContext db; //Linq instansen
        private UserRepository userRep;
        /// <summary>
        /// Konstruktør
        /// </summary>
        public MessageRepository() 
        {
            db = new ChannelDataClassesDataContext();
            userRep = new UserRepository();
        }
        /// <summary>
        /// Metoden returnerer alle meldingene i DB
        /// </summary>
        /// <returns></returns>
        public IQueryable<Message> showAllMessages() 
        {
            return db.Messages;
        }
        /// <summary>
        /// Metode for å returnere alle meldingene i databasen som tilhører et visst chatroom.
        /// Meldingen blir også gjort om til en sting før den returneres.
        /// </summary>
        /// <param name="id">chatrom id</param>
        /// <returns>Array av strings</returns>
        public List<Message> showChatroomMessages(int id)
        {
            List<Message> messages = (from o in db.Messages
                                      where o.Chatroom.Equals(id)
                                      select o).ToList();
            return messages;
        }
        /// <summary>
        /// Henter ut en melding etter id
        /// </summary>
        /// <param name="id">Meldings id</param>
        /// <returns>message objekt</returns>
        public Message showMessage(int id)
        {
            Message message = (from o in db.Messages
                                where o.Id.Equals(id)
                                select o).FirstOrDefault();
            return message;
        }
        /// <summary>
        /// Metoden oppdaterer databasen etter hvile objekt som kommer inn.
        /// </summary>
        /// <param name="message">meldingsobjekt</param>
        public void updateMessage(Message message)
        {
            db.SubmitChanges();
        }
        /// <summary>
        /// Legger et nytt meldingsobjekt i databasen
        /// </summary>
        /// <param name="newMessage">Meldingsobjekt</param>
        public void addMessage(Message newMessage)
        {
            db.Messages.InsertOnSubmit(newMessage);
            db.SubmitChanges();
        }
        /// <summary>
        /// Sletter en melding fra databasen
        /// </summary>
        /// <param name="message">Meldingsobjekt</param>
        public void deleteMessage(Message message)
        {
            db.Messages.DeleteOnSubmit(message);
            db.SubmitChanges();
        }

        public int addImage(Image image)
        {
            db.Images.InsertOnSubmit(image);
            db.SubmitChanges();
            return image.id;
        }

        public Image getFile(int id)
        {
            Image fil = (from o in db.Images
                         where o.id.Equals(id)
                         select o).FirstOrDefault();
            return fil;
        }

        public void deleteFile(Image fil)
        {
            db.Images.DeleteOnSubmit(fil);
            db.SubmitChanges();
        }
    }
}