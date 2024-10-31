using AcountService.dto.request.discount;
using AcountService.dto.response.Discount;
using AcountService.entity;
using AcountService.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Linq.Expressions;

namespace AcountService.service
{
    public class DiscountService
    {
        private readonly DataContext _context;
        public DiscountService(DataContext context)
        {
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
                    return "mã code đã tồn tại";
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
                throw new Exception("Có lỗi xảy ra vui lòng thử lại" + ex.Message);
            }


        
        }

        // Kiểm tra tính hợp lệ của mã giảm giá
        public async Task<DiscountResponse> ConfrimCode(string  code)
        {
            try
            {
                var discount = await _context.Discount.FirstOrDefaultAsync(d => d.code == code);
                if (discount == null)
                {
                    throw new Exception("Mã không tồn tại");
                }


                var respone = new DiscountResponse()
                {
                    DiscountId = discount.DiscountId,
                    code = code,
                    Percent = discount.Percent,
                };

                return respone ;
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi xảy ra vui lòng thử lại" + ex.Message);
            }



        }
    }
}
