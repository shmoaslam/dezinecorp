using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
    public class DezineCorpDataRefOnlyMap : NopEntityTypeConfiguration<DezineCorpDataRefOnly>
    {

        public DezineCorpDataRefOnlyMap()
        {
            this.ToTable("DezineCorpDataRefOnly");
            this.HasKey(tp => tp.Id);

            this.HasRequired(tp => tp.Product)
               .WithMany(p => p.DezineCorpDataRefOnlys)
               .HasForeignKey(tp => tp.ProductId);
        }

    }
}
