using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Catalog;
using Nop.Admin.Models.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Services.Logging;
using Nop.Core.Domain.Customers;
using Nop.Services;
using Nop.Services.Media;

namespace Nop.Admin.Controllers
{
    public class CustomerReminderController : BaseAdminController
    {

        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerTagService _customerTagService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly ICustomerReminderService _customerReminderService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IMessageTokenProvider _messageTokenProvider;

        #endregion

        #region Constructors

        public CustomerReminderController(ICustomerService customerService,
            ICustomerActivityService customerActivityService,
            ICustomerAttributeService customerAttributeService,
            ICustomerTagService customerTagService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IStoreService storeService,
            IVendorService vendorService,
            ICustomerReminderService customerReminderService,
            IEmailAccountService emailAccountService,
            IDateTimeHelper dateTimeHelper,
            ICategoryService categoryService,
            IProductService productService,
            IPictureService pictureService,
            IMessageTokenProvider messageTokenProvider)
        {
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._customerAttributeService = customerAttributeService;
            this._customerTagService = customerTagService;
            this._localizationService = localizationService;
            this._manufacturerService = manufacturerService;
            this._storeService = storeService;
            this._vendorService = vendorService;
            this._customerReminderService = customerReminderService;
            this._emailAccountService = emailAccountService;
            this._dateTimeHelper = dateTimeHelper;
            this._categoryService = categoryService;
            this._productService = productService;
            this._pictureService = pictureService;
            this._messageTokenProvider = messageTokenProvider;
        }

        #endregion

        #region Utilities
        [NonAction]
        public virtual void PrepareReminderLevelModel(CustomerReminderModel.ReminderLevelModel model, CustomerReminder customerReminder)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var emailAccounts = _emailAccountService.GetAllEmailAccounts();
            foreach (var item in emailAccounts)
            {
                model.EmailAccounts.Add(new SelectListItem
                {
                    Text = item.Email,
                    Value = item.Id.ToString()
                });
            }
            model.AllowedTokens = string.Join(", ", _messageTokenProvider.GetListOfCustomerReminderAllowedTokens(customerReminder.ReminderRule));
        }
        #endregion

        #region Customer reminders

        public virtual ActionResult Index() => RedirectToAction("List");

