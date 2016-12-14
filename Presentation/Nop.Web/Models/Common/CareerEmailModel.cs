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
    [Validator(typeof(CareerEmailValidator))]
    public partial class CareerEmailModel : BaseNopModel
    {
        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Your Email")]
        public string Email { get; set; }

        [DisplayName("Comments")]
        public string Comments { get; set; }

        [DisplayName("Area of Interest")]
        public string AreaOfInterest { get; set; }

        [DisplayName("Additional Comments")]
        public string ExtraComments { get; set; }

        [DisplayName("Attachment")]
        public string File { get; set; }

        public string JobType { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }


        public enum JobTypeEnum
        {
            [Display(Name = "Account Representative")]
            Account_Representative,

            [Display(Name = "Shipper/Receiver")]
            Shipper_Receiver
        }
    }
}