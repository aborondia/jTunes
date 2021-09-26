using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jTunes.Models
{
  public static class jTunesHelper
  {
    private static jTunesDBEntities db = new jTunesDBEntities();

    public static SelectList UserSelect = new SelectList(db.Users, "Id", "Name");
    public static SelectList SongSelect = new SelectList(db.Songs, "Id", "Name");
    public static SelectList YearSelect
    {
      get
      {
        List<SelectListItem> yearList = new List<SelectListItem>();

        for (int i = 2000; i <= DateTime.Now.Year; i++)
        {
          yearList.Add(new SelectListItem() { Value = i.ToString() });
        }

        return new SelectList(yearList, "Value", "Value");
      }
    }

    public static SelectList MonthSelect
    {
      get
      {
        List<SelectListItem> monthList = new List<SelectListItem>();

        for (int i = 1; i <= 12; i++)
        {
          monthList.Add(new SelectListItem() { Text = MonthNumberToName(i), Value = i.ToString() });
        }

        return new SelectList(monthList, "Value", "Text");
      }
    }

    public static string MonthNumberToName(int number)
    {
      switch (number)
      {
        case 1: return "January";
        case 2: return "February";
        case 3: return "March";
        case 4: return "April";
        case 5: return "May";
        case 6: return "June";
        case 7: return "July";
        case 8: return "August";
        case 9: return "September";
        case 10: return "October";
        case 11: return "November";
        case 12: return "December";
        default: return null;
      }
    }


    private static IGrouping<int, UserSong> GetTopSongWithSales()
    {
      var groupedSongSales = db.UserSongs.GroupBy(ss => ss.SongId)
  .OrderByDescending(ss => ss.Count());
      var highestSalesSong = groupedSongSales.FirstOrDefault();

      return highestSalesSong;
    }

    public static Song GetTopSellingSong()
    {
      int songId = GetTopSongWithSales().Select(ss => ss.SongId).FirstOrDefault();
      return db.Songs.Find(songId);
    }

    public static int GetTopSellingSongSales()
    {
      return GetTopSongWithSales().Count();
    }

    private static IGrouping<int, UserSong> GetTopSellingArtistWithSales()
    {
      var artistsWithSales = db.UserSongs.GroupBy(aws => aws.Song.ArtistId);
      var topArtistWithSales = artistsWithSales.OrderByDescending(aws => aws.Count()).FirstOrDefault();


      return topArtistWithSales;
    }

    public static Artist GetTopSellingArtist()
    {
      var artistId = GetTopSellingArtistWithSales().Select(taws => taws.Song.ArtistId).FirstOrDefault();
      return db.Artists.Find(artistId);
    }

    public static int GetTopSellingArtistSales()
    {
      return GetTopSellingArtistWithSales().Count();
    }

    public static Dictionary<string, int> GetTopArtistIndividualSongSales()
    {
      var topSellingArtistWithSales = GetTopSellingArtistWithSales();
      var songsWithIndividualSales = topSellingArtistWithSales.GroupBy(us => us.Song.Name);
      var songsWithSalesTotal = songsWithIndividualSales.ToDictionary(song => song.Key, song => song.Count());

      return songsWithSalesTotal;
    }

    public static int? GetValidRating(int? rating)
    {
      if (rating == null || rating < 1)
      {
        rating = 1;
      }

      if (rating > 10)
      {
        rating = 10;
      }

      return rating;
    }

    public static Dictionary<KeyValuePair<Song, int>, double> GetTopThreeRatedSongs()
    {
      int chartNumber = 1;
      var songsWithRatings = db.UserSongs
        .GroupBy(us => us.Song.Id)
        .OrderByDescending(us => us.Average(s => s.Rating));
      var topThreeSongsWithSales = songsWithRatings
        .Take(3)
        .ToDictionary(
        swr => new KeyValuePair<Song, int>(db.Songs.First(s => s.Id == swr.Key), chartNumber++),
        swr => Math.Round((double)swr.Average(sr => sr.Rating), 2)
        );

      return topThreeSongsWithSales;
    }

    public static void PurchaseSong(int userId, int songId)
    {
      var user = db.Users.Find(userId);
      var song = db.Songs.Find(songId);

      if (user.Money >= song.Price)
      {
        var purchase = new UserSong();

        user.Money -= song.Price;
        purchase.UserId = userId;
        purchase.SongId = songId;
        purchase.PurchaseDate = DateTime.Now;
        db.UserSongs.Add(purchase);
        db.SaveChanges();
      }
    }

    public static bool PurchaseSuccessful(int userId, int? songId)
    {
      var purchase = db.UserSongs.FirstOrDefault(us => us.UserId == userId && us.SongId == songId);

      return purchase == null ? false : true;

    }

    public static void RefundSong(int userSongId)
    {
      var purchase = db.UserSongs.Find(userSongId);
      var user = db.Users.Find(purchase.UserId);
      if (purchase.PurchaseDate.AddDays(15) >= DateTime.Now)
      {
        user.Money += purchase.Song.Price;
        db.UserSongs.Remove(purchase);
        db.SaveChanges();
      }
    }

    public static bool RefundSuccessful(int purchaseId)
    {
      var purchase = db.UserSongs.Find(purchaseId);

      return purchase == null ? true : false;

    }

    public static Dictionary<string, IEnumerable<IGrouping<int, Song>>> GetArtistsWithSongs()
    {
      var artistsWithSongs = db.Songs.GroupBy(s => s.ArtistId).ToList();
      var artistsWithSongsDictionary = new Dictionary<string, IEnumerable<IGrouping<int, Song>>>();

      foreach (var artistEntry in artistsWithSongs)
      {
        var artistName = db.Artists.Find(artistEntry.Key).Name;
        artistsWithSongsDictionary[artistName] = artistEntry.GroupBy(s => GetTotalSongSales(s.Id));
      }

      return artistsWithSongsDictionary;
    }

    public static int GetTotalSongSales(int songId)
    {
      return db.UserSongs.Where(us => us.SongId == songId).Count();
    }
  }
}