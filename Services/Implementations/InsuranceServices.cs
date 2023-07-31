using InsuranceAPI.DAL.Entities;
using InsuranceAPI.DAL.Repositories.Interfaces;
using InsuranceAPI.Models.ResponseViewModels;
using InsuranceAPI.Services.Interfaces;
using System.Reflection;

namespace InsuranceAPI.Services.Implementations
{
    public class InsuranceServices : IInsuranceServices
    {
        private readonly IInsuranceRepositories _repositories;
        private async Task<TemplateResponseModels> GetTemplate()
        {
            return await _repositories.GetTemplateDB();
        }

        private async Task<UserResponseModels> GetUser(int id)
        {
            return await _repositories.GetUserDB(id);
        }

        private string PopulateHtmlTemplateWithUserData(string htmlTemplate, UserResponseModels user)
        {
            PropertyInfo[] properties = typeof(Users).GetProperties();

            foreach (var property in properties)
            {
                string placeholder = $"{{{{ {property.Name} }}}}";
                object value = property.GetValue(user);
                string valueString = value != null ? value.ToString() : string.Empty;

                htmlTemplate = htmlTemplate.Replace(placeholder, valueString);
            }

            return htmlTemplate; 
        }
        public string FinalApi(int id)
        {
            var template = GetTemplate();
            var userbody = GetUser(id);

            var html = PopulateHtmlTemplateWithUserData(template.HTML, userbody);

            return html;

        }
    }
}
