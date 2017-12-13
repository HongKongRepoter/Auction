using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions; // Password regex

namespace Auctions.Models
{
    public class BidViewModel
    {
        [Required(ErrorMessage="Your bid cannot be empty")]

        public float Bid {get;set;}
    }

}