using InsuranceAPI.DAL.Entities;
using InsuranceAPI.Models.ResponseViewModels;

namespace InsuranceAPI.DAL.Repositories.Interfaces
{
    public interface IInsuranceRepositories
    {
        public Task<TemplateResponseModels> GetTemplateDB();
        public Task<UserResponseModels> GetUserDB(int id);
    }
}
