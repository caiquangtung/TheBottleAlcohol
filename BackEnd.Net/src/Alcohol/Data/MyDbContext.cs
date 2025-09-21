using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Models;
using Microsoft.EntityFrameworkCore;
using Alcohol.Models.Enums;

namespace Alcohol.Data
{
      public class MyDbContext : DbContext
      {
            public DbSet<Account> Accounts { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<OrderDetail> OrderDetails { get; set; }
            public DbSet<Product> Products { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<Brand> Brands { get; set; }
            public DbSet<Recipe> Recipes { get; set; }
            public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
            public DbSet<RecipeCategory> RecipeCategories { get; set; }
            public DbSet<ImportOrder> ImportOrders { get; set; }
            public DbSet<ImportOrderDetail> ImportOrderDetails { get; set; }
            public DbSet<Supplier> Suppliers { get; set; }
            public DbSet<OrderStatus> OrderStatuses { get; set; }
            public DbSet<Inventory> Inventories { get; set; }
            public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
            public DbSet<Cart> Carts { get; set; }
            public DbSet<CartDetail> CartDetails { get; set; }
            public DbSet<Wishlist> Wishlists { get; set; }
            public DbSet<WishlistDetail> WishlistDetails { get; set; }
            public DbSet<Review> Reviews { get; set; }
            public DbSet<Shipping> Shippings { get; set; }
            public DbSet<Discount> Discounts { get; set; }
            public DbSet<Notification> Notifications { get; set; }
            public DbSet<Payment> Payments { get; set; }
            public DbSet<RefreshToken> RefreshTokens { get; set; }

            public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                  base.OnModelCreating(modelBuilder);

                  // Configure MySQL specific settings
                  modelBuilder.HasCharSet("utf8mb4");
                  modelBuilder.UseCollation("utf8mb4_unicode_ci");

                  // Configure RefreshToken
                  modelBuilder.Entity<RefreshToken>(entity =>
                  {
                        entity.ToTable("RefreshToken");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
                        entity.Property(e => e.UserId).IsRequired();
                        entity.Property(e => e.ExpiryDate).IsRequired();
                        entity.Property(e => e.IsRevoked).IsRequired();
                        entity.Property(e => e.CreatedAt).IsRequired();
                        entity.Property(e => e.DeviceInfo).HasMaxLength(500);
                        entity.Property(e => e.IpAddress).HasMaxLength(50);
                        entity.Property(e => e.UserAgent).HasMaxLength(500);

                        // Relationship with Account
                        entity.HasOne(e => e.User)
                              .WithMany()
                              .HasForeignKey(e => e.UserId)
                              .OnDelete(DeleteBehavior.Cascade);
                  });

                  // 1. Account
                  modelBuilder.Entity<Account>(entity =>
                  {
                        entity.ToTable("Account");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
                        entity.Property(e => e.DateOfBirth).IsRequired();
                        entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
                        entity.Property(e => e.Gender).IsRequired()
                              .HasConversion<string>();
                        entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
                        entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                        entity.Property(e => e.Role).IsRequired()
                              .HasConversion<string>();
                        entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                        entity.Property(e => e.Status).IsRequired();
                        entity.Property(e => e.CreatedAt).IsRequired();
                        entity.Property(e => e.UpdatedAt);
                        entity.HasMany(e => e.Orders)
                              .WithOne(e => e.Account)
                              .HasForeignKey(e => e.AccountId);
                  });

                  // 2. Order
                  modelBuilder.Entity<Order>(entity =>
                  {
                        entity.ToTable("Order");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.OrderDate).IsRequired();
                        entity.Property(e => e.TotalAmount).IsRequired();
                        entity.Property(e => e.PaymentMethod).IsRequired()
                              .HasConversion<string>();
                        entity.Property(e => e.ShippingMethod).IsRequired()
                              .HasConversion<string>();
                        entity.Property(e => e.Status).IsRequired()
                              .HasConversion<string>();
                        entity.Property(e => e.ShippingAddress).IsRequired();
                        entity.Property(e => e.ShippingPhone).IsRequired();
                        entity.Property(e => e.ShippingName).IsRequired();
                        entity.Property(e => e.Note);
                        entity.Property(e => e.CreatedAt).IsRequired();
                        entity.Property(e => e.UpdatedAt);
                        entity.HasOne(e => e.Account)
                              .WithMany(e => e.Orders)
                              .HasForeignKey(e => e.AccountId)
                              .IsRequired();
                        entity.HasMany(e => e.OrderStatuses)
                              .WithOne(e => e.Order)
                              .HasForeignKey(e => e.OrderId);
                        entity.HasMany(e => e.OrderDetails)
                              .WithOne(e => e.Order)
                              .HasForeignKey(e => e.OrderId);
                  });

                  // 3. OrderDetail
                  modelBuilder.Entity<OrderDetail>(entity =>
                  {
                        entity.ToTable("OrderDetail");
                        entity.HasKey(e => new { e.OrderId, e.ProductId });
                        entity.Property(e => e.UnitPrice).IsRequired();
                        entity.Property(e => e.Quantity).IsRequired();
                        entity.Property(e => e.TotalAmount).IsRequired();
                        entity.HasOne(e => e.Order)
                              .WithMany(e => e.OrderDetails)
                              .HasForeignKey(e => e.OrderId);
                        entity.HasOne(e => e.Product)
                              .WithMany(e => e.OrderDetails)
                              .HasForeignKey(e => e.ProductId);
                  });

                  // 4. Product
                  modelBuilder.Entity<Product>(entity =>
                  {
                        entity.ToTable("Product");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                        entity.Property(e => e.Description).HasMaxLength(500);
                        entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
                        entity.Property(e => e.Origin).IsRequired().HasMaxLength(100);
                        entity.Property(e => e.Volume).IsRequired();
                        entity.Property(e => e.AlcoholContent).IsRequired();
                        entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
                        entity.Property(e => e.StockQuantity).IsRequired();
                        entity.Property(e => e.Status).IsRequired();
                        entity.Property(e => e.ImageUrl).HasMaxLength(500);
                        entity.Property(e => e.Age);
                        entity.Property(e => e.Flavor).HasMaxLength(100);
                        entity.Property(e => e.SalesCount).IsRequired();
                        entity.Property(e => e.MetaTitle).HasMaxLength(200);
                        entity.Property(e => e.MetaDescription).HasMaxLength(500);
                        entity.Property(e => e.CreatedAt).IsRequired();
                        entity.Property(e => e.UpdatedAt);

                        // Relationships
                        entity.HasOne(e => e.Category)
                              .WithMany(e => e.Products)
                              .HasForeignKey(e => e.CategoryId)
                              .OnDelete(DeleteBehavior.SetNull);

                        entity.HasOne(e => e.Brand)
                              .WithMany(e => e.Products)
                              .HasForeignKey(e => e.BrandId)
                              .OnDelete(DeleteBehavior.SetNull);

                        entity.HasOne(e => e.Inventory)
                              .WithOne(e => e.Product)
                              .HasForeignKey<Inventory>(e => e.ProductId);
                  });

                  // 5. Category
                  modelBuilder.Entity<Category>(entity =>
                  {
                        entity.ToTable("Category");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                        entity.Property(e => e.Description).HasMaxLength(500);
                        entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
                        entity.Property(e => e.DisplayOrder).IsRequired();
                        entity.Property(e => e.IsActive).IsRequired();
                        entity.Property(e => e.CreatedAt).IsRequired();

                        // Self-referencing relationship
                        entity.HasOne(e => e.Parent)
                              .WithMany(e => e.Children)
                              .HasForeignKey(e => e.ParentId)
                              .OnDelete(DeleteBehavior.Restrict);
                  });

                  // 6. Brand
                  modelBuilder.Entity<Brand>(entity =>
                  {
                        entity.ToTable("Brand");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                        entity.Property(e => e.Description).HasMaxLength(500);
                        entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
                        entity.Property(e => e.LogoUrl).HasMaxLength(500);
                        entity.Property(e => e.Website).HasMaxLength(200);
                        entity.Property(e => e.DisplayOrder).IsRequired();
                        entity.Property(e => e.IsActive).IsRequired();
                        entity.Property(e => e.MetaTitle).HasMaxLength(200);
                        entity.Property(e => e.MetaDescription).HasMaxLength(500);
                        entity.Property(e => e.CreatedAt).IsRequired();
                        entity.HasMany(e => e.Products)
                              .WithOne(e => e.Brand)
                              .HasForeignKey("BrandId");
                  });

                  // 7. Recipe
                  modelBuilder.Entity<Recipe>(entity =>
                  {
                        entity.ToTable("Recipe");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                        entity.Property(e => e.Description).HasMaxLength(500);
                        entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
                        entity.Property(e => e.ImageUrl).HasMaxLength(500);
                        entity.Property(e => e.Instructions).IsRequired();
                        entity.Property(e => e.Difficulty).IsRequired().HasMaxLength(50);
                        entity.Property(e => e.PreparationTime).IsRequired();
                        entity.Property(e => e.Servings).IsRequired();
                        entity.Property(e => e.DisplayOrder).IsRequired();
                        entity.Property(e => e.IsActive).IsRequired();
                        entity.Property(e => e.MetaTitle).HasMaxLength(200);
                        entity.Property(e => e.MetaDescription).HasMaxLength(500);
                        entity.Property(e => e.CreatedAt).IsRequired();
                        entity.Property(e => e.UpdatedAt);

                        // Changed from many-to-many to direct relationship
                        entity.HasOne(e => e.Category)
                              .WithMany()
                              .HasForeignKey(e => e.CategoryId)
                              .OnDelete(DeleteBehavior.SetNull);
                  });

                  // 8. RecipeIngredient
                  modelBuilder.Entity<RecipeIngredient>(entity =>
                  {
                        entity.ToTable("RecipeIngredient");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                        entity.Property(e => e.Quantity).IsRequired();
                        entity.Property(e => e.Unit).HasMaxLength(50);
                        entity.Property(e => e.Notes);

                        entity.HasOne(e => e.Recipe)
                              .WithMany(e => e.Ingredients)
                              .HasForeignKey(e => e.RecipeId)
                              .OnDelete(DeleteBehavior.Cascade);

                        entity.HasOne(e => e.Product)
                              .WithMany(e => e.RecipeIngredients)
                              .HasForeignKey(e => e.ProductId)
                              .OnDelete(DeleteBehavior.SetNull);
                  });

                  // 10. ImportOrder
                  modelBuilder.Entity<ImportOrder>(entity =>
                  {
                        entity.ToTable("ImportOrder");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.ImportDate).IsRequired();
                        entity.Property(e => e.TotalAmount).IsRequired();
                        entity.Property(e => e.Profit).IsRequired();
                        entity.HasOne(e => e.Supplier)
                              .WithMany(e => e.ImportOrders)
                              .HasForeignKey("SupplierId")
                              .IsRequired();
                        entity.HasOne(e => e.Manager)
                              .WithMany()
                              .HasForeignKey("ManagerId")
                              .IsRequired();
                        entity.HasMany(e => e.ImportOrderDetails)
                              .WithOne(e => e.ImportOrder)
                              .HasForeignKey(e => e.ImportOrderId);
                  });

