using FluentValidation;
using Nop.Core.Domain.Common;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Common;

namespace Nop.Web.Validators.Common
{
    public class QuoteValidator : BaseNopValidator<QuoteModel>
    {
        public QuoteValidator()
        {
            RuleFor(x => x.PostalCode).NotEmpty().WithMessage("Postal Code is Mandatory");
            RuleFor(x => x.Quantity).NotEmpty().WithMessage("Quantity is mandatory");
            RuleFor(x => x.ProductNumber).NotEmpty().WithMessage("Product number not present");
            RuleFor(x => x.State).NotEmpty().WithMessage("State not present");
        }
    }
}