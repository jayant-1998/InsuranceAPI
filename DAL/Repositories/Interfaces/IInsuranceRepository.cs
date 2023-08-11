using InsuranceAPI.Models.ResponseViewModels;

namespace InsuranceAPI.DAL.Repositories.Interfaces
{
    public interface IInsuranceRepository
    {
        public Task<TemplateResponseViewModel> GetHtmlTemplateAsync();
        public Task<UserResponseViewModel> GetUserAsync(int id);
        public Task<string> InsertDocumentAsync(UserResponseViewModel user, byte[] pdf);
        public Task<IEnumerable<EmailResponseViewModel>> GetEmailsAsync();
        public Task<bool> UpdateEmailAsync(EmailResponseViewModel email,bool isSend);
        public Task<string> InsertEmailAsync(UserResponseViewModel user, byte[] email);
        public Task<bool> IsEmailExitsAsync(UserResponseViewModel user);
    }
}