                  // 11. ImportOrderDetail
                  modelBuilder.Entity<ImportOrderDetail>(entity =>
                  {
                        entity.ToTable("ImportOrderDetail");
                        entity.HasKey(e => new { e.ImportOrderId, e.ProductId });
                        entity.Property(e => e.Quantity).IsRequired();
                        entity.Property(e => e.ImportPrice).IsRequired();
                        entity.Property(e => e.TotalAmount).IsRequired();
                        entity.Property(e => e.Status).IsRequired();
                        entity.HasOne(e => e.Product)
                              .WithMany(e => e.ImportOrderDetails)
                              .HasForeignKey(e => e.ProductId);
                        entity.HasOne(e => e.ImportOrder)
                              .WithMany(e => e.ImportOrderDetails)
                              .HasForeignKey(e => e.ImportOrderId);
                  });

                  // 12. Supplier
                  modelBuilder.Entity<Supplier>(entity =>
                  {
                        entity.ToTable("Supplier");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                        entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                        entity.Property(e => e.Email).HasMaxLength(100);
                        entity.Property(e => e.Address).HasMaxLength(200);
                        entity.Property(e => e.Status).HasDefaultValue(SupplierStatusType.Active);
                        entity.HasMany(e => e.ImportOrders)
                              .WithOne(e => e.Supplier)
                              .HasForeignKey("SupplierId");
                  });

