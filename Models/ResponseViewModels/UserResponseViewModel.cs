
namespace InsuranceAPI.Models.ResponseViewModels
{
    public class UserResponseViewModel
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string policyNumber { get; set; }
        public int age { get; set; }
        public int salary { get; set; }
        public string occupation { get; set; }
        public DateTime policyExpiryDate { get; set; }
        public string productCode { get; set; }
        public string email { get; set; }
    }
}