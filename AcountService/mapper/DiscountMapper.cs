using AcountService.dto.response.Discount;
using AcountService.entity;
using AutoMapper;

namespace BanVatLieuXayDung.mapper
{
    public class DiscountMapper : Profile
    {
        public DiscountMapper()
        {
            CreateMap<Discount, DiscountResponse>();
        }
    }
}
