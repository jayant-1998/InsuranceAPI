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

        public InsuranceServices(IServiceProvider serviceProvider)
        {
            _repositories = serviceProvider.GetRequiredService<IInsuranceRepositories>();
        }
        public string PopulateHtmlTemplateWithUserData(string htmlTemplate, UserResponseModels user)
        {
            PropertyInfo[] properties = typeof(UserResponseModels).GetProperties();

            foreach (var property in properties)
            {
                string placeholder = $"{{{{{property.Name}}}}}";
                object value = property.GetValue(user);
                string valueString = value != null ? value.ToString() : string.Empty;

                htmlTemplate = htmlTemplate.Replace(placeholder, valueString);
            }

            return htmlTemplate; 
        }
        public async Task<string> FinalApi(int id)
        {
            var template = await _repositories.GetTemplateDB();
            var userbody = await _repositories.GetUserDB(id).ConfigureAwait(false);

            var html = PopulateHtmlTemplateWithUserData(template.HTML,userbody);

            return html;

        }

        public async Task<TemplateResponseModels> GetTemplate()
        {
            var temp = await _repositories.GetTemplateDB();

            return temp;
        }
    }
}
