using Microsoft.EntityFrameworkCore;
using Kitcheer.API.Entities;
using Kitcheer.API.Enums;

namespace Kitcheer.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<StorageLocation> StorageLocations { get; set; }
    public DbSet<ProductTemplate> ProductTemplates { get; set; }
    public DbSet<StoredProduct> StoredProducts { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<ShoppingListItem> ShoppingListItems { get; set; }
    public DbSet<ProductMovement> ProductMovements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global query filter a DeleteFl mez? alapján
        modelBuilder.Entity<StorageLocation>().HasQueryFilter(e => !e.DeleteFl);
        modelBuilder.Entity<ProductTemplate>().HasQueryFilter(e => !e.DeleteFl);
        modelBuilder.Entity<StoredProduct>().HasQueryFilter(e => !e.DeleteFl);
        modelBuilder.Entity<ShoppingList>().HasQueryFilter(e => !e.DeleteFl);
        modelBuilder.Entity<ShoppingListItem>().HasQueryFilter(e => !e.DeleteFl);
        modelBuilder.Entity<ProductMovement>().HasQueryFilter(e => !e.DeleteFl);

        // Unique constraint a ProductTemplate-en (Brand + Name kombináció)
        modelBuilder.Entity<ProductTemplate>()
            .HasIndex(p => new { p.Brand, p.Name })
            .IsUnique()
            .HasFilter("\"DeleteFl\" = false");

        // Indexek a jobb teljesítményért
        modelBuilder.Entity<StoredProduct>()
            .HasIndex(p => new { p.ProductTemplateId, p.StorageLocationId, p.ExpiryDate })
            .HasFilter("\"DeleteFl\" = false");

        modelBuilder.Entity<ShoppingListItem>()
            .HasIndex(p => new { p.ShoppingListId, p.ProductTemplateId })
            .HasFilter("\"DeleteFl\" = false");

        // Enum konfigurációk
        modelBuilder.Entity<StorageLocation>()
            .Property(e => e.Type)
            .HasConversion<string>();

        modelBuilder.Entity<ProductTemplate>()
            .Property(e => e.ProductType)
            .HasConversion<string>();

        modelBuilder.Entity<ProductMovement>()
            .Property(e => e.MovementType)
            .HasConversion<string>();

        // JSON mez?k konfigurálása PostgreSQL-hez
        modelBuilder.Entity<StorageLocation>()
            .Property(e => e.AdditionalData)
            .HasColumnType("jsonb");

        modelBuilder.Entity<ProductTemplate>()
            .Property(e => e.ProductData)
            .HasColumnType("jsonb");

        modelBuilder.Entity<StoredProduct>()
            .Property(e => e.ProductDetails)
            .HasColumnType("jsonb");

        modelBuilder.Entity<ShoppingList>()
            .Property(e => e.ListData)
            .HasColumnType("jsonb");

        modelBuilder.Entity<ShoppingListItem>()
            .Property(e => e.ItemData)
            .HasColumnType("jsonb");

        modelBuilder.Entity<ProductMovement>()
            .Property(e => e.MovementData)
            .HasColumnType("jsonb");

        // BaseEntity RekordChange mez? konfigurálása
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property("RekordChange")
                    .HasColumnType("jsonb");
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateRekordChanges();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateRekordChanges();
        return base.SaveChanges();
    }

    private void UpdateRekordChanges()
    {
        var entities = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entity in entities)
        {
            entity.Entity.UpdateRekordChange();
        }
    }
}