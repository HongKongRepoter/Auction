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
    public class AuctionController : Controller
    {
        private AuctionContext _context { get; set; }
        public AuctionController(AuctionContext context)
        {
            _context = context;
        }
        private User GetLoggedUser()
        {
            return _context.Users.SingleOrDefault(user => user.UserId == (int)HttpContext.Session.GetInt32("UserId"));
        }
        private List<Auction> GetAuctions()
        {
            return _context.Auctions.Include(a=>a.Seller).Include(a=>a.Bids).ThenInclude(bb=>bb.Bidder).OrderBy(aa=>aa.EndDate).ToList();
        }
        private List<Auction> CheckAuction()
        {
            List<Auction> Auctions = GetAuctions();
            foreach(Auction auction in Auctions)
            {
                TimeSpan Remain = auction.EndDate - DateTime.Now;
                if(Remain.Days<=0)
                {
                    if(auction.Bids.Count>0)
                    {
                        User TopBidder = _context.Users.SingleOrDefault(u=>u.UserId==auction.HighestBid);
                        TopBidder.Wallet = TopBidder.Wallet - auction.StartBid;
                        auction.Seller.Wallet = auction.Seller.Wallet + auction.StartBid;
                    }
                    _context.Auctions.Remove(auction);
                    foreach(Bid bid in auction.Bids)
                    {
                        _context.Bids.Remove(bid);
                    }
                }
            }
            _context.SaveChanges();
            return GetAuctions();
        }
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.LoggedUser = GetLoggedUser();
            ViewBag.Auctions = CheckAuction();
            return View();
        }
        [HttpGet]
        [Route("Auction/New")]
        public IActionResult New()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [Route("Auction/Create")]
        public IActionResult Create(AuctionViewModel model)
        {
            if(ModelState.IsValid)
            {
                Auction NewAuction = new Auction(); 
                NewAuction.Name = model.Name; 
                NewAuction.Description = model.Description;
                NewAuction.SellerId = (int)HttpContext.Session.GetInt32("UserId"); 
                NewAuction.StartBid = model.Bid; 
                NewAuction.EndDate =  model.EndDate; 
                NewAuction.CreatedAt = DateTime.Now;
                NewAuction.UpdatedAt = DateTime.Now;
                _context.Auctions.Add(NewAuction);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View("New");
        }
        [HttpGet]
        [Route("Auction/Show/{AuctionId}")]
        public IActionResult Show(int AuctionId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Auction = _context.Auctions.Include(a=>a.Seller).Include(a=>a.Bids).ThenInclude(bb=>bb.Bidder).SingleOrDefault(aa=>aa.AuctionId==AuctionId);
            return View();
        }
        [HttpPost]
        [Route("Auction/NewBid/{AuctionId}")]
        public IActionResult NewBid(int AuctionId, BidViewModel model)
        {
            Auction Auction = _context.Auctions.Include(a => a.Seller).Include(a => a.Bids).ThenInclude(bb => bb.Bidder).SingleOrDefault(aa => aa.AuctionId == AuctionId);
            ViewBag.Auction = Auction;
            User LoggedUser = GetLoggedUser();
            if(ModelState.IsValid)
            {
                if(Auction.SellerId==LoggedUser.UserId)
                {
                    ViewBag.TooLow = "You cannot place bids on your own item";
                } else {
                    if (model.Bid > LoggedUser.Wallet)
                    {
                        ViewBag.TooLow = "You do not have enough money in your wallet";
                    }
                    else
                    {
                        if (model.Bid < Auction.StartBid)
                        {
                            ViewBag.TooLow = "Bid cannot be less than highest bid";
                        }
                        else
                        {
                            Bid NewBid = new Bid();
                            NewBid.Amount = model.Bid;
                            NewBid.AuctionId = Auction.AuctionId;
                            NewBid.UserId = (int)HttpContext.Session.GetInt32("UserId");
                            Auction.StartBid = model.Bid;
                            Auction.HighestBid = (int)HttpContext.Session.GetInt32("UserId");
                            _context.Bids.Add(NewBid);
                            _context.SaveChanges();
                            return RedirectToAction("Dashboard");
                        }
                    }
                }
            }
            return View("Show"); 
        }
        [HttpPost]
        [Route("Auction/Delete/{AuctionId}")]
        public IActionResult Delete(int AuctionId)
        {
            Auction RemoveAuction = _context.Auctions.Include(a => a.Seller).Include(a => a.Bids).ThenInclude(bb => bb.Bidder).SingleOrDefault(aa=>aa.AuctionId==AuctionId);
            _context.Auctions.Remove(RemoveAuction);
            foreach (Bid bid in RemoveAuction.Bids)
            {
                _context.Bids.Remove(bid);
            }
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
