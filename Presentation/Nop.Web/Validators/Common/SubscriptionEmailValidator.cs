using Microsoft.Ajax.Utilities;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace Nop.Web.Validators.Common  
{
    public class SubscriptionEmailValidator : BaseNopValidator<SubscriptionEmailModel>
    {
        public SubscriptionEmailValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please provide your first name!");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Please provide your email!");
            RuleFor(x => x.PostalCode).NotEmpty().WithMessage("Please provide your Postal Code!");
            RuleFor(x => x.Province).NotEmpty().WithMessage("Please provide your Province!");
        }
    }
}