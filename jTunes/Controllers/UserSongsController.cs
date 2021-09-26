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
  public class UserSongsController : Controller
  {
    private jTunesDBEntities db = new jTunesDBEntities();

    // GET: UserSongs
    public ActionResult Index(string Message = "", string MessageStyle = "")
    {
      ViewBag.Message = Message;
      ViewBag.MessageStyle = MessageStyle;
      var userSongs = db.UserSongs.Include(u => u.Song).Include(u => u.User);
      return View(userSongs.ToList());
    }

    public ActionResult MonthlySales()
    {
      ViewBag.Year = jTunesHelper.YearSelect;
      ViewBag.Month = jTunesHelper.MonthSelect;
      ViewBag.TotalSales = null;

      return View();
    }

    [HttpPost]
    public ActionResult MonthlySales(int Year, int Month)
    {
      ViewBag.Year = jTunesHelper.YearSelect;
      ViewBag.Month = jTunesHelper.MonthSelect;
      var salesWithingTimeFrame = db.UserSongs.Where(us => us.PurchaseDate.Year == Year && us.PurchaseDate.Month == Month);

      if (salesWithingTimeFrame.Count() > 0)
      {
        ViewBag.TotalSales = $"Total Sales: {salesWithingTimeFrame.Sum(us => us.Song.Price).ToString("c2")}";
      }
      else
      {
        ViewBag.TotalSales = "There were no sales within the time frame specified.";
      }

      return View();
    }

    // GET: UserSongs/Details/5
    public ActionResult Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UserSong userSong = db.UserSongs.Find(id);
      if (userSong == null)
      {
        return HttpNotFound();
      }
      return View(userSong);
    }

    // GET: UserSongs/Create
    public ActionResult Create()
    {
      ViewBag.SongId = new SelectList(db.Songs, "Id", "Name");
      ViewBag.UserId = new SelectList(db.Users, "Id", "Name");
      return View();
    }

    // POST: UserSongs/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,UserId,SongId,Rating,PurchaseDate")] UserSong userSong)
    {
      if (ModelState.IsValid)
      {
        var songAlreadyPurchased = db.UserSongs.FirstOrDefault(us => us.UserId == userSong.UserId && us.SongId == userSong.SongId);

        if (songAlreadyPurchased != null)
        {
          var user = songAlreadyPurchased.User;
          var song = songAlreadyPurchased.Song;
          var artist = db.Artists.Find(song.ArtistId);
          return RedirectToAction("Index", new { Message = $"{user.Name} already owns {song.Name} by {artist.Name}.", MessageStyle = "text-danger" });
        }

        userSong.Rating = jTunesHelper.GetValidRating(userSong.Rating);

        db.UserSongs.Add(userSong);
        db.SaveChanges();
        return RedirectToAction("Index", new { Message = $"Song successfully purchased.", MessageStyle = "text-success" });
      }

      ViewBag.SongId = new SelectList(db.Songs, "Id", "Name", userSong.SongId);
      ViewBag.UserId = new SelectList(db.Users, "Id", "Name", userSong.UserId);
      return View(userSong);
    }

    // GET: UserSongs/Edit/5
    public ActionResult Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UserSong userSong = db.UserSongs.Find(id);
      if (userSong == null)
      {
        return HttpNotFound();
      }
      ViewBag.SongId = new SelectList(db.Songs, "Id", "Name", userSong.SongId);
      ViewBag.UserId = new SelectList(db.Users, "Id", "Name", userSong.UserId);
      return View(userSong);
    }

    // POST: UserSongs/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,UserId,SongId,Rating,PurchaseDate")] UserSong userSong)
    {
      if (ModelState.IsValid)
      {
        userSong.Rating = jTunesHelper.GetValidRating(userSong.Rating);
        db.Entry(userSong).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      ViewBag.SongId = new SelectList(db.Songs, "Id", "Name", userSong.SongId);
      ViewBag.UserId = new SelectList(db.Users, "Id", "Name", userSong.UserId);
      return View(userSong);
    }

    // GET: UserSongs/Delete/5
    public ActionResult Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      UserSong userSong = db.UserSongs.Find(id);
      if (userSong == null)
      {
        return HttpNotFound();
      }
      return View(userSong);
    }

    // POST: UserSongs/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
      UserSong userSong = db.UserSongs.Find(id);
      db.UserSongs.Remove(userSong);
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
