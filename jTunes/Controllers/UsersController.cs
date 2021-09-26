using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using jTunes;
using jTunes.Models;

namespace jTunes.Controllers
{
  public class UsersController : Controller
  {
    private jTunesDBEntities db = new jTunesDBEntities();

    // GET: Users
    public ActionResult Index(string Message = "", string MessageStyle = "")
    {
      ViewBag.Message = Message;
      ViewBag.MessageStyle = MessageStyle;
      return View(db.Users.ToList());
    }

    public ActionResult PurchasedSongs()
    {
      ViewBag.User = jTunesHelper.UserSelect;
      ViewBag.UserName = "";

      return View();
    }

    [HttpPost]
    public ActionResult PurchasedSongs(int User)
    {
      ViewBag.User = jTunesHelper.UserSelect;
      ViewBag.UserName = db.Users.Find(User).Name;
      IEnumerable<UserSong> purchasedSongs = db.UserSongs.Where(ps => ps.UserId == User);

      return View(purchasedSongs);
    }

    // GET: Users/Details/5
    public ActionResult Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      User user = db.Users.Find(id);
      if (user == null)
      {
        return HttpNotFound();
      }
      return View(user);
    }

    // GET: Users/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: Users/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,Money,Name")] User user)
    {
      if (ModelState.IsValid)
      {
        var duplicateUserName = db.Users.FirstOrDefault(u => u.Name.ToLower() == user.Name.ToLower());

        if (duplicateUserName != null)
        {
          return RedirectToAction("Index", new { Message = $"Could not create user. The user name {user.Name} is already taken.", MessageStyle = "text-danger" });
        }

        if (user.Money != null)
        {
          if (user.Money < 0)
          {
            user.Money = 0;
          }
          else
          {
            user.Money = Math.Round((double)user.Money, 2);
          }
        }

        db.Users.Add(user);
        db.SaveChanges();
        return RedirectToAction("Index", new { Message = $"User {user.Name} successfully added to the database.", MessageStyle = "text-success" });
      }

      return View(user);
    }

    // GET: Users/Edit/5
    public ActionResult Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      User user = db.Users.Find(id);
      if (user == null)
      {
        return HttpNotFound();
      }
      return View(user);
    }

    // POST: Users/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,Money,Name")] User user)
    {
      if (ModelState.IsValid)
      {
        if (user.Money != null)
        {
          if (user.Money < 0)
          {
            user.Money = 0;
          }
          else
          {
            user.Money = Math.Round((double)user.Money, 2);
          }
        }



        db.Entry(user).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      return View(user);
    }

    // GET: Users/Delete/5
    public ActionResult Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      User user = db.Users.Find(id);
      if (user == null)
      {
        return HttpNotFound();
      }
      return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
      User user = db.Users.Find(id);
      db.Users.Remove(user);
      db.SaveChanges();
      return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
