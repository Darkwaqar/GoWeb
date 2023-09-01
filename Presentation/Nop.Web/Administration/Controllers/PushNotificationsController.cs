using Nop.Admin.Models.PushNotifications;
using Nop.Core.Domain.PushNotifications;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.PushNotifications;
using Nop.Web.Framework.Kendoui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class PushNotificationsController : BaseAdminController
    {
        private readonly PushNotificationsSettings _pushNotificationsSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly ICustomerService _customerService;
        private readonly IPictureService _pictureService;
        private readonly IDateTimeHelper _dateTimeHelper;

        public PushNotificationsController(
            PushNotificationsSettings pushNotificationsSettings,
            ILocalizationService localizationService,
            ISettingService settingService,
            IPushNotificationsService pushNotificationsService,
            ICustomerService customerService,
            IPictureService pictureService,
            IDateTimeHelper dateTimeHelper)
        {
            _pushNotificationsSettings = pushNotificationsSettings;
            _localizationService = localizationService;
            _settingService = settingService;
            _pushNotificationsService = pushNotificationsService;
            _customerService = customerService;
            _pictureService = pictureService;
            _dateTimeHelper = dateTimeHelper;
        }

        public ActionResult Send()
        {
            var model = new PushModel
            {
                MessageText = _localizationService.GetResource("Admin.PushNotifications.MessageTextPlaceholder"),
                Title = _localizationService.GetResource("Admin.PushNotifications.MessageTitlePlaceholder"),
                PictureId = _pushNotificationsSettings.PictureId,
                ClickUrl = _pushNotificationsSettings.ClickUrl
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Send(PushModel model)
        {
            if (!string.IsNullOrEmpty(_pushNotificationsSettings.PrivateApiKey) && !string.IsNullOrEmpty(model.MessageText))
            {
                _pushNotificationsSettings.PictureId = model.PictureId;
                _pushNotificationsSettings.ClickUrl = model.ClickUrl;
                _settingService.SaveSetting(_pushNotificationsSettings);
                var pictureUrl = _pictureService.GetPictureUrl(model.PictureId);
                var result = (_pushNotificationsService.SendPushNotification(model.Title, model.MessageText, pictureUrl, model.ClickUrl));
                if (result.Item1)
                {
                    SuccessNotification(result.Item2);
                }
                else
                {
                    ErrorNotification(result.Item2);
                }
            }
            else
            {
                ErrorNotification(_localizationService.GetResource("PushNotifications.Error.PushApiMessage"));
            }

            return RedirectToAction("Send");
        }

        public ActionResult Messages()
        {
            var model = new MessagesModel
            {
                Allowed = _pushNotificationsService.GetAllowedReceivers(),
                Denied = _pushNotificationsService.GetDeniedReceivers()
            };

            return View(model);
        }

        public ActionResult Receivers()
        {
            var model = new ReceiversModel
            {
                Allowed = _pushNotificationsService.GetAllowedReceivers(),
                Denied = _pushNotificationsService.GetDeniedReceivers()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult PushMessagesList(DataSourceRequest command)
        {
            var messages = _pushNotificationsService.GetPushMessages(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = messages.Select(x => new PushMessageListModel
                {
                    Id = x.Id,
                    Text = x.Text,
                    Title = x.Title,
                    SentOn = _dateTimeHelper.ConvertToUserTime(x.SentOn, DateTimeKind.Utc),
                    NumberOfReceivers = x.NumberOfReceivers
                }),
                Total = messages.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult PushReceiversList(DataSourceRequest command)
        {
            var receivers = _pushNotificationsService.GetPushReceivers(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult();
            var list = new List<PushRegistrationListModel>();
            foreach (var receiver in receivers)
            {
                var gridReceiver = new PushRegistrationListModel();

                var customer = _customerService.GetCustomerById(receiver.CustomerId);
                if (customer == null)
                {
                    _pushNotificationsService.DeletePushReceiver(receiver);
                    continue;
                }

                if (!string.IsNullOrEmpty(customer.Email))
                {
                    gridReceiver.CustomerEmail = customer.Email;
                }
                else
                {
                    gridReceiver.CustomerEmail = _localizationService.GetResource("Admin.Customers.Guest");
                }

                gridReceiver.CustomerId = receiver.CustomerId;
                gridReceiver.Id = receiver.Id;
                gridReceiver.RegisteredOn = _dateTimeHelper.ConvertToUserTime(receiver.RegisteredOn, DateTimeKind.Utc);
                gridReceiver.Token = receiver.Token;
                gridReceiver.Allowed = receiver.Allowed;

                list.Add(gridReceiver);
            }

            gridModel.Data = list;
            gridModel.Total = receivers.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult DeleteReceiver(int id)
        {
            var receiver = _pushNotificationsService.GetPushReceiver(id);
            _pushNotificationsService.DeletePushReceiver(receiver);

            return RedirectToAction("List");
        }
    }
}