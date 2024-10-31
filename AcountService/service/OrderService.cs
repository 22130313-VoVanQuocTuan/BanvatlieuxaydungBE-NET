using AcountService.dto.request.Order;
using AcountService.dto.response.Order;
using AcountService.entity;
using AcountService.Repository;
using AcountService.status;
using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;

namespace AcountService.service
{
    public class OrderService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public OrderService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //Tạo đơn hàng
        public async Task<OrderResponse> CreateOrder(OrderRequest request)
        {


            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
                if (user == null)
                {
                    throw new Exception("Người dùng không tồn tại.");
                }

                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                if (cart == null)
                {
                    throw new Exception("Giỏ hàng không tồn tại.");
                }

                var cartp = await _context.CartProducts.FirstOrDefaultAsync(c => c.CartId == cart.CartId);
                if (cartp == null)
                {
                    throw new Exception("Không có sản phẩm trong giỏ hàng.");
                }



                var order = _mapper.Map<Order>(request);
                order.OrderDate = DateTime.Now;
                order.User = user;
                order.UserId = request.UserId;

                           
                order.IsPaid = true;
              
              

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

             

                var infoUserOrder = new InfoUserOrder
                {
                    Email = request.Email,
                    OrderId = order.OrderId,  // Sử dụng OrderId đã lưu
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    City = request.City,
                                        

                };


                _context.InfoUserOrders.Add(infoUserOrder);
                await _context.SaveChangesAsync();


                var response = _mapper.Map<OrderResponse>(order);
                response.IsSuccess = true;
                response.Message = "Đơn hàng được tạo thành công.";
                

                return response;

            }





            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}", ex);
            }
        }
    }
}
        

  
    
