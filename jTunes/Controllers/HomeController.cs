using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jTunes.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult About()
    {
      ViewBag.Message = "jTunes is the up and coming definitive music streaming site!";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Send any inquiries to 123 Fake St., Nowhere, NO, Box10001010";

      return View();
    }
  }
}