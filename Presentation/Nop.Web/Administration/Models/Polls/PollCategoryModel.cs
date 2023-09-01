using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Admin.Validators.Polls;
using Nop.Core.Domain.Media;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Polls
{
    [Validator(typeof(PollCategoryValidator))]
    public partial class PollCategoryModel : BaseNopEntityModel, ILocalizedModel<PollCategoryLocalizedModel>
    {
        public PollCategoryModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }
            Locales = new List<PollCategoryLocalizedModel>();
           

            SelectedCustomerRoleIds = new List<int>();
            AvailableCustomerRoles = new List<SelectListItem>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();

            AvailableVendors = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Parent")]
        public int ParentPollCategoryId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSize")]
        public int PageSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.AllowCustomersToSelectPageSize")]
        public bool AllowCustomersToSelectPageSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PageSizeOptions")]
        public string PageSizeOptions { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.ShowOnHomePage")]
        public bool ShowOnHomePage { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.IncludeInTopMenu")]
        public bool IncludeInTopMenu { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<PollCategoryLocalizedModel> Locales { get; set; }


        //ACL (customer roles)
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.AclCustomerRoles")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedCustomerRoleIds { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }

        //store mapping
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }



        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.VendorId")]
        public int VendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        //picture thumbnail
        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; internal set; }

        #region Nested classes

        public partial class PollCategoryPollModel : BaseNopEntityModel
        {
            public int PollCategoryId { get; set; }

            public int PollId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Polls.Fields.Poll")]
            public string PollName { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Polls.Fields.IsFeaturedPoll")]
            public bool IsFeaturedPoll { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Categories.Polls.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
        }

        public partial class SearchablePollCategoryModel : BaseNopEntityModel
        {

            [NopResourceDisplayName("Admin.Catalog.Categories.Polls.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
            public string PollCategoryName { get; set; }
            public string PollCategoryPictureUrl { get; set; }
            public string SearchablePictureUrl { get; set; }
        }

        public partial class CollectionPollCategoryModel : BaseNopEntityModel
        {

            [NopResourceDisplayName("Admin.Catalog.Categories.Polls.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }
            public string PollCategoryName { get; set; }
            public string PollCategoryPictureUrl { get; set; }
            public string CollectionLogoUrl { get; set; }
            public string CollectionPictureUrl { get; set; }
            public string CollectionDiscription { get; set; }
            public string CollectionName { get; set; }
        }

        public partial class AddPollCategoryPollModel : BaseNopModel
        {
            public AddPollCategoryPollModel()
            {
                AvailableCategories = new List<SelectListItem>();
                AvailableManufacturers = new List<SelectListItem>();
                AvailableStores = new List<SelectListItem>();
                AvailableVendors = new List<SelectListItem>();
            }

            [NopResourceDisplayName("Admin.Catalog.Polls.List.SearchPollName")]
            [AllowHtml]
            public string SearchPollName { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Polls.List.SearchPollCategory")]
            public int SearchPollCategoryId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Polls.List.SearchManufacturer")]
            public int SearchManufacturerId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Polls.List.SearchStore")]
            public int SearchStoreId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Polls.List.SearchVendor")]
            public int SearchVendorId { get; set; }
            [NopResourceDisplayName("Admin.Catalog.Polls.List.SearchPollType")]
            public int SearchPollTypeId { get; set; }

            public IList<SelectListItem> AvailableCategories { get; set; }
            public IList<SelectListItem> AvailableManufacturers { get; set; }
            public IList<SelectListItem> AvailableStores { get; set; }
            public IList<SelectListItem> AvailableVendors { get; set; }
            public int PollCategoryId { get; set; }

            public int[] SelectedPollIds { get; set; }
        }

        #endregion
    }

    public partial class PollCategoryLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.Description")]
        [AllowHtml]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaKeywords")]
        [AllowHtml]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaDescription")]
        [AllowHtml]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.MetaTitle")]
        [AllowHtml]
        public string MetaTitle { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.Fields.SeName")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}