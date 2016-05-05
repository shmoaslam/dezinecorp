using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public class DezineCorpAdditionalPricing : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        public string AddColourOption { get; set; }
        public string AddCol_1 { get; set; }
        public string AddCol_2 { get; set; }
        public string AddCol_3 { get; set; }
        public string AddCol_4 { get; set; }
        public string AddColPriceCode { get; set; }
        public string DecalOption { get; set; }
        public string Decal_1 { get; set; }
        public string Decal_2 { get; set; }
        public string Decal_3 { get; set; }
        public string Decal_4 { get; set; }
        public string DecalPriceCode { get; set; }
        public string LaserEngravingOption { get; set; }
        public string Laser_1 { get; set; }
        public string Laser_2 { get; set; }
        public string Laser_3 { get; set; }
        public string Laser_4 { get; set; }
        public string LaserPriceCode { get; set; }

    }
}
