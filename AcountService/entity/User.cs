using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcountService.entity
{
    
    public class User:IdentityUser
    {
        public string Email { get; set; }
        public EmailVerificationCode EmailVerificationCode { get; set; }

        public DateTime? Dob { get; set; }

        public string? City { get; set; }

        public Cart Cart { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
       
     


        public ICollection<InfoUserOrder> InfoUserOrder { get; set; } = new List<InfoUserOrder>();

        



    }
}
