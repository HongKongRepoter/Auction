using System;
using System.Collections.Generic; // to use Lists
using System.ComponentModel.DataAnnotations.Schema;

namespace Auctions.Models
{
    public class Bid
    {
        public int BidId { get; set; }
        public int AuctionId {get;set;}
        public Auction Auction {get;set;}
        public int UserId {get;set;}
        public User Bidder {get;set;}
        public float Amount {get;set;}
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }



}