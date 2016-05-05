using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
   public class DezineCorpRelatedProductMap : NopEntityTypeConfiguration<DezineCorpRelatedProduct>
    {
        public DezineCorpRelatedProductMap()
        {
            this.ToTable("DezineCorpRelatedProduct");
            this.HasKey(tp => tp.Id);

            this.HasRequired(tp => tp.Product)
               .WithMany(p => p.DezineCorpRelatedProducts)
               .HasForeignKey(tp => tp.ProductId);
        }
    }
}
