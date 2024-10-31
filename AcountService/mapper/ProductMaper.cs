using AcountService.dto.request.product;
using AcountService.dto.response.product;
using AcountService.entity;
using AutoMapper;

namespace AcountService.mapper
{
    public class ProductMaper :Profile
    {
        public ProductMaper() {
            CreateMap<CreateProductRequest, Product>();
            CreateMap<Product, ProductDetailResponse>()
              .ForMember(dest => dest.UrlImage, opt => opt.MapFrom(src => src.UrlImage)); // Ánh xạ UrlImage từ Product
        }
    }
}
