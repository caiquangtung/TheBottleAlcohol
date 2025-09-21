using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Product;
using Alcohol.DTOs.Discount;

namespace Alcohol.Services.Interfaces;

public interface IProductDiscountService
{
    Task<ProductResponseDto> GetProductWithDiscountAsync(int productId);
    Task<List<ProductResponseDto>> GetProductsWithDiscountAsync();
    Task<decimal> CalculateDiscountedPriceAsync(int productId);
    Task<List<DiscountResponseDto>> GetActiveDiscountsForProductAsync(int productId);
    Task<bool> IsDiscountApplicableAsync(int discountId, int productId);
    Task<ProductResponseDto> ApplyDiscountToProductAsync(ProductResponseDto product);
}
