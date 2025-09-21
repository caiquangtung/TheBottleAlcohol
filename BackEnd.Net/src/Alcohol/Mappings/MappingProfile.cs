using System;
using Alcohol.DTOs.Order;
using Alcohol.DTOs.Category;
using Alcohol.DTOs.Supplier;
using Alcohol.DTOs.ImportOrder;
using Alcohol.DTOs.Product;
using Alcohol.DTOs.Account;
using Alcohol.DTOs.OrderDetail;
using Alcohol.DTOs.Brand;
using Alcohol.DTOs.Cart;
using Alcohol.DTOs.CartDetail;
using Alcohol.Models;
using Alcohol.Models.Enums;
using AutoMapper;
using Alcohol.DTOs.Wishlist;
using Alcohol.DTOs.WishlistDetail;
using Alcohol.DTOs.Review;
using Alcohol.DTOs.Recipe;
using Alcohol.DTOs.Notification;
using Alcohol.DTOs.Discount;
using Alcohol.DTOs.Payment;
using Alcohol.DTOs.Inventory;
using Alcohol.DTOs.InventoryTransaction;

namespace Alcohol.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Account mappings
        CreateMap<Account, AccountResponseDto>();
        CreateMap<AccountCreateDto, Account>();
        CreateMap<AccountUpdateDto, Account>();

        // Product mappings
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null))
            .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.DiscountedPrice, opt => opt.Ignore())
            .ForMember(dest => dest.ActiveDiscounts, opt => opt.Ignore())
            .ForMember(dest => dest.HasDiscount, opt => opt.Ignore())
            .ForMember(dest => dest.DiscountAmount, opt => opt.Ignore())
            .ForMember(dest => dest.DiscountPercentage, opt => opt.Ignore());
        CreateMap<ProductCreateDto, Product>()
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Name.ToLower().Replace(" ", "-")))
            .ForMember(dest => dest.SalesCount, opt => opt.MapFrom(src => 0));
        CreateMap<ProductUpdateDto, Product>();

        // Cart mappings
        CreateMap<Cart, CartResponseDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : null));
        CreateMap<CartCreateDto, Cart>();
        CreateMap<CartUpdateDto, Cart>();

        // CartDetail mappings
        CreateMap<CartDetail, CartDetailResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : null));
        CreateMap<CartDetailCreateDto, CartDetail>();
        CreateMap<CartDetailUpdateDto, CartDetail>();

        // Order mappings
        CreateMap<Order, OrderResponseDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Account != null ? src.Account.FullName : null))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Account != null ? src.Account.Email : null))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Account != null ? src.Account.PhoneNumber : null))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        CreateMap<OrderCreateDto, Order>()
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => Enum.Parse<PaymentMethodType>(src.PaymentMethod, true)))
            .ForMember(dest => dest.ShippingMethod, opt => opt.MapFrom(src => Enum.Parse<ShippingMethodType>(src.ShippingMethod, true)))
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
            // Prevent AutoMapper from creating/tracking OrderDetails here; handled manually in service
            .ForMember(dest => dest.OrderDetails, opt => opt.Ignore());
        CreateMap<OrderUpdateDto, Order>();

        // OrderDetail mappings
        CreateMap<OrderDetail, OrderDetailResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId));
        CreateMap<OrderDetailCreateDto, OrderDetail>();
        CreateMap<OrderDetailUpdateDto, OrderDetail>();

        // ImportOrder mappings
        CreateMap<ImportOrder, ImportOrderResponseDto>()
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager != null ? src.Manager.FullName : null));
        CreateMap<ImportOrderCreateDto, ImportOrder>();
        CreateMap<ImportOrderUpdateDto, ImportOrder>();

        // ImportOrderDetail mappings
        CreateMap<ImportOrderDetail, ImportOrderDetailResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));
        CreateMap<ImportOrderDetailCreateDto, ImportOrderDetail>();
        CreateMap<ImportOrderDetailUpdateDto, ImportOrderDetail>();

        // Category mappings
        CreateMap<Category, CategoryResponseDto>()
            .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null))
            .ForMember(dest => dest.ParentSlug, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Slug : null))
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products != null ? src.Products.Count : 0))
            .ForMember(dest => dest.ChildrenCount, opt => opt.MapFrom(src => src.Children != null ? src.Children.Count : 0));

        CreateMap<CategoryCreateDto, Category>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.MetaTitle, opt => opt.MapFrom(src => src.MetaTitle))
            .ForMember(dest => dest.MetaDescription, opt => opt.MapFrom(src => src.MetaDescription));

        CreateMap<CategoryUpdateDto, Category>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.MetaTitle, opt => opt.MapFrom(src => src.MetaTitle))
            .ForMember(dest => dest.MetaDescription, opt => opt.MapFrom(src => src.MetaDescription));

        // Supplier mappings
        CreateMap<Supplier, SupplierResponseDto>();
        CreateMap<SupplierCreateDto, Supplier>();
        CreateMap<SupplierUpdateDto, Supplier>();

        // Brand mappings
        CreateMap<Brand, BrandResponseDto>();
        CreateMap<BrandCreateDto, Brand>()
            .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Name.ToLower().Replace(" ", "-")));
        CreateMap<BrandUpdateDto, Brand>();

        // Discount mappings
        CreateMap<Discount, DiscountResponseDto>();
        CreateMap<DiscountCreateDto, Discount>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        CreateMap<DiscountUpdateDto, Discount>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Wishlist mappings
        CreateMap<Wishlist, WishlistResponseDto>()
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.CustomerId));
        CreateMap<WishlistCreateDto, Wishlist>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.AccountId));
        CreateMap<WishlistDetailCreateDto, WishlistDetail>();
        CreateMap<WishlistDetail, WishlistDetailResponseDto>();

        // Review mappings
        CreateMap<Review, ReviewResponseDto>()
            .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : null));
        CreateMap<ReviewCreateDto, Review>().ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.AccountId));
        CreateMap<ReviewUpdateDto, Review>();

        // Recipe mappings
        CreateMap<Recipe, RecipeResponseDto>()
            .ForMember(dest => dest.PrepTime, opt => opt.MapFrom(src => src.PreparationTime))
            .ForMember(dest => dest.CookTime, opt => opt.MapFrom(src => 0)) // Default value since model doesn't have CookTime
            .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(src => false)) // Default value since model doesn't have IsFeatured
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => new List<RecipeCategoryDto>())) // Default empty list
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

        // RecipeCategory mappings
        CreateMap<RecipeCategory, RecipeCategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Category != null ? src.Category.Id : 0))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Category != null ? src.Category.Description : ""));
        CreateMap<RecipeCreateDto, Recipe>()
            .ForMember(dest => dest.PreparationTime, opt => opt.MapFrom(src => src.PrepTime))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Name.ToLower().Replace(" ", "-")))
            .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        CreateMap<RecipeUpdateDto, Recipe>()
            .ForMember(dest => dest.PreparationTime, opt => opt.MapFrom(src => src.PrepTime))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // RecipeIngredient mappings
        CreateMap<RecipeIngredient, RecipeIngredientDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));
        CreateMap<RecipeIngredientCreateDto, RecipeIngredient>();
        CreateMap<RecipeIngredientUpdateDto, RecipeIngredient>();

        // Notification mappings
        CreateMap<Notification, NotificationResponseDto>()
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Content));
        CreateMap<NotificationCreateDto, Notification>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Message));
        CreateMap<NotificationUpdateDto, Notification>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Message));

        // Payment mappings
        CreateMap<Payment, PaymentResponseDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
        CreateMap<PaymentCreateDto, Payment>();
        CreateMap<PaymentUpdateDto, Payment>();

        // Inventory mappings
        CreateMap<Inventory, InventoryResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));
        CreateMap<InventoryCreateDto, Inventory>()
            .ForMember(dest => dest.ReorderLevel, opt => opt.MapFrom(src => 10)) // Default reorder level
            .ForMember(dest => dest.AverageCost, opt => opt.MapFrom(src => 0)) // Default average cost = 0
            .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => 0)) // Default total value = 0
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        CreateMap<InventoryUpdateDto, Inventory>()
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // InventoryTransaction mappings
        CreateMap<InventoryTransaction, InventoryTransactionResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType));
        CreateMap<InventoryTransactionCreateDto, InventoryTransaction>()
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => Enum.Parse<InventoryTransactionType>(src.Type, true)))
            .ForMember(dest => dest.ReferenceType, opt => opt.MapFrom(src => Enum.Parse<ReferenceType>(src.ReferenceType, true)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => InventoryTransactionStatusType.Pending))
            .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        CreateMap<InventoryTransactionUpdateDto, InventoryTransaction>()
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => Enum.Parse<InventoryTransactionType>(src.Type, true)))
            .ForMember(dest => dest.ReferenceType, opt => opt.MapFrom(src => Enum.Parse<ReferenceType>(src.ReferenceType, true)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}