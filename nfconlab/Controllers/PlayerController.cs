using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nfconlab.Models;

namespace nfconlab.Controllers
{
    public class PlayerController : Controller
    {
        private QuestionDb db = new QuestionDb();

        //
        // GET: /Player/

        public ActionResult Index()
        {
            return View(db.Players.ToList());
        }

        //
        // GET: /Player/Details/5

        public ActionResult Details(int id = 0)
        {
            PlayerItem playeritem = db.Players.Find(id);
            if (playeritem == null)
            {
                return HttpNotFound();
            }
            return View(playeritem);
        }

        //
        // GET: /Player/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Player/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PlayerItem playeritem)
        {
            if (ModelState.IsValid)
            {
                db.Players.Add(playeritem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(playeritem);
        }

        //
        // GET: /Player/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id = 0)
        {
            PlayerItem playeritem = db.Players.Find(id);
            if (playeritem == null)
            {
                return HttpNotFound();
            }
            return View(playeritem);
        }

        //
        // POST: /Player/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PlayerItem playeritem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(playeritem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(playeritem);
        }

        //
        // GET: /Player/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id = 0)
        {
            PlayerItem playeritem = db.Players.Find(id);
            if (playeritem == null)
            {
                return HttpNotFound();
            }
            return View(playeritem);
        }

        //
        // POST: /Player/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlayerItem playeritem = db.Players.Find(id);
            db.Players.Remove(playeritem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Player/MyStats

        public ActionResult MyStats()
        {
            try
            {
                string uid = TempData["FacebookId"].ToString();
                var me = from p in db.Players
                         where p.User_ID.Equals(uid)
                         select p;
                return View(me.ToList());
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}