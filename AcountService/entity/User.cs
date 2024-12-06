using BanVatLieuXayDung.entity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcountService.entity
{

    public class User : IdentityUser
    {
        public string Email { get; set; }
        public EmailVerificationCode EmailVerificationCode { get; set; }

        public string Address { get; set; }
        public string PhoneNumber {  get; set; }
        public string FullName {  get; set; }

        public Cart Cart { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Password_reset> password_Resets { get; set; } = new List<Password_reset>();
        public ICollection<InfoUserOrder> infoUserOrders { get; set; } = new List<InfoUserOrder>();
    }
}