using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public class DezineCorpTierPrice : BaseEntity
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
        /// Gets or sets the Quantity Level Code
        /// </summary>
        public string QuantityLevel { get; set; }

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
        /// Gets or sets the Price 5
        /// </summary>
        public string Price5 { get; set; }

        /// <summary>
        /// Gets or sets the Price 6
        /// </summary>
        public string Price6 { get; set; }

        /// <summary>
        /// Gets or sets the Price 7
        /// </summary>
        public string Price7 { get; set; }

        /// <summary>
        /// Gets or sets the Price 8
        /// </summary>
        public string Price8 { get; set; }

        /// <summary>
        /// Gets or sets the Discount Code
        /// </summary>
        public string DiscountCode { get; set; }

    }
}
