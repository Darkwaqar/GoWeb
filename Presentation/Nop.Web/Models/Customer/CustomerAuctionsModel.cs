using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Customer
{
    public partial class CustomerAuctionsModel : BaseNopModel
    {
        public CustomerAuctionsModel()
        {
            Items = new List<CustomerAuctionModel>();
        }
        public IList<CustomerAuctionModel> Items { get; set; }
        public int CustomerId { get; set; }


        #region Nested classes
        public partial class CustomerAuctionModel : BaseNopModel
        {
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string CurrentBidAmount { get; set; }
            public decimal CurrentBidAmountValue { get; set; }
            public string BidAmount { get; set; }
            public decimal BidAmountValue { get; set; }
            public DateTime EndBidDate { get; set; }
            public bool Ended { get; set; }
            public bool HighestBidder { get; set; }
            public int? OrderId { get; set; }
        }
        #endregion
    }
}