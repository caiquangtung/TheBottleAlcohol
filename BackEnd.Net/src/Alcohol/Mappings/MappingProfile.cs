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
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null));
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
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        CreateMap<OrderCreateDto, Order>();
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
    }
}