using InsuranceAPI.DAL.Entities;
using InsuranceAPI.Models.ResponseViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceAPI.DAL.Repositories.Interfaces
{
    public interface IInsuranceRepositories
    {
        public Task<TemplateResponseModels> GetTemplateDB();
        public Task<UserResponseModels> GetUserDB(int id);
        public Task<string> InsertIntoDocument(UserResponseModels user, byte[] pdf);
    }
}
