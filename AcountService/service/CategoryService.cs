using AcountService.dto.request.Category;
using AcountService.dto.response.Category;
using AcountService.entity;
using AcountService.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;


namespace AcountService.service
{
    public class CategoryService
    {
        public readonly DataContext _context;
        public readonly IMapper _mapper;

        public CategoryService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // thêm danh mục
        public async Task<CategoryResponse> AddCategoryAsync(CreateCategoryRequest request)
        {
            try
            {
                var categoryName = await _context.Categories.FirstOrDefaultAsync(c => c.Name == request.name);
                if (categoryName != null)
                {
                    throw new Exception("Danh mục đã tồn tại");
                }
                var category = _mapper.Map<Category>(request);
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                var response = _mapper.Map<CategoryResponse>(category);
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }

        // xóa danh mục
        public async Task<string> DeleteCategoryAsync(int CategotyId)
        {
            try
            {
                var categoryName = await _context.Categories.FirstOrDefaultAsync(c => c.CategotyId == CategotyId);
                if (categoryName == null)
                {
                    throw new Exception("Danh mục ko tồn tại");
                }

                _context.Categories.Remove(categoryName);
                await _context.SaveChangesAsync();


                return "Danh mục và các sản phẩm liên quan đã được xóa.";

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }
        // Cập nhật danh mục
        public async Task<string> UpdateCategoryAsync(int CategotyId, UpdateCategoryRequest request)
        {
            try
            {
                var categoryName = await _context.Categories.FirstOrDefaultAsync(c => c.CategotyId == CategotyId);
                if (categoryName == null)
                {
                    return ("Danh mục ko tồn tại");
                }
                if (categoryName.Equals(request.Name))
                {
                    return "Tên danh mục đã tồn tại";
                }

                categoryName.Name = request.Name;



                await _context.SaveChangesAsync();


                return "Cập nhật thành công danh mục";

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }


        }
        // lấy ra danh sách danh mục
        public async Task<List<CategoryResponse>> GetListCategoryAsync()
        {
            try
            {
                var categoryName = await _context.Categories.ToListAsync();


                var respone = _mapper.Map<List<CategoryResponse>>(categoryName);

                return respone;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
    }
    }
