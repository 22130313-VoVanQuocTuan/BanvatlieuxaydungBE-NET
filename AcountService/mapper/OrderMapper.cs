using AcountService.dto.request.Order;
using AcountService.dto.response.Order;
using AcountService.entity;
using AcountService.status;
using AutoMapper;

namespace AcountService.mapper
{
    public class OrderMapper : Profile
    {

        public OrderMapper()
        {

            // Ánh xạ từ OrderRequest sang Order
            CreateMap<OrderRequest, Order>()
               
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.Now)) // Thiết lập OrderDate là thời điểm hiện tại
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OrderStatus.Pending)) // Mặc định là Pending
            .ForMember(dest => dest.Payment, opt => opt.Ignore()) // Bỏ qua nếu Payment không có trong request
            .ForMember(dest => dest.Shipment, opt => opt.Ignore()) // Bỏ qua Shipment nếu không cần
            .ForMember(dest => dest.User, opt => opt.Ignore()); // Chỉ ánh xạ UserId chứ không bao gồm User entity

            CreateMap<Order, OrderResponse>()
                  .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<OrderRequest, InfoUserOrder>();
        }
    }
}
