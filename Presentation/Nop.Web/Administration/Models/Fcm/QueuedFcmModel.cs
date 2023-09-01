using FluentValidation.Attributes;
using Nop.Admin.Validators.Messages;
using Nop.Core.Domain.Fcm;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Admin.Models.Fcm
{
    [Validator(typeof(QueuedFcmValidator))]
    public partial class QueuedFcmModel : BaseNopEntityModel
    {

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.Id")]
        public override int Id { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.Priority")]
        public string PriorityName { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.From")]
        [AllowHtml]
        public string From { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.FromName")]
        [AllowHtml]
        public string FromName { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.DeviceId")]
        [AllowHtml]
        public string DeviceId { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.Title")]
        [AllowHtml]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.Message")]
        [AllowHtml]
        public string Message { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.Image")]
        [AllowHtml]
        public string Image { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.Action")]
        [AllowHtml]
        public string Action { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.Package")]
        [AllowHtml]
        public string Package { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.AppKey")]
        [AllowHtml]
        public string AppKey { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.SendImmediately")]
        public bool SendImmediately { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.DontSendBeforeDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? DontSendBeforeDate { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.SentTries")]
        public int SentTries { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.SentOn")]
        public DateTime? SentOn { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.GotoLink")]
        public string GotoLink { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.Intent")]
        public int Intent { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.FcmType")]
        public FcmType FcmType { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.FcmApplicationType")]
        public FcmApplicationType FcmApplicationType { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.Icon")]
        public string Icon { get; set; }


        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.VendorId")]
        public int VendorId { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedFcms.Fields.StoreId")]
        public int StoreId { get; set; }
    }
}