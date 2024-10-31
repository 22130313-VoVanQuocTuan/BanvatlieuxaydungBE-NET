using AcountService.dto.request.cart;
using AcountService.dto.response.Cart;
using AcountService.dto.response.product;
using AcountService.entity;
using AcountService.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcountService.service
{
    public class CartService
    {
        public readonly DataContext _context;
        public readonly IMapper _mapper;

        public CartService(DataContext context , IMapper mapper)

        {
            _context = context;
            _mapper = mapper;
        }

        // Phương thức để thêm sản phẩm vào giỏ hàng
        public async Task<string> AddProductAsync(AddProductToCartRequest request)
        {
            try
            {
                
                var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
                if (!userExists)
                {
                    return "Vui lòng đăng nhập trước khi thêm sản phẩm";
                }
                // Kiểm tra xem giỏ hàng có tồn tại cho người dùng không
                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                if (cart == null)
                {
                    // Nếu giỏ hàng chưa tồn tại, tạo giỏ hàng mới
                    cart = new Cart { UserId = request.UserId, CartProducts = new List<CartProduct>() };
                    await _context.Carts.AddAsync(cart);
                }

                // Kiểm tra sản phẩm trong giỏ hàng
                var cartProduct = await _context.CartProducts
                    .FirstOrDefaultAsync(cp => cp.ProductId == request.ProductId && cp.CartId == cart.CartId);

                var product = _context.Products.FirstOrDefault(cp => cp.ProductId.Equals(request.ProductId));
                if (cartProduct != null)
                {

                    // Cập nhật số lượng nếu sản phẩm đã có trong giỏ
                    cartProduct.Quantity++;

                    if (product != null)
                    {
                        cartProduct.TotalPrice = cartProduct.Quantity * product.Price;
                    }
                }
                else
                {
                    // Thêm sản phẩm mới vào giỏ hàng
                    var newCartProduct = new CartProduct
                    {
                        ProductId = request.ProductId,
                        Quantity = 1,
                        Price = product.Price,
                        TotalPrice =  product.Price,
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
                throw new Exception(e.Message);
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
                    await _context.SaveChangesAsync();


                }
                return "đã xóa sản phẩm ra khỏi giỏ hàng";

            }
            catch (Exception e)
            {
                throw new Exception("Có lỗi xảy ra vui lòng thử lại" + e.Message);
            }

        }
        // Cập  nhật số lượng
        public async Task<string> UpdateQuantityInCart(UpdateQuantityInCartRequest request)
        {
            try
            {
                if (request == null || request.CartProductId <= 0 || request.Quantity <= 0)
                {
                    return "Thông tin không hợp lệ.";
                }

                var product = await _context.CartProducts.FirstOrDefaultAsync(p => p.CartProductId == request.CartProductId);
                if (product == null)
                {
                    return "Sản phẩm không tồn tại.";
                }

                product.Quantity = request.Quantity; // Cập nhật số lượng
                product.TotalPrice = product.Quantity * product.Price; // Cập nhật tổng giá

                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

                return "Đã cập nhật số lượng sản phẩm trong giỏ hàng.";
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
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
  
                return response ;
 
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
    }

