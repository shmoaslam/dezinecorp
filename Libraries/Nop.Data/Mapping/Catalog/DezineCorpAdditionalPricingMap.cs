using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
    public class DezineCorpAdditionalPricingMap : NopEntityTypeConfiguration<DezineCorpAdditionalPricing>
    {
        public DezineCorpAdditionalPricingMap()
        {
            this.ToTable("DezineCorpAdditionalPricing");
            this.HasKey(tp => tp.Id);

            this.HasRequired(tp => tp.Product)
               .WithMany(p => p.DezineCorpAdditionalPricings)
               .HasForeignKey(tp => tp.ProductId);
        }
    }
}