        public virtual ActionResult List() => View();

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            var customeractions = _customerReminderService.GetCustomerReminders();
            var gridModel = new DataSourceResult
            {
                Data = customeractions.Select(x => new { Id = x.Id, Name = x.Name, Active = x.Active, Rule = x.ReminderRule.ToString() }),
                Total = customeractions.Count()
            };
            return Json(gridModel);
        }

        public virtual ActionResult Create()
        {
            var model = new CustomerReminderModel();
            model.StartDateTime = DateTime.Now;
            model.EndDateTime = DateTime.Now.AddMonths(1);
            model.LastUpdateDate = DateTime.Now.AddDays(-7);
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(CustomerReminderModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var customerreminder = model.ToEntity();
                customerreminder.StartDateTimeUtc = _dateTimeHelper.ConvertToUtcTime(model.StartDateTime);
                customerreminder.EndDateTimeUtc = _dateTimeHelper.ConvertToUtcTime(model.EndDateTime);
                customerreminder.LastUpdateDate = _dateTimeHelper.ConvertToUtcTime(model.LastUpdateDate);
                _customerReminderService.InsertCustomerReminder(customerreminder);
                //activity log
                _customerActivityService.InsertActivity("AddNewCustomerReminder", _localizationService.GetResource("ActivityLog.AddNewCustomerReminder"), customerreminder.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerReminder.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = customerreminder.Id }) : RedirectToAction("List");
            }
            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(id);
            if (customerReminder == null)
                return RedirectToAction("List");
            var model = customerReminder.ToModel();
            model.StartDateTime = _dateTimeHelper.ConvertToUserTime(customerReminder.StartDateTimeUtc);
            model.EndDateTime = _dateTimeHelper.ConvertToUserTime(customerReminder.EndDateTimeUtc);
            model.LastUpdateDate = _dateTimeHelper.ConvertToUserTime(customerReminder.LastUpdateDate);
            model.ConditionCount = customerReminder.Conditions.Count();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(CustomerReminderModel model, bool continueEditing)
        {
            var customerreminder = _customerReminderService.GetCustomerReminderById(model.Id);
            if (customerreminder == null)
                return RedirectToAction("List");
            try
            {
                if (ModelState.IsValid)
                {
                    if (customerreminder.Conditions.Count() > 0)
                        model.ReminderRuleId = customerreminder.ReminderRuleId;
                    if (model.ReminderRuleId == 0)
                        model.ReminderRuleId = customerreminder.ReminderRuleId;

                    customerreminder = model.ToEntity(customerreminder);
                    customerreminder.StartDateTimeUtc = _dateTimeHelper.ConvertToUtcTime(model.StartDateTime);
                    customerreminder.EndDateTimeUtc = _dateTimeHelper.ConvertToUtcTime(model.EndDateTime);
                    customerreminder.LastUpdateDate = _dateTimeHelper.ConvertToUtcTime(model.LastUpdateDate);
                    _customerReminderService.UpdateCustomerReminder(customerreminder);
                    _customerActivityService.InsertActivity("EditCustomerReminder", _localizationService.GetResource("ActivityLog.EditCustomerReminder"), customerreminder.Name);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerReminder.Updated"));
                    return continueEditing ? RedirectToAction("Edit", new { id = customerreminder.Id }) : RedirectToAction("List");
                }
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = customerreminder.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult Run(int Id)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(Id);
            if (customerReminder == null)
                return RedirectToAction("List");

            if (customerReminder.ReminderRule == CustomerReminderRuleEnum.AbandonedCart)
                _customerReminderService.Task_AbandonedCart(customerReminder.Id);

            if (customerReminder.ReminderRule == CustomerReminderRuleEnum.Birthday)
                _customerReminderService.Task_Birthday(customerReminder.Id);

            if (customerReminder.ReminderRule == CustomerReminderRuleEnum.LastActivity)
                _customerReminderService.Task_LastActivity(customerReminder.Id);

            if (customerReminder.ReminderRule == CustomerReminderRuleEnum.LastPurchase)
                _customerReminderService.Task_LastPurchase(customerReminder.Id);

            if (customerReminder.ReminderRule == CustomerReminderRuleEnum.RegisteredCustomer)
                _customerReminderService.Task_RegisteredCustomer(customerReminder.Id);

            SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerReminder.Run"));
            return RedirectToAction("Edit", new { id = Id });
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(id);
            if (customerReminder == null)
                return RedirectToAction("List");
            try
            {
                if (ModelState.IsValid)
                {
                    _customerActivityService.InsertActivity("DeleteCustomerReminder", _localizationService.GetResource("ActivityLog.DeleteCustomerReminder"), customerReminder.Name);
                    _customerReminderService.DeleteCustomerReminder(customerReminder);
                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerReminder.Deleted"));
                    return RedirectToAction("List");
                }
                else
                {
                    return RedirectToAction("Edit", new { id = id });
                }
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customerReminder.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult History(DataSourceRequest command, int customerReminderId)
        {
            //we use own own binder for searchCustomerRoleIds property 
            var history = _customerReminderService.GetAllCustomerReminderHistory(customerReminderId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var items = new List<CustomerReminderHistoryModel>();
            foreach (var item in history)
            {
                items.Add(new CustomerReminderHistoryModel
                {
                    Id = item.Id,
                    Email = (_customerService.GetCustomerById(item.CustomerId)).Email,
                    SendDate = _dateTimeHelper.ConvertToUserTime(item.SendDate, DateTimeKind.Utc),
                    Level = item.Level,
                    OrderId = item.OrderId > 0
                });
            }
            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = history.TotalCount
            };
            return Json(gridModel);
        }

        #endregion

        #region Condition

        [HttpPost]
        public virtual ActionResult Conditions(int customerReminderId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            var gridModel = new DataSourceResult
            {
                Data = customerReminder.Conditions.Select(x => new
                { Id = x.Id, Name = x.Name, Condition = x.Condition.ToString() }),
                Total = customerReminder.Conditions.Count()
            };
            return Json(gridModel);
        }

        public virtual ActionResult AddCondition(int customerReminderId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            if (customerReminder == null)
                return RedirectToAction("Edit", new { id = customerReminderId });

            var model = new CustomerReminderModel.ConditionModel();
            model.CustomerReminderId = customerReminder.Id;
            foreach (CustomerReminderConditionTypeEnum item in Enum.GetValues(typeof(CustomerReminderConditionTypeEnum)))
            {
                if (customerReminder.ReminderRule == CustomerReminderRuleEnum.AbandonedCart || customerReminder.ReminderRule == CustomerReminderRuleEnum.CompletedOrder
                    || customerReminder.ReminderRule == CustomerReminderRuleEnum.UnpaidOrder)
                    model.ConditionType.Add(new SelectListItem()
                    {
                        Value = ((int)item).ToString(),
                        Text = item.ToString()
                    });
                else
                {
                    if (item != CustomerReminderConditionTypeEnum.Product &&
                        item != CustomerReminderConditionTypeEnum.Manufacturer &&
                        item != CustomerReminderConditionTypeEnum.Category)
                    {
                        model.ConditionType.Add(new SelectListItem()
                        {
                            Value = ((int)item).ToString(),
                            Text = item.ToString()
                        });

                    }
                }
            }
            return View(model);

        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult AddCondition(CustomerReminderModel.ConditionModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var customerReminder = _customerReminderService.GetCustomerReminderById(model.CustomerReminderId);
                if (customerReminder == null)
                {
                    return RedirectToAction("List");
                }
                var condition = new CustomerReminder.ReminderCondition()
                {
                    Name = model.Name,
                    ConditionTypeId = model.ConditionTypeId,
                    ConditionId = model.ConditionId,
                };
                customerReminder.Conditions.Add(condition);
                _customerReminderService.UpdateCustomerReminder(customerReminder);

                _customerActivityService.InsertActivity("AddNewCustomerReminderCondition", _localizationService.GetResource("ActivityLog.AddNewCustomerReminder"), customerReminder.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerReminder.Condition.Added"));

                return continueEditing ? RedirectToAction("EditCondition", new { customerReminderId = customerReminder.Id, cid = condition.Id }) : RedirectToAction("Edit", new { id = customerReminder.Id });
            }

            return View(model);
        }

        public virtual ActionResult EditCondition(int customerReminderId, int cid)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            if (customerReminder == null)
                return RedirectToAction("List");

            var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == cid);
            if (condition == null)
                return RedirectToAction("List");
            var model = condition.ToModel();
            model.CustomerReminderId = customerReminder.Id;
            foreach (CustomerReminderConditionTypeEnum item in Enum.GetValues(typeof(CustomerReminderConditionTypeEnum)))
            {
                model.ConditionType.Add(new SelectListItem()
                {
                    Value = ((int)item).ToString(),
                    Text = item.ToString()
                });
            }
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult EditCondition(int customerReminderId, int cid, CustomerReminderModel.ConditionModel model, bool continueEditing)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            if (customerReminder == null)
                return RedirectToAction("List");

            var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == cid);
            if (condition == null)
                return RedirectToAction("List");
            try
            {
                if (ModelState.IsValid)
                {
                    condition = model.ToEntity(condition);
                    _customerReminderService.UpdateCustomerReminder(customerReminder);
                    _customerActivityService.InsertActivity("EditCustomerReminderCondition", _localizationService.GetResource("ActivityLog.EditCustomerReminderCondition"), customerReminder.Name);
                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerReminderCondition.Updated"));
                    return continueEditing ? RedirectToAction("EditCondition", new { customerReminderId = customerReminder.Id, cid = condition.Id }) : RedirectToAction("Edit", new { id = customerReminder.Id });
                }
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = customerReminder.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult ConditionDelete(int Id, int customerReminderId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            if (customerReminder != null)
            {
                var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == Id);
                if (condition != null)
                {
                    customerReminder.Conditions.Remove(condition);
                    _customerReminderService.UpdateCustomerReminder(customerReminder);
                }
            }
            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult ConditionDeletePosition(int id, int customerReminderId, int conditionId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == conditionId);

            if (condition.ConditionTypeId == (int)CustomerReminderConditionTypeEnum.Product)
            {
                var product = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "product");
                _customerReminderService.DeleteConditionEntitiy(product);
                _customerReminderService.UpdateCustomerReminder(customerReminder);
            }
            if (condition.ConditionTypeId == (int)CustomerReminderConditionTypeEnum.Category)
            {
                var category = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "category");
                _customerReminderService.DeleteConditionEntitiy(category);
                _customerReminderService.UpdateCustomerReminder(customerReminder);
            }
            if (condition.ConditionTypeId == (int)CustomerReminderConditionTypeEnum.Manufacturer)
            {
                var manufacturer = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "manufacturer");
                _customerReminderService.DeleteConditionEntitiy(manufacturer);
                _customerReminderService.UpdateCustomerReminder(customerReminder);
            }

            if (condition.ConditionTypeId == (int)CustomerReminderConditionTypeEnum.CustomerRole)
            {
                var customerRole = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "customerRole");
                _customerReminderService.DeleteConditionEntitiy(customerRole);
                _customerReminderService.UpdateCustomerReminder(customerReminder);
            }
            if (condition.ConditionTypeId == (int)CustomerReminderConditionTypeEnum.CustomerTag)
            {
                var customertag = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "customerTag");
                _customerReminderService.DeleteConditionEntitiy(customertag);
                _customerReminderService.UpdateCustomerReminder(customerReminder);
            }

            if (condition.ConditionTypeId == (int)CustomerReminderConditionTypeEnum.CustomCustomerAttribute)
            {
                condition.CustomCustomerAttributes.Remove(condition.CustomCustomerAttributes.FirstOrDefault(x => x.Id == id));
                _customerReminderService.UpdateCustomerReminder(customerReminder);
            }
            if (condition.ConditionTypeId == (int)CustomerReminderConditionTypeEnum.CustomerRegisterField)
            {
                condition.CustomerRegistration.Remove(condition.CustomerRegistration.FirstOrDefault(x => x.Id == id));
                _customerReminderService.UpdateCustomerReminder(customerReminder);
            }
            return new NullJsonResult();
        }

        #region Condition Category

        [HttpPost]
        public virtual ActionResult ConditionCategory(int customerReminderId, int conditionId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "category"))
            {
                var category = _categoryService.GetCategoryById(item.EntityId);
                items.Add(Tuple.Create(item.EntityId, category?.Name));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, CategoryName = x.Item2 }),
                Total = customerReminder.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        public ActionResult CategoryAddPopup(int customerReminderId, int conditionId)
        {
            var model = new CustomerReminderModel.ConditionModel.AddCategoryConditionModel
            {
                ConditionId = conditionId,
                CustomerReminderId = customerReminderId
            };
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult CategoryAddPopupList(DataSourceRequest command, CustomerReminderModel.ConditionModel.AddCategoryConditionModel model)
        {
            var categories = _categoryService.GetAllCategories(model.SearchCategoryName,
                pageIndex: command.Page - 1, pageSize: command.PageSize, showHidden: true);
            var items = new List<CategoryModel>();
            foreach (var item in categories)
            {
                var categoryModel = item.ToModel();
                categoryModel.Breadcrumb = item.GetFormattedBreadCrumb(_categoryService);
                items.Add(categoryModel);
            }
            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = categories.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult CategoryAddPopup(string btnId, string formId, CustomerReminderModel.ConditionModel.AddCategoryConditionModel model)
        {
            if (model.SelectedCategoryIds != null)
            {
                foreach (int id in model.SelectedCategoryIds)
                {
                    var customerReminder = _customerReminderService.GetCustomerReminderById(model.CustomerReminderId);
                    if (customerReminder != null)
                    {
                        var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                        if (condition != null)
                        {
                            if (condition.Entity.Where(x => x.EntityId == id && x.EntityName == "category").Count() == 0)
                            {
                                condition.Entity.Add(new ReminderConditionEntity
                                {
                                    EntityId = id,
                                    EntityName = "category"
                                });
                                _customerReminderService.UpdateCustomerReminder(customerReminder);
                            }
                        }
                    }
                }
            }
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Condition Manufacturer
        [HttpPost]
        public virtual ActionResult ConditionManufacturer(int customerReminderId, int conditionId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "manufacturer"))
            {
                var manuf = _manufacturerService.GetManufacturerById(item.EntityId);
                items.Add(Tuple.Create(item.EntityId, manuf?.Name));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, ManufacturerName = x.Item2 }),
                Total = customerReminder.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        public ActionResult ManufacturerAddPopup(int customerReminderId, int conditionId)
        {
            var model = new CustomerReminderModel.ConditionModel.AddManufacturerConditionModel
            {
                ConditionId = conditionId,
                CustomerReminderId = customerReminderId
            };
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ManufacturerAddPopupList(DataSourceRequest command, CustomerReminderModel.ConditionModel.AddManufacturerConditionModel model)
        {
            var manufacturers = _manufacturerService.GetAllManufacturers(model.SearchManufacturerName, 0,
                command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = manufacturers.Select(x => x.ToModel()),
                Total = manufacturers.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult ManufacturerAddPopup(string btnId, string formId, CustomerReminderModel.ConditionModel.AddManufacturerConditionModel model)
        {
            if (model.SelectedManufacturerIds != null)
            {
                foreach (int id in model.SelectedManufacturerIds)
                {
                    var customerReminder = _customerReminderService.GetCustomerReminderById(model.CustomerReminderId);
                    if (customerReminder != null)
                    {
                        var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                        if (condition != null)
                        {
                            if (condition.Entity.Where(x => x.EntityId == id && x.EntityName == "manufacturer").Count() == 0)
                            {
                                condition.Entity.Add(new ReminderConditionEntity
                                {
                                    EntityId = id,
                                    EntityName = "manufacturer"
                                });
                                _customerReminderService.UpdateCustomerReminder(customerReminder);
                            }
                        }
                    }
                }
            }
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }
        #endregion

        #region Condition Product

        [HttpPost]
        public virtual ActionResult ConditionProduct(int customerReminderId, int conditionId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "product"))
            {
                var prod = _productService.GetProductById(item.EntityId);
                items.Add(Tuple.Create(item.EntityId, prod?.Name));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, ProductName = x.Item2 }),
                Total = customerReminder.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        public virtual ActionResult ProductAddPopup(int customerReminderId, int conditionId)
        {
            var model = new CustomerReminderModel.ConditionModel.AddProductToConditionModel();
            model.ConditionId = conditionId;
            model.CustomerReminderId = customerReminderId;
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var c in categories)
                model.AvailableCategories.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
                model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList().ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ProductAddPopupList(DataSourceRequest command, CustomerActionConditionModel.AddProductToConditionModel model)
        {
            var products = _productService.SearchProducts(
               categoryIds: new List<int> { model.SearchCategoryId },
               manufacturerId: model.SearchManufacturerId,
               storeId: model.SearchStoreId,
               vendorId: model.SearchVendorId,
               productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
               keywords: model.SearchProductName,
               pageIndex: command.Page - 1,
               pageSize: command.PageSize,
               showHidden: true
               );
            var gridModel = new DataSourceResult();
            gridModel.Data = products.Select(x =>
            {
                var productModel = x.ToModel();
                //little performance optimization: ensure that "FullDescription" is not returned
                productModel.FullDescription = "";

                //picture
                var defaultProductPicture = _pictureService.GetPicturesByProductId(x.Id, 1).FirstOrDefault();
                productModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultProductPicture, 75, true);

                return productModel;
            });
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual ActionResult ProductAddPopup(string btnId, string formId, CustomerReminderModel.ConditionModel.AddProductToConditionModel model)
        {
            if (model.SelectedProductIds != null)
            {
                foreach (int id in model.SelectedProductIds)
                {
                    var customerReminder = _customerReminderService.GetCustomerReminderById(model.CustomerReminderId);
                    if (customerReminder != null)
                    {
                        var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                        if (condition != null)
                        {
                            if (condition.Entity.Where(x => x.EntityId == id && x.EntityName == "product").Count() == 0)
                            {
                                condition.Entity.Add(new ReminderConditionEntity
                                {
                                    EntityId = id,
                                    EntityName = "product"
                                });
                                _customerReminderService.UpdateCustomerReminder(customerReminder);
                            }
                        }
                    }
                }
            }
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }


        #endregion

        #region Customer Tags

        [HttpPost]
        public virtual ActionResult ConditionCustomerTag(int customerReminderId, int conditionId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "customerTag"))
            {
                var tag = _customerTagService.GetCustomerTagById(item.EntityId);
                items.Add(Tuple.Create(item.EntityId, tag?.Name));
            }

            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, CustomerTag = x.Item2 }),
                Total = customerReminder.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }
        [HttpPost]
        public virtual ActionResult ConditionCustomerTagInsert(CustomerReminderModel.ConditionModel.AddCustomerTagConditionModel model)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(model.CustomerReminderId);
            if (customerReminder != null)
            {
                var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    if (condition.Entity.Where(x => x.EntityId == model.CustomerTagId && x.EntityName == "customerTag").Count() == 0)
                    {
                        condition.Entity.Add(new ReminderConditionEntity
                        {
                            EntityId = model.CustomerTagId,
                            EntityName = "customerTag"
                        });
                        _customerReminderService.UpdateCustomerReminder(customerReminder);
                    }
                }
            }
            return new NullJsonResult();
        }

        [HttpGet]
        public virtual ActionResult CustomerTags()
        {
            var customerTag = (_customerTagService.GetAllCustomerTags()).Select(x => new { Id = x.Id, Name = x.Name });
            return Json(customerTag);
        }
        #endregion

        #region Condition Customer role

        [HttpPost]
        public virtual ActionResult ConditionCustomerRole(int customerReminderId, int conditionId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "customerRole"))
            {
                var role = _customerService.GetCustomerRoleById(item.EntityId);
                items.Add(Tuple.Create(item.EntityId, role?.Name));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, CustomerRole = x.Item2 }),
                Total = customerReminder.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionCustomerRoleInsert(CustomerReminderModel.ConditionModel.AddCustomerRoleConditionModel model)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(model.CustomerReminderId);
            if (customerReminder != null)
            {
                var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    if (condition.Entity.Where(x => x.EntityId == model.CustomerRoleId && x.EntityName == "customerRole").Count() == 0)
                    {
                        condition.Entity.Add(new ReminderConditionEntity
                        {
                            EntityId = model.CustomerRoleId,
                            EntityName = "customerRole"
                        });
                        _customerReminderService.UpdateCustomerReminder(customerReminder);
                    }
                }
            }
            return new NullJsonResult();
        }

        [HttpGet]
        public virtual ActionResult CustomerRoles()
        {
            var customerRole = (_customerService.GetAllCustomerRoles()).Select(x => new { Id = x.Id, Name = x.Name });
            return Json(customerRole);
        }

        #endregion

        #region Condition Customer Register
        [HttpPost]
        public virtual ActionResult ConditionCustomerRegister(int customerReminderId, int conditionId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == conditionId);

            var gridModel = new DataSourceResult
            {
                Data = condition != null ? condition.CustomerRegistration.Select(z => new
                {
                    Id = z.Id,
                    CustomerRegisterName = z.RegisterField,
                    CustomerRegisterValue = z.RegisterValue
                })
                    : null,
                Total = customerReminder.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionCustomerRegisterInsert(CustomerReminderModel.ConditionModel.AddCustomerRegisterConditionModel model)
        {

            var customerReminder = _customerReminderService.GetCustomerReminderById(model.CustomerReminderId);
            if (customerReminder != null)
            {
                var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var _cr = new CustomerRegister()
                    {
                        RegisterField = model.CustomerRegisterName,
                        RegisterValue = model.CustomerRegisterValue,
                    };
                    condition.CustomerRegistration.Add(_cr);
                    _customerReminderService.UpdateCustomerReminder(customerReminder);
                }
            }
            return new NullJsonResult();
        }
        [HttpPost]
        public virtual ActionResult ConditionCustomerRegisterUpdate(CustomerReminderModel.ConditionModel.AddCustomerRegisterConditionModel model)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(model.CustomerReminderId);
            if (customerReminder != null)
            {
                var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var cr = condition.CustomerRegistration.FirstOrDefault(x => x.Id == model.Id);
                    cr.RegisterField = model.CustomerRegisterName;
                    cr.RegisterValue = model.CustomerRegisterValue;
                    _customerReminderService.UpdateCustomerReminder(customerReminder);
                }
            }
            return new NullJsonResult();
        }

        [HttpGet]
        public ActionResult CustomerRegisterFields()
        {
            var list = new List<Tuple<string, string>>();
            list.Add(Tuple.Create("Gender", "Gender"));
            list.Add(Tuple.Create("Company", "Company"));
            list.Add(Tuple.Create("CountryId", "CountryId"));
            list.Add(Tuple.Create("City", "City"));
            list.Add(Tuple.Create("StateProvinceId", "StateProvinceId"));
            list.Add(Tuple.Create("StreetAddress", "StreetAddress"));
            list.Add(Tuple.Create("ZipPostalCode", "ZipPostalCode"));
            list.Add(Tuple.Create("Phone", "Phone"));
            list.Add(Tuple.Create("Fax", "Fax"));

            var customer = list.Select(x => new { Id = x.Item1, Name = x.Item2 });
            return Json(customer);
        }
        #endregion

        #region Condition Custom Customer Attribute

        private string CustomerAttribute(string registerField)
        {
            string _field = registerField;
            var _rfString = registerField.Split(':');
            var _rf = Array.ConvertAll(_rfString, int.Parse);
            if (_rf.Count() > 1)
            {
                var ca = _customerAttributeService.GetCustomerAttributeById(_rf.FirstOrDefault());
                if (ca != null)
                {
                    _field = ca.Name;
                    if (ca.CustomerAttributeValues.FirstOrDefault(x => x.Id == _rf.LastOrDefault()) != null)
                    {
                        _field = ca.Name + "->" + ca.CustomerAttributeValues.FirstOrDefault(x => x.Id == _rf.LastOrDefault()).Name;
                    }
                }

            }

            return _field;
        }

        [HttpPost]
        public virtual ActionResult ConditionCustomCustomerAttribute(int customerReminderId, int conditionId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, string, string, string>>();
            foreach (var item in condition.CustomCustomerAttributes)
            {
                items.Add(Tuple.Create(item.Id, CustomerAttribute(item.RegisterField), item.RegisterField, item.RegisterValue));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, CustomerAttributeId = x.Item2, CustomerAttributeName = x.Item3, CustomerAttributeValue = x.Item4 }),
                Total = customerReminder.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionCustomCustomerAttributeInsert(CustomerReminderModel.ConditionModel.AddCustomCustomerAttributeConditionModel model)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(model.CustomerReminderId);
            if (customerReminder != null)
            {
                var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var _cr = new CustomerRegister()
                    {
                        RegisterField = model.CustomerAttributeName,
                        RegisterValue = model.CustomerAttributeValue,
                    };
                    condition.CustomCustomerAttributes.Add(_cr);
                    _customerReminderService.UpdateCustomerReminder(customerReminder);
                }
            }
            return new NullJsonResult();

        }
        [HttpPost]
        public virtual ActionResult ConditionCustomCustomerAttributeUpdate(CustomerReminderModel.ConditionModel.AddCustomCustomerAttributeConditionModel model)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(model.CustomerReminderId);
            if (customerReminder != null)
            {
                var condition = customerReminder.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var cr = condition.CustomCustomerAttributes.FirstOrDefault(x => x.Id == model.Id);
                    cr.RegisterField = model.CustomerAttributeName;
                    cr.RegisterValue = model.CustomerAttributeValue;
                    _customerReminderService.UpdateCustomerReminder(customerReminder);
                }
            }
            return new NullJsonResult();
        }

        [HttpGet]
        public virtual ActionResult CustomCustomerAttributeFields()
        {
            var list = new List<Tuple<string, string>>();
            foreach (var item in _customerAttributeService.GetAllCustomerAttributes())
            {
                if (item.AttributeControlType == AttributeControlType.Checkboxes ||
                    item.AttributeControlType == AttributeControlType.DropdownList ||
                    item.AttributeControlType == AttributeControlType.RadioList)
                {
                    foreach (var value in item.CustomerAttributeValues)
                    {
                        list.Add(Tuple.Create(string.Format("{0}:{1}", item.Id, value.Id), item.Name + "->" + value.Name));
                    }
                }
            }
            var customer = list.Select(x => new { Id = x.Item1, Name = x.Item2 });
            return Json(customer);
        }
        #endregion

        #endregion

        #region Levels

        [HttpPost]
        public virtual ActionResult Levels(int customerReminderId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            var gridModel = new DataSourceResult
            {
                Data = customerReminder.Levels.Select(x => new
                { Id = x.Id, Name = x.Name, Level = x.Level }).OrderBy(x => x.Level),
                Total = customerReminder.Levels.Count()
            };
            return Json(gridModel);
        }

        public virtual ActionResult AddLevel(int customerReminderId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            var model = new CustomerReminderModel.ReminderLevelModel();
            model.CustomerReminderId = customerReminderId;
            PrepareReminderLevelModel(model, customerReminder);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult AddLevel(CustomerReminderModel.ReminderLevelModel model, bool continueEditing)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(model.CustomerReminderId);
            if (customerReminder == null)
            {
                return RedirectToAction("List");
            }
            if (customerReminder.Levels.Where(x => x.Level == model.Level).Count() > 0)
            {
                ModelState.AddModelError("Error-LevelExists", _localizationService.GetResource("Admin.Customers.CustomerReminderLevel.Exists"));
            }

            if (ModelState.IsValid)
            {
                var level = new CustomerReminder.ReminderLevel()
                {
                    Name = model.Name,
                    Level = model.Level,
                    BccEmailAddresses = model.BccEmailAddresses,
                    Body = model.Body,
                    EmailAccountId = model.EmailAccountId,
                    Subject = model.Subject,
                    Day = model.Day,
                    Hour = model.Hour,
                    Minutes = model.Minutes,
                };

                customerReminder.Levels.Add(level);
                _customerReminderService.UpdateCustomerReminder(customerReminder);
                _customerActivityService.InsertActivity("AddNewCustomerReminderLevel", _localizationService.GetResource("ActivityLog.AddNewCustomerReminderLevel"), customerReminder.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerReminderLevel.Added"));
                return continueEditing ? RedirectToAction("EditLevel", new { customerReminderId = customerReminder.Id, cid = level.Id }) : RedirectToAction("Edit", new { id = customerReminder.Id });
            }
            PrepareReminderLevelModel(model, customerReminder);
            return View(model);
        }

        public virtual ActionResult EditLevel(int customerReminderId, int cid)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            if (customerReminder == null)
            {
                return RedirectToAction("List");
            }

            var level = customerReminder.Levels.FirstOrDefault(x => x.Id == cid);
            if (level == null)
                return RedirectToAction("List");

            var model = level.ToModel();
            model.CustomerReminderId = customerReminderId;
            PrepareReminderLevelModel(model, customerReminder);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult EditLevel(int customerReminderId, int cid, CustomerReminderModel.ReminderLevelModel model, bool continueEditing)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            if (customerReminder == null)
                return RedirectToAction("List");

            var level = customerReminder.Levels.FirstOrDefault(x => x.Id == cid);
            if (level == null)
                return RedirectToAction("List");

            if (level.Level != model.Level)
            {
                if (customerReminder.Levels.Where(x => x.Level == model.Level).Count() > 0)
                {
                    ModelState.AddModelError("Error-LevelExists", _localizationService.GetResource("Admin.Customers.CustomerReminderLevel.Exists"));
                }
            }
            try
            {
                if (ModelState.IsValid)
                {
                    level.Level = model.Level;
                    level.Name = model.Name;
                    level.Subject = model.Subject;
                    level.BccEmailAddresses = model.BccEmailAddresses;
                    level.Body = model.Body;
                    level.EmailAccountId = model.EmailAccountId;
                    level.Day = model.Day;
                    level.Hour = model.Hour;
                    level.Minutes = model.Minutes;
                    _customerReminderService.UpdateCustomerReminder(customerReminder);
                    _customerActivityService.InsertActivity("EditCustomerReminderCondition", _localizationService.GetResource("ActivityLog.EditCustomerReminderLevel"), customerReminder.Name);

                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerReminderLevel.Updated"));
                    return continueEditing ? RedirectToAction("EditLevel", new { customerReminderId = customerReminder.Id, cid = level.Id }) : RedirectToAction("Edit", new { id = customerReminder.Id });
                }
                PrepareReminderLevelModel(model, customerReminder);
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = customerReminder.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult DeleteLevel(int Id, int customerReminderId)
        {
            var customerReminder = _customerReminderService.GetCustomerReminderById(customerReminderId);
            if (customerReminder != null)
            {
                var level = customerReminder.Levels.FirstOrDefault(x => x.Id == Id);
                if (level != null)
                {
                    customerReminder.Levels.Remove(level);
                    _customerReminderService.UpdateCustomerReminder(customerReminder);
                }
            }
            return new NullJsonResult();
        }
        #endregion
    }
}