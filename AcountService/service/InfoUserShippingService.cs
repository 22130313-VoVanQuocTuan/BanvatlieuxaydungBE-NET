using AcountService.entity;
using AcountService.Repository;
using AutoMapper;
using BanVatLieuXayDung.dto.request;
using BanVatLieuXayDung.dto.response.InfoOrderResponse;
using Microsoft.EntityFrameworkCore;

namespace BanVatLieuXayDung.service
{
    public class InfoUserShippingService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public InfoUserShippingService(DataContext dataContext,     IMapper mapper)
        {
            _mapper = mapper;
            _context = dataContext;
        }

        // Thêm thông tin vận chuyển
        public async Task<string> UpdateInfoShipping(UpdateInfoShippingRequest request)
        {
            try
            {
                var info = await _context.InfoUserOrders.FirstOrDefaultAsync(i => i.UserId ==request.UserId);
                if (info == null)
                {
                    // Ánh xạ từ request sang entity
                    var infoUserOrder = _mapper.Map<InfoUserOrder>(request);
                    // Gán UserId từ request (nếu cần thiết)
                    infoUserOrder.UserId = request.UserId;
                    // Thêm bản ghi vào context
                    await _context.InfoUserOrders.AddAsync(infoUserOrder);
                }
                else
                {
                    info.Address = request.Address;
                    info.PhoneNumber = request.PhoneNumber;
                    info.FullName   = request.FullName;
                    info.Email = request.Email;

                }
                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return "Thêm thông tin thành công";
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu có hệ thống logging)
                Console.WriteLine($"Lỗi khi thêm thông tin vận chuyển: {ex.Message}");

                // Ném lỗi cụ thể để dễ gỡ lỗi
                throw new Exception("Đã xảy ra lỗi khi thêm thông tin vận chuyển. Chi tiết: " + ex.Message);
            }
        }
        //Lấy  thông tin theo userId
        public async Task<InfoOrderResponse> GetInfoShipping(string userId)
        {
            try
            {
                var info = await _context.InfoUserOrders
                                  .FirstOrDefaultAsync(i => i.UserId == userId);
                if (info == null)
                {
                    throw new  Exception("Không tìm thấy thông tin vận chuyển cho người dùng này.");
                }  // Trả về danh sách kết quả
                var response =  _mapper.Map<InfoOrderResponse>(info);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        
    }
}
