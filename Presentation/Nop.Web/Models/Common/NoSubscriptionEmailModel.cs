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
    [Validator(typeof(NoSubscriptionEmailValidator))]
    public class NoSubscriptionEmailModel : BaseNopModel
    {
        [DisplayName("Email")]
        public string Email { get; set; }

      
    }
}