using AcountService.dto.request.discount;
using AcountService.dto.response.Discount;
using AcountService.entity;
using AcountService.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Linq.Expressions;

namespace AcountService.service
{
    public class DiscountService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public DiscountService(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        // tạo mã giảm giá
        public async Task<string> CreateDiscount(DiscountResquest resquest)
        {
            try
            {
                var discount = await _context.Discount.FirstOrDefaultAsync(d => d.code == resquest.code);
                if (discount != null)
                {
                    throw new Exception( "mã code đã tồn tại");
                }
                Discount discountt = new Discount()
                {
                    code = resquest.code,
                    Percent = resquest.Percent,
                };
                _context.Discount.Add(discountt);
                await _context.SaveChangesAsync();

                return "đã tạo thành công mã giảm giá";
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra vui lòng thử lại " + ex.Message);
            }


        
        }

        // Kiểm tra tính hợp lệ của mã giảm giá
        public async Task<DiscountResponse> ConfrimCode( string code, string UserId)
        {
            try
            {
                var discount = await _context.Discount.FirstOrDefaultAsync(d => d.code == code.ToUpper());
                if (discount == null)
                {
                    throw new KeyNotFoundException("Mã giảm giá không tồn tại.");
                }

                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == UserId);
                if (cart == null)
                {
                    throw new Exception("Không tìm thấy giỏ hàng cho người dùng này.");
                }

                if (discount.Percent > cart.TotalPrice)
                {
                    throw new Exception("Giảm giá không thể lớn hơn tổng giá trị giỏ hàng.");
                }
                if (cart.code == null)
                {
                    cart.code_discount = discount.Percent; // Áp dụng giá trị giảm giá từ mã mới
                    cart.discount_amount += cart.code_discount; // Tổng giảm giá
                    cart.code = code.ToUpper(); // Lưu mã giảm giá
                    cart.TotalPrice -= discount.Percent; // Cập nhật tổng giá trị giỏ hà
                }
                else {
                    if (cart.code == code.ToUpper())
                    {
                        throw new Exception("Mã đã được áp dụng");
                    }
                    // Nếu mã mới khác mã cũ
                    if (cart.code != code.ToUpper())
                    {
                        // Khôi phục giá trị giỏ hàng trước khi áp mã cũ
                        cart.TotalPrice += cart.code_discount;

                        // Áp dụng mã giảm giá mới
                        cart.code = code.ToUpper(); // Cập nhật mã mới
                        cart.code_discount = discount.Percent; // Cập nhật giá trị giảm giá từ mã mới
                        cart.discount_amount += cart.code_discount ; // Tổng giảm giá
                        cart.TotalPrice -= discount.Percent; // Cập nhật tổng giá trị giỏ hàng
                    }

                }

                    var response = new DiscountResponse()
                    {
                        DiscountId = discount.DiscountId,
                        code = code,
                        Percent = discount.Percent,

                    };
                    await _context.SaveChangesAsync();


                    return response;
                
            }
      
            catch (Exception ex)
            {
                 throw new Exception(ex.Message);
            }
        }

        //  Lấy ra danh sách mã giảm giá
        public async Task<List<DiscountResponse>> getDiscountAsync()
            {
            try
            {
                var discount = await _context.Discount.ToListAsync();
                if (discount == null)
                {
                    throw new Exception("Không có mã nào");
                }

                var response = _mapper.Map<List<DiscountResponse>>(discount);
                return response.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra vui lòng thử lại" + ex.Message);
            }


            }

        //  Xóa khuyến mãi
        public async Task<string> DeleteDiscountAsync(int id)
        {
            try
            {
                var discount = await _context.Discount.FirstOrDefaultAsync(d => d.DiscountId == id);
                if (discount == null)
                {
                    throw new Exception("Không có mã nào");
                }
                _context.Discount.Remove(discount);
                _context.SaveChanges();

                return "Mã đã được xóa";
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra vui lòng thử lại" + ex.Message);
            }


        }
    }
}
