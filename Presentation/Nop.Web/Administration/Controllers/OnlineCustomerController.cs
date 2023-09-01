using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Models.Customers;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Services.Stores;

namespace Nop.Admin.Controllers
{
    public partial class OnlineCustomerController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly CustomerSettings _customerSettings;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreService _storeService;

        #endregion

        #region Constructors

        public OnlineCustomerController(ICustomerService customerService,
            IGeoLookupService geoLookupService, IDateTimeHelper dateTimeHelper,
            CustomerSettings customerSettings,
            IPermissionService permissionService, ILocalizationService localizationService,
            IStoreService storeService)
        {
            this._customerService = customerService;
            this._geoLookupService = geoLookupService;
            this._dateTimeHelper = dateTimeHelper;
            this._customerSettings = customerSettings;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
            this._storeService = storeService;
        }

        #endregion
        
        #region Methods

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            var customers = _customerService.GetOnlineCustomers(DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes),
                null, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = customers.Select(PrepareOnlineCustomerModelForList),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }


        [NonAction]
        protected virtual OnlineCustomerModel PrepareOnlineCustomerModelForList(Customer customer)
        {
            var store = _storeService.GetStoreById(customer.RegisteredInStoreId);

            return new OnlineCustomerModel
            {
                Id = customer.Id,
                StoreName = store != null ? store.Name : "Unknown store",
                CustomerInfo = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                LastIpAddress = customer.LastIpAddress,
                Location = _geoLookupService.LookupCountryName(customer.LastIpAddress),
                LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc),
                LastVisitedPage = _customerSettings.StoreLastVisitedPage ?
                        customer.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage) :
                        _localizationService.GetResource("Admin.Customers.OnlineCustomers.Fields.LastVisitedPage.Disabled"),
            };
        }

        #endregion
    }
}
