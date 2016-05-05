using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public class DezineCorpProductKeyword : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        public string Keyword_1 { get; set; }
        public string Keyword_2 { get; set; }
        public string Keyword_3 { get; set; }
        public string Keyword_4 { get; set; }
        public string Keyword_5 { get; set; }
        public string Keyword_6 { get; set; }
        public string Keyword_Color { get; set; }
        public string keyword_Linename { get; set; }
        public string Keyword_Colour_Primary { get; set; }
        public string Keyword_Colour_Secondary { get; set; }

    }
}
