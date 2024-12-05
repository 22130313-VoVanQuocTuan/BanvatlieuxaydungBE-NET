using AcountService.entity;
using AutoMapper;
using BanVatLieuXayDung.dto.request;
using BanVatLieuXayDung.dto.response.InfoOrderResponse;

namespace BanVatLieuXayDung.mapper
{
    public class InfoShippingMapper:Profile
    {
        public InfoShippingMapper() {
            CreateMap<UpdateInfoShippingRequest, InfoUserOrder>();
            CreateMap<InfoUserOrder, InfoOrderResponse>();
        }
    }
}
