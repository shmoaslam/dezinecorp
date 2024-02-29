using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Common;

namespace Nop.Web.Models.Common
{
    [Validator(typeof(QuoteValidator))]
    public partial class QuoteModel : BaseNopModel
    {
        public string ProductNumber { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public int Quantity { get; set; }
        public double FinalQuote { get; set; }
        public string IsQuoteForResidential { get; set; }
        public bool SuccessfullySent { get; set; }
        public string ErrorMessage { get; set; }

    }
}