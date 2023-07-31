namespace InsuranceAPI.Models.ResponseViewModels
{
    public class ApiResponseModel
    {
        public DateTime Timestamp { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public object Body { get; set; }
    }
}
