using System;
using System.ComponentModel.DataAnnotations;

namespace MyLand.Models
{
    public class Listing
    {
        [Key]
        public int ListingId { get; set; }
        public int ListingType { get; set; }
        public string ListingTitle { get; set; }
        public string ListingDescription { get; set; }
        public int ListingPrice { get; set; }
        public int ListingSize { get; set; }
        public string ListingPhoto { get; set; }
        public string ListingDate { get; set; }
        public string UserName { get; set; }
        public int ListingActive { get; set; }
    }
}
