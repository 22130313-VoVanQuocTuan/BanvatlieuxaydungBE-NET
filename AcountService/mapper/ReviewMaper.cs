using AcountService.entity;
using AutoMapper;
using BanVatLieuXayDung.dto.request.rating;

namespace BanVatLieuXayDung.mapper
{
    public class ReviewMaper:Profile
    {
        public ReviewMaper()
        {
            CreateMap<ReviewRequest, Review>();
        }
    }
}
