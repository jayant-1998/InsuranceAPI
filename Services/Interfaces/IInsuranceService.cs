namespace InsuranceAPI.Services.Interfaces
{
    public interface IInsuranceService
    {
        public Task<string> PopulateDataAndCreatePdfAsync(int id);
        public Task<byte[]> HtmlToPdfAsync(string html,int id);
        public Task<bool> GetEmailsAsync();
    }
}
