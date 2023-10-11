﻿namespace Microservices.EmailAPI.Models.Dto
{
    public class CartDto
    {
        public CartHeaderDto? CartHeader { get; set; } = null!;
        public IEnumerable<CartDetailsDto>? CartDetails { get; set; }
    }
}
