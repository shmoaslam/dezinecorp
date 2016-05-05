using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
    public class DezineCorpProductKeywordMap : NopEntityTypeConfiguration<DezineCorpProductKeyword>
    {
        public DezineCorpProductKeywordMap()
        {
            this.ToTable("DezineCorpProductKeyword");
            this.HasKey(tp => tp.Id);

            this.HasRequired(tp => tp.Product)
               .WithMany(p => p.DezineCorpProductKeywords)
               .HasForeignKey(tp => tp.ProductId);
        }
    }
}
