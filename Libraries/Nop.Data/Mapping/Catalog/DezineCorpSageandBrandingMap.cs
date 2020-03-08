using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
    class DezineCorpSageandBrandingMap :  NopEntityTypeConfiguration<DezineCorpSageandBranding>
    {
        public DezineCorpSageandBrandingMap()
        {
            this.ToTable("DezinecorpSageandBrandingData");
            this.HasKey(tp => tp.Id);

            this.HasRequired(tp => tp.Product)
               .WithMany(p => p.DezineCorpSageandBrandings)
               .HasForeignKey(tp => tp.ProductId);
        }

    }
}
