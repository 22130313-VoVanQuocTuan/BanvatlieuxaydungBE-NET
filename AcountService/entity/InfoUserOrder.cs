namespace AcountService.entity
{
    public class InfoUserOrder
    {

        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }



      
        

    }
}
