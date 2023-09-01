using System;

namespace Nop.Web.Areas.Mservices.Models.Customer
{
    public class CustomerApi
    {
        public Guid UserID { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public int NumberOfCartItems { get; set; }
        public int NumberOfWishlistItems { get; set; }
    }
}