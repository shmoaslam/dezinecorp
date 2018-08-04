using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Catalog;
using Nop.Web.Validators.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Nop.Web.Models.Common
{
    [Validator(typeof(SubscriptionEmailValidator))]
    public class SubscriptionEmailModel : BaseNopModel
    {
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Company")]
        public string Company { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }

        [DisplayName("Province")]
        public string Province { get; set; }


    }
}