using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Admin.Models.Catalog;
using Nop.Services.Helpers;
using System.Web.Mvc;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Services;
using Nop.Services.Messages;
using Nop.Services.Media;

namespace Nop.Admin.Controllers
{
    public partial class CustomerActionController : BaseAdminController
    {
        #region Fields
        private readonly ICustomerService _customerService;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerTagService _customerTagService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly ICustomerActionService _customerActionService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IBannerService _bannerService;
        private readonly IInteractiveFormService _interactiveFormService;
        private readonly IPictureService _pictureService;

        #endregion

        #region Constructors

        public CustomerActionController(
            ICustomerService customerService,
            ICustomerAttributeService customerAttributeService,
            ICustomerTagService customerTagService,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IStoreService storeService,
            IVendorService vendorService,
            ICustomerActionService customerActionService,
            IProductAttributeService productAttributeService,
            ISpecificationAttributeService specificationAttributeService,
            IDateTimeHelper dateTimeHelper,
            IMessageTemplateService messageTemplateService,
            IBannerService bannerService,
            IInteractiveFormService interactiveFormService,
            IPictureService pictureService)
        {
            this._customerService = customerService;
            this._customerAttributeService = customerAttributeService;
            this._customerTagService = customerTagService;
            this._localizationService = localizationService;
            this._customerActivityService = customerActivityService;
            this._productService = productService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._storeService = storeService;
            this._vendorService = vendorService;
            this._customerActionService = customerActionService;
            this._productAttributeService = productAttributeService;
            this._specificationAttributeService = specificationAttributeService;
            this._dateTimeHelper = dateTimeHelper;
            this._messageTemplateService = messageTemplateService;
            this._bannerService = bannerService;
            this._interactiveFormService = interactiveFormService;
            this._pictureService = pictureService;
        }

        #endregion

