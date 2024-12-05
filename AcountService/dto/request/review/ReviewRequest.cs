namespace BanVatLieuXayDung.dto.request.rating
{
    public class ReviewRequest
    {
        public string UserId { get; set; }
        public int ProductId {  get; set; }
        public string Comment { get; set; }
    }
}
