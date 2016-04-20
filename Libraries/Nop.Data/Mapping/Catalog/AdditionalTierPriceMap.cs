using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
    public partial class AdditionalTierPriceMap : NopEntityTypeConfiguration<AdditionalTierPrice>
    {
        public AdditionalTierPriceMap()
        {
            this.ToTable("AdditionalTierPrice");
            this.HasKey(atp => atp.Id);
            this.Property(atp => atp.Price).HasPrecision(18, 4);
            this.Property(atp => atp.Code).IsOptional();


            this.HasRequired(atp => atp.TierPrice)
                .WithMany(tp => tp.AdditionalTierPrices)
                .HasForeignKey(atp => atp.TierPriceId);

            this.HasRequired(atp => atp.AdditionalTierPriceType)
                .WithMany(atpt => atpt.AdditionalTierPrices)
                .HasForeignKey(atp => atp.TypeId);

        }
    }
}
 