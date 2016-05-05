using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
    public class DezineCorpTierPriceMap : NopEntityTypeConfiguration<DezineCorpTierPrice>
    {
        public DezineCorpTierPriceMap()
        {
            this.ToTable("DezineCorpTierPrice");
            this.HasKey(tp => tp.Id);

            this.HasRequired(tp => tp.Product)
               .WithMany(p => p.DezineCorpTierPrices)
               .HasForeignKey(tp => tp.ProductId);
        }
    }
}
