using System;
using System.Web.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using Nop.Services.Affiliates;
using Nop.Core.Domain.Affiliates;

namespace Nop.Web.Areas.Mservices.Controllers
{
    public class SMSController : TwilioController
    {
        private readonly IAffiliateService _affiliateService;
        private readonly ICouponService _couponService;

        public SMSController(IAffiliateService affiliateService,
            ICouponService couponService)
        {
            this._affiliateService = affiliateService;
            this._couponService = couponService;
        }

        #region SMS Hook
        [HttpPost]
        public TwiMLResult SMSWebhook(SmsRequest request)
        {
            var response = new MessagingResponse();
            if (request.Body != null)
            {
                var splittedOption = request.Body.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedOption.Length != 2) {
                    response.Message("Please Send SMS with VendorID space coupon Code xxxxx xxxxxx");
                    return TwiML(response);
                }
                   

                if (!int.TryParse(splittedOption[0], out int AffiliatedId))
                {
                    response.Message("Please Send SMS with VendorID space coupon Code xxxxx xxxxxx");
                    return TwiML(response);
                }
                    

                var affiliated = _affiliateService.GetAffiliateById(AffiliatedId);
                if (affiliated == null) {
                    response.Message("Invalid Vendor Id");
                    return TwiML(response);
                }
                   

                string CouponCode = splittedOption[1];

                var coupon = _couponService.GetCouponByCouponCode(CouponCode);
                if (coupon == null)
                {
                    response.Message("Invalid coupon code");
                    return TwiML(response);
                }

                var couponUsageHistory = new CouponUsageHistory
                {
                    AccountSid = request.AccountSid,
                    FromSender = request.From,
                    ToRecipient = request.To,
                    FromCity = request.FromCity,
                    FromState = request.FromState,
                    FromZip = request.FromZip,
                    FromCountry = request.FromCountry,
                    ToCity = request.ToCity,
                    ToState = request.ToState,
                    ToZip = request.ToZip,
                    ToCountry = request.ToCountry,
                    Coupon = coupon,
                    AffiliateId = affiliated.Id,
                    UsedValue = 0,
                    CreatedOnUtc = DateTime.UtcNow
                };
                coupon.CouponUsageHistory.Add(couponUsageHistory);
                _couponService.UpdateCoupon(coupon);

                response.Message("Coupon added Thank you!");
            }
            else
            {
                response.Message("Please Provide Vendor And Coupon Id");
            }
            return TwiML(response);

        }

        #endregion
    }
}