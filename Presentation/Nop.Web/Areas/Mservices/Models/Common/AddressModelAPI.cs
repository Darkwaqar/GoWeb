﻿namespace Nop.Web.MServices.Models.Common
{
   
    public partial class AddressMobileModel
    {

        public int Id { get; set; }

        public string FirstName { get; set; }
    
        public string LastName { get; set; }
        
        public string Email { get; set; }

        public string Company { get; set; }

        public int? CountryId { get; set; }
    
        public string CountryName { get; set; }

        public int? StateProvinceId { get; set; }
        
        public string StateProvinceName { get; set; }
        public string City { get; set; }
       
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string ZipPostalCode { get; set; }

        public string PhoneNumber { get; set; }

    }
}