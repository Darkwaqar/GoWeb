using Newtonsoft.Json;
using Nop.Core.Domain.Fcm;
using Nop.Core.Domain.Messages;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Fcm
{

    /// <summary>
    /// Fcm sender
    /// </summary>
    public partial class FcmSender : IFcmSender
    {
        private static Uri FireBasePushNotificationsURL = new Uri("https://fcm.googleapis.com/fcm/send");
        private readonly IDeviceService _deviceService;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        public FcmSender(IDeviceService deviceService,
            ILogger logger,
            ILocalizationService localizationService)
        {
            this._deviceService = deviceService;
            this._logger = logger;
            this._localizationService = localizationService;
        }

        //private static string ServerKey = "AIzaSyD3387xB5MTRaMdhmWO5RFjwQVpFAMDe20";
        public bool SendFcmAsync(QueuedFcm queuedFcm, FcmAction action)
        {
            Object messageInformation = new object { };
            if (queuedFcm.FcmType == FcmType.Simple)
            {
                messageInformation = new
                {
                    notification = new
                    {
                        title = queuedFcm.Title,
                        body = queuedFcm.Message,
                        badge = 1,
                        icon = queuedFcm.Icon
                    },
                    data = new
                    {
                        fcmtype = queuedFcm.FcmType,
                        fcmApplicationType = queuedFcm.FcmApplicationType,
                        fcmActionType = action.FcmActionType,
                        vendorId = queuedFcm.VendorId
                    },
                    to = queuedFcm.DeviceId
                };
            }
            else if (queuedFcm.FcmType == FcmType.WebActivity)
            {
                messageInformation = new
                {
                    notification = new
                    {
                        title = queuedFcm.Title,
                        body = queuedFcm.Message,
                        badge = 1,
                        icon = queuedFcm.Icon,
                        click_action = queuedFcm.GotoLink
                    },
                    data = new
                    {
                        button_action = queuedFcm.GotoLink,
                        fcmtype = queuedFcm.FcmType,
                        fcmApplicationType = queuedFcm.FcmApplicationType,
                        fcmActionType = action.FcmActionType,
                        vendorId = queuedFcm.VendorId
                    },
                    to = queuedFcm.DeviceId
                };
            }
            else
            {
                messageInformation = new
                {
                    notification = new
                    {
                        title = queuedFcm.Title,
                        body = queuedFcm.Message,
                        badge = 1,
                        icon = queuedFcm.Icon,
                        click_action = action.FcmActionType
                    },
                    data = new
                    {
                        image = queuedFcm.Image,
                        button_name = action.Name,
                        button_action = action.Extra,
                        fcmtype = queuedFcm.FcmType,
                        fcmApplicationType = queuedFcm.FcmApplicationType,
                        fcmActionType= action.FcmActionType,
                        vendorId=queuedFcm.VendorId
                    },
                    mutable_content = true,
                    to = queuedFcm.DeviceId
                };
            }

            //Object to JSON STRUCTURE => using Newtonsoft.Json;
            string jsonMessage = JsonConvert.SerializeObject(messageInformation);
            //Create request to Firebase API
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            tRequest.ContentType = "application/json";

            Byte[] byteArray = Encoding.UTF8.GetBytes(jsonMessage);
            tRequest.Headers.Add(string.Format("Authorization: key={0}", queuedFcm.AppKey));
            tRequest.ContentLength = byteArray.Length;
            try
            {
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                var response = JsonConvert.DeserializeObject<JsonResponse>(sResponseFromServer);

                                if (response.failure > 0)
                                {
                                    _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error occured while sending push notification." + sResponseFromServer);
                                    var device = _deviceService.GetDeviceById(queuedFcm.DevicePId);
                                     device.Active = false;
                                     device.Error = sResponseFromServer;
                                    _deviceService.UpdateDevice(device);
                                }

                                //return (true, string.Format(_localizationService.GetResource("Admin.PushNotifications.MessageSent"), response.success, response.failure));
                                return (true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //return (false, ex.Message);
                _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error occured while sending push notification." , ex.Message);
                return (false);
            }
        }

        public class Result
        {
            public string error { get; set; }
        }

        public class JsonResponse
        {
            public long multicast_id { get; set; }
            public int success { get; set; }
            public int failure { get; set; }
            public int canonical_ids { get; set; }
            public List<Result> results { get; set; }
        }
    }
}
