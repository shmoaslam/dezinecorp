using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    public class DezineCorpTierPricingSlab : BaseEntity
    {
        public string PriceCategory { get; set; }
        public int? Quantity_1 { get; set; }
        public int? Quantity_2 { get; set; }
        public int? Quantity_3 { get; set; }
        public int? Quantity_4 { get; set; }
        public int? Quantity_5 { get; set; }
        public int? Quantity_6 { get; set; }
        public int? Quantity_7 { get; set; }
        public int? Quantity_8 { get; set; }

    }
}