                  // 13. OrderStatus
                  modelBuilder.Entity<OrderStatus>(entity =>
                  {
                        entity.ToTable("OrderStatus");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.OrderId).IsRequired();
                        entity.Property(e => e.Status).IsRequired()
                              .HasConversion<string>();
                        entity.Property(e => e.UpdateDate).IsRequired();
                        entity.HasOne(e => e.Order)
                              .WithMany(e => e.OrderStatuses)
                              .HasForeignKey(e => e.OrderId);
                  });

                  // Inventory configuration
                  modelBuilder.Entity<Inventory>(entity =>
                  {
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Quantity).IsRequired();
                        entity.HasOne(e => e.Product)
                              .WithOne(p => p.Inventory)
                              .HasForeignKey<Inventory>(e => e.ProductId);
                  });

                  // InventoryTransaction configuration
                  modelBuilder.Entity<InventoryTransaction>(entity =>
                  {
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Quantity).IsRequired();
                        entity.Property(e => e.TransactionType).IsRequired();
                        entity.Property(e => e.ReferenceType).IsRequired();
                        entity.Property(e => e.Status).IsRequired();
                        entity.HasOne(e => e.Product)
                              .WithMany()
                              .HasForeignKey(e => e.ProductId);
                  });

                  // Configure relationships
                  modelBuilder.Entity<Cart>()
                        .HasOne(c => c.Customer)
                        .WithOne(a => a.Cart)
                        .HasForeignKey<Cart>(c => c.CustomerId);

                  modelBuilder.Entity<OrderStatus>()
                        .HasOne(os => os.Order)
                        .WithMany(o => o.OrderStatuses)
                        .HasForeignKey(os => os.OrderId);

                  // Configure Discount
                  modelBuilder.Entity<Discount>(entity =>
                  {
                        entity.ToTable("Discount");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                        entity.Property(e => e.DiscountAmount).IsRequired();
                        entity.Property(e => e.StartDate).IsRequired();
                        entity.Property(e => e.EndDate).IsRequired();
                        entity.Property(e => e.IsActive).IsRequired();
                        entity.HasMany(e => e.Products)
                              .WithMany(p => p.Discounts)
                              .UsingEntity(j => j.ToTable("DiscountProduct"));
                  });

                  // Configure Notification
                  modelBuilder.Entity<Notification>(entity =>
                  {
                        entity.ToTable("Notification");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.UserId).IsRequired();
                        entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                        entity.Property(e => e.Content).IsRequired();
                        entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
                        entity.Property(e => e.IsRead).IsRequired();
                        entity.Property(e => e.CreatedAt).IsRequired();
                        entity.Property(e => e.ReadAt);
                        entity.HasOne(e => e.User)
                              .WithMany()
                              .HasForeignKey(e => e.UserId);
                  });

                  // Configure Payment
                  modelBuilder.Entity<Payment>(entity =>
                  {
                        entity.ToTable("Payment");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.OrderId).IsRequired();
                        entity.Property(e => e.AccountId).IsRequired();
                        entity.Property(e => e.Amount).IsRequired();
                        entity.Property(e => e.Status).IsRequired()
                              .HasConversion<string>();
                        entity.Property(e => e.CreatedAt).IsRequired();
                        entity.Property(e => e.UpdatedAt);
                        entity.HasOne(e => e.Order)
                              .WithMany()
                              .HasForeignKey(e => e.OrderId);
                        entity.HasOne(e => e.Account)
                              .WithMany()
                              .HasForeignKey(e => e.AccountId);
                  });

                  // Configure Cart
                  modelBuilder.Entity<Cart>(entity =>
                  {
                        entity.ToTable("Cart");
                        entity.HasKey(e => e.Id);
                        entity.Property(e => e.Id).ValueGeneratedOnAdd();
                        entity.Property(e => e.CustomerId).IsRequired();
                        entity.HasOne(e => e.Customer)
                              .WithOne()
                              .HasForeignKey<Cart>(e => e.CustomerId);
                  });

                  // 14. Review
                  modelBuilder.Entity<Review>(entity =>
                  {
                        entity.ToTable("Reviews");
                        entity.HasKey(e => e.Id);
                        // Thêm unique index cho CustomerId + ProductId
                        entity.HasIndex(e => new { e.CustomerId, e.ProductId }).IsUnique();
                  });
            }
      }
}