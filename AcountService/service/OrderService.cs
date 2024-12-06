using AcountService.dto.request.Order;
using AcountService.dto.response.Order;
using AcountService.entity;
using AcountService.Repository;
using AcountService.status;
using AutoMapper;
using Azure;
using Azure.Core;
using BanVatLieuXayDung.dto.response.Order;
using Microsoft.EntityFrameworkCore;
using static NuGet.Packaging.PackagingConstants;

namespace AcountService.service
{
    public class OrderService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public OrderService(DataContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        //Hiển thị  đơn hàng
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
                    throw new Exception("Giỏ hàng trống hoặc không tồn tại.");
                }

                //lấy thông tin vận chuyển
                var infoshipping = await _context.InfoUserOrders.FirstOrDefaultAsync(i => i.UserId == request.UserId);

                if (infoshipping == null)
                {
                    throw new Exception("Vui lòng cập nhật thông tin nhận hàng");
                }

                // tạo ran đom mã đơn hàng
                var random = new Random();
                var order = new Order
                {
                    UserId = request.UserId,
                    OrderDate = DateTime.Now,
                    TotalPrice = cart.TotalPrice,
                    Status = "đang xử lý",
                    discount_amount = cart.discount_amount,
                    shipping_fee = cart.shipping_fee,
                    shipping_address = infoshipping.Address,
                    Email = infoshipping.Email,
                    FullName = infoshipping.FullName,
                    PhoneNumber = infoshipping.PhoneNumber,
                    Note = infoshipping.Note,
                    payment_method = request.PaymentMethod,
                    payment_status = "chưa thanh toán",
                    TrackingNumber = random.Next(10000, 100000).ToString(),


                    OrderDetails = cart.CartProducts.Select(cp => new OrderDetail
                    {
                        ProductName = cp.Product.Name,
                        Quantity = cp.Quantity,
                        Price = cp.Price,
                        Discount = cp.discount_amount,
                        TotalPrice = cp.TotalPrice,

                    }).ToList()
                };
                // Lưu đơn hàng vào cơ sở dữ liệu
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Lưu order và có OrderId tự động sinh ra

                // Lưu đơn hàng vào cơ sở dữ liệu
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Lưu order và có OrderId tự động sinh ra

                var respone = _mapper.Map<OrderResponse>(order);
                respone.TotalAmount = cart.CartProducts.Sum(c => c.TotalPrice);
                respone.Quantity = cart.TotalItems;
                respone.TrackingNumber = order.TrackingNumber;
                return respone;

            
             
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra: " + ex.Message);
            }
        }

        //Hủy hóa đơn
        public async Task<string> deleteOrder(int orderId)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    throw new Exception("Hóa đơn không tồn tại");
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return "Hóa đơn đã bị hủy";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        // Xác nhận hóa đơn
        public async Task<string> confirmOrderCOD(int orderId, string userId)
        {
            try
            {
                // Lấy đơn hàng
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
                if (order == null)
                {
                    throw new Exception("Hóa đơn không tồn tại");
                }

                // Lấy giỏ hàng của người dùng
                var cart = await _context.Carts
                      .Include(c => c.CartProducts)  // Tải CartProducts
                      .FirstOrDefaultAsync(o => o.UserId == userId);
                if (cart == null)
                {
                    throw new Exception("Giỏ hàng không tồn tại");
                }

                // Kiểm tra phương thức thanh toán
                if (order.payment_method == "cod")
                {
                    // Cập nhật trạng thái thanh toán của đơn hàng
                    order.payment_status = "chờ thanh toán";
                }

                // Xóa các sản phẩm trong giỏ hàng nếu có
                if (cart.CartProducts.Any())
                {
                    _context.CartProducts.RemoveRange(cart.CartProducts);
                }

                // Cập nhật các thuộc tính còn lại của giỏ hàng
                cart.shipping_fee = 0;
                cart.code = "";
                cart.TotalPrice = 0;
                cart.code_discount = 0;
                cart.discount_amount = 0;
                cart.promotion_discount = 0;

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return "Đã xác nhận đơn hàng";
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                throw new Exception(ex.Message);
            }
        }


        //LẤY RA DANH SACH ĐƠN HÀNG THEO NGÀY MƯỚI NHẤT
        public async Task<List<OrderNewResponse>> getOrderNew()
        {

            // Lấy ra danh sách đơn hàng sắp xếp theo ngày tạo giảm dần
            var orders = await _context.Orders
                                       .OrderByDescending(o => o.OrderDate) // Sắp xếp theo ngày tạo giảm dần
                                       .ToListAsync();                       // Lấy toàn bộ danh sách đơn hàng

            // Chuyển đổi tất cả đơn hàng thành OrderNewResponse
            var response = orders.Select(o => new OrderNewResponse
            {
                OrderId = o.OrderId,
                OrderStatus = o.Status,
                PaymentStatus = o.payment_status, // Đảm bảo đúng tên thuộc tính (có thể là PaymentStatus thay vì payment_status)
                Price = o.TotalPrice
            }).ToList();

            return response; // Trả về danh sách OrderNewResponse
        }

        //LẤY RA TOP KHÁCH HÀNG CÓ TỔNG TIỀN MUA NHIỀU NHÁT
        public async Task<List<UserOrder>> GetTopUserOrder()
        {
            // Lấy danh sách đơn hàng, nhóm theo userId và tính tổng giá trị đơn hàng cho mỗi người dùng
            var listUser = await _context.Orders
                .GroupBy(o => o.UserId) // Nhóm theo userId
                .Select(group => new UserOrder
                {
                    UserId = group.Key, // userId của người dùng
                    TotalPrice = group.Sum(o => o.TotalPrice) // Tính tổng tiền chi tiêu của người dùng
                })
                .OrderByDescending(u => u.TotalPrice) // Sắp xếp theo tổng tiền chi tiêu giảm dần
                .Take(5) // Lấy 5 người dùng có tổng tiền chi tiêu cao nhất
                .ToListAsync();

            // Nếu bạn cần thêm thông tin như tên người dùng, bạn có thể thực hiện một truy vấn bổ sung để lấy thông tin từ bảng Users
            var userIds = listUser.Select(u => u.UserId).ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            // Cập nhật danh sách với thông tin người dùng từ bảng Users
            foreach (var userOrder in listUser)
            {
                var user = users.FirstOrDefault(u => u.Id == userOrder.UserId);
                if (user != null)
                {
                    userOrder.username = user.UserName; // Giả sử Username là trường trong bảng Users
                }
            }

            return listUser; // Trả về danh sách 5 người dùng có tổng chi tiêu cao nhất
        }
    }
}








