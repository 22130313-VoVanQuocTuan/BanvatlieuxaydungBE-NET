using AcountService.entity;
using AcountService.Repository;
using AutoMapper;
using BanVatLieuXayDung.dto.request.rating;
using Microsoft.EntityFrameworkCore;

namespace BanVatLieuXayDung.service
{
    public class ReviewService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public ReviewService(DataContext context, IMapper mapper)

        {
            _context = context;
            _mapper = mapper;
        }

        //TẠO ĐÁNH GIÁ
        public async Task<string> CreateRate(ReviewRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
            {
                return "Không thể đánh giá";
            }
            var response = _mapper.Map<Review>(request);
            response.CreatedAt = DateTime.Now;
            await _context.Reviews.AddAsync(response);
            await _context.SaveChangesAsync();
            return "Đánh giá thành công";
        }

        // XÓA ĐÁNH GIÁ
        public async Task<string> DeleteReview(int reviewId)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId);
            if (review == null)
            {
                return "đánh giá không tồn tại";
            }
   
             _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return "xóa thành công";
        }

        // LẤY RA DANH SÁCH ĐÁNH GIÁ XẾP THEO NGÀY
        public async Task<List<Review>> GetAllReview()
        {
            var review =   _context.Reviews.OrderByDescending(r => r.CreatedAt).ToList();
            if (review == null)
            {
                throw new Exception("Danh sách trống ");
            }
            return review;
    
        }
    }
}
