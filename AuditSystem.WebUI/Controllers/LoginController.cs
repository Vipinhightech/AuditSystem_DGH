using AuditSystem.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AuditSystem.WebUI.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AuthRequest auth)
        {
            if (!ModelState.IsValid)
            {
                return View(auth);
            }
            LoginManager oAuth = new LoginManager();
            auth.AppID = oAuth.GetAppId(auth.UserId);
            AuthResponse oAuthResponse = oAuth.ValidateUser(auth);

            if (oAuthResponse == null || oAuthResponse.APPLICATION != auth.AppID.ToString())
            {
                ModelState.AddModelError("", "Login failed");
                return View();
            }


            if (Convert.ToBoolean(oAuthResponse.Status) == true)
            {
                string userData = string.Join(",", oAuthResponse.UserRole);
                //FormsAuthentication.SetAuthCookie(authResponse.UserId, false);
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1, // Ticket version
                oAuthResponse.UserId, // Username
                DateTime.Now, // Issue date
                DateTime.Now.AddMinutes(15), // Expiration
                false, // Persistent
                userData, // User data (roles)
                FormsAuthentication.FormsCookiePath);

                // Encrypt the ticket
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                // Create the cookie
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);

                Session["UserName"] = oAuthResponse.UserName;
                Session["UserId"] = oAuthResponse.UserId;
                Session["Roles"] = oAuthResponse.UserRole;

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Login attempt");
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Login");
        }

        //private AuthResponse IsvalidUser(AuthRequest auth)
        //{
        //    AuthResponse response = new AuthResponse { UserRole = new List<string>() };
        //    if (auth.userName == "test" && auth.Password == "Test")
        //    {
        //        response.UserId = "test";
        //        response.Status = "True";
        //        response.UserName = auth.userName;
        //        response.UserRole.Add("User");
        //    }
        //    if (auth.userName == "admin" && auth.Password == "Admin")
        //    {
        //        response.UserId = "admin";
        //        response.Status = "True";
        //        response.UserName = auth.userName;
        //        response.UserRole.Add("Admin");
        //        response.UserRole.Add("User");
        //    }
        //    return response;
        //}
    }
}