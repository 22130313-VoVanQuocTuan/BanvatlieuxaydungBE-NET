using AcountService.dto.request.product;
using AcountService.dto.response.product;
using AcountService.entity;
using AcountService.mapper;
using AcountService.Repository;
using AutoMapper;
using BanVatLieuXayDung.dto.response.product;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AcountService.service
{
    public class ProductService
    {
        private readonly DataContext _context;
        private readonly IMapper _productMaper;

        public ProductService(DataContext context, IMapper productMaper)
        {
            _context = context;
            _productMaper = productMaper;
        }

        //Thêm sản phẩm
        public async Task<ProductDetailResponse> createProductAsync(CreateProductRequest request)
        {
            try
            {

                var productName = await _context.Products
                                                .FirstOrDefaultAsync(p => p.Name.ToLower() == request.Name.ToLower());
                if (productName != null)
                {
                    throw new Exception("Sản phẩm đã tồn tại");
                }
                // Kiểm tra danh mục có tồn tại không
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategotyId == request.CategoryId);

                if (category == null)
                {
                    throw new Exception("Danh mục không tồn tại.");
                }

                var product = _productMaper.Map<Product>(request);
                // Kiểm tra và lưu ảnh
                if (request.UrlImage != null)
                {
                    // Tạo đường dẫn cho ảnh
                    var imagePath = Path.Combine("wwwroot/images", request.UrlImage.FileName);

                    // Lưu ảnh vào thư mục wwwroot/images
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await request.UrlImage.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn ảnh vào thuộc tính UrlImage trong sản phẩm
                    product.UrlImage = $"/images/{request.UrlImage.FileName}";
                }
                product.CategoryId = request.CategoryId;
                product.CreatedAt = DateTime.Now; // Ghi lại thời gian tạo
                product.UpdatedAt = DateTime.Now; // Ghi lại thời gian cập nhật
                product.Status = request.Status; // Mặc định là sản phẩm hoạt động


                await _context.AddAsync(product);
                await _context.SaveChangesAsync();

                

                var response = _productMaper.Map<ProductDetailResponse>(product);

                return response;


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        // Cập nhật sản phẩm
        public async Task<ProductDetailResponse> updateProductAsync(int id, UpdateProductRequest request)
        {
            try
            {
                // Tìm sản phẩm theo ID
                var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

                if (existingProduct == null)
                {
                    throw new Exception("Sản phẩm không tồn tại.");
                }

             

                // Cập nhật các trường nếu có thay đổi
                if ( request.Name != existingProduct.Name)
                {
                    existingProduct.Name = request.Name;
                }
                if (request.CategoryId != existingProduct.CategoryId)
                {
                    existingProduct.CategoryId = request.CategoryId;
                }
                if ( request.Description != existingProduct.Description)
                {
                    existingProduct.Description = request.Description;
                }
                if ( request.Price != existingProduct.Price)
                {
                    existingProduct.Price = request.Price;
                }
                if (request.StockQuantity != existingProduct.StockQuantity)
                {
                    existingProduct.StockQuantity = request.StockQuantity;
                }
                if (request.Status != existingProduct.Status)
                {
                    existingProduct.Status = request.Status;
                }

                // Xử lý cập nhật ảnh nếu có
                if (request.UrlImage != null)
                {
                    // Tạo đường dẫn cho ảnh cũ
                    var oldImagePath = Path.Combine("wwwroot", existingProduct.UrlImage.TrimStart('/'));
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }

                    // Tạo đường dẫn mới
                    var newImagePath = Path.Combine("wwwroot/images", request.UrlImage.FileName);
                    using (var stream = new FileStream(newImagePath, FileMode.Create))
                    {
                        await request.UrlImage.CopyToAsync(stream);
                    }

                    existingProduct.UrlImage = $"/images/{request.UrlImage.FileName}";
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                // Trả về sản phẩm đã cập nhật
                var response = _productMaper.Map<ProductDetailResponse>(existingProduct);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        //Xóa sản phẩm
        public async Task<string> DeleteProduct(int productId)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    return "Sản phẩm không tồn tại";
                }

                var imagePath = Path.Combine("wwwroot", product.UrlImage.TrimStart('/'));
                File.Delete(imagePath);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();


                return "Sản phẩm đã được xóa";


            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete product {productId}");
            }

        }

        // Lấy ra danh sách sản phẩm
        public async Task<object> GetListProduct(int page, int size)
        {
            try
            {
                // Lấy tất cả sản phẩm từ cơ sở dữ liệu
                var result = await _context.Products.ToListAsync();

                // Sắp xếp theo ngày tạo sản phẩm (giảm dần)
                var sortDate = result.OrderByDescending(p => p.CreatedAt).ToList();

                // Tính tổng số sản phẩm
                var totalCount = sortDate.Count();

                // Áp dụng phân trang
                var paginatedProducts = sortDate.Skip((page - 1) * size).Take(size).ToList();

                var response = new List<ProductDetailResponse>();

                foreach (var product in paginatedProducts)
                {
                    // Kiểm tra nếu sản phẩm có khuyến mãi
                    var promotional = await _context.PromotionalProducts
                        .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

                    // Ánh xạ sản phẩm sang ProductDetailResponse
                    var productResponse = _productMaper.Map<ProductDetailResponse>(product);

                    // Nếu có khuyến mãi, thêm DiscountPercentage vào ProductDetailResponse
                    if (promotional != null)
                    {
                        productResponse.DiscountPercentage = promotional.DiscountPercentage;
                    }

                    // Thêm sản phẩm đã ánh xạ vào danh sách phản hồi
                    response.Add(productResponse);
                }
                // Trả về cả danh sách sản phẩm và tổng số sản phẩm
                return new
                {
                    products = response,
                    totalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Tìm sản phẩm theo id

        public async Task<ProductDetailResponse> getProductAsync(int id)
        {
            try
            {
                var productId = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                if (productId == null)
                {
                    throw new Exception("Không tìm thấy sản phẩm");
                }
                var response = _productMaper.Map<ProductDetailResponse>(productId);
                return response;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //Cập nhật số lượng sản phẩm
        public async Task<string> UpdateStock(UpdateStockProductRequest request)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == request.ProductId);
                if(product == null)
                {
                    throw new Exception("Sản phẩm không tông tại");

                }
                // Kiểm tra số lượng không âm
                if (request.Quantity < 0)
                {
                    throw new Exception("Số lượng không được nhỏ hơn 0");
                }
                product.StockQuantity = request.Quantity;
                await  _context.SaveChangesAsync();
                return "Cập nhật thành công số lượng sản phẩm";

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Lấy ra danh sách sản phẩm theo danh mục
        public async Task<object> GetListProductInCategoryId(int page, int size, int categoryId)
        {
            try
            {
                // Lấy tổng số sản phẩm trong danh mục
                var totalCount = await _context.Products
                    .Where(p => p.CategoryId == categoryId) // Lọc theo CategoryId
                    .CountAsync();

                // Lấy danh sách sản phẩm theo danh mục
                var products = await _context.Products
                    .Where(p => p.CategoryId == categoryId)   // Lấy sản phẩm theo CategoryId
                    .OrderByDescending(p => p.CreatedAt)      // Sắp xếp theo ngày tạo
                    .Skip((page - 1) * size)                  // Bỏ qua số lượng sản phẩm đã chỉ định
                    .Take(size)                               // Lấy số lượng sản phẩm cần thiết
                    .ToListAsync();

                var response = new List<ProductDetailResponse>();

                foreach (var product in products)
                {
                    // Kiểm tra nếu sản phẩm có khuyến mãi
                    var promotional = await _context.PromotionalProducts
                        .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

                    // Ánh xạ sản phẩm sang ProductDetailResponse
                    var productResponse = _productMaper.Map<ProductDetailResponse>(product);

                    // Nếu có khuyến mãi, thêm DiscountPercentage vào ProductDetailResponse
                    if (promotional != null)
                    {
                        productResponse.DiscountPercentage = promotional.DiscountPercentage;
                    }

                    // Thêm sản phẩm đã ánh xạ vào danh sách phản hồi
                    response.Add(productResponse);
                }

                // Trả về danh sách sản phẩm và tổng số sản phẩm trong danh mục
                return new
                {
                    products = response,
                    totalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Lấy ra mô tả của sản phẩm
        public async Task<DescriptionResponse> getDescriptionAsync(int productId) 
        { 

            var des= await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (des == null)
            {
                throw new Exception( "mô tả không tông tại");
            }

            DescriptionResponse descriptionResponse = new DescriptionResponse()
            {
                Description = des.Description
            };
            return descriptionResponse;
        
        }
        //TÌM KIẾM SẢN PHẨM THEO TÊN BÁT KÌ
        public async Task<List<Product>> SearchProductsByKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<Product>();

            return await _context.Products
                                 .Where(p => EF.Functions.Like(p.Name, $"%{keyword}%"))
                                 .ToListAsync();
        }
    }

}