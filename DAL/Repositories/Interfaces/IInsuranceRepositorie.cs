using InsuranceAPI.DAL.Entities;
using InsuranceAPI.Models.ResponseViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.DAL.Repositories.Interfaces
{
    public interface IInsuranceRepositorie
    {
        public Task<TemplateResponseModel> GetTemplateDBAsync();
        public Task<UserResponseModel> GetUserDBAsync(int id);
        public Task<string> InsertIntoDocumentDBAsync(UserResponseModel user, byte[] pdf);
        public Task<IEnumerable<EmailResponseModel>> GetAllEmailDBAsync();
        public Task<bool> UpdateEmailDBAsync(EmailResponseModel email,bool isSend);
        public Task<string> InsertIntoEmailDBAsync(UserResponseModel user, byte[] email);
        public Task<bool> IsEmailExitsDBAsync(UserResponseModel user);
    }
}
