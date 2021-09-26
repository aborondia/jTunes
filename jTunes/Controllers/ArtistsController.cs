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
  public class ArtistsController : Controller
  {
    private jTunesDBEntities db = new jTunesDBEntities();

    public ActionResult Index(string Message = "", string MessageStyle = "")
    {
      ViewBag.Message = Message;
      ViewBag.MessageStyle = MessageStyle;
      return View(db.Artists.ToList());
    }

    public ActionResult TopArtistBySales()
    {
      var artist = jTunesHelper.GetTopSellingArtist();
      var individualSales = jTunesHelper.GetTopArtistIndividualSongSales();
      ViewBag.ArtistName = artist.Name;
      ViewBag.TotalSales = jTunesHelper.GetTopSellingArtistSales();

      return View(individualSales);
    }

    public ActionResult ArtistSongSales()
    {
      return View(jTunesHelper.GetArtistsWithSongs());
    }

    public ActionResult Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Artist artist = db.Artists.Find(id);
      if (artist == null)
      {
        return HttpNotFound();
      }
      return View(artist);
    }

    public ActionResult Create()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,Name")] Artist artist)
    {
      if (ModelState.IsValid)
      {
        var duplicateArtistName = db.Artists.FirstOrDefault(a => a.Name == artist.Name);

        if (duplicateArtistName != null)
        {
          return RedirectToAction("Index", new { Message = $"Could not create artist. The artist name {artist.Name} is already in the database.", MessageStyle = "text-danger" });
        }

        db.Artists.Add(artist);
        db.SaveChanges();
        return RedirectToAction("Index", new { Message = $"Artist {artist.Name} successfully added to the database.", MessageStyle = "text-success" });
      }

      return View(artist);
    }

    public ActionResult Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Artist artist = db.Artists.Find(id);
      if (artist == null)
      {
        return HttpNotFound();
      }
      return View(artist);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,Name")] Artist artist)
    {
      if (ModelState.IsValid)
      {
        db.Entry(artist).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      return View(artist);
    }

    public ActionResult Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Artist artist = db.Artists.Find(id);
      if (artist == null)
      {
        return HttpNotFound();
      }
      return View(artist);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
      Artist artist = db.Artists.Find(id);
      db.Artists.Remove(artist);
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
