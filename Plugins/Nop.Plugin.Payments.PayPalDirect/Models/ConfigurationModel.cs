﻿using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Awein.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Awein.Fields.ClientId")]
        public string ClientId { get; set; }
        public bool ClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Awein.Fields.ClientSecret")]
        public string ClientSecret { get; set; }
        public bool ClientSecret_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Awein.Fields.WebhookId")]
        public string WebhookId { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Awein.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Awein.Fields.PassPurchasedItems")]
        public bool PassPurchasedItems { get; set; }
        public bool PassPurchasedItems_OverrideForStore { get; set; }

        public int TransactModeId { get; set; }
        public bool TransactModeId_OverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.Awein.Fields.TransactMode")]
        public SelectList TransactModeValues { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Awein.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Awein.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }
    }
}