﻿using FluentValidation;
using FluentValidation.Results;
using Nop.Admin.Models.Vendors;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Vendors
{
    public partial class VendorValidator : BaseNopValidator<VendorModel>
    {
        public VendorValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Vendors.Fields.Name.Required"));
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Admin.Vendors.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"));
            RuleFor(x => x.PageSizeOptions).Must(ValidatorUtilities.PageSizeOptionsValidator).WithMessage(localizationService.GetResource("Admin.Vendors.Fields.PageSizeOptions.ShouldHaveUniqueItems"));
            Custom(x =>
            {
                if (!x.AllowCustomersToSelectPageSize && x.PageSize <= 0)
                    return new ValidationFailure("PageSize", localizationService.GetResource("Admin.Vendors.Fields.PageSize.Positive"));

                return null;
            });
            RuleFor(x => x.StandAloneAppPackageIdAndroid).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.vendors.Fields.StandAloneAppPackageIdAndroid.Required")).When(x=>x.StandAloneAppAndroid);
            RuleFor(x => x.StandAloneAppPackageIdIos).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.vendors.Fields.StandAloneAppPackageIdAndroid.Required")).When(x => x.StandAloneAppIos);
            RuleFor(x => x.StandAloneAppUrlSchemesIos).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.vendors.Fields.StandAloneAppUrlSchemesIos.Required")).When(x => x.StandAloneAppIos);
            RuleFor(x => x.StandAloneAppIdIos).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.vendors.Fields.StandAloneAppUrlSchemesIos.Required")).When(x => x.StandAloneAppIos);

            SetDatabaseValidationRules<Vendor>(dbContext);
        }
    }
}