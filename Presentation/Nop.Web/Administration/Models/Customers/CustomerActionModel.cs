﻿using FluentValidation.Attributes;
using Nop.Core.Domain.Customers;
using Nop.Admin.Validators.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc;
using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Admin.Models.Customers
{
    [Validator(typeof(CustomerActionValidator))]
    public partial class CustomerActionModel : BaseNopEntityModel
    {
        public CustomerActionModel()
        {
            this.ActionType = new List<SelectListItem>();
            this.Banners = new List<SelectListItem>();
            this.InteractiveForms = new List<SelectListItem>();
            this.MessageTemplates = new List<SelectListItem>();
            this.CustomerRoles = new List<SelectListItem>();
            this.CustomerTags = new List<SelectListItem>();

        }

        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.ActionTypeId")]
        public int ActionTypeId { get; set; }
        public IList<SelectListItem> ActionType { get; set; }


        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.ConditionId")]
        public int ConditionId { get; set; }
        public int ConditionCount { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.ReactionType")]
        public int ReactionTypeId { get; set; }
        public CustomerReactionTypeEnum ReactionType
        {
            get { return (CustomerReactionTypeEnum)ReactionTypeId; }
            set { this.ReactionTypeId = (int)value; }
        }

        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.Banner")]
        public int BannerId { get; set; }
        public IList<SelectListItem> Banners { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.InteractiveForm")]
        public int InteractiveFormId { get; set; }
        public IList<SelectListItem> InteractiveForms { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.MessageTemplate")]
        public int MessageTemplateId { get; set; }
        public IList<SelectListItem> MessageTemplates { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.CustomerRole")]
        public int CustomerRoleId { get; set; }
        public IList<SelectListItem> CustomerRoles { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.CustomerTag")]
        public int CustomerTagId { get; set; }
        public IList<SelectListItem> CustomerTags { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.StartDateTime")]
        public DateTime StartDateTime { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAction.Fields.EndDateTime")]
        public DateTime EndDateTime { get; set; }

    }

}