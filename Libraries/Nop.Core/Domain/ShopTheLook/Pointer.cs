namespace Nop.Core.Domain.ShopTheLook
{
    /// <summary>
    /// Represents a Pointer
    /// </summary>
    public partial class Pointer : BaseEntity
    {
        /// <summary>
        /// Gets or sets the pointer X
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Gets or sets the pointer Y
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        /// Gets or sets the pointer TaggedProductId
        /// </summary>
        public int TaggedProductId { get; set; }

        /// <summary>
        /// Gets or sets the pointer ProductId
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the pointer PictureId
        /// </summary>
        public int PictureId { get; set; }
    }
}
