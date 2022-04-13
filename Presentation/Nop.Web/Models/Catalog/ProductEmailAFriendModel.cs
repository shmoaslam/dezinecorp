﻿using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Catalog;

namespace Nop.Web.Models.Catalog
{
    [Validator(typeof(ProductEmailAFriendValidator))]
    public partial class ProductEmailAFriendModel : BaseNopModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public string ProductSeName { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Products.EmailAFriend.FriendEmail")]
        public string FriendEmail { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Products.EmailAFriend.YourEmailAddress")]
        public string YourEmailAddress { get; set; }

        [AllowHtml]
        [NopResourceDisplayName("Products.EmailAFriend.PersonalMessage")]
        public string PersonalMessage { get; set; }

        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }

        public string CaptchaClientKey { get; set; }
        public bool DisplayCaptcha { get; set; }

        //public bool IsEmailForm { get; set; }


        //public string Name { get; set; }

        //public string Email { get; set; }
        
        //public string PhoneNumber { get; set; }

        //public string Company { get; set; }
        //public string Query { get; set; }
        //public string ProductNumber { get; set; }
        //public int Quantity { get; set; }
    }
}