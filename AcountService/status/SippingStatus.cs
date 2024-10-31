namespace AcountService.status
{
    public enum SippingStatus
    {
        InTransit,        // Đang vận chuyển
        OutForDelivery,  // Đang giao
        Delivered ,       // Đã giao thành công
        FailedDelivery,  // Giao hàng thất bại
        Returned,        // Đã hoàn trả
        Cancelled        // Đã hủy
    }
}
