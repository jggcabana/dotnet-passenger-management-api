using Microsoft.EntityFrameworkCore;
using Qless.Repositories.Data.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qless.Repositories.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Card> Card { get; set; }

        public DbSet<CardType> CardType { get; set; }

        public DbSet<Discount> Discount { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // for decimal data types
            SetDecimalProps(modelBuilder);

            modelBuilder.Entity<CardType>()
                .HasMany(ct => ct.Discounts)
                .WithMany(d => d.CardTypes)
                .UsingEntity(ct_d =>
                {
                    ct_d.Property("DiscountsId").HasColumnName("DiscountId");
                    ct_d.Property("CardTypesId").HasColumnName("CardTypeId");
                    ct_d.HasData(new[]
                    {
                        new { CardTypesId = 3, DiscountsId = 1 },
                        new { CardTypesId = 3, DiscountsId = 2 },
                    });
                });

            modelBuilder.Entity<CardType>()
                .ToTable("CardTypes", builder => builder.Property("Id").HasColumnName("CardTypeId"))
                .HasData(
                    new CardType
                    {
                        Id = 1,
                        Name = "regular",
                        Description = "regular card",
                        BaseRate = 15,
                        StartingBalance = 100,
                        MaximumIdleDurationYears = 5,
                        MaximumBalance = 10000
                    },
                    new CardType
                    {
                        Id = 2,
                        Name = "discounted",
                        Description = "discounted card",
                        BaseRate = 10,
                        StartingBalance = 500,
                        MaximumIdleDurationYears = 3,
                        MaximumBalance = 10000
                    },
                    new CardType
                    {
                        Id = 3,
                        Name = "specialDiscounted",
                        Description = "special discounted card",
                        BaseRate = 10,
                        StartingBalance = 500,
                        MaximumIdleDurationYears = 3,
                        MaximumBalance = 10000
                    });

            modelBuilder.Entity<Discount>()
                .ToTable("Discounts", builder => builder.Property("Id").HasColumnName("DiscountId"))
                .HasData(
                new Discount
                {
                    Id = 1,
                    Name = "base",
                    Description = "Base 20% discount for special cards",
                    StartDate = null,
                    EndDate = null,
                    DiscountValue = 20,
                    IsFlatDiscount = false,
                    AvailmentLimit = -1,
                    AvailmentLimitPerDay = -1,
                    Type = "basic"
                },
                new Discount
                {
                    Id = 2,
                    Name = "succeeding",
                    Description = "3% discount for succeeding trips",
                    StartDate = null,
                    EndDate = null,
                    DiscountValue = 3,
                    IsFlatDiscount = false,
                    AvailmentLimit = -1,
                    AvailmentLimitPerDay = 4,
                    Type = "special"
                });

            modelBuilder.Entity<Card>()
                .ToTable("Cards", builder => builder.Property("Id").HasColumnName("CardId"));

            base.OnModelCreating(modelBuilder);
        }

        private void SetDecimalProps(ModelBuilder modelBuilder)
        {
            var decimalProps = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => (System.Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType) == typeof(decimal));

            foreach (var property in decimalProps)
            {
                property.SetPrecision(18);
                property.SetScale(4);
            }
        }
    }
}
