using AcountService.dto.request.cart;
using AcountService.dto.response.Cart;
using AcountService.entity;
using AcountService.Repository;
using AutoMapper;
using BanVatLieuXayDung.dto.response.Cart;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcountService.service
{
    public class CartService
    {
        public readonly DataContext _context;
        public readonly IMapper _mapper;

        public CartService(DataContext context, IMapper mapper)

        {
            _context = context;
            _mapper = mapper;
        }

        // Phương thức để thêm sản phẩm vào giỏ hàng
        public async Task<string> AddProductAsync(AddProductToCartRequest request)
        {
            try
            {

                // Kiểm tra người dùng đã đăng nhập chưa
                var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
                if (!userExists)
                {
                    return "Vui lòng đăng nhập trước khi thêm sản phẩm";
                }

                // Kiểm tra xem giỏ hàng có tồn tại cho người dùng không
                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                if (cart == null )
                {
                    // Nếu giỏ hàng chưa tồn tại, tạo giỏ hàng mới
                    cart = new Cart { UserId = request.UserId, CartProducts = new List<CartProduct>() };
                    await _context.Carts.AddAsync(cart);
                }


                // Kiểm tra sản phẩm trong giỏ hàng
                var cartProduct = await _context.CartProducts
                    .FirstOrDefaultAsync(cp => cp.ProductId == request.ProductId && cp.CartId == cart.CartId);

                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == request.ProductId);
                if (product == null)
                {
                    return "Sản phẩm không tồn tại";
                }

                var promotional = await _context.PromotionalProducts.FirstOrDefaultAsync(p => p.ProductId == request.ProductId);
                decimal discount_amounts = 0;
                decimal price = product.Price;
                decimal totalPrice = price;

                // Nếu có khuyến mãi, tính giá giảm
                if (promotional != null)
                {
                    discount_amounts = price * promotional.DiscountPercentage;
                    totalPrice = price - discount_amounts;
                }

                if (cartProduct != null)
                {
                    // Nếu sản phẩm đã có trong giỏ, cập nhật số lượng và giá trị
                    cartProduct.Quantity += 1;

                    // Cập nhật tổng giá dựa trên số lượng mới
                    cartProduct.discount_amount = cartProduct.discount_amount + discount_amounts;
                    cartProduct.Price = price * cartProduct.Quantity;
                    cartProduct.TotalPrice = cartProduct.Quantity * totalPrice;  // Cập nhật tổng giá cho số lượng mới
                }
                else
                {

                    // Nếu sản phẩm chưa có trong giỏ, tạo mới một bản ghi cho giỏ hàng
                    var newCartProduct = new CartProduct
                    {
                        ProductId = request.ProductId,
                        Quantity = 1,
                        discount_amount = discount_amounts,
                        Price = price,
                        TotalPrice = totalPrice,
                        Cart = cart
                    };
                    await _context.CartProducts.AddAsync(newCartProduct);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
                return "Đã thêm sản phẩm vào giỏ hàng";
            }
            catch (Exception e)
            {
                // Ghi lại thông tin lỗi nếu cần thiết
                throw new Exception("Đã xảy ra lỗi khi thêm sản phẩm vào giỏ hàng: " + e.Message);
            }
        }

        //Xóa sản phẩm trong giỏ hàng
        public async Task<string> DeleteProductInCart(int cartProductId)
        {
            try
            {
                var product = _context.CartProducts.FirstOrDefault(p => p.CartProductId == cartProductId);

                if (product == null)
                {
                    return "sản phẩm không tồn tại";
                }
                else
                {
                    _context.CartProducts.Remove(product);
                    // Lấy giỏ hàng chứa sản phẩm này
                    var cart = _context.Carts.FirstOrDefault(c => c.CartId == product.CartId);

                    if (cart != null)
                    {
                        // Cập nhật tổng tiền của giỏ hàng
                        cart.TotalPrice -= product.TotalPrice;


                        // Đảm bảo tổng tiền không âm
                        if (cart.TotalPrice < 0)
                        {
                            cart.TotalPrice = 0;
                        }
                    }


                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _context.SaveChangesAsync();




                }
                return "đã xóa sản phẩm ra khỏi giỏ hàng";

            }
            catch (Exception e)
            {
                throw new Exception("Có lỗi xảy ra vui lòng thử lại" + e.Message);
            }

        }
        //Cập nhật số lượng sản phẩm
        public async Task<string> UpdateQuantityInCart(UpdateQuantityInCartRequest request)
        {
            try
            {
                // Kiểm tra thông tin đầu vào
                if (request == null || request.CartProductId <= 0 || request.Quantity < 1)
                {
                    return "Thông tin không hợp lệ. Số lượng phải lớn hơn hoặc bằng 1.";
                }

                // Lấy sản phẩm từ giỏ hàng
                var cartProduct = await _context.CartProducts.FirstOrDefaultAsync(p => p.CartProductId == request.CartProductId);
                if (cartProduct == null)
                {
                    return "Sản phẩm không tồn tại trong giỏ hàng.";
                }

                // Lấy thông tin sản phẩm gốc
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartProduct.ProductId);
                if (product == null)
                {
                    return "Sản phẩm không tồn tại trong hệ thống.";
                }

                // Lấy giỏ hàng liên quan
                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartProduct.CartId);
                if (cart == null)
                {
                    return "Giỏ hàng không tồn tại.";
                }



                // Cập nhật số lượng
                cartProduct.Quantity = request.Quantity;

                // Kiểm tra và tính khuyến mãi
                var prom = await _context.PromotionalProducts.FirstOrDefaultAsync(p => p.ProductId == cartProduct.ProductId);
                decimal discountAmount = 0;
                if (prom != null && prom.DiscountPercentage > 0)
                {
                    discountAmount = (request.Quantity * product.Price) * prom.DiscountPercentage;
                }

                // Tính lại giá trị sau khuyến mãi
                cartProduct.Price = product.Price * cartProduct.Quantity;
                cartProduct.TotalPrice = (request.Quantity * product.Price) - discountAmount;
                cartProduct.discount_amount = discountAmount;



                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return "Cập nhật số lượng thành công.";
            }
            catch (Exception e)
            {
                // Log lỗi chi tiết hơn tại đây nếu cần
                throw new Exception($"Lỗi khi cập nhật số lượng: {e.Message}");
            }
        }

        // lấy danh sách sản phẩm trong giỏ hàng
        public async Task<List<ProductInCartResponse>> GetListProductAsync(ClaimsPrincipal user)
        {
            try
            {
                // Lấy userId từ Claims
                var userId = user.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("User ID không tồn tại.");
                }
                // Tìm giỏ hàng theo UserId
                var cart = await _context.Carts
                    .Include(c => c.CartProducts)
                    .ThenInclude(cp => cp.Product) // Nhúng thông tin sản phẩm
                    .FirstOrDefaultAsync(c => c.UserId == userId);
                if (cart == null)
                {
                    throw new Exception("Giỏ hàng không tồn tại.");
                }

                // Lấy danh sách sản phẩm từ CartProducts dựa trên CartId
                var productsInCart = await _context.CartProducts
                    .Where(cp => cp.CartId == cart.CartId)
                    .Include(cp => cp.Product) // Nhúng thông tin sản phẩm
                    .ToListAsync(); // Lấy tất cả sản phẩm trong giỏ hàng

                // Ánh xạ danh sách cartProducts sang List<ProductInCartResponse>
                var response = _mapper.Map<List<ProductInCartResponse>>(productsInCart);

                return response;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //Lấy các giá trị trong giỏ hàng
        public async Task<CartResponse> GetCartSummaryAsync(string userId)
        {
            try
            {
                // Lấy giỏ hàng của người dùng
                var cart = await _context.Carts
                    .Include(c => c.CartProducts) // Bao gồm cả sản phẩm trong giỏ hàng
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    return new CartResponse
                    {
                        TotalPrice = 0,
                        TotalItem = 0,
                        TotalDiscount = 0,
                        ShippingFee = 0
                    };
                }

                // Tính tổng tiền
                decimal Price = cart.CartProducts.Sum(cp => cp.TotalPrice);


                // Tính tổng giảm giá
                decimal totalDiscount = cart.CartProducts.Sum(cp => cp.discount_amount) + cart.code_discount ;

                // Tính tổng số lượng
                int totalItems = cart.CartProducts.Count(cp => cp.CartId == cart.CartId);

                // Giả sử phí vận chuyển là 10% tổng giá trị đơn hàng, hoặc tối thiểu là 20
                decimal shippingFee = Price * (10m / 100);

                //Tổng tiền
                var totalprice = Price + shippingFee - totalDiscount ;

                // Cập nhật giỏ hàng
                cart.TotalItems = totalItems;
                cart.promotion_discount = totalDiscount;
                cart.discount_amount = totalDiscount;
                cart.TotalPrice = totalprice; // Cộng phí vận chuyển và trừ giảm giá
                cart.shipping_fee = shippingFee;

                // nếu giá trị giảm gió nhỏ hơn không thì xóa mã giảm giá và chuyển giá trị về lại 0
                if (cart.TotalPrice < 0)
                {
                    cart.TotalPrice = 0;
                    cart.code = "";
                   cart.code_discount = 0;

                }

                await _context.SaveChangesAsync();

                // Trả về thông tin giỏ hàng 
                return new CartResponse
                {
                    Price = Price,
                    TotalPrice = totalprice,
                    TotalItem = totalItems,
                    TotalDiscount = totalDiscount,
                    ShippingFee = shippingFee,
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Lỗi khi tính tổng giỏ hàng: {e.Message}");
            }
        }
    }
    }


