﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Messages;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Services.SMS;
using Nop.Admin.Models.SMS;
using Nop.Core.Domain.SMS;

namespace Nop.Admin.Controllers
{
    public partial class SMSTemplateController : BaseAdminController
    {
        #region Fields

        private readonly ISMSTemplateService _smsTemplateService;
        private readonly INumberAccountService _numberAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion Fields

        #region Constructors

        public SMSTemplateController(ISMSTemplateService smsTemplateService, 
            INumberAccountService emailAccountService,
            ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, 
            IMessageTokenProvider messageTokenProvider, 
            IPermissionService permissionService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IWorkflowMessageService workflowMessageService,
            ICustomerActivityService customerActivityService)
        {
            this._smsTemplateService = smsTemplateService;
            this._numberAccountService = emailAccountService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._messageTokenProvider = messageTokenProvider;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._workflowMessageService = workflowMessageService;
            this._customerActivityService = customerActivityService;
        }

        #endregion
        
        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(SMSTemplate mt, SMSTemplateModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.BccNumberAddresses,
                                                           localized.BccNumberAddresses,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.Subject,
                                                           localized.Subject,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.Body,
                                                           localized.Body,
                                                           localized.LanguageId);

               _localizedEntityService.SaveLocalizedValue(mt,
                                                            x => x.NumberAccountId,
                                                            localized.NumberAccountId,
                                                            localized.LanguageId);
            }
        }
        
        [NonAction]
        protected virtual void PrepareStoresMappingModel(SMSTemplateModel model, SMSTemplate smsTemplate, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && smsTemplate != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(smsTemplate).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(SMSTemplate smsTemplate, SMSTemplateModel model)
        {
            smsTemplate.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(smsTemplate);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(smsTemplate, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion
        
        #region Methods

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSMSTemplates))
                return AccessDeniedView();

            var model = new SMSTemplateListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command, SMSTemplateListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSMSTemplates))
                return AccessDeniedKendoGridJson();

            var messageTemplates = _smsTemplateService.GetAllSMSTemplates(model.SearchStoreId);
            var gridModel = new DataSourceResult
            {
                Data = messageTemplates.Select(x =>
                {
                    var templateModel = x.ToModel();
                    PrepareStoresMappingModel(templateModel, x, false);
                    var stores = _storeService
                            .GetAllStores()
                            .Where(s => !x.LimitedToStores || templateModel.SelectedStoreIds.Contains(s.Id))
                            .ToList();
                    for (int i = 0; i < stores.Count; i++)
                    {
                        templateModel.ListOfStores += stores[i].Name;
                        if (i != stores.Count - 1)
                            templateModel.ListOfStores += ", ";
                    }
                    return templateModel;
                }),
                Total = messageTemplates.Count
            };

            return Json(gridModel);
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSMSTemplates))
                return AccessDeniedView();

            var messageTemplate = _smsTemplateService.GetSMSTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");
            
            var model = messageTemplate.ToModel();
            model.SendImmediately = !model.DelayBeforeSend.HasValue;
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            var allowedTokens = string.Join(", ", _messageTokenProvider.GetListOfAllowedTokens(messageTemplate.GetTokenGroups()));
            model.AllowedTokens = string.Format("{0}{1}{1}{2}{1}", allowedTokens, Environment.NewLine,
                _localizationService.GetResource("Admin.ContentManagement.SMSTemplates.Tokens.ConditionalStatement"));
            //available email accounts
            foreach (var ea in _numberAccountService.GetAllNumberAccounts())
                model.AvailableNumberAccounts.Add(new SelectListItem { Text = ea.DisplayName, Value = ea.Id.ToString() });
            //Store
            PrepareStoresMappingModel(model, messageTemplate, false);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.BccNumberAddresses = messageTemplate.GetLocalized(x => x.BccNumberAddresses, languageId, false, false);
                locale.Subject = messageTemplate.GetLocalized(x => x.Subject, languageId, false, false);
                locale.Body = messageTemplate.GetLocalized(x => x.Body, languageId, false, false);

                locale.NumberAccountId = messageTemplate.GetLocalized(x => x.NumberAccountId, languageId, false, false);
                //available email accounts (we add "Standard" value for localizable field)
                locale.AvailableNumberAccounts.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Admin.ContentManagement.SMSTemplates.Fields.NumberAccount.Standard"),
                    Value = "0"
                });
                foreach (var ea in _numberAccountService.GetAllNumberAccounts())
                    locale.AvailableNumberAccounts.Add(new SelectListItem
                    {
                        Text = ea.DisplayName,
                        Value = ea.Id.ToString(),
                        Selected =  ea.Id == locale.NumberAccountId
                    });
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual ActionResult Edit(SMSTemplateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSMSTemplates))
                return AccessDeniedView();

            var messageTemplate = _smsTemplateService.GetSMSTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                messageTemplate = model.ToEntity(messageTemplate);
                //attached file
                if (!model.HasAttachedDownload)
                    messageTemplate.AttachedDownloadId = 0;
                if (model.SendImmediately)
                    messageTemplate.DelayBeforeSend = null;
                _smsTemplateService.UpdateSMSTemplate(messageTemplate);

                //activity log
                _customerActivityService.InsertActivity("EditMessageTemplate", _localizationService.GetResource("ActivityLog.EditMessageTemplate"), messageTemplate.Id);

                //Stores
                SaveStoreMappings(messageTemplate, model);
                //locales
                UpdateLocales(messageTemplate, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.SMSTemplates.Updated"));
                
                if (continueEditing)
                {
                    return RedirectToAction("Edit",  new {id = messageTemplate.Id});
                }
                return RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            var allowedTokens = string.Join(", ", _messageTokenProvider.GetListOfAllowedTokens(messageTemplate.GetTokenGroups()));
            model.AllowedTokens = string.Format("{0}{1}{1}{2}{1}", allowedTokens, Environment.NewLine,
                _localizationService.GetResource("Admin.ContentManagement.SMSTemplates.Tokens.ConditionalStatement"));
            //available email accounts
            foreach (var ea in _numberAccountService.GetAllNumberAccounts())
                model.AvailableNumberAccounts.Add(new SelectListItem { Text = ea.DisplayName, Value = ea.Id.ToString() });
            //locales (update email account dropdownlists)
            foreach (var locale in model.Locales)
            {
                //available email accounts (we add "Standard" value for localizable field)
                locale.AvailableNumberAccounts.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Admin.ContentManagement.SMSTemplates.Fields.NumberAccount.Standard"),
                    Value = "0"
                });
                foreach (var ea in _numberAccountService.GetAllNumberAccounts())
                    locale.AvailableNumberAccounts.Add(new SelectListItem
                    {
                        Text = ea.DisplayName,
                        Value = ea.Id.ToString(),
                        Selected = ea.Id == locale.NumberAccountId
                    });
            }
            //Store
            PrepareStoresMappingModel(model, messageTemplate, true);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSMSTemplates))
                return AccessDeniedView();

            var messageTemplate = _smsTemplateService.GetSMSTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            _smsTemplateService.DeleteSMSTemplate(messageTemplate);

            //activity log
            _customerActivityService.InsertActivity("DeleteMessageTemplate", _localizationService.GetResource("ActivityLog.DeleteMessageTemplate"), messageTemplate.Id);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.SMSTemplates.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("message-template-copy")]
        public virtual ActionResult CopyTemplate(SMSTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSMSTemplates))
                return AccessDeniedView();

            var messageTemplate = _smsTemplateService.GetSMSTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            try
            {
                var newMessageTemplate = _smsTemplateService.CopySMSTemplate(messageTemplate);
                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.SMSTemplates.Copied"));
                return RedirectToAction("Edit", new { id = newMessageTemplate.Id });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = model.Id });
            }
        }

        public virtual ActionResult TestTemplate(int id, int languageId = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSMSTemplates))
                return AccessDeniedView();

            var messageTemplate = _smsTemplateService.GetSMSTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            var model = new TestSMSTemplateModel
            {
                Id = messageTemplate.Id,
                LanguageId = languageId
            };

            //filter tokens to the current template
            var subject = messageTemplate.GetLocalized(mt => mt.Subject, languageId);
            var body = messageTemplate.GetLocalized(mt => mt.Body, languageId);
            model.Tokens = _messageTokenProvider.GetListOfAllowedTokens().Where(x => subject.Contains(x) || body.Contains(x)).ToList();

            return View(model);
        }

        [HttpPost, ActionName("TestTemplate")]
        [FormValueRequired("send-test")]
        [ValidateInput(false)]
        public virtual ActionResult TestTemplate(TestSMSTemplateModel model, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSMSTemplates))
                return AccessDeniedView();

            var messageTemplate = _smsTemplateService.GetSMSTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("List");

            var tokens = new List<Token>();
            foreach (var formKey in form.AllKeys)
                if (formKey.StartsWith("token_", StringComparison.InvariantCultureIgnoreCase))
                {
                    var tokenKey = formKey.Substring("token_".Length).Replace("%", "");
                    var stringValue = form[formKey];

                    //try get non-string value
                    object tokenValue;
                    bool boolValue;
                    int intValue;
                    decimal decimalValue;
                    if (bool.TryParse(stringValue, out boolValue))
                        tokenValue = boolValue;
                    else if (int.TryParse(stringValue, out intValue))
                        tokenValue = intValue;
                    else if (decimal.TryParse(stringValue, out decimalValue))
                        tokenValue = decimalValue;
                    else
                        tokenValue = stringValue;

                    tokens.Add(new Token(tokenKey, tokenValue));
                }

            _workflowMessageService.SendTestSMS(messageTemplate.Id, model.SendTo, tokens, model.LanguageId);

            if (ModelState.IsValid)
            {
                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.SMSTemplates.Test.Success"));
            }

            return RedirectToAction("Edit", new {id = messageTemplate.Id});
        }

        #endregion
    }
}
