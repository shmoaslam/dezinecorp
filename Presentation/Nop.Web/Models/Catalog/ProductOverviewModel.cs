using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{


    public partial class ColorCode
    {
        public string Code { get; set; }
        public string ColorName { get; set; }
        public string HexCode { get; set; }
    }
    public partial class ProductOverViewGroupModel
    {

        public ProductOverViewGroupModel()
        {
            ProductOverviewModels = new List<ProductOverviewModel>();
        }
        public IList<ProductOverviewModel> ProductOverviewModels { get; set; }
        public string FamilyCode { get; set; }
    }

    public partial class ProductOverviewModel : BaseNopEntityModel
    {
        public ProductOverviewModel()
        {
            ProductPrice = new ProductPriceModel();
            DefaultPictureModel = new PictureModel();
            SpecificationAttributeModels = new List<ProductSpecificationModel>();
            ReviewOverviewModel = new ProductReviewOverviewModel();
        }

        public string Name { get; set; }
        public string FamilyCode { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string SeName { get; set; }
        public string SKU { get; set; }
        public bool MarkAsNew { get; set; }
        public string Material { get; set; }
        public string ItemIsNew { get; set; }
        public ColorCode FirstColor { get; set; }
        public ColorCode SecondColor { get; set; }
        public bool IsDefault { get; set; }
        public string GroupId { get; set; }
        public int OrderId { get; set; }
        //price
        public ProductPriceModel ProductPrice { get; set; }
        //picture
        public PictureModel DefaultPictureModel { get; set; }
        //specification attributes
        public IList<ProductSpecificationModel> SpecificationAttributeModels { get; set; }
        //price
        public ProductReviewOverviewModel ReviewOverviewModel { get; set; }

		#region Nested Classes

        public partial class ProductPriceModel : BaseNopModel
        {
            public string OldPrice { get; set; }
            public string Price { get; set; }
            public decimal PriceValue { get; set; }

            public bool DisableBuyButton { get; set; }
            public bool DisableWishlistButton { get; set; }
            public bool DisableAddToCompareListButton { get; set; }

            public bool AvailableForPreOrder { get; set; }
            public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

            public bool IsRental { get; set; }

            public bool ForceRedirectionAfterAddingToCart { get; set; }

            /// <summary>
            /// A value indicating whether we should display tax/shipping info (used in Germany)
            /// </summary>
            public bool DisplayTaxShippingInfo { get; set; }

            public string PriceDesc { get; set; }
        }

		#endregion
    }
}