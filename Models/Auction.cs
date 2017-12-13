using System;
using System.Collections.Generic; // to use Lists
using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Models
{
    public class Auction
    {
        public int AuctionId {get;set;}
        public string Name {get;set;}
        public string Description {get;set;}
        public int SellerId { get;set;}
        public User Seller {get;set;}
        public float StartBid {get;set;}
        public int HighestBid {get;set;}
        public DateTime EndDate {get;set;}
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}
        public List<Bid> Bids {get;set;}
        public Auction()
        {
            Bids = new List<Bid>();
        }
    }



}