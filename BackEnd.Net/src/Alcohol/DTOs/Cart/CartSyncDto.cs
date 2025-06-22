using System.Collections.Generic;

namespace Alcohol.DTOs.Cart
{
    public class CartSyncDto
    {
        public List<CartSyncItemDto> Items { get; set; }
        public byte[] RowVersion { get; set; }
    }
} 