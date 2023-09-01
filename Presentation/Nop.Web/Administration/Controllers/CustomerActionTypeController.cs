using Nop.Admin.Extensions;
using Nop.Admin.Models.Customers;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class CustomerActionTypeController : BaseAdminController
    {

        #region Fields
        private readonly ICustomerActionService _customerActionService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        #endregion Fields

        #region Constructors

        public CustomerActionTypeController(
            ICustomerActionService customerActionService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService)
        {
            this._customerActionService = customerActionService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
        }

        #endregion

        #region Methods

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedKendoGridJson();

            var models = _customerActionService.GetCustomerActionType()
                .Select(x => x.ToModel())
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = models,
                Total = models.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ActionUpdate(CustomerActionTypeModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var activityTypes = _customerActionService.GetCustomerActionTypeById(model.Id);
            if (activityTypes == null)
                return Content("Action Type cannot be loaded");

            activityTypes.Enabled = model.Enabled;
            _customerActionService.UpdateCustomerActionType(activityTypes);

            //activity log
            _customerActivityService.InsertActivity("EditActionType", _localizationService.GetResource("ActivityLog.EditTask"), activityTypes.Id);

            return new NullJsonResult();
        }

        #endregion
    }

}