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
        private PlayerDb db = new PlayerDb();

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
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        //
        // GET: /Player/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Player/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Player player)
        {
            if (ModelState.IsValid)
            {
                db.Players.Add(player);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(player);
        }

        //
        // GET: /Player/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        //
        // POST: /Player/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Player player)
        {
            if (ModelState.IsValid)
            {
                db.Entry(player).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(player);
        }

        //
        // GET: /Player/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        //
        // POST: /Player/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Player player = db.Players.Find(id);
            db.Players.Remove(player);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // POST: /Player/Identify
        // Segédosztály felhasználó azonosításhoz
        public class myJSONforPlayer
        {
            public myJSONforPlayer(string id, string date)
            {
                UserID = id;
                Date = date;
            }
            public myJSONforPlayer()
            {
                UserID = "Semmi";
                Date = "1000-01-01";
            }
            public string UserID { get; set; }
            public string Date { get; set; }
        }

        //Felhasználó azonosítás POST függvény
        [HttpPost]
        public string Identify(myJSONforPlayer json)
        {
            //Ha nem null értéket kapott
            if (json != null)
            {
                //Felhasználó eltárolása
                if (json.UserID.Equals("Semmi"))
                    return "User identification failed: NOT VALID USER";
                int id = int.Parse(json.UserID);
                Session["UserID"] = json.UserID;
                Session["Date"] = json.Date;
                //Sikeres azonosítás
                //Ha van ilyen felhasználó kész
                Player player = null;

                string uid = "" + id;

                player = db.Players.FirstOrDefault(p => p.User_ID.Equals(uid));
                
                
                
                
                if (player != null)
                    return "User identification complete";
                //Ha nincs, csinálni kell
                db.Players.Add(new Player
                {
                    User_ID = "" + id
                });
                db.SaveChanges();
                return "New user added";
            }
            //Azonosítási hiba
            return "User identification failed: NULL JSON";
        }

        //
        // POST: /Player/AddPoint
        //Felhasználói pontszám növelése, valamint kérdés hozzáadása a megválaszoltakhoz
        [HttpPost]
        public string AddPoint(QuestionItem question, object userId)
        {
            Player player = null;
            string uid = (string)userId;
            //Játékos lekérése
            try
            {
                player = db.Players.FirstOrDefault(p => p.User_ID.Equals(uid));
            }
            catch
            {
                return "Not valid user";
            }
            //Válasz sorszámának hozzáadása a megválaszolt kérdésekhez
            if (player.Answered != null)
                player.Answered += ",";
            player.Answered += "" + question.QuestionItemId;
            //Pontszám növelése, és mentés
            player.Points += 1;
            db.SaveChanges();
            return "Added";
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}