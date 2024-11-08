using AcountService.Repository;
using AutoMapper;
using BanVatLieuXayDung.dto.request.Promotional;
using BanVatLieuXayDung.dto.response.Prommotional;
using BanVatLieuXayDung.entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanVatLieuXayDung.service
{
    public class PromotionalService


    {
        public readonly DataContext _context;
        public readonly IMapper _mapper;

        public PromotionalService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //Lấy danh sách khuyến mãi
        public async Task<List<PromotionalResponse>> GetListPromotions()
        {
            try
            {
                var prom = await _context.PromotionalProducts
               .Include(p => p.Product) // Nạp thông tin sản phẩm
               .ToListAsync();
                var response = _mapper.Map<List<PromotionalResponse>>(prom);
                return response;
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        // Thêm một khuyến mãi mới
        public async Task<PromotionalResponse> CreatePromotional(CreatePromotionalRequest request)
        {
            try
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId == request.ProductId);
                if (product == null)
                {
                    throw new Exception("Sản phẩm không tồn tại");
                }
                var Prom = await _context.PromotionalProducts.FirstOrDefaultAsync(p => p.ProductId == request.ProductId);
                if (Prom != null)
                {
                    throw new Exception("Sản phẩm đang được khuyến mãi");

                }
                if (request.EndDate <= request.StartDate)
                {
                    throw new Exception("Thời gian hết hạn phải lớn hơn thời gian tạo");
                }
                  var Promotional =  _mapper.Map<PromotionalProducts>(request);
                  Promotional.ConditionDescription ??= "Mặc định";  // Giá trị mặc định nếu không có giá trị từ request
                _context.PromotionalProducts.Add(Promotional);
                  await _context.SaveChangesAsync();

                var Response = _mapper.Map<PromotionalResponse>(Promotional);
                 

                return Response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        // Xóa khuyến mãi 
        public async Task<string> DeletePromotional(int promId)
        {
            try
            {
                var Prom = await _context.PromotionalProducts.FirstOrDefaultAsync(p => p.ProductId == promId);
                if (Prom == null)
                {
                    throw new Exception("Sản phẩm không tồn tại");

                }
                _context.PromotionalProducts.Remove(Prom);
                _context.SaveChanges();


                return $"xóa thành công khuyến mãi của sản phẩm có id: {promId}";
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        // Cập nhật khuyến mãi 
        public async Task<PromotionalResponse> UpdatePromotional(int saleId,UpdatePromotionalRequest request)
        {
            try
            {
                var Prom = await _context.PromotionalProducts.FirstOrDefaultAsync(p => p.SaleId == saleId);
                if (Prom == null)
                {
                    throw new Exception("Sản phẩm không tồn tại");

                }

                Prom.SaleId = Prom.SaleId;
                Prom.ProductId = Prom.ProductId;
                Prom.DiscountPercentage = request.DiscountPercentage;
                Prom.StartDate = request.StartDate;
                Prom.EndDate = request.EndDate;

                 
                await _context.SaveChangesAsync();
                var  Response = _mapper.Map<PromotionalResponse>(Prom);
               



                return Response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        // Lấy thông tin khuyến mãi theo id
        public async Task<PromotionalResponse> GetPromotional(int productId)
        {
            try
            {
                var Prom = await _context.PromotionalProducts.FirstOrDefaultAsync(p => p.ProductId == productId);
                if (Prom == null)
                {
                    throw new Exception("Sản phẩm không tồn tại");

                }

                var Response = _mapper.Map<PromotionalResponse>(Prom);



                return Response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

    }
}
