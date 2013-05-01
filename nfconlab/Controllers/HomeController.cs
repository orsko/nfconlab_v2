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
                int id = int.Parse(json.UserID);
                Session["UserID"] = json.UserID;
                Session["Date"] = json.Date;
                //Sikeres azonosítás
                //Ha van ilyen felhasználó kész
                PlayerItem player = null;

                string uid = "" + id;

                player = db.Players.FirstOrDefault(p => p.User_ID.Equals(uid));


                if (player != null)
                    return "User identification complete";
                //Ha nincs, csinálni kell
                db.Players.Add(new PlayerItem
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
        public PlayerItem AddPoint(QuestionItem question, object userId)
        {
            PlayerItem player = null;
            string uid = (string)userId;
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
            string userId = "";
            try
            {
                userId = (string)Session["UserID"];
                player = db.Players.FirstOrDefault(p => p.User_ID.Equals(userId));
                if(player != null && player.QuestionItems.Contains(db.Questions.Find(id)))
                //if (player.Answered!=null && player.Answered.Split(',').Contains(id.ToString()))
                    return "Already answered question";
            }
            catch
            {
                return userId + "Not identified player";
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

                //Helyes válasz vizsgálata
                QuestionItem questionitem = db.Questions.Find(id);
                string code = "false";
                //Ha helyes a válasz
                PlayerItem player = null;
                if (questionitem.RightAnswer.Equals(answer))
                {
                    //Vissza kell adni, hogy jó
                    code = "true";
                    //Hozzá kell adni a megválaszolt kérdésekhez
                    try
                    {
                        string userid = (string) Session["UserID"];
                        player = db.Players.FirstOrDefault(p => p.User_ID.Equals(userid));
                        player.QuestionItems.Add(questionitem);
                    }
                    catch
                    {
                        throw;
                    }
                    //player = AddPoint(questionitem, Session["UserID"]);
                }
                else
                {
                    string uID = (string)Session["UserID"];
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