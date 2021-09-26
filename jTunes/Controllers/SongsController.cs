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
  public class SongsController : Controller
  {
    private jTunesDBEntities db = new jTunesDBEntities();

    public ActionResult Index(string Message = "", string MessageStyle = "")
    {
      ViewBag.Message = Message;
      ViewBag.MessageStyle = MessageStyle;
      var songs = db.Songs.Include(s => s.Artist);
      return View(songs.ToList());
    }

    public ActionResult SongPurchaseCount()
    {
      ViewBag.Song = jTunesHelper.SongSelect;
      ViewBag.SongName = "";
      ViewBag.TotalPurchases = null;
      return View();
    }

    [HttpPost]
    public ActionResult SongPurchaseCount(int Song)
    {
      IEnumerable<UserSong> songPurchases = db.UserSongs.Where(us => us.SongId == Song);
      ViewBag.Song = jTunesHelper.SongSelect;
      ViewBag.SongName = db.Songs.Find(Song).Name;
      ViewBag.TotalPurchases = songPurchases.Count();

      return View(songPurchases);
    }

    public ActionResult TopSongBySales()
    {
      ViewBag.SaleCount = jTunesHelper.GetTopSellingSongSales();

      return View(jTunesHelper.GetTopSellingSong());
    }
    public ActionResult TopThreeSongsByRating()
    {
      return View(jTunesHelper.GetTopThreeRatedSongs());
    }

    public ActionResult ChoosePurchaser()
    {
      ViewBag.User = jTunesHelper.UserSelect;

      return View();
    }

    [HttpPost]
    public ActionResult ChoosePurchaser(int User)
    {
      return RedirectToAction("PurchaseSong", "Songs", new { User = User });
    }

    public ActionResult ChooseRefundee()
    {
      ViewBag.User = jTunesHelper.UserSelect;

      return View();
    }

    [HttpPost]
    public ActionResult ChooseRefundee(int User)
    {
      return RedirectToAction("RefundSong", "Songs", new { User = User });
    }

    public ActionResult PurchaseSong(int User, int? Song, string Message = "", string MessageStyle = "")
    {
      var user = db.Users.Find(User);
      var ownedSongIds = user.UserSongs.Select(us => us.SongId);
      var purchasableSongs = db.Songs.Where(s => !ownedSongIds.Contains(s.Id));
      var song = db.Songs.Find(Song);
      ViewBag.UserPurchasing = user;
      ViewBag.Wallet = user.Money == null ? 0.ToString("c2") : Math.Round((double)user.Money, 2).ToString("c2");
      ViewBag.Message = Message;
      ViewBag.MessageStyle = MessageStyle;

      if (song != null)
      {
        jTunesHelper.PurchaseSong(User, (int)Song);

        if (jTunesHelper.PurchaseWasSuccessful(User, Song))
        {
          return RedirectToAction("PurchaseSong", jTunesHelper.SuccessfulPurchaseRedirect(User, song.Name, song.Artist.Name));
        }
        else
        {
          return RedirectToAction("PurchaseSong", jTunesHelper.UnsuccessfulPurchaseRedirect(User, song.Name, song.Artist.Name));
        }
      }

      return View(purchasableSongs);
    }

    public ActionResult RefundSong(int User, int? Song = null, string Message = "", string MessageStyle = "")
    {
      var user = db.Users.Find(User);
      var usersPurchases = user.UserSongs;
      var purchase = usersPurchases.FirstOrDefault(up => up.UserId == user.Id && up.SongId == Song);
      var song = db.Songs.Find(Song);
      ViewBag.UserRequestingRefund = user;
      ViewBag.Message = Message;
      ViewBag.MessageStyle = MessageStyle;

      if (purchase != null)
      {
        jTunesHelper.RefundSong(purchase.Id);

        if (jTunesHelper.RefundSuccessful(purchase.Id))
        {
          return RedirectToAction("RefundSong", jTunesHelper.SuccessfulRefundRedirect(User, song.Name, song.Artist.Name));
        }
        else
        {
          return RedirectToAction("RefundSong", jTunesHelper.UnsuccessfulRefundRedirect(User, song.Name, song.Artist.Name));
        }
      }

      return View(usersPurchases);
    }


    // GET: Songs/Details/5
    public ActionResult Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Song song = db.Songs.Find(id);
      if (song == null)
      {
        return HttpNotFound();
      }
      return View(song);
    }

    // GET: Songs/Create
    public ActionResult Create()
    {
      ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name");
      return View();
    }

    // POST: Songs/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,ArtistId,Price,Name")] Song song)
    {
      if (ModelState.IsValid)
      {
        var duplicateSong = db.Songs.FirstOrDefault(s => s.Name.ToLower() == song.Name.ToLower() && s.ArtistId == song.ArtistId);

        if (duplicateSong != null)
        {
          return RedirectToAction("Index", new { Message = $"Could not create song. {song.Name} is already in the database under {db.Artists.Find(song.ArtistId).Name}.", MessageStyle = "text-danger" });
        }

        db.Songs.Add(song);
        db.SaveChanges();
        return RedirectToAction("Index", new { Message = $"{song.Name} by {db.Artists.Find(song.ArtistId).Name} successfully added to the database.", MessageStyle = "text-success" });
      }

      ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", song.ArtistId);
      return View(song);
    }

    // GET: Songs/Edit/5
    public ActionResult Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Song song = db.Songs.Find(id);
      if (song == null)
      {
        return HttpNotFound();
      }
      ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", song.ArtistId);
      return View(song);
    }

    // POST: Songs/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,ArtistId,Price,Name")] Song song)
    {
      if (ModelState.IsValid)
      {
        db.Entry(song).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      ViewBag.ArtistId = new SelectList(db.Artists, "Id", "Name", song.ArtistId);
      return View(song);
    }

    // GET: Songs/Delete/5
    public ActionResult Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Song song = db.Songs.Find(id);
      if (song == null)
      {
        return HttpNotFound();
      }
      return View(song);
    }

    // POST: Songs/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
      Song song = db.Songs.Find(id);
      db.Songs.Remove(song);
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
