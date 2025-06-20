﻿using BanVatLieuXayDung.entity;
using System.ComponentModel.DataAnnotations;

namespace AcountService.entity
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string UrlImage {  get; set; }
        public decimal Price { get; set; }
       

        public int StockQuantity { get; set; } // Số lượng có sẵn của sản phẩm

        public DateTime CreatedAt { get; set; } // Ngày tạo sản phẩm

        public DateTime UpdatedAt { get; set; } // Ngày cập nhật sản phẩm

        public string? Status { get; set; } // Trạng thái hoạt động của sản phẩm

        // Quan hệ với Category
        public int CategoryId { get; set; } // Danh mục sản phẩm
        public virtual Category Category { get; set; }  // Dùng từ khóa "virtual" để hỗ trợ lazy loading

        
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
       

        public ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();


        public ICollection<PromotionalProducts> PromotionalProducts { get; set; } = new List<PromotionalProducts>();
    }
}
