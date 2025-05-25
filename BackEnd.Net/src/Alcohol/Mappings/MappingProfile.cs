using System;
using Alcohol.DTOs.Order;
using Alcohol.DTOs.Category;
using Alcohol.DTOs.Supplier;
using Alcohol.DTOs.ImportOrder;
using Alcohol.DTOs.Product;
using Alcohol.DTOs.Account;
using Alcohol.DTOs.OrderDetail;
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
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();

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
        CreateMap<Category, CategoryResponseDto>();
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<CategoryUpdateDto, Category>();

        // Supplier mappings
        CreateMap<Supplier, SupplierResponseDto>();
        CreateMap<SupplierCreateDto, Supplier>();
        CreateMap<SupplierUpdateDto, Supplier>();
    }
}