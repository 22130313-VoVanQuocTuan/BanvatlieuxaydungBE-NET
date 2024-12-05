    using AcountService.entity;
    using AcountService.status;
using BanVatLieuXayDung.dto.response.Order;

    namespace AcountService.dto.response.Order
    {
        public class OrderResponse
        {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal shipping_fee { get; set; }
        public decimal discount_amount { get; set; }
        public string payment_method { get; set; }
        public string payment_status { get; set; }
        public string shipping_address { get; set; }
        public string Email {  get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Note { get; set; }
        public string TrackingNumber { get; set; } 

        public int Quantity { get; set; }
        public List<OrderDetailResponse> OrderDetailsRespone { get; set; } // Thông tin chi tiết đơn hàng

    }
    }