        #region Utilities
        protected void CheckValidateModel(CustomerActionModel model)
        {
            if ((model.ReactionType == CustomerReactionTypeEnum.Banner) && model.BannerId == 0)
                ModelState.AddModelError("error", "Banner is required");
            if ((model.ReactionType == CustomerReactionTypeEnum.InteractiveForm) && model.InteractiveFormId == 0)
                ModelState.AddModelError("error", "Interactive form is required");
            if ((model.ReactionType == CustomerReactionTypeEnum.Email) && model.MessageTemplateId == 0)
                ModelState.AddModelError("error", "Email is required");
            if ((model.ReactionType == CustomerReactionTypeEnum.AssignToCustomerRole) && model.CustomerRoleId == 0)
                ModelState.AddModelError("error", "Customer role is required");
            if ((model.ReactionType == CustomerReactionTypeEnum.AssignToCustomerTag) && model.CustomerTagId == 0)
                ModelState.AddModelError("error", "Tag is required");
        }
        public virtual void PrepareCustomerActionModel(CustomerActionModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var banners = _bannerService.GetAllBanners();
            foreach (var item in banners)
            {
                model.Banners.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });

            }
            var message = _messageTemplateService.GetAllMessageTemplates(0);
            foreach (var item in message)
            {
                model.MessageTemplates.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            var customerRole = _customerService.GetAllCustomerRoles();
            foreach (var item in customerRole)
            {
                model.CustomerRoles.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            var customerTag = _customerTagService.GetAllCustomerTags();
            foreach (var item in customerTag)
            {
                model.CustomerTags.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            foreach (var item in _customerActionService.GetCustomerActionType())
            {
                model.ActionType.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            foreach (var item in _interactiveFormService.GetAllForms())
            {
                model.InteractiveForms.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });

            }


        }

        [NonAction]
        public virtual List<int> ConditionType(string ConditionType)
        {

            if (!String.IsNullOrWhiteSpace(ConditionType))
            {
                var ids = new List<int>();
                var rangeArray = ConditionType
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                foreach (string str1 in rangeArray)
                {
                    int tmp1;
                    if (int.TryParse(str1, out tmp1))
                        ids.Add(tmp1);
                }
                return ids;
            }

            return new List<int>();
        }

        #endregion

        #region Customer Actions

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List() => View();

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command)
        {
            var customeractions = _customerActionService.GetCustomerActions();
            var actions = _customerActionService.GetCustomerActionType();
            var gridModel = new DataSourceResult
            {
                Data = customeractions.Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Active = x.Active,
                    ActionType = actions.FirstOrDefault(y => y.Id == x.ActionTypeId)?.Name
                }),
                Total = customeractions.Count()
            };
            return Json(gridModel);
        }

        public virtual ActionResult Create()
        {
            var model = new CustomerActionModel();

            model.Active = true;
            model.StartDateTime = DateTime.Now;
            model.EndDateTime = DateTime.Now.AddMonths(1);
            model.ReactionTypeId = (int)CustomerReactionTypeEnum.Banner;

            PrepareCustomerActionModel(model);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(CustomerActionModel model, bool continueEditing)
        {
            CheckValidateModel(model);
            if (ModelState.IsValid)
            {
                var customeraction = model.ToEntity();
                customeraction.StartDateTimeUtc = _dateTimeHelper.ConvertToUserTime(model.StartDateTime);
                customeraction.EndDateTimeUtc = _dateTimeHelper.ConvertToUserTime(model.EndDateTime);
                _customerActionService.InsertCustomerAction(customeraction);
                //activity log
                _customerActivityService.InsertActivity("AddNewCustomerAction", _localizationService.GetResource("ActivityLog.AddNewCustomerAction"), customeraction.Name);
                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerAction.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = customeraction.Id }) : RedirectToAction("List");
            }
            PrepareCustomerActionModel(model);
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            var customerAction = _customerActionService.GetCustomerActionById(id);
            if (customerAction == null)
                return RedirectToAction("List");

            var model = customerAction.ToModel();
            model.StartDateTime = _dateTimeHelper.ConvertToUserTime(customerAction.StartDateTimeUtc);
            model.EndDateTime = _dateTimeHelper.ConvertToUserTime(customerAction.EndDateTimeUtc);
            model.ConditionCount = customerAction.Conditions.Count();
            PrepareCustomerActionModel(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(CustomerActionModel model, bool continueEditing)
        {
            CheckValidateModel(model);

            var customeraction = _customerActionService.GetCustomerActionById(model.Id);
            if (customeraction == null)
                return RedirectToAction("List");
            try
            {
                if (ModelState.IsValid)
                {
                    if (customeraction.Conditions.Count() > 0)
                        model.ActionTypeId = customeraction.ActionTypeId;
                    if (model.ActionTypeId == 0)
                        model.ActionTypeId = customeraction.ActionTypeId;

                    customeraction = model.ToEntity(customeraction);
                    customeraction.StartDateTimeUtc = _dateTimeHelper.ConvertToUserTime(model.StartDateTime);
                    customeraction.EndDateTimeUtc = _dateTimeHelper.ConvertToUserTime(model.EndDateTime);
                    _customerActionService.UpdateCustomerAction(customeraction);
                    _customerActivityService.InsertActivity("EditCustomerAction", _localizationService.GetResource("ActivityLog.EditCustomerAction"), customeraction.Name);
                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerAction.Updated"));
                    return continueEditing ? RedirectToAction("Edit", new { id = model.Id }) : RedirectToAction("List");
                }
                PrepareCustomerActionModel(model);
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = model.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult History(DataSourceRequest command, int customerActionId)
        {
            var history = _customerActionService.GetAllCustomerActionHistory(customerActionId,
                pageIndex: command.Page-1,
                pageSize: command.PageSize);
            var items = new List<Tuple<string, DateTime>>();
            foreach (var item in history)
            {
                var customer = _customerService.GetCustomerById(item.CustomerId);
                var Email = customer != null ? String.IsNullOrEmpty(customer.Email) ? "(unknown)" : customer.Email : "(unknown)";
                var CreateDate = _dateTimeHelper.ConvertToUserTime(item.CreateDateUtc, DateTimeKind.Utc);
                items.Add(Tuple.Create(Email, CreateDate));
            }
            var gridModel = new DataSourceResult
            {
                Data = items,
                Total = history.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            var customerAction = _customerActionService.GetCustomerActionById(id);
            if (customerAction == null)
                return RedirectToAction("List");

            try
            {
                //activity log
                _customerActivityService.InsertActivity("DeleteCustomerAction", _localizationService.GetResource("ActivityLog.DeleteCustomerAction"), customerAction.Name);
                _customerActionService.DeleteCustomerAction(customerAction);
                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerAction.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customerAction.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult Conditions(int customerActionId)
        {
            var conditions = _customerActionService.GetCustomerActionById(customerActionId);
            var gridModel = new DataSourceResult
            {
                Data = conditions.Conditions.Select(x => new { Id = x.Id, Name = x.Name, Condition = x.CustomerActionConditionType.ToString() }),
                Total = conditions.Conditions.Count()
            };
            return Json(gridModel);
        }

        #endregion

        #region Conditions

        public virtual ActionResult AddCondition(int customerActionId)
        {
            var customerAction = _customerActionService.GetCustomerActionById(customerActionId);
            var actionType = _customerActionService.GetCustomerActionTypeById(customerAction.ActionTypeId);

            var model = new CustomerActionConditionModel();
            model.CustomerActionId = customerActionId;


            foreach (var item in ConditionType(actionType.ConditionType))
            {
                model.CustomerActionConditionType.Add(new SelectListItem()
                {
                    Value = item.ToString(),
                    Text = ((CustomerActionConditionTypeEnum)item).ToString()
                });
            }
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult AddCondition(CustomerActionConditionModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
                if (customerAction == null)
                {
                    return RedirectToAction("List");
                }
                var condition = new CustomerAction.ActionCondition()
                {
                    Name = model.Name,
                    CustomerActionConditionTypeId = model.CustomerActionConditionTypeId,
                    ConditionId = model.ConditionId,
                };
                customerAction.Conditions.Add(condition);
                _customerActionService.UpdateCustomerAction(customerAction);

                _customerActivityService.InsertActivity("AddNewCustomerActionCondition", _localizationService.GetResource("ActivityLog.AddNewCustomerAction"), customerAction.Name);
                SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerActionCondition.Added"));
                return continueEditing ? RedirectToAction("EditCondition", new { customerActionId = customerAction.Id, cid = condition.Id }) : RedirectToAction("Edit", new { id = customerAction.Id });
            }
            return View(model);
        }

        public virtual ActionResult EditCondition(int customerActionId, int cid)
        {
            var customerAction = _customerActionService.GetCustomerActionById(customerActionId);
            if (customerAction == null)
                return RedirectToAction("List");
            var actionType = _customerActionService.GetCustomerActionTypeById(customerAction.ActionTypeId);
            var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == cid);
            if (condition == null)
                return RedirectToAction("List");

            var model = condition.ToModel();

            model.CustomerActionId = customerActionId;

            foreach (var item in ConditionType(actionType.ConditionType))
            {
                model.CustomerActionConditionType.Add(new SelectListItem()
                {
                    Value = item.ToString(),
                    Text = ((CustomerActionConditionTypeEnum)item).ToString()
                });
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult EditCondition(int customerActionId, int cid, CustomerActionConditionModel model, bool continueEditing)
        {
            var customerAction = _customerActionService.GetCustomerActionById(customerActionId);
            if (customerAction == null)
                return RedirectToAction("List");

            var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == cid);
            if (condition == null)
                return RedirectToAction("List");
            try
            {
                if (ModelState.IsValid)
                {
                    condition = model.ToEntity(condition);
                    _customerActionService.UpdateCustomerAction(customerAction);
                    //activity log
                    _customerActivityService.InsertActivity("EditCustomerActionCondition", _localizationService.GetResource("ActivityLog.EditCustomerActionCondition"), customerAction.Name);
                    SuccessNotification(_localizationService.GetResource("Admin.Customers.CustomerActionCondition.Updated"));
                    return continueEditing ? RedirectToAction("EditCondition", new { customerActionId = customerAction.Id, cid = condition.Id }) : RedirectToAction("Edit", new { id = customerAction.Id });
                }
                return View(model);
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = customerAction.Id });
            }
        }

        [HttpPost]
        public virtual ActionResult ConditionDelete(int Id, int customerActionId)
        {
            var customerAction = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == Id);
            customerAction.Conditions.Remove(condition);
            _customerActionService.UpdateCustomerAction(customerAction);
            return new NullJsonResult();
        }

        [HttpPost]
        public virtual ActionResult ConditionDeletePosition(int id, int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);

            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.Product)
            {
                var product = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "product");
                _customerActionService.DeleteConditionEntitiy(product);
                _customerActionService.UpdateCustomerAction(customerActions);
            }
            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.Category)
            {
                var category = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "category");
                _customerActionService.DeleteConditionEntitiy(category);
                _customerActionService.UpdateCustomerAction(customerActions);

            }
            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.Manufacturer)
            {
                var manufacturer = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "manufacturer");
                _customerActionService.DeleteConditionEntitiy(manufacturer);
                _customerActionService.UpdateCustomerAction(customerActions);
            }
            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.Vendor)
            {
                var vendor = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "vendor");
                _customerActionService.DeleteConditionEntitiy(vendor);
                _customerActionService.UpdateCustomerAction(customerActions);
            }
            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.ProductAttribute)
            {
                var productAttribute = condition.ProductAttribute.FirstOrDefault(x => x.ProductAttributeId == id);
                condition.ProductAttribute.Remove(productAttribute);
                _customerActionService.UpdateCustomerAction(customerActions);
            }
            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.ProductSpecification)
            {
                var productspecification = condition.ProductSpecifications.FirstOrDefault(x => x.ProductSpecificationId == id);
                condition.ProductSpecifications.Remove(productspecification);
                _customerActionService.UpdateCustomerAction(customerActions);
            }
            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.CustomerRole)
            {
                var customerRole = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "customerRole");
                _customerActionService.DeleteConditionEntitiy(customerRole);
                _customerActionService.UpdateCustomerAction(customerActions);
            }
            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.CustomerTag)
            {
                var customertag = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "customerTag");
                _customerActionService.DeleteConditionEntitiy(customertag);
                _customerActionService.UpdateCustomerAction(customerActions);
            }

            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.CustomCustomerAttribute)
            {
                condition.CustomCustomerAttributes.Remove(condition.CustomCustomerAttributes.FirstOrDefault(x => x.Id == id));
                _customerActionService.UpdateCustomerAction(customerActions);
            }
            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.CustomerRegisterField)
            {
                condition.CustomerRegistration.Remove(condition.CustomerRegistration.FirstOrDefault(x => x.Id == id));
                _customerActionService.UpdateCustomerAction(customerActions);
            }
            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.UrlCurrent)
            {
                condition.UrlCurrent.Remove(condition.UrlCurrent.FirstOrDefault(x => x.Id == id));
                _customerActionService.UpdateCustomerAction(customerActions);
            }
            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.UrlReferrer)
            {
                condition.UrlReferrer.Remove(condition.UrlReferrer.FirstOrDefault(x => x.Id == id));
                _customerActionService.UpdateCustomerAction(customerActions);
            }
            if (condition.CustomerActionConditionTypeId == (int)CustomerActionConditionTypeEnum.Store)
            {
                var store = condition.Entity.FirstOrDefault(x => x.EntityId == id && x.EntityName == "store");
                _customerActionService.DeleteConditionEntitiy(store);
                _customerActionService.UpdateCustomerAction(customerActions);
            }

            return new NullJsonResult();
        }
        #endregion

        #region Condition Product

        [HttpPost]
        public virtual ActionResult ConditionProduct(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "product"))
            {
                var product = (_productService.GetProductById(item.EntityId))?.Name;
                items.Add(Tuple.Create(item.EntityId, product));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, ProductName = x.Item2 }),
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }


        public virtual ActionResult ProductAddPopup(int customerActionId, int conditionId)
        {
            var model = new CustomerActionConditionModel.AddProductToConditionModel();
            model.CustomerActionConditionId = conditionId;
            model.CustomerActionId = customerActionId;
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
        public virtual ActionResult ProductAddPopup(string btnId, string formId, CustomerActionConditionModel.AddProductToConditionModel model)
        {
            if (model.SelectedProductIds != null)
            {
                foreach (int id in model.SelectedProductIds)
                {
                    var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
                    if (customerAction != null)
                    {
                        var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.CustomerActionConditionId);
                        if (condition != null)
                        {
                            if (condition.Entity.Where(x => x.EntityId == id && x.EntityName == "product").Count() == 0)
                            {
                                condition.Entity.Add(new ActionConditionEntity
                                {
                                    EntityId = id,
                                    EntityName = "product"
                                });
                                _customerActionService.UpdateCustomerAction(customerAction);
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

        #region Condition Category

        [HttpPost]
        public virtual ActionResult ConditionCategory(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "category"))
            {
                var category = (_categoryService.GetCategoryById(item.EntityId))?.Name;
                items.Add(Tuple.Create(item.EntityId, category));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, CategoryName = x.Item2 }),
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        public virtual ActionResult CategoryAddPopup(int customerActionId, int conditionId)
        {
            var model = new CustomerActionConditionModel.AddCategoryConditionModel
            {
                CustomerActionConditionId = conditionId,
                CustomerActionId = customerActionId
            };
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult CategoryAddPopupList(DataSourceRequest command, CustomerActionConditionModel.AddCategoryConditionModel model)
        {
            var categories = _categoryService.GetAllCategories(model.SearchCategoryName,
                pageIndex: command.Page - 1, pageSize: command.PageSize, showHidden: true);
            var items = new List<CategoryModel>();
            foreach (var x in categories)
            {
                var categoryModel = x.ToModel();
                categoryModel.Breadcrumb = x.GetFormattedBreadCrumb(_categoryService);
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
        public virtual ActionResult CategoryAddPopup(string btnId, string formId, CustomerActionConditionModel.AddCategoryConditionModel model)
        {
            if (model.SelectedCategoryIds != null)
            {
                foreach (int id in model.SelectedCategoryIds)
                {
                    var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
                    if (customerAction != null)
                    {
                        var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.CustomerActionConditionId);
                        if (condition != null)
                        {
                            if (condition.Entity.Where(x => x.EntityId == id && x.EntityName == "category").Count() == 0)
                            {
                                condition.Entity.Add(new ActionConditionEntity
                                {
                                    EntityId = id,
                                    EntityName = "category"
                                });
                                _customerActionService.UpdateCustomerAction(customerAction);
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
        public virtual ActionResult ConditionManufacturer(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);

            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "manufacturer"))
            {
                var manufacturer = (_manufacturerService.GetManufacturerById(item.EntityId))?.Name;
                items.Add(Tuple.Create(item.EntityId, manufacturer));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, ManufacturerName = x.Item2 }),
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        public virtual ActionResult ManufacturerAddPopup(int customerActionId, int conditionId)
        {
            var model = new CustomerActionConditionModel.AddManufacturerConditionModel
            {
                CustomerActionConditionId = conditionId,
                CustomerActionId = customerActionId
            };
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult ManufacturerAddPopupList(DataSourceRequest command, CustomerActionConditionModel.AddManufacturerConditionModel model)
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
        public virtual ActionResult ManufacturerAddPopup(string btnId, string formId, CustomerActionConditionModel.AddManufacturerConditionModel model)
        {
            if (model.SelectedManufacturerIds != null)
            {
                foreach (int id in model.SelectedManufacturerIds)
                {
                    var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
                    if (customerAction != null)
                    {
                        var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.CustomerActionConditionId);
                        if (condition != null)
                        {
                            if (condition.Entity.Where(x => x.EntityId == id && x.EntityName == "manufacturer").Count() == 0)
                            {
                                condition.Entity.Add(new ActionConditionEntity
                                {
                                    EntityId = id,
                                    EntityName = "manufacturer"
                                });
                                _customerActionService.UpdateCustomerAction(customerAction);
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

        #region Condition Vendor

        [HttpPost]
        public virtual ActionResult ConditionVendor(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var vendors =  _vendorService.GetAllVendors();
            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "vendor"))
            {
                var VendorName = vendors.FirstOrDefault(y => y.Id == item.EntityId).Name;
                items.Add(Tuple.Create(item.EntityId, VendorName));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, VendorName = x.Item2 }),
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionVendorInsert(CustomerActionConditionModel.AddVendorConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    if (condition.Entity.Where(x => x.EntityId == model.VendorId && x.EntityName == "vendor").Count() == 0)
                    {
                        condition.Entity.Add(new ActionConditionEntity
                        {
                            EntityId = model.VendorId,
                            EntityName = "vendor"
                        });
                        _customerActionService.UpdateCustomerAction(customerAction);
                    }
                }
            }
            return new NullJsonResult();
        }

        [HttpGet]
        public virtual ActionResult Vendors()
        {
            var customerVendors = (_vendorService.GetAllVendors()).Select(x => new { Id = x.Id, Name = x.Name });
            return Json(customerVendors);
        }
        #endregion

        #region Condition Customer role

        [HttpPost]
        public virtual ActionResult ConditionCustomerRole(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "customerRole"))
            {
                var roles = (_customerService.GetCustomerRoleById(item.EntityId))?.Name;
                items.Add(Tuple.Create(item.EntityId, roles));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, CustomerRole = x.Item2 }),
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionCustomerRoleInsert(CustomerActionConditionModel.AddCustomerRoleConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    if (condition.Entity.Where(x => x.EntityId == model.CustomerRoleId && x.EntityName == "customerRole").Count() == 0)
                    {
                        condition.Entity.Add(new ActionConditionEntity
                        {
                            EntityId = model.CustomerRoleId,
                            EntityName = "customerRole"
                        });
                        _customerActionService.UpdateCustomerAction(customerAction);
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

        #region Stores

        [HttpGet]
        public virtual ActionResult Stores()
        {
            var stores = (_storeService.GetAllStores()).Select(x => new { Id = x.Id, Name = x.Name });
            return Json(stores);
        }

        [HttpPost]
        public virtual ActionResult ConditionStore(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "store"))
            {
                var store = (_storeService.GetStoreById(item.EntityId))?.Name;
                items.Add(Tuple.Create(item.EntityId, store));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, Store = x.Item2 }),
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionStoreInsert(CustomerActionConditionModel.AddStoreConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    if (condition.Entity.Where(x => x.EntityId == model.StoreId && x.EntityName == "store").Count() == 0)
                    {
                        condition.Entity.Add(new ActionConditionEntity
                        {
                            EntityId = model.StoreId,
                            EntityName="store"
                        });
                        _customerActionService.UpdateCustomerAction(customerAction);
                    }
                }
            }
            return new NullJsonResult();
        }

        #endregion

        #region Customer Tags

        [HttpPost]
        public virtual ActionResult ConditionCustomerTag(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, string>>();
            foreach (var item in condition.Entity.Where(x => x.EntityName == "customerTag"))
            {
                var tag = (_customerTagService.GetCustomerTagById(item.EntityId))?.Name;
                items.Add(Tuple.Create(item.EntityId, tag));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, CustomerTag = x.Item2 }),
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionCustomerTagInsert(CustomerActionConditionModel.AddCustomerTagConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    if (condition.Entity.Where(x => x.EntityId == model.CustomerTagId && x.EntityName == "customerTag").Count() == 0)
                    {
                        condition.Entity.Add(new ActionConditionEntity
                        {
                            EntityId = model.CustomerTagId,
                            EntityName = "customerTag"
                        });
                        _customerActionService.UpdateCustomerAction(customerAction);
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

        #region Condition Product Attributes

        [HttpPost]
        public virtual ActionResult ConditionProductAttribute(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var items = new List<Tuple<int, int, string>>();
            foreach (var item in condition.ProductAttribute)
            {
                var pa = (_productAttributeService.GetProductAttributeById(item.ProductAttributeId))?.Name;
                items.Add(Tuple.Create(item.Id, item.ProductAttributeId, pa));
            }
            var gridModel = new DataSourceResult
            {
                Data = items.Select(x => new { Id = x.Item1, ProductAttributeId = x.Item2, ProductAttributeName = x.Item3 }),
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionProductAttributeInsert(CustomerActionConditionModel.AddProductAttributeConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var _pv = new CustomerAction.ActionCondition.ActionConditionProductAttributeValue()
                    {
                        ProductAttributeId = model.ProductAttributeId,
                        Name = model.Name
                    };
                    condition.ProductAttribute.Add(_pv);
                    _customerActionService.UpdateCustomerAction(customerAction);
                }
            }
            return new NullJsonResult();
        }
        [HttpPost]
        public virtual ActionResult ConditionProductAttributeUpdate(CustomerActionConditionModel.AddProductAttributeConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var pva = condition.ProductAttribute.FirstOrDefault(x => x.Id == model.Id);
                    pva.ProductAttributeId = model.ProductAttributeId;
                    pva.Name = model.Name;
                    _customerActionService.UpdateCustomerAction(customerAction);
                }
            }
            return new NullJsonResult();
        }

        [HttpGet]
        public virtual ActionResult ProductAttributes()
        {
            var customerAttr = (_productAttributeService.GetAllProductAttributes()).Select(x => new { Id = x.Id, Name = x.Name });
            return Json(customerAttr);
        }
        #endregion

        #region Condition Product Specification

        [HttpPost]
        public virtual ActionResult ConditionProductSpecification(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);
            var specs = _specificationAttributeService.GetSpecificationAttributes();
            var gridModel = new DataSourceResult
            {
                Data = condition?.ProductSpecifications
                        .Select(z => new
                        {
                            Id = z.Id,
                            SpecificationId = z.ProductSpecificationId,
                            SpecificationName = specs.FirstOrDefault(x => x.Id == z.ProductSpecificationId)?.Name,
                            SpecificationValueName = z.ProductSpecificationValueId > 0 ? specs.FirstOrDefault(x => x.Id == z.ProductSpecificationId).SpecificationAttributeOptions.FirstOrDefault(x => x.Id == z.ProductSpecificationValueId).Name : "Undefined"
                        }),
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionProductSpecificationInsert(CustomerActionConditionModel.AddProductSpecificationConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    if (condition.ProductSpecifications.Where(x => x.ProductSpecificationId == model.SpecificationId && x.ProductSpecificationValueId == model.SpecificationValueId).Count() == 0)
                    {
                        var _ps = new CustomerAction.ActionCondition.ActionConditionProductSpecification()
                        {
                            ProductSpecificationId = model.SpecificationId,
                            ProductSpecificationValueId = model.SpecificationValueId
                        };
                        condition.ProductSpecifications.Add(_ps);
                        _customerActionService.UpdateCustomerAction(customerAction);
                    }
                }
            }
            return new NullJsonResult();
        }

        [HttpGet]
        public virtual ActionResult ProductSpecification()
        {
            var customerAttr = (_specificationAttributeService.GetSpecificationAttributes()).Select(x => new { Id = x.Id, Name = x.Name });
            return Json(customerAttr);
        }

        [HttpGet]
        public virtual ActionResult ProductSpecificationValue(int specificationId)
        {
            if (specificationId == 0)
                return new NullJsonResult();

            var customerSpec = (_specificationAttributeService.GetSpecificationAttributeById(specificationId)).SpecificationAttributeOptions.Select(x => new { Id = x.Id, Name = x.Name });
            return Json(customerSpec);
        }

        #endregion

        #region Condition Customer Register

        [HttpPost]
        public virtual ActionResult ConditionCustomerRegister(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);

            var gridModel = new DataSourceResult
            {
                Data = condition?.CustomerRegistration.Select(z => new
                {
                    Id = z.Id,
                    CustomerRegisterName = z.RegisterField,
                    CustomerRegisterValue = z.RegisterValue
                }),
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionCustomerRegisterInsert(CustomerActionConditionModel.AddCustomerRegisterConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var _cr = new CustomerRegister()
                    {
                        RegisterField = model.CustomerRegisterName,
                        RegisterValue = model.CustomerRegisterValue,
                    };
                    condition.CustomerRegistration.Add(_cr);
                    _customerActionService.UpdateCustomerAction(customerAction);
                }
            }
            return new NullJsonResult();
        }
        [HttpPost]
        public virtual ActionResult ConditionCustomerRegisterUpdate(CustomerActionConditionModel.AddCustomerRegisterConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var cr = condition.CustomerRegistration.FirstOrDefault(x => x.Id == model.Id);
                    cr.RegisterField = model.CustomerRegisterName;
                    cr.RegisterValue = model.CustomerRegisterValue;
                    _customerActionService.UpdateCustomerAction(customerAction);
                }
            }
            return new NullJsonResult();
        }

        [HttpGet]
        public virtual ActionResult CustomerRegisterFields()
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
            var _rf = registerField.Split(':');
            if (_rf.Count() > 1)
            {
                var ca = _customerAttributeService.GetCustomerAttributeById(int.Parse(_rf.FirstOrDefault()));
                if (ca != null)
                {
                    _field = ca.Name;
                    if (ca.CustomerAttributeValues.FirstOrDefault(x => x.Id == int.Parse(_rf.LastOrDefault())) != null)
                    {
                        _field = ca.Name + "->" + ca.CustomerAttributeValues.FirstOrDefault(x => x.Id == int.Parse(_rf.LastOrDefault())).Name;
                    }
                }

            }

            return _field;
        }

        [HttpPost]
        public virtual ActionResult ConditionCustomCustomerAttribute(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);

            var gridModel = new DataSourceResult
            {
                Data = condition?.CustomCustomerAttributes.Select(z => new
                {
                    Id = z.Id,
                    CustomerAttributeId = CustomerAttribute(z.RegisterField),
                    CustomerAttributeName = z.RegisterField,
                    CustomerAttributeValue = z.RegisterValue
                }),
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionCustomCustomerAttributeInsert(CustomerActionConditionModel.AddCustomCustomerAttributeConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var _cr = new CustomerRegister()
                    {
                        RegisterField = model.CustomerAttributeName,
                        RegisterValue = model.CustomerAttributeValue,
                    };
                    condition.CustomCustomerAttributes.Add(_cr);
                    _customerActionService.UpdateCustomerAction(customerAction);
                }
            }
            return new NullJsonResult();
        }
        [HttpPost]
        public virtual ActionResult ConditionCustomCustomerAttributeUpdate(CustomerActionConditionModel.AddCustomCustomerAttributeConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var cr = condition.CustomCustomerAttributes.FirstOrDefault(x => x.Id == model.Id);
                    cr.RegisterField = model.CustomerAttributeName;
                    cr.RegisterValue = model.CustomerAttributeValue;
                    _customerActionService.UpdateCustomerAction(customerAction);
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

        #region Url Referrer

        [HttpPost]
        public virtual ActionResult ConditionUrlReferrer(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);

            var gridModel = new DataSourceResult
            {
                Data = condition != null ? condition.UrlReferrer.Select(z => new { Id = z.Id, Name = z.Name }) : null,
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionUrlReferrerInsert(CustomerActionConditionModel.AddUrlConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var _url = new CustomerAction.ActionCondition.Url()
                    {
                        Name = model.Name
                    };
                    condition.UrlReferrer.Add(_url);
                    _customerActionService.UpdateCustomerAction(customerAction);
                }
            }
            return new NullJsonResult();
        }
        [HttpPost]
        public virtual ActionResult ConditionUrlReferrerUpdate(CustomerActionConditionModel.AddUrlConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var _url = condition.UrlReferrer.FirstOrDefault(x => x.Id == model.Id);
                    _url.Name = model.Name;
                    _customerActionService.UpdateCustomerAction(customerAction);
                }
            }
            return new NullJsonResult();
        }

        #endregion

        #region Url Current

        [HttpPost]
        public virtual ActionResult ConditionUrlCurrent(int customerActionId, int conditionId)
        {
            var customerActions = _customerActionService.GetCustomerActionById(customerActionId);
            var condition = customerActions.Conditions.FirstOrDefault(x => x.Id == conditionId);

            var gridModel = new DataSourceResult
            {
                Data = condition != null ? condition.UrlCurrent.Select(z => new { Id = z.Id, Name = z.Name }) : null,
                Total = customerActions.Conditions.Where(x => x.Id == conditionId).Count()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public virtual ActionResult ConditionUrlCurrentInsert(CustomerActionConditionModel.AddUrlConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var _url = new CustomerAction.ActionCondition.Url()
                    {
                        Name = model.Name
                    };
                    condition.UrlCurrent.Add(_url);
                    _customerActionService.UpdateCustomerAction(customerAction);
                }
            }
            return new NullJsonResult();
        }
        [HttpPost]
        public virtual ActionResult ConditionUrlCurrentUpdate(CustomerActionConditionModel.AddUrlConditionModel model)
        {
            var customerAction = _customerActionService.GetCustomerActionById(model.CustomerActionId);
            if (customerAction != null)
            {
                var condition = customerAction.Conditions.FirstOrDefault(x => x.Id == model.ConditionId);
                if (condition != null)
                {
                    var _url = condition.UrlCurrent.FirstOrDefault(x => x.Id == model.Id);
                    _url.Name = model.Name;
                    _customerActionService.UpdateCustomerAction(customerAction);
                }
            }
            return new NullJsonResult();
        }

        #endregion
    }
}