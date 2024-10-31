namespace AcountService.status
{
   
        public enum OrderStatus
        {
            Pending,  //Chưa giải quyết,
            Processing, //Đang xử lý
            Shipped,   // đã vẫn chuyển
            Delivered,   //đã giao hàng 
            Canceled // Đã hủy
        }
    }

