namespace Betazon_Milano.Models
{
    public class CustomerPass
    {
        public int id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string mailAddress { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
