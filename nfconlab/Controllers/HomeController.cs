using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using nfconlab.Models;
using System.Web.Security;

namespace nfconlab.Controllers
{
    public class HomeController : Controller
    {
        private QuestionDb db = new QuestionDb();

        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View(db.Questions.ToList());           
        }

        //
        // GET: /Home/Details/5

        public ActionResult Details(int id = 0)
        {
            QuestionItem questionitem = db.Questions.Find(id);
            if (questionitem == null)
            {
                return HttpNotFound();
            }

            return View(questionitem);
       
            
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
                long id = long.Parse(json.UserID);
                Session["UserID"] = long.Parse(json.UserID);
                Session["Date"] = json.Date;
                //Sikeres azonosítás
                //Ha van ilyen felhasználó kész
                PlayerItem player = null;

                player = db.Players.FirstOrDefault(p => p.User_ID.Equals(id));


                if (player != null)
                    return "User identification complete";
                //Ha nincs, csinálni kell
                db.Players.Add(new PlayerItem
                {
                    User_ID = id
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
        public PlayerItem AddPoint(QuestionItem question, object userId)
        {
            PlayerItem player = null;
            long uid = (long)userId;
            //Játékos lekérése
            try
            {
                player = db.Players.FirstOrDefault(p => p.User_ID.Equals(uid));
            }
            catch
            {
                return null;
            }
            //Válasz sorszámának hozzáadása a megválaszolt kérdésekhez
            if (player.Answered != null)
                player.Answered += ",";
            player.Answered += "" + question.QuestionItemId;
            //Pontszám növelése, és mentés
            player.Points += 1;
            db.SaveChanges();
            return player;
        }

        //
        // GET: /Home/Questions/5
        //Kérdés lekérése REST API-n keresztül
        public string Questions(int id = 0)
        {
            //Csak olyan kérdést szabas, mi még nincs megválaszolva
            PlayerItem player = null;
            long userId;
            try
            {
                userId = (long)Session["UserID"];
                player = db.Players.FirstOrDefault(p => p.User_ID.Equals(userId));
                if (player != null && player.QuestionItems.Contains(db.Questions.Find(id)))
                {
                    /*************************TESZTHEZ NEM KELL***********************************
                     *return "Already answered question";
                     */
                }
                //if (player.Answered!=null && player.Answered.Split(',').Contains(id.ToString()))

                    
            }
            catch
            {
                return "Not identified player";
            }

            //Kérdés lekérése az adatbázisból
            QuestionItem questionitem = db.Questions.Find(id);
            //Azonosító elmentése
            Session["QuestionID"] = id;
            if (questionitem == null)
            {
                return "null";
            }
            //Kimeneti JSON megfelelően formázva
            JsonResult json = new JsonResult();
            if (questionitem.Image == null) questionitem.Image = "http://nfconlab.azurewebsites.net/Images/noimg.png";
            json.Data = "{"
                                + "\"Date\":\"" + questionitem.Date + "\","
                                + "\"Question\":\"" + questionitem.Question + "\","
                                + "\"Answers\":"
                                + "{"
                                    + "\"Answer1\":\"" + questionitem.Answer1 + "\","
                                    + "\"Answer2\":\"" + questionitem.Answer2 + "\","
                                    + "\"Answer3\":\"" + questionitem.Answer3 + "\","
                                    + "\"Answer4\":\"" + questionitem.Answer4 + "\""
                                + "},"
                                + "\"Image\":\"" + questionitem.Image + "\""
                            + "}";

            return json.Data.ToString();
        }

        //
        // GET: /Home/AllQuestions
        // Minden kérdés lekérése
        public string AllQuestions()
        {

            var json = string.Empty;
            foreach (var questionitem in db.Questions)
            {
                json += "{"
                             + "\"Answers\":"
                             + "{"
                             + "\"Answer1\":\"" + questionitem.Answer1 + "\","
                             + "\"Answer2\":\"" + questionitem.Answer2 + "\","
                             + "\"Answer3\":\"" + questionitem.Answer3 + "\","
                             + "\"Answer4\":\"" + questionitem.Answer4 + "\","
                             + "\"RightAnswer\":\"" + questionitem.RightAnswer + "\""
                             + "},"
                             + "\"Date\":\"" + questionitem.Date + "\","
                             + "\"Image\":\"" + questionitem.Image + "\","
                             + "\"Position\":\"" + questionitem.Location + "\","
                             + "\"Question\":\"" + questionitem.Question + "\","                                                         
                             + "\"QuestionItemId\":\"" + questionitem.QuestionItemId + "\""                             
                             + "}\n";
            }
            return json;
        }

        //
        // POST: /Home/Questions
        //Segédosztály a válaszadáshoz
        public class myJSONforAnswer
        {
            public myJSONforAnswer(string str)
            {
                Answer = str;
            }
            public myJSONforAnswer()
            {
                Answer = "Semmi";
            }
            public string Answer { get; set; }            
        }

        //Válasz adás POST üzenet
        [HttpPost]
        public string Questions(myJSONforAnswer json)
        {
            int id = -1;
            //Ha nem null-t kapott
            if (json != null)
            {
                try
                {
                    //Ha a lekért kérdés létezik
                    id = (int)Session["QuestionID"];
                }
                catch
                {
                    return "NOT VALID QUESTION";
                }
                //Válasz eltárolása
                string answer = json.Answer;

                //Ha már megválaszolta, nem lehet többször
                PlayerItem player = null;
                try
                {
                    long userid = (long)Session["UserID"];
                    player = db.Players.FirstOrDefault(p => p.User_ID.Equals(userid));                  
                }
                catch
                {
                    return "Not identified player";
                }
               
                QuestionItem questionitem = db.Questions.Find(id);
                if (player.QuestionItems.Contains(questionitem))
                {
                    return "Already answered question";
                }
                player.QuestionItems.Add(questionitem);
                string code = "false";

                //Helyes válasz vizsgálata
                if (questionitem.RightAnswer.Equals(answer))
                {
                    //Vissza kell adni, hogy jó
                    code = "true";
                    //Hozzá kell adni a megválaszolt kérdésekhez
                    player.QuestionItems.Add(questionitem);
                    //player = AddPoint(questionitem, Session["UserID"]);
                }
                else
                {
                    long uID = (long)Session["UserID"];
                    player = db.Players.FirstOrDefault(p => p.User_ID.Equals(uID));
                }

                //Következő kérdés, ennek kell visszaadni a pozícióját
                QuestionItem nextQuestion = null;
                               
                //Megválaszolt kérdések
                if (player != null)
                {
                    QuestionItem[] answers = player.QuestionItems.ToArray();

                    QuestionItem[] questions = db.Questions.ToArray();

                    foreach(var q in questions)
                    {
                        //Ha nem válaszolta még meg és van ilyen kérdés visszaadja
                        if (!answers.Contains(q) && !q.Equals(questionitem))
                        {
                            nextQuestion = q;
                                                    
                            break;
                        }
                    }

                    //pont adás  
                    player.Points += 1;
                }

                
                string nextPos = "0.0,0.0";              
                
                if (nextQuestion != null)
                    nextPos = nextQuestion.Location;

                //Visszaadandó JSON a megfelelő formátumban
                JsonResult response = new JsonResult();
                response.Data = "{"
                                     + "\"Response\":\"" + code + "\","
                                     + "\"Position\":\"" + nextPos + "\""
                              + "}";
                if (player != null)
                {
                    player.Answered = "";
                    foreach (var a in player.QuestionItems.ToArray())
                    {
                        player.Answered += a.QuestionItemId.ToString() + ",";
                    }
                }

                db.SaveChanges();

                return response.Data.ToString();
            }
            return "NULL JSON";
        }

        //
        // GET: /Home/GetMyPoints

        public string GetMyPoints()
        {
            var json = string.Empty;
            try
            {
                //User ID lekérése
                var uid = (long)Session["UserID"];
                //Játékos
                var me = db.Players.FirstOrDefault(p => p.User_ID.Equals(uid));
                //Sorrend
                var all = (from p in db.Players
                          orderby p.Points descending
                          select p).ToList();
                
                //Ranglista pozíció (+1, hogy ne 0-tól kezdődjön)
                int index = all.IndexOf(me) + 1;

                //Pontjainak kiírása
                json += "{"
                        + "\"Points\":\"" + me.Points + "\","
                        + "\"Position\":\"" + index + "\""
                        + "}";

                return json;
            }
            catch (Exception)
            {
                return "Not identified player";
            }
        }

        //
        // GET: /Home/GetTopPlayers

        public string GetTopPlayers()
        {
            JsonResult json = new JsonResult();
            try
            {
                var q = from p in db.Players
                        orderby p.Points descending 
                        select p;
                var top = q.Take(10);
                json.Data = "{"
                            + "\"players\":"
                            + "[";

                foreach (var player in top)
                {
                    json.Data += "{"
                                + "\"Id\":\"" + player.User_ID + "\","
                                + "\"Points\":\"" + player.Points + "\""
                                + "},";               
                }
                json.Data = json.Data.ToString().Remove(json.Data.ToString().Length - 1);
                json.Data += "]"
                           + "}";
                return json.Data.ToString();
            }
            catch (Exception)
            {
                return "Something went wrong";
            }
        }

        //
        // GET: /Home/GetMyCoordinates

        public string GetMyCoordinates()
        {
            JsonResult json = new JsonResult();
            try
            {
                long uid = (long)Session["UserID"];
                var me = db.Players.FirstOrDefault(p => p.User_ID.Equals(uid));                
                json.Data = "{"
                            + "\"Coordinates\":"
                            + "[";

                foreach (var q in me.QuestionItems)
                {
                    json.Data += "{"
                                + "\"Location\":\"" + q.Location + "\""
                                + "},";
                }
                json.Data = json.Data.ToString().Remove(json.Data.ToString().Length - 1);
                json.Data += "]"
                           + "}";
                return json.Data.ToString();
            }
            catch (Exception)
            {
                return "Somethong went wrong";
            }
        }

        //
        // GET: /Home/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Home/Create

        [HttpPost]
        public ActionResult Create(QuestionItem questionitem)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Add(questionitem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(questionitem);
        }

        //
        // GET: /Home/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int id = 0)
        {
            QuestionItem questionitem = db.Questions.Find(id);
            if (questionitem == null)
            {
                return HttpNotFound();
            }
            return View(questionitem);
        }

        //
        // POST: /Home/Edit/5

        [HttpPost]
        public ActionResult Edit(QuestionItem questionitem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionitem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(questionitem);
        }

        //
        // GET: /Home/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int id = 0)
        {
            QuestionItem questionitem = db.Questions.Find(id);
            if (questionitem == null)
            {
                return HttpNotFound();
            }
            return View(questionitem);
        }

        //
        // POST: /Home/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            QuestionItem questionitem = db.Questions.Find(id);
            db.Questions.Remove(questionitem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Home/Players/
        [Authorize(Roles = "admin")]
        public ActionResult Players()
        {
            return View(db.Players.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}