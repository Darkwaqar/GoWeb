using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Stores;
using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Banners
{
    /// <summary>
    /// Represents a banner
    /// </summary>
    public partial class Banner : BaseEntity, ILocalizedEntity, IStoreMappingSupported
    {

        private ICollection<BannerPicture> _productPictures;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }


        public int VendorId { get; set; }

        public bool Published { get; set; }

        public int DisplayOrder { get; set; }


        /// <summary>
        /// Gets or sets the collection of BannerPicture
        /// </summary>
        public virtual ICollection<BannerPicture> BannerPictures
        {
            get { return _productPictures ?? (_productPictures = new List<BannerPicture>()); }
            protected set { _productPictures = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
        /// </summary>
        public bool LimitedToStores { get; set; }
    }
}
