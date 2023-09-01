using System;

namespace Nop.Core.Domain.Fcm
{
    public partial class Device : BaseEntity, ICloneable
    {
        /// <summary>
        /// Gets or sets a value indicating the device id 
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the device brand 
        /// </summary>
        public string Brand { get; set; }


        /// <summary>
        /// Gets or sets a value indicating the device Os version 
        /// </summary>
        public string OSVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the device Carrier 
        /// </summary>
        public string Carrier { get; set; }


        /// <summary>
        /// Gets or sets a value indicating the device OS 
        /// </summary>
        public FcmApplicationType DeviceOS { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the device RAM
        /// </summary>
        public string DeviceRam { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the device Package
        /// </summary>
        public string Package { get; set; }


        /// <summary>
        /// Gets or sets a value indicating the device longitude
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the device latitude
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Device is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value if error accuring during sending of notification
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance Update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }


        public int CustomerId { get; set; }

        public object Clone()
        {
            var devic = new Device
            {
                CustomerId=this.CustomerId,
                DeviceId = this.DeviceId,
                Brand = this.Brand,
                OSVersion = this.OSVersion,
                Carrier = this.Carrier,
                DeviceOS = this.DeviceOS,
                DeviceRam = this.DeviceRam,
                Longitude = this.Longitude,
                Latitude = this.Latitude,
                Active = this.Active,
                CreatedOnUtc = this.CreatedOnUtc,
                UpdatedOnUtc = this.UpdatedOnUtc,
            };
            return devic;
        }
    }
}
