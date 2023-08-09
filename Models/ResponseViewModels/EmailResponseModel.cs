namespace InsuranceAPI.Models.ResponseViewModels
{
    public class EmailResponseModel
    {
        public int ID { get; set; }
        public int userId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public byte[] attachment { get; set; }
        public int attempts { get; set; } 
        public int maxAttempts { get; set; }
        public bool isSend { get; set; } 
        public DateTime? createdAt { get; set; }
        public DateTime? modifiedAt { get; set; }
    }
}
