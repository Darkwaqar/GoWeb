using FluentValidation.Attributes;
using Nop.Admin.Validators.Magazines;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Admin.Models.Magazines
{
    [Validator(typeof(MagazineValidator))]
    public class MagazineModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.Download")]
        [UIHint("Download")]
        public int DownloadId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.UnlimitedDownloads")]
        public bool UnlimitedDownloads { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.MaxNumberOfDownloads")]
        public int MaxNumberOfDownloads { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.DownloadExpirationDays")]
        [UIHint("Int32Nullable")]
        public int? DownloadExpirationDays { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.HasSampleDownload")]
        public bool HasSampleDownload { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.SampleDownload")]
        [UIHint("Download")]
        public int SampleDownloadId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.HasUserAgreement")]
        public bool HasUserAgreement { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.UserAgreementText")]
        [AllowHtml]
        public string UserAgreementText { get; set; }

        //picture thumbnail
        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalogn.Magazine.Fields.PictureId")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.CreatedOnUtc")]
        public DateTime? CreatedOn { get; set; }
        [NopResourceDisplayName("Admin.Catalog.Magazine.Fields.UpdatedOnUtc")]
        public DateTime? UpdatedOn { get; set; }
    }
}