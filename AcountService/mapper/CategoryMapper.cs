using AcountService.dto.request.Category;
using AcountService.dto.response.Category;
using AcountService.entity;
using AutoMapper;

namespace AcountService.mapper
{
    public class CategoryMapper:Profile
    {
        public CategoryMapper() {
            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<Category, CategoryResponse>();
        }
    }
}
