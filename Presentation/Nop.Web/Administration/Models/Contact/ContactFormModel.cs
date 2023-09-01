using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Admin.Models.Contact
{
    public class ContactFormModel : BaseNopEntityModel
    {
        public ContactFormModel()
        {
            this.AvailableEmailAccounts = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.Store")]
        public string Store { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.Email")]
        public string Email { get; set; }

        public string FullName { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.IpAddress")]
        public string IpAddress { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.Subject")]
        public string Subject { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.Enquiry")]
        public string Enquiry { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.ContactAttributeDescription")]
        public string ContactAttributeDescription { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.EmailAccountName")]

        public string EmailAccountName { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.SendImmediately")]
        public bool SendImmediately { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.SentOn")]
        public DateTime? SentOn { get; set; }


        [NopResourceDisplayName("Admin.System.ContactForm.Fields.EmailAccount")]
        public int EmailAccountId { get; set; }
        public IList<SelectListItem> AvailableEmailAccounts { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.FromName")]
        [AllowHtml]
        public string FromName { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.To")]
        [AllowHtml]
        public string To { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.ToName")]
        [AllowHtml]
        public string ToName { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.ReplyTo")]
        [AllowHtml]
        public string ReplyTo { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.ReplyToName")]
        [AllowHtml]
        public string ReplyToName { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.CC")]
        [AllowHtml]
        public string CC { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.Bcc")]
        [AllowHtml]
        public string Bcc { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.Body")]
        [AllowHtml]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.AttachmentFilePath")]
        [AllowHtml]
        public string AttachmentFilePath { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.AttachedDownload")]
        [UIHint("Download")]
        public int AttachedDownloadId { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.DontSendBeforeDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? DontSendBeforeDate { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.SentTries")]
        public int SentTries { get; set; }

        [NopResourceDisplayName("Admin.System.ContactForm.Fields.QueuedEmailId")]
        public int QueuedEmailId { get; set; }
    }
}