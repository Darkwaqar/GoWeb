using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.ShopTheLook
{
    public partial class PointerModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.ShopTheLook.Pointer.X")]
        public decimal X { get; set; }
        [NopResourceDisplayName("Admin.ShopTheLook.Pointer.Y")]
        public decimal Y { get; set; }
        [NopResourceDisplayName("Admin.ShopTheLook.Pointer.TaggedProductId")]
        public int TaggedProductId { get; set; }
        [NopResourceDisplayName("Admin.ShopTheLook.Pointer.ProductId")]
        public int ProductId { get; set; }
        [NopResourceDisplayName("Admin.ShopTheLook.Pointer.PictureId")]
        public int PictureId { get; set; }
    }
}