using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Data.Mapping.Catalog
{
    public class AdditionalTierPriceTypeMap : NopEntityTypeConfiguration<AdditionalTierPriceType>
    {
        public AdditionalTierPriceTypeMap()
        {
            this.ToTable("AdditionalTierPriceType");

            this.HasKey(atpt => atpt.Id);
            this.Property(atpt => atpt.Type).IsRequired();
            this.Property(atpt => atpt.Description).IsOptional();


    
        }
    }
}
