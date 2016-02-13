using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class TierPriceMap : NopEntityTypeConfiguration<TierPrice>
    {
        public TierPriceMap()
        {
            this.ToTable("TierPrice");
            this.HasKey(tp => tp.Id);
            this.Property(tp => tp.Price).HasPrecision(18, 4);
            this.Property(tp => tp.Disc).IsOptional();
            this.Property(tp => tp.PriceCode).IsOptional();

            this.HasRequired(tp => tp.Product)
                .WithMany(p => p.TierPrices)
                .HasForeignKey(tp => tp.ProductId);

            this.HasOptional(tp => tp.CustomerRole)
                .WithMany()
                .HasForeignKey(tp => tp.CustomerRoleId)
                .WillCascadeOnDelete(true);
        }
    }
}