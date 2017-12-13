using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Auctions.Models;
using Microsoft.AspNetCore.Http; // to use Sessions
using Microsoft.EntityFrameworkCore; // use Entity
using Microsoft.AspNetCore.Identity;

namespace Auctions.Controllers
{
    public class HomeController : Controller
    {
        private AuctionContext _context {get;set;}
        public HomeController(AuctionContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            if(HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Dashboard", "Auction");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            User CheckUser = _context.Users.SingleOrDefault(user => user.Username == model.Username);
            if (CheckUser != null)
            {
                ViewBag.Err = "Username is already registered";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    User NewUser = new User();
                    NewUser.Username = model.Username;
                    NewUser.FirstName = model.FirstName;
                    NewUser.LastName = model.LastName;
                    NewUser.Password = Hasher.HashPassword(NewUser, model.Password);
                    NewUser.Wallet = 1000;
                    NewUser.CreatedAt = DateTime.Now;
                    NewUser.UpdatedAt = DateTime.Now;
                    _context.Users.Add(NewUser);
                    _context.SaveChanges();
                    User loggedUser = _context.Users.SingleOrDefault(user => user.Username == model.Username);
                    HttpContext.Session.SetInt32("UserId", loggedUser.UserId);
                    return RedirectToAction("Index");
                }
            }
            return View("Index");
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login(string LogUser, string LogPassword)
        {
            if (LogPassword != null || LogUser != null)
            {
                User CheckUser = _context.Users.SingleOrDefault(user => user.Username == LogUser);
                if (CheckUser != null) // if user was found
                {
                    var Hasher = new PasswordHasher<User>();
                    if (0 != Hasher.VerifyHashedPassword(CheckUser, CheckUser.Password, LogPassword))
                    {
                        HttpContext.Session.SetInt32("UserId", CheckUser.UserId);
                        // if password matched
                        return RedirectToAction("Index");
                    }

                }
                ViewBag.Err = "Username and/or Password is incorrect";
            }
            return View("Index");
        }

        [HttpPost]
        [Route("Home/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
