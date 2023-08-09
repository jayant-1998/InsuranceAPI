namespace InsuranceAPI.Services.Interfaces
{
    public interface IInsuranceService
    {
        public Task<string> populateDataAndCreatePdfSaveInDbAsync(int id);
        public Task<byte[]> HtmlToPdfAsync(string html,int id);
        public Task<bool> SendAllEmailAsync();
    }
}
