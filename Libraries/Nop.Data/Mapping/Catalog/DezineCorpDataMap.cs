using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
    public partial class DezineCorpDataMap : NopEntityTypeConfiguration<DezineCorpData>
    {
        public DezineCorpDataMap()
        {
            this.ToTable("DezineCorpData");
            this.HasKey(tp => tp.Id);
            this.Property(tp => tp.NewPage).IsOptional();
            this.Property(tp => tp.ItemIsNew).IsOptional(); 

            this.HasRequired(tp => tp.Product)
               .WithMany(p => p.DezineCorpDatas)
               .HasForeignKey(tp => tp.ProductId);
        }
    }
}
