using AcountService.entity;
using System.ComponentModel.DataAnnotations;

namespace AcountService.dto.request.Order
{
    public class OrderRequest
    {
        public string UserId { get; set; } // Thêm UserId

        [Required(ErrorMessage ="Vui lòng chọn phương thức thanh toán")]
        public string PaymentMethod { get; set; }
       }
}
