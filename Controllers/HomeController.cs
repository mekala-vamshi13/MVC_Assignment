using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;

namespace User_Management.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {

                return View();
            }
        public ActionResult ResetPassword(string token)
        {
          
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(string token, string newPassword, string confirmPassword)
        {
            // Check if the passwords match
            if (newPassword != confirmPassword)
            {
                ViewBag.Message = "Passwords do not match.";
                return View();
            }
            ViewBag.Message = "Your password has been reset successfully!";
            return RedirectToAction("Index"); // Redirect to a confirmation page or login
        }
        public ActionResult Login()
        {
            return View();
        }



    }
    /*   
       */

    /*
      public ActionResult About()
      {
          ViewBag.Message = "Your application description page.";

          return View();
      }

      public ActionResult Contact()
      {
          ViewBag.Message = "Your contact page.";

          return View();
      }
  }*/

}