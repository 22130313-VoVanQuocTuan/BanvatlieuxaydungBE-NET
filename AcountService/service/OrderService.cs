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
        private readonly IConfiguration _configuration;

        public OrderService(DataContext context, IMapper mapper, IConfiguration configuration)
        {
            
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }

        //Tạo đơn hàng
        public async Task<OrderResponse> CreateOrder(OrderRequest request)
        {
            try
            {
                // Tìm giỏ hàng của người dùng
                var cart = await _context.Carts
                    .Include(c => c.CartProducts)
                    .ThenInclude(cp => cp.Product)
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId);

                if (cart == null || !cart.CartProducts.Any())
                {
                    return new OrderResponse
                    {
                        IsSuccess = false,
                        Message = "Giỏ hàng trống hoặc không tồn tại."
                    };
                }

                var vnpReturnUrl = _configuration.GetValue<string>("VNPay:ReturnUrl") ?? "http://127.0.0.1:5500/src/Users/pages/vnpay-return.html";

                // Tạo đối tượng Order
                var order = new Order
                {
                    UserId = request.UserId,
                    OrderDate = DateTime.Now,
                    TotalPrice = cart.CartProducts.Sum(cp => cp.TotalPrice),
                    Status = OrderStatus.Pending,
                    IsPaid = false,
                    OrderNote = request.OrderNote,
                    Shipment = new Shipment { ShipmentMethod = request.ShipmentMethod },
                    InfoUserOrder = new InfoUserOrder
                    {
                        FullName = request.FullName,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        Address = request.Address,
                        City = request.City
                    },
                    OrderDetails = cart.CartProducts.Select(cp => new OrderDetail
                    {
                        ProductName = cp.Product.Name,
                        Quantity = cp.Quantity,
                        TotalPrice = cp.TotalPrice
                    }).ToList()
                };

                // Lưu đơn hàng vào cơ sở dữ liệu
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Lưu order và có OrderId tự động sinh ra

                // Tạo thông tin thanh toán
                var payment = new Payment
                {
                    Amount = cart.CartProducts.Sum(cp => cp.TotalPrice),
                    Method = "VNPay",  // Phương thức thanh toán VNPay
                    PaymentDate = DateTime.Now,
                    IsSuccessful = false,  // Ban đầu trạng thái thanh toán là chưa thành công
                    VnpReturnUrl = vnpReturnUrl ?? "default-url-here",
                    OrderId = order.OrderId  // Liên kết với đơn hàng
                };

                // Lưu thông tin thanh toán vào cơ sở dữ liệu
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Xóa giỏ hàng sau khi đã tạo đơn hàng
                _context.CartProducts.RemoveRange(cart.CartProducts);
                await _context.SaveChangesAsync();

                string paymentUrl = string.Empty;

             
                return new OrderResponse
                {
                    IsSuccess = true,
                    Message = "Đơn hàng đã được tạo thành công.",
                    OrderDetails = order.OrderDetails,
                    PaymentUrl = paymentUrl // Trả về URL thanh toán cho người dùng
                };
            }
            catch (Exception ex)
            {
                return new OrderResponse
                {
                    IsSuccess = false,
                    Message = $"Đã xảy ra lỗi khi tạo đơn hàng: {ex.Message}"
                };
            }
        }
    
    }
}

        

  
    
