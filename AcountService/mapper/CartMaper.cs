using AcountService.dto.response.Cart;
using AcountService.dto.response.product;
using AcountService.entity;
using AutoMapper;

namespace AcountService.mapper
{
    public class CartMaper:Profile

    {
        public CartMaper() {
            CreateMap<CartProduct, ProductInCartResponse>()

            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name)) // Ánh xạ Name từ Product
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Product.Description)) // Ánh xạ Description từ Product
            .ForMember(dest => dest.UrlImage, opt => opt.MapFrom(src => src.Product.UrlImage)) // Ánh xạ UrlImage từ Product
           .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.CartId));


        }
    }
}
