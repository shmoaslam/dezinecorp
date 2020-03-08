using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductDetailsModel : BaseNopEntityModel
    {
        public ProductDetailsModel()
        {
            DefaultPictureModel = new PictureModel();
            PictureModels = new List<PictureModel>();
            GiftCard = new GiftCardModel();
            ProductPrice = new ProductPriceModel();
            AddToCart = new AddToCartModel();
            ProductAttributes = new List<ProductAttributeModel>();
            AssociatedProducts = new List<ProductDetailsModel>();
            VendorModel = new VendorBriefInfoModel();
            Breadcrumb = new ProductBreadcrumbModel();
            ProductTags = new List<ProductTagModel>();
            ProductSpecifications= new List<ProductSpecificationModel>();
            ProductManufacturers = new List<ManufacturerModel>();
            ProductReviewOverview = new ProductReviewOverviewModel();
            TierPrices = new List<TierPriceModel>();
        }

        //picture(s)
        public bool DefaultPictureZoomEnabled { get; set; }
        public PictureModel DefaultPictureModel { get; set; }
        public IList<PictureModel> PictureModels { get; set; }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string ProductTemplateViewPath { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }

        public bool ShowSku { get; set; }
        public string Sku { get; set; }

        public bool ShowManufacturerPartNumber { get; set; }
        public string ManufacturerPartNumber { get; set; }

        public bool ShowGtin { get; set; }
        public string Gtin { get; set; }

        public bool ShowVendor { get; set; }
        public VendorBriefInfoModel VendorModel { get; set; }

        public bool HasSampleDownload { get; set; }

        public GiftCardModel GiftCard { get; set; }

        public bool IsShipEnabled { get; set; }
        public bool IsFreeShipping { get; set; }
        public bool FreeShippingNotificationEnabled { get; set; }
        public string DeliveryDate { get; set; }


        public bool IsRental { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }

        public string StockAvailability { get; set; }

        public bool DisplayBackInStockSubscription { get; set; }

        public bool EmailAFriendEnabled { get; set; }
        public bool CompareProductsEnabled { get; set; }

        public string PageShareCode { get; set; }

        public ProductPriceModel ProductPrice { get; set; }

        public AddToCartModel AddToCart { get; set; }

        public ProductBreadcrumbModel Breadcrumb { get; set; }

        public IList<ProductTagModel> ProductTags { get; set; }

        public IList<ProductAttributeModel> ProductAttributes { get; set; }

        public IList<ProductSpecificationModel> ProductSpecifications { get; set; }

        public IList<ManufacturerModel> ProductManufacturers { get; set; }

        public ProductReviewOverviewModel ProductReviewOverview { get; set; }

        public IList<TierPriceModel> TierPrices { get; set; }
        
        public IList<DezineCorpTierPriceModel> DTierPrices { get; set; }
        

        public DezineCorpData DData { get; set; }

       // public DezinecorpBrandingData BrandingData { get; set; }

        public IList<DezineCorpRelatedOrFamilyProduct> DRelatedProducts { get; set; }

        public IList<DezineCorpRelatedOrFamilyProduct> DBrandingProducts { get; set; }

        public IList<DezineCorpRelatedOrFamilyProduct> DFamilyProducts { get; set; }

        //a list of associated products. For example, "Grouped" products could have several child "simple" products
        public IList<ProductDetailsModel> AssociatedProducts { get; set; }

        public bool DisplayDiscontinuedMessage { get; set; }

        #region Nested Classes

        public partial class ProductBreadcrumbModel : BaseNopModel
        {
            public ProductBreadcrumbModel()
            {
                CategoryBreadcrumb = new List<CategorySimpleModel>();
            }

            public bool Enabled { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public IList<CategorySimpleModel> CategoryBreadcrumb { get; set; }
        }

        public partial class AddToCartModel : BaseNopModel
        {
            public AddToCartModel()
            {
                this.AllowedQuantities = new List<SelectListItem>();
            }
            public int ProductId { get; set; }

            //qty
            [NopResourceDisplayName("Products.Qty")]
            public int EnteredQuantity { get; set; }
            public string MinimumQuantityNotification { get; set; }
            public List<SelectListItem> AllowedQuantities { get; set; }

            //price entered by customers
            [NopResourceDisplayName("Products.EnterProductPrice")]
            public bool CustomerEntersPrice { get; set; }
            [NopResourceDisplayName("Products.EnterProductPrice")]
            public decimal CustomerEnteredPrice { get; set; }
            public String CustomerEnteredPriceRange { get; set; }

            public bool DisableBuyButton { get; set; }
            public bool DisableWishlistButton { get; set; }

            //rental
            public bool IsRental { get; set; }

            //pre-order
            public bool AvailableForPreOrder { get; set; }
            public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

            //updating existing shopping cart item?
            public int UpdatedShoppingCartItemId { get; set; }
        }

        public partial class ProductPriceModel : BaseNopModel
        {
            /// <summary>
            /// The currency (in 3-letter ISO 4217 format) of the offer price 
            /// </summary>
            public string CurrencyCode { get; set; }

            public string OldPrice { get; set; }

            public string Price { get; set; }
            public string PriceWithDiscount { get; set; }
            public decimal PriceValue { get; set; }

            public bool CustomerEntersPrice { get; set; }

            public bool CallForPrice { get; set; }

            public int ProductId { get; set; }

            public bool HidePrices { get; set; }

            //rental
            public bool IsRental { get; set; }
            public string RentalPrice { get; set; }

            /// <summary>
            /// A value indicating whether we should display tax/shipping info (used in Germany)
            /// </summary>
            public bool DisplayTaxShippingInfo { get; set; }
            /// <summary>
            /// PAngV baseprice (used in Germany)
            /// </summary>
            public string BasePricePAngV { get; set; }
        }

        public partial class GiftCardModel : BaseNopModel
        {
            public bool IsGiftCard { get; set; }

            [NopResourceDisplayName("Products.GiftCard.RecipientName")]
            [AllowHtml]
            public string RecipientName { get; set; }
            [NopResourceDisplayName("Products.GiftCard.RecipientEmail")]
            [AllowHtml]
            public string RecipientEmail { get; set; }
            [NopResourceDisplayName("Products.GiftCard.SenderName")]
            [AllowHtml]
            public string SenderName { get; set; }
            [NopResourceDisplayName("Products.GiftCard.SenderEmail")]
            [AllowHtml]
            public string SenderEmail { get; set; }
            [NopResourceDisplayName("Products.GiftCard.Message")]
            [AllowHtml]
            public string Message { get; set; }

            public GiftCardType GiftCardType { get; set; }
        }

        public partial class TierPriceModel : BaseNopModel
        {
            public string Price { get; set; }

            public int Quantity { get; set; }

        }

        public partial class DezineCorpTierPriceModel : BaseNopModel
        {
            public string PriceName { get; set; }

            /// <summary>
            /// Gets or sets the Price 1
            /// </summary>
            public string Price1 { get; set; }

            /// <summary>
            /// Gets or sets the Price 2
            /// </summary>
            public string Price2 { get; set; }

            /// <summary>
            /// Gets or sets the Price 3
            /// </summary>
            public string Price3 { get; set; }

            /// <summary>
            /// Gets or sets the Price 4
            /// </summary>
            public string Price4 { get; set; }

            /// <summary>
            /// Gets or sets the Discount Code
            /// </summary>
            public string DiscountCode { get; set; }
        }

        public partial class DezineCorpData
        {

            /// <summary>
            /// Gets or sets the new page string
            /// </summary>
            public string NewPage { get; set; }

            /// <summary>
            /// Gets or sets the Item is New
            /// </summary>
            public string ItemIsNew { get; set; }

            /// <summary>
            /// Gets or sets the Guarenteed Stock
            /// </summary>
            public string GuarenteedStock { get; set; }

            /// <summary>
            /// Gets or sets the Materials
            /// </summary>
            public string Materials { get; set; }

            /// <summary>
            /// Gets or sets the Features
            /// </summary>
            public string Features { get; set; }

            /// <summary>
            /// Gets or sets the Includes
            /// </summary>
            public string Includes { get; set; }

            /// <summary>
            /// Gets or sets the Specail Packaging
            /// </summary>
            public string SpecailPackaging { get; set; }

            /// <summary>
            /// Gets or sets the Capacity
            /// </summary>
            public string Capacity { get; set; }

            /// <summary>
            /// Gets or sets the Size
            /// </summary>
            public string Size { get; set; }

            /// <summary>
            /// Gets or sets the Imprint Area In Outboard
            /// </summary>
            public string ImprintAreaInOutboard { get; set; }

            /// <summary>
            /// Gets or sets the Imprint Area Wrap Around
            /// </summary>
            public string ImprintAreaWrapAround { get; set; }

            /// <summary>
            /// Gets or sets the Decorating Option
            /// </summary>
            public string DecoratingOption { get; set; }

            /// <summary>
            /// Gets or sets the Peices Per Cartoon
            /// </summary>
            public string PeicesPerCartoon { get; set; }

            /// <summary>
            /// Gets or sets the Weight Per Cartoon
            /// </summary>
            public string WeightPerCartoon { get; set; }

            /// <summary>
            /// Gets or sets the Blank Line
            /// </summary>
            public string BlankLine { get; set; }

            /// <summary>
            /// Gets or sets the Protective Packaging
            /// </summary>
            public string ProtectivePackaging { get; set; }

            /// <summary>
            /// Gets or sets the Refer To Catalogue Page
            /// </summary>
            public string ReferToCataloguePage { get; set; }

            /// <summary>
            /// Gets or sets the Pricing Flag
            /// </summary>
            public string PricingFlag { get; set; }

            /// <summary>
            /// Gets or sets the Made in Canada
            /// </summary>
            public string MadeinCanada { get; set; }

            /// <summary>
            /// Gets or sets the Made in North America
            /// </summary>
            public string MadeinNorthAmerica { get; set; }

            /// <summary>
            /// Gets or sets the Inventory Flag
            /// </summary>
            public string InventoryFlag { get; set; }

            /// <summary>
            /// Gets or sets the Pricing Code
            /// </summary>
            public string PricingCode { get; set; }

            /// <summary>
            /// Gets or sets the Pricing Footer Note
            /// </summary>
            public string PricingFooterNote { get; set; }

            /// <summary>
            /// Gets or sets the Setup Per Colour
            /// </summary>
            public string SetupPerColour { get; set; }

            /// <summary>
            /// Gets or sets the Repeat Setup
            /// </summary>
            public string RepeatSetup { get; set; }

            /// <summary>
            /// Gets or sets the Deboss Setup
            /// </summary>
            public string DebossSetup { get; set; }

            /// <summary>
            /// Gets or sets the Repeat Deboss
            /// </summary>
            public string RepeatDeboss { get; set; }

            /// <summary>
            /// Gets or sets the Decal Setup
            /// </summary>
            public string DecalSetup { get; set; }

            /// <summary>
            /// Gets or sets the Repeat Decal
            /// </summary>
            public string RepeatDecal { get; set; }

            /// <summary>
            /// Gets or sets the Laser Setup
            /// </summary>
            public string LaserSetup { get; set; }

            /// <summary>
            /// Gets or sets the Repeat Laser
            /// </summary>
            public string RepeatLaser { get; set; }

            /// <summary>
            /// Gets or sets the Additional Charge 1
            /// </summary>
            public string AdditionalCharge1 { get; set; }

            /// <summary>
            /// Gets or sets the Additional Charge 2
            /// </summary>
            public string AdditionalCharge2 { get; set; }

            /// <summary>
            /// Gets or sets the Additional Charge 3
            /// </summary>
            public string AdditionalCharge3 { get; set; }

            /// <summary>
            /// Gets or sets the Additional Charge 4
            /// </summary>
            public string AdditionalCharge4 { get; set; }

            /// <summary>
            /// Gets or sets the Repeat Term
            /// </summary>
            public string RepeatTerm { get; set; }

            /// <summary>
            /// Gets or sets the Final Note
            /// </summary>
            public string FinalNote { get; set; }

            /// <summary>
            /// Gets or sets the Regular Price
            /// </summary>
            public string RegularPrice { get; set; }

            /// <summary>
            /// Gets or sets the Regular Price 1
            /// </summary>
            public string RegularPrice1 { get; set; }

            /// <summary>
            /// Gets or sets the Regular Price 2
            /// </summary>
            public string RegularPrice2 { get; set; }

            /// <summary>
            /// Gets or sets the Regular Price 3
            /// </summary>
            public string RegularPrice3 { get; set; }

            /// <summary>
            /// Gets or sets the Regular Price 4
            /// </summary>
            public string RegularPrice4 { get; set; }

            /// <summary>
            /// Gets or sets the Regular Price Code
            /// </summary>
            public string RegularPriceCode { get; set; }

            /// <summary>
            /// Gets or sets the Special Price Ends
            /// </summary>
            public string SpecialPriceEnds { get; set; }

            /// <summary>
            /// Gets or sets the Special Price Ends
            /// </summary>
            public string CartonDimensions { get; set; }

            /// <summary>
            /// Gets or sets the Special Price Ends
            /// </summary>
            public string VisualHeading { get; set; }

            /// <summary>
            /// Gets or sets the Special Price Ends
            /// </summary>
            public string FamilyCode { get; set; }

            /// <summary>
            /// Gets or sets the Special Price Ends
            /// </summary>
            public string VisualPrice { get; set; }


        }

        //public partial class DezinecorpBrandingData 
        //{

        //    public string UseAlternateImprintType { get; set; }

        //    public string SageProductSize { get; set; }

        //    public string SageDescription { get; set; }

        //    public string BrandingA { get; set; }

        //    public string BrandingALocation1 { get; set; }

        //    public string BrandingALocation1MeasurementType { get; set; }

        //    public double? BrandingALocation1Heigth { get; set; }

        //    public double? BrandingALocation1Width { get; set; }

        //    public string BrandingALocation2 { get; set; }

        //    public string BrandingALocation2MeasurementType { get; set; }

        //    public double? BrandingALocation2Heigth { get; set; }

        //    public double? BrandingALocation2Width { get; set; }

        //    public string BrandingB { get; set; }

        //    public string BrandingBLocation1 { get; set; }

        //    public string BrandingBLocation1MeasurementType { get; set; }

        //    public double? BrandingBLocation1Heigth { get; set; }

        //    public double? BrandingBLocation1Width { get; set; }

        //    public string BrandingBLocation2 { get; set; }

        //    public string BrandingBLocation2MeasurementType { get; set; }

        //    public double? BrandingBLocation2Heigth { get; set; }

        //    public double? BrandingBLocation2Width { get; set; }

        //    public string BrandingC { get; set; }

        //    public string BrandingCProductNumber { get; set; }

        //    public string BrandingD { get; set; }

        //    public string BrandingDProductNumber { get; set; }

        //    public string MappedItemNumber { get; set; }

        //    public string BrandingAProductNumber { get; set; }

        //    public string BrandingBProductNumber { get; set; }

        //    public string BrandingE { get; set; }

        //    public string BrandingEProductNumber { get; set; }

        //    public string BrandingF { get; set; }

        //    public string BrandingFProductNumber { get; set; }

        //    public string BrandingFamily { get; set; }
        //}

        public partial class DezineCorpRelatedOrFamilyProduct
        {
            public string Name { get; set; }
            public string SKU { get; set; }
            public string SEName { get; set; }
            public PictureModel DefaultPicture { get; set; }

            public string BrandingType { get; set; }
        }

        public partial class ProductAttributeModel : BaseNopEntityModel
        {
            public ProductAttributeModel()
            {
                AllowedFileExtensions = new List<string>();
                Values = new List<ProductAttributeValueModel>();
            }

            public int ProductId { get; set; }

            public int ProductAttributeId { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public string TextPrompt { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }
            /// <summary>
            /// Selected day value for datepicker
            /// </summary>
            public int? SelectedDay { get; set; }
            /// <summary>
            /// Selected month value for datepicker
            /// </summary>
            public int? SelectedMonth { get; set; }
            /// <summary>
            /// Selected year value for datepicker
            /// </summary>
            public int? SelectedYear { get; set; }

            /// <summary>
            /// A value indicating whether this attribute depends on some other attribute
            /// </summary>
            public bool HasCondition { get; set; }

            /// <summary>
            /// Allowed file extensions for customer uploaded files
            /// </summary>
            public IList<string> AllowedFileExtensions { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<ProductAttributeValueModel> Values { get; set; }

        }

        public partial class ProductAttributeValueModel : BaseNopEntityModel
        {
            public ProductAttributeValueModel()
            {
                PictureModel = new PictureModel();
            }

            public string Name { get; set; }

            public string ColorSquaresRgb { get; set; }

            public string PriceAdjustment { get; set; }

            public decimal PriceAdjustmentValue { get; set; }

            public bool IsPreSelected { get; set; }

            //picture model is used when we want to override a default product picture when some attribute is selected
            public PictureModel PictureModel { get; set; }
        }

		#endregion
    }
}