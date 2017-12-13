using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions; // Password regex

namespace Auctions.Models
{
    public class AuctionViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [MinLength(3, ErrorMessage = "Product name must be at least 3 characters")]
        [DataType(DataType.Text)]
        public string Name {get;set;}

        [Required(ErrorMessage = "Product description is required")]
        [MinLength(10, ErrorMessage = "Product description must be at least 10 characters")]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Starting bid is required")]
        [MinValue]
        public float Bid {get;set;}

        [Required(ErrorMessage = "End date is required")]
        [ValidateDate]
        [DataType(DataType.Date)]
        public DateTime EndDate {get;set;}
    }
    public class MinValue : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if((float)value>0)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Starting bid is required");
        }
    } 
    public class ValidateDate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime Today = DateTime.Now;
            if ((DateTime)value >= Today)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Cannot have your end date in the past");
        }
    }
}