using System;
using System.Collections.Generic; // to use Lists
using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username {get;set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public float Wallet {get;set;}
        public List<Auction> MyAuctions {get;set;}
        public List<Bid> MyBids {get;set;}
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public User()
        {
            MyAuctions = new List<Auction>();
            MyBids = new List<Bid>();
        }
    }
}