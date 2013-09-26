using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChat.Models;

namespace WebChat.Controllers
{
    /// <summary>
    /// User Controller, tar seg av alt som har med brukere og gjøre
    /// Dette er for Admin brukere som da kan slette vanlige brukere
    /// eller endre dem.
    /// </summary>
    public class UserController : Controller
    {

        private UserRepository userRep;
        private MessageRepository messageRep;

        /// <summary>
        /// Konstruktør
        /// </summary>
        public UserController()
        {
            userRep = new UserRepository();
            messageRep = new MessageRepository();
        }

        /// <summary>
        /// GET: /User/
        /// </summary>
        /// <returns>Index viewet</returns>
        [Authorize]
        public ActionResult Index()
        {
            List<aspnet_User> users = userRep.showAllUsers().ToList();
            return View(users);
        }

        /// <summary>
        /// GET: /User/Details/id
        /// Returnerer den brukeren du vil ha
        /// </summary>
        /// <param name="id">brukerens id</param>
        [Authorize]
        public ActionResult Details(string id)
        {
            aspnet_User User = userRep.showUser(id);
            if (User == null) 
                return View("NotFound");
            return View(User);
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
        /// GET: /User/Edit/id
        /// Sender deg til ett endre skjema
        /// </summary>
        /// <param name="id">Brukerens id</param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(string id)
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
        public ActionResult Edit(string id, FormCollection collection)
        {
            try
            {
                aspnet_User User = userRep.showUser(id);
                UpdateModel(User);
                userRep.updateUser(User);
 
                return RedirectToAction("Index");
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
        public ActionResult Delete(string id)
        {
                aspnet_User User = userRep.showUser(id);
                if (User == null)
                    return View("NotFound");
                userRep.deleteUser(User);
 
                return RedirectToAction("Index");
        }
    }
}
