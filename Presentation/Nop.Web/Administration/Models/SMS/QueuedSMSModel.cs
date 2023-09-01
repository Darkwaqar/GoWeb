using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Admin.Validators.SMS;

namespace Nop.Admin.Models.SMS
{
    [Validator(typeof(QueuedSMSValidator))]
    public partial class QueuedSMSModel: BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.Id")]
        public override int Id { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.Priority")]
        public string PriorityName { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.From")]
        [AllowHtml]
        public string From { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.FromName")]
        [AllowHtml]
        public string FromName { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.To")]
        [AllowHtml]
        public string To { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.ToName")]
        [AllowHtml]
        public string ToName { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.ReplyTo")]
        [AllowHtml]
        public string ReplyTo { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.ReplyToName")]
        [AllowHtml]
        public string ReplyToName { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.CC")]
        [AllowHtml]
        public string CC { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.Bcc")]
        [AllowHtml]
        public string Bcc { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.Subject")]
        [AllowHtml]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.AttachmentFilePath")]
        [AllowHtml]
        public string AttachmentFilePath { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.AttachedDownload")]
        [UIHint("Download")]
        public int AttachedDownloadId { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.SendImmediately")]
        public bool SendImmediately { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.DontSendBeforeDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? DontSendBeforeDate { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.SentTries")]
        public int SentTries { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.SentOn")]
        public DateTime? SentOn { get; set; }

        [NopResourceDisplayName("Admin.System.QueuedSMS.Fields.NumberAccountName")]
        [AllowHtml]
        public string NumberAccountName { get; set; }
    }
}