using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public class DezineCorpData : BaseEntity
    {

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

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
}
