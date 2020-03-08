using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
   public class DezineCorpSageandBranding : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        public string UseAlternateImprintType { get; set; }
        public string SageProductSize { get; set; }
        public string SageDescription { get; set; }
        public string BrandingA { get; set; }
        public string BrandingALocation1 { get; set; }
        public string BrandingALocation1MeasurementType { get; set; }
        public float BrandingALocation1Heigth { get; set; }
        public float BrandingALocation1Width { get; set; }
        public string BrandingALocation2 { get; set; }
        public string BrandingALocation2MeasurementType { get; set; }
        public string BrandingALocation2Heigth { get; set; }
        public string BrandingALocation2Width { get; set; }
        public string BrandingB { get; set; }
        public string BrandingBLocation1 { get; set; }
        public string BrandingBLocation1MeasurementType { get; set; }
        public float BrandingBLocation1Heigth { get; set; }
        public float BrandingBLocation1Width { get; set; }
        public string BrandingBLocation2 { get; set; }
        public string BrandingBLocation2MeasurementType { get; set; }
        public float BrandingBLocation2Heigth { get; set; }
        public float BrandingBLocation2Width { get; set; }
        public string BrandingC { get; set; }
        public string BrandingCProductNumber { get; set; }
        public string BrandingD { get; set; }
        public string BrandingDProductNumber { get; set; }
        public string MappedItemNumber { get; set; }

    }
}
