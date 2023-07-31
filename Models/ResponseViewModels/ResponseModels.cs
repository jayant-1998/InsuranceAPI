using System.ComponentModel.DataAnnotations;

namespace InsuranceAPI.Models.ResponseViewModels
{
    public class ResponseModel
    {
        public string Name { get; set; }
        public int PolicyNumber { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }
        public string Occupation { get; set; }
        public DateTime PolicyExpiryDate { get; set; }
        public string HTML { get; set; }
        public string ProductCode { get; set; }
    }
}
