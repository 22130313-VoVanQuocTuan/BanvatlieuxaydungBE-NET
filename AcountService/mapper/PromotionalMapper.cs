using AcountService.dto.request.product;
using AutoMapper;
using BanVatLieuXayDung.dto.request.Promotional;
using BanVatLieuXayDung.dto.response.Prommotional;
using BanVatLieuXayDung.entity;
using System.Xml;

namespace BanVatLieuXayDung.mapper
{
    public class PromotionalMapper : Profile
    {
        public PromotionalMapper()
        {
            CreateMap<CreatePromotionalRequest, PromotionalProducts>();
            CreateMap<PromotionalProducts, PromotionalResponse>()
                 .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name)) // Ánh xạ UrlImage từ Product
                 .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.UrlImage))
                 .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price));


        }
    }
}
