using Microsoft.EntityFrameworkCore;
using OrderTracker.Models;
using OrderTracker.Resources;

namespace OrderTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Carrier> Carriers { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Store).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            entity.HasMany(e => e.Items)
                  .WithOne(i => i.Order)
                  .HasForeignKey(i => i.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Carrier)
                  .WithMany()
                  .HasForeignKey(e => e.CarrierId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.PaymentMethod)
                  .WithMany()
                  .HasForeignKey(e => e.PaymentMethodId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(300);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Carrier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<PaymentMethod>().HasData(
            new PaymentMethod { Id = 1, Name = AppText.SeedPaymentMethodCreditCard, SortOrder = 1 },
            new PaymentMethod { Id = 2, Name = AppText.SeedPaymentMethodDebitCard, SortOrder = 2 },
            new PaymentMethod { Id = 3, Name = AppText.SeedPaymentMethodPayPal, SortOrder = 3 },
            new PaymentMethod { Id = 4, Name = AppText.SeedPaymentMethodBizum, SortOrder = 4 },
            new PaymentMethod { Id = 5, Name = AppText.SeedPaymentMethodTransfer, SortOrder = 5 },
            new PaymentMethod { Id = 6, Name = AppText.SeedPaymentMethodCashOnDelivery, SortOrder = 6 }
        );

        modelBuilder.Entity<Carrier>().HasData(
            new Carrier { Id = 1, Name = AppText.SeedCarrierCorreos, TrackingUrlTemplate = "https://www.correos.es/es/es/herramientas/localizador/envios/detalle?tracking-number={0}", SortOrder = 1 },
            new Carrier { Id = 2, Name = AppText.SeedCarrierCorreosExpress, TrackingUrlTemplate = "https://www.correos.es/es/es/herramientas/localizador/envios/detalle?tracking-number={0}", SortOrder = 2 },
            new Carrier { Id = 3, Name = AppText.SeedCarrierSeur, TrackingUrlTemplate = "https://www.seur.com/livetracking/?segOnlineIdentificador={0}", SortOrder = 3 },
            new Carrier { Id = 4, Name = AppText.SeedCarrierMrw, TrackingUrlTemplate = "https://www.mrw.es/seguimiento_envios/MRW_resultados_consulta.asp?Franquicia=&Abonado=&Dep=&Numero={0}", SortOrder = 4 },
            new Carrier { Id = 5, Name = AppText.SeedCarrierDhl, TrackingUrlTemplate = "https://www.dhl.com/es-es/home/tracking.html?tracking-id={0}", SortOrder = 5 },
            new Carrier { Id = 6, Name = AppText.SeedCarrierGls, TrackingUrlTemplate = "https://gls-group.eu/track/{0}", SortOrder = 6 },
            new Carrier { Id = 7, Name = AppText.SeedCarrierUps, TrackingUrlTemplate = "https://www.ups.com/track?loc=es_ES&tracknum={0}", SortOrder = 7 },
            new Carrier { Id = 8, Name = AppText.SeedCarrierFedEx, TrackingUrlTemplate = "https://www.fedex.com/fedextrack/?trknbr={0}", SortOrder = 8 },
            new Carrier { Id = 9, Name = AppText.SeedCarrierAmazonLogistics, TrackingUrlTemplate = "https://www.amazon.es/progress-tracker/package/?ref_=pe_2640170_620568780&_encoding=UTF8&itemId={0}", SortOrder = 9 },
            new Carrier { Id = 10, Name = AppText.SeedCarrierNacex, TrackingUrlTemplate = "https://www.nacex.es/seguimientoDetalle.do?agencia_origen=&numero_albaran={0}", SortOrder = 10 },
            new Carrier { Id = 11, Name = AppText.SeedCarrierCtt, TrackingUrlTemplate = null, SortOrder = 11 },
            new Carrier { Id = 12, Name = AppText.SeedCarrierZeleris, TrackingUrlTemplate = "https://www.zeleris.com/seguimiento?referencia={0}", SortOrder = 12 },
            new Carrier { Id = 13, Name = AppText.SeedCarrierEcoScooting, TrackingUrlTemplate = "https://ecoscooting.com/tracking/{0}", SortOrder = 13 },
            new Carrier { Id = 14, Name = AppText.SeedCarrierOther, TrackingUrlTemplate = null, SortOrder = 99 }
        );
    }
}
