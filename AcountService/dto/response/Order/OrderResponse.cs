    using AcountService.entity;
    using AcountService.status;

    namespace AcountService.dto.response.Order
    {
        public class OrderResponse
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
          public List<OrderDetail> OrderDetails { get; set; } // Thông tin chi tiết đơn hàng
          public string PaymentUrl { get; set; } // Liên kết thanh toán VNPay
    }
    }
