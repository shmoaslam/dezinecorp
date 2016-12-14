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
    public class CareerEmailValidator : BaseNopValidator<CareerEmailModel>
    {
        public CareerEmailValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please provide your name!");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Please provide your email!");
        }
    }
}