using InsuranceAPI.DAL.Entities;
using InsuranceAPI.Models.ResponseViewModels;

namespace InsuranceAPI.Services.Interfaces
{
    public interface IInsuranceServices
    {
        private Task<TemplateResponseModels> GetTemplate();
        private Task<UserResponseModels> GetUser(int id);
        private string PopulateHtmlTemplateWithUserData(string htmlTemplate, UserResponseModels user);
        public string FinalApi(int id);

    }
}
