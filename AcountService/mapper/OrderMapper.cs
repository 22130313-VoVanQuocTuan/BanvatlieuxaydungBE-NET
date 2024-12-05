using AcountService.dto.request.Order;
using AcountService.dto.response.Order;
using AcountService.entity;
using AcountService.status;
using AutoMapper;
using BanVatLieuXayDung.dto.response.Order;

namespace AcountService.mapper
{
    public class OrderMapper : Profile
    {

        public OrderMapper()
        {


            // Ánh xạ từ Order sang OrderResponse
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.OrderDetailsRespone, opt => opt.MapFrom(src => src.OrderDetails)); // Map chi tiết đơn hàng
            

            // Ánh xạ từ OrderDetail sang OrderDetailResponse
            CreateMap<OrderDetail, OrderDetailResponse>();
        }
    }
}
