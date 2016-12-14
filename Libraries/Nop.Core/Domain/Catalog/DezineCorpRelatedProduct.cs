using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public class DezineCorpRelatedProduct : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        public string Related_1 { get; set; }
        public string Related_2 { get; set; }
        public string Related_3 { get; set; }
        public string Related_4 { get; set; }
        public string Related_5 { get; set; }
        public string Related_6 { get; set; }

    }
}
